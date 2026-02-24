using ProtoEngine.Components;
using ProtoEngine.Core;

namespace ProtoEngine.Tests.Core;

public class GameStateTests
{
    [Fact]
    public void AddEntity_CanBeRetrievedById()
    {
        var state = new GameState();
        var entity = new Entity { Tag = "npc" };

        state.AddEntity(entity);

        Assert.Same(entity, state.GetEntity(entity.Id));
    }

    [Fact]
    public void GetEntity_UnknownId_ReturnsNull()
    {
        var state = new GameState();

        Assert.Null(state.GetEntity("nonexistent"));
    }

    [Fact]
    public void RemoveEntity_NoLongerRetrievable()
    {
        var state = new GameState();
        var entity = new Entity();
        state.AddEntity(entity);

        state.RemoveEntity(entity.Id);

        Assert.Null(state.GetEntity(entity.Id));
    }

    [Fact]
    public void EntitiesWith_ReturnsOnlyMatchingEntities()
    {
        var state = new GameState();
        var withStats = new Entity();
        withStats.Add(new StatsComponent());
        var withoutStats = new Entity();

        state.AddEntity(withStats);
        state.AddEntity(withoutStats);

        var result = state.EntitiesWith<StatsComponent>().ToList();

        Assert.Single(result);
        Assert.Same(withStats, result[0]);
    }

    [Fact]
    public void Player_DefaultsToPlayerTag()
    {
        var state = new GameState();

        Assert.Equal("player", state.Player.Tag);
    }
}
