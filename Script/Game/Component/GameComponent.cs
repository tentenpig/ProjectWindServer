namespace ProjectWindServer.Game.Component;

/// <summary>
/// 모든 컴포넌트의 부모 클래스
/// Entity에 부착하여 기능을 확장
/// </summary>
public abstract class GameComponent
{
    public Entity Owner { get; private set; } = null!;

    internal void SetOwner(Entity owner) => Owner = owner;

    public virtual void OnAttach() { }
    public virtual void OnDetach() { }
    public virtual void Update(float deltaTime) { }
}
