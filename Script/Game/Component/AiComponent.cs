namespace ProjectWindServer.Game.Component;

/// <summary>
/// AI 관련 컴포넌트
/// Enemy 전용 — 순찰, 추적, 공격 등의 상태 관리
/// </summary>
public class AiComponent : GameComponent
{
    public AiState State { get; set; } = AiState.Idle;

    public override void Update(float deltaTime)
    {
        // TODO: 상태별 AI 로직 구현
    }
}

public enum AiState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Return
}
