namespace ProjectWindServer.Game.Component;

/// <summary>
/// 전투 관련 컴포넌트
/// Player, Enemy 모두 사용
/// </summary>
public class CombatComponent : GameComponent
{
    public int Hp { get; set; } = 100;
    public int MaxHp { get; set; } = 100;
    public int Attack { get; set; } = 10;
    public int Defense { get; set; } = 5;
    public bool IsAlive => Hp > 0;

    public void TakeDamage(int damage)
    {
        var actual = Math.Max(0, damage - Defense);
        Hp = Math.Max(0, Hp - actual);
    }
}
