using ProjectWindServer.Game.Component;

namespace ProjectWindServer.Game;

/// <summary>
/// 적 엔티티
/// Combat, Movement, Ai 컴포넌트를 기본 장착
/// </summary>
public class Enemy : Entity
{
    public string EnemyType { get; set; } = "";

    public Enemy()
    {
        AddComponent<CombatComponent>();
        AddComponent<MovementComponent>();
        AddComponent<AiComponent>();
    }
}
