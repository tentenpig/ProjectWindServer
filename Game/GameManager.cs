using Shared.Models;

namespace ProjectWindServer.Game;

/// <summary>
/// 서버 전체 게임 상태를 관리
/// 모든 GameRoom과 PlayerSession을 총괄
/// </summary>
public class GameManager
{
    private readonly Dictionary<string, GameRoom> _rooms = new();
    private readonly Dictionary<int, PlayerSession> _allSessions = new();
    private int _nextPlayerId = 1;

    public void LoadMaps(IEnumerable<MapData> maps)
    {
        foreach (var map in maps)
        {
            _rooms[map.Id] = new GameRoom(map);
            Console.WriteLine($"[GameManager] Map loaded: {map.Id} ({map.Name}) {map.Width}x{map.Height}");
        }
    }

    public GameRoom? GetRoom(string mapId)
    {
        return _rooms.GetValueOrDefault(mapId);
    }

    public PlayerSession CreateSession(string name, string startMapId)
    {
        var session = new PlayerSession
        {
            PlayerId = _nextPlayerId++,
            Name = name,
            MapId = startMapId
        };

        var room = GetRoom(startMapId);
        if (room != null)
        {
            session.Position = new Vec2Int(room.Map.StartX, room.Map.StartY);
            room.Enter(session);
        }

        _allSessions[session.PlayerId] = session;
        return session;
    }

    public void RemoveSession(int playerId)
    {
        _allSessions.Remove(playerId);
    }

    public IReadOnlyCollection<PlayerSession> GetAllSessions() => _allSessions.Values;
}
