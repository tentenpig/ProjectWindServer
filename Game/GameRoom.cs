using Shared.Models;

namespace ProjectWindServer.Game;

/// <summary>
/// 하나의 맵(방)을 관리하는 게임 룸
/// 해당 맵에 있는 모든 플레이어와 몬스터를 관리
/// </summary>
public class GameRoom
{
    public MapData Map { get; }
    private readonly Dictionary<int, PlayerSession> _players = new();
    private readonly object _lock = new();

    public GameRoom(MapData map)
    {
        Map = map;
    }

    public void Enter(PlayerSession session)
    {
        lock (_lock)
        {
            _players[session.PlayerId] = session;
            Console.WriteLine($"[GameRoom:{Map.Id}] Player {session.PlayerId} entered at ({Map.StartX}, {Map.StartY})");
        }
    }

    public void Leave(PlayerSession session)
    {
        lock (_lock)
        {
            _players.Remove(session.PlayerId);
            Console.WriteLine($"[GameRoom:{Map.Id}] Player {session.PlayerId} left");
        }
    }

    public bool TryMove(int playerId, Vec2Int target)
    {
        if (Map.IsBlocked(target.X, target.Y))
            return false;

        // TODO: 서버 권위적 이동 검증 (거리, 속도 체크)
        return true;
    }
}
