using Shared.Models;

namespace ProjectWindServer.Game;

/// <summary>
/// 하나의 맵(방)을 관리하는 게임 룸
/// 해당 맵에 있는 모든 Entity(Player, Enemy)를 관리
/// </summary>
public class GameRoom
{
    public MapData Map { get; }
    private readonly Dictionary<int, Entity> _entities = new();
    private readonly object _lock = new();

    public GameRoom(MapData map)
    {
        Map = map;
    }

    public void Enter(Entity entity)
    {
        lock (_lock)
        {
            _entities[entity.Id] = entity;
            Console.WriteLine($"[GameRoom:{Map.Id}] {entity.GetType().Name} {entity.Id} entered");
        }
    }

    public void Leave(Entity entity)
    {
        lock (_lock)
        {
            _entities.Remove(entity.Id);
            Console.WriteLine($"[GameRoom:{Map.Id}] {entity.GetType().Name} {entity.Id} left");
        }
    }

    public bool TryMove(int entityId, Vec2Int target)
    {
        if (Map.IsBlocked(target.X, target.Y))
            return false;

        // TODO: 서버 권위적 이동 검증 (거리, 속도 체크)
        return true;
    }

    public IReadOnlyCollection<Entity> GetAllEntities()
    {
        lock (_lock)
        {
            return _entities.Values.ToList();
        }
    }
}
