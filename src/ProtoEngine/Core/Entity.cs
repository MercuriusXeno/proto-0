namespace ProtoEngine.Core;

public class Entity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Tag { get; set; } = string.Empty;

    private readonly Dictionary<Type, IComponent> _components = new();

    public T Add<T>(T component) where T : IComponent
    {
        _components[typeof(T)] = component;
        return component;
    }

    public T? Get<T>() where T : class, IComponent
        => _components.TryGetValue(typeof(T), out var c) ? (T)c : null;

    public bool Has<T>() where T : IComponent
        => _components.ContainsKey(typeof(T));

    public void Remove<T>() where T : IComponent
        => _components.Remove(typeof(T));

    public IEnumerable<IComponent> AllComponents => _components.Values;
}
