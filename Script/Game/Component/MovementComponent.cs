namespace ProjectWindServer.Game.Component;

/// <summary>
/// 이동 관련 컴포넌트
/// Player, Enemy 모두 사용
/// </summary>
public class MovementComponent : GameComponent
{
    public float MoveSpeed { get; set; } = 5f;

    public bool TryMove(GameRoom room, Shared.Models.Vec2Int target)
    {
        if (room.Map.IsBlocked(target.X, target.Y))
            return false;

        Owner.Position = target;
        return true;
    }
}
