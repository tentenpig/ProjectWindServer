using ProjectWindServer.Game.Component;

namespace ProjectWindServer.Game;

/// <summary>
/// 접속한 플레이어 한 명의 세션 정보
/// </summary>
public class Player : Entity
{
    public int AccountId { get; set; }

    /// <summary>
    /// 마지막 체크포인트 이후 변경이 있었는지
    /// </summary>
    public bool IsDirty { get; set; }

    public void MarkDirty() => IsDirty = true;
    public void ClearDirty() => IsDirty = false;

    public Player()
    {
        AddComponent<CombatComponent>();
        AddComponent<MovementComponent>();
    }
}
