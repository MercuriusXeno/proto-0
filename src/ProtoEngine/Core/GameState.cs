namespace ProtoEngine.Core;

public class GameState
{
    public GameClock Clock { get; } = new();
    public Entity Player { get; set; } = new() { Tag = "player" };

    private readonly Dictionary<string, Entity> _entities = new();

    public IReadOnlyDictionary<string, Entity> Entities => _entities;

    public void AddEntity(Entity entity) => _entities[entity.Id] = entity;

    public void RemoveEntity(string id) => _entities.Remove(id);

    public Entity? GetEntity(string id)
        => _entities.TryGetValue(id, out var e) ? e : null;

    public IEnumerable<Entity> EntitiesWith<T>() where T : IComponent
        => _entities.Values.Where(e => e.Has<T>());

    public bool IsInitialized { get; set; }
}
