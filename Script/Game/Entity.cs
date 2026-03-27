using Shared.Models;
using ProjectWindServer.Game.Component;

namespace ProjectWindServer.Game;

/// <summary>
/// 게임 월드에 존재하는 모든 오브젝트의 부모 클래스
/// 컴포넌트를 부착하여 기능을 확장
/// </summary>
public abstract class Entity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string MapId { get; set; } = "town_01";
    public Vec2Int Position { get; set; }

    private readonly Dictionary<Type, GameComponent> _components = new();

    public T AddComponent<T>() where T : GameComponent, new()
    {
        var component = new T();
        component.SetOwner(this);
        _components[typeof(T)] = component;
        component.OnAttach();
        return component;
    }

    public T? GetComponent<T>() where T : GameComponent
    {
        return _components.TryGetValue(typeof(T), out var component) ? (T)component : null;
    }

    public bool HasComponent<T>() where T : GameComponent
    {
        return _components.ContainsKey(typeof(T));
    }

    public void RemoveComponent<T>() where T : GameComponent
    {
        if (_components.Remove(typeof(T), out var component))
        {
            component.OnDetach();
        }
    }

    public void UpdateComponents(float deltaTime)
    {
        foreach (var component in _components.Values)
        {
            component.Update(deltaTime);
        }
    }
}
