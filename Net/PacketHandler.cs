using System.Text.Json;
using ProjectWindServer.DB;
using ProjectWindServer.Game;
using Shared.Protocol;

namespace ProjectWindServer.Net;

/// <summary>
/// 수신된 패킷을 처리하는 핸들러
/// ClientSession과 GameManager를 연결
/// </summary>
public class PacketHandler
{
    private readonly GameManager _gameManager;
    private readonly AccountRepository _accountRepo;
    private readonly CheckpointManager _checkpointManager;
    private readonly Dictionary<int, PlayerSession> _sessionMap = new(); // sessionId → PlayerSession

    public PacketHandler(GameManager gameManager, AccountRepository accountRepo, CheckpointManager checkpointManager)
    {
        _gameManager = gameManager;
        _accountRepo = accountRepo;
        _checkpointManager = checkpointManager;
    }

    public void RegisterSession(ClientSession client)
    {
        client.OnPacketReceived += HandlePacket;
        client.OnDisconnected += HandleDisconnect;
    }

    private void HandlePacket(ClientSession client, ushort type, byte[] payload)
    {
        switch ((PacketType)type)
        {
            case PacketType.C_Login:
                HandleLogin(client, JsonSerializer.Deserialize<C_Login>(payload)!);
                break;
            case PacketType.C_Move:
                HandleMove(client, JsonSerializer.Deserialize<C_Move>(payload)!);
                break;
            case PacketType.C_Chat:
                // TODO: 채팅 구현
                break;
            default:
                Console.WriteLine($"[PacketHandler] Unknown packet type: {type}");
                break;
        }
    }

    private async void HandleLogin(ClientSession client, C_Login packet)
    {
        Console.WriteLine($"[PacketHandler] Login request: {packet.AccountName}");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(packet.Password);
        var account = await _accountRepo.GetOrCreateAccountAsync(packet.AccountName, passwordHash);

        // 비밀번호 검증 (기존 계정인 경우 저장된 해시와 비교)
        if (!BCrypt.Net.BCrypt.Verify(packet.Password, account.PasswordHash))
        {
            client.Send((ushort)PacketType.S_Login, new S_Login
            {
                Success = false,
                Message = "비밀번호가 일치하지 않습니다."
            });
            return;
        }

        var playerSession = _gameManager.CreateSession(packet.AccountName, account.MapId);
        playerSession.AccountId = account.Id;
        playerSession.Position = new Shared.Models.Vec2Int(account.PosX, account.PosY);
        _sessionMap[client.SessionId] = playerSession;

        client.Send((ushort)PacketType.S_Login, new S_Login
        {
            Success = true,
            PlayerId = playerSession.PlayerId,
            MapId = playerSession.MapId,
            Position = playerSession.Position
        });

        // 같은 방의 다른 플레이어들에게 스폰 알림
        // TODO: 브로드캐스트 구현
    }

    private void HandleMove(ClientSession client, C_Move packet)
    {
        if (!_sessionMap.TryGetValue(client.SessionId, out var playerSession))
            return;

        var room = _gameManager.GetRoom(playerSession.MapId);
        if (room == null) return;

        if (room.TryMove(playerSession.PlayerId, packet.Position))
        {
            playerSession.Position = packet.Position;
            playerSession.MarkDirty();

            // TODO: 같은 방의 다른 플레이어들에게 이동 브로드캐스트
        }
    }

    private async void HandleDisconnect(ClientSession client)
    {
        Console.WriteLine($"[PacketHandler] Client disconnected: Session {client.SessionId}");

        if (_sessionMap.TryGetValue(client.SessionId, out var playerSession))
        {
            // 강제 저장 후 퇴장
            await _checkpointManager.ForceSaveAsync(playerSession);

            var room = _gameManager.GetRoom(playerSession.MapId);
            room?.Leave(playerSession);
            _gameManager.RemoveSession(playerSession.PlayerId);
            _sessionMap.Remove(client.SessionId);
        }
    }
}
