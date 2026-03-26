using System.Text.Json;
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
    private readonly Dictionary<int, PlayerSession> _sessionMap = new(); // sessionId → PlayerSession

    public PacketHandler(GameManager gameManager)
    {
        _gameManager = gameManager;
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

    private void HandleLogin(ClientSession client, C_Login packet)
    {
        Console.WriteLine($"[PacketHandler] Login request: {packet.AccountName}");

        var playerSession = _gameManager.CreateSession(packet.AccountName, "town_01");
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

            // TODO: 같은 방의 다른 플레이어들에게 이동 브로드캐스트
        }
    }

    private void HandleDisconnect(ClientSession client)
    {
        Console.WriteLine($"[PacketHandler] Client disconnected: Session {client.SessionId}");

        if (_sessionMap.TryGetValue(client.SessionId, out var playerSession))
        {
            var room = _gameManager.GetRoom(playerSession.MapId);
            room?.Leave(playerSession);
            _sessionMap.Remove(client.SessionId);
        }
    }
}
