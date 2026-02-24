using ProtoEngine.Components;
using ProtoEngine.Core;

namespace ProtoEngine.Tests.Core;

public class EntityTests
{
    [Fact]
    public void Add_Component_CanBeRetrieved()
    {
        var entity = new Entity();

        entity.Add(new StatsComponent { Strength = 15 });

        Assert.Equal(15, entity.Get<StatsComponent>()!.Strength);
    }

    [Fact]
    public void Get_MissingComponent_ReturnsNull()
    {
        var entity = new Entity();

        Assert.Null(entity.Get<StatsComponent>());
    }

    [Fact]
    public void Has_WithComponent_ReturnsTrue()
    {
        var entity = new Entity();
        entity.Add(new StatsComponent());

        Assert.True(entity.Has<StatsComponent>());
    }

    [Fact]
    public void Has_WithoutComponent_ReturnsFalse()
    {
        var entity = new Entity();

        Assert.False(entity.Has<StatsComponent>());
    }

    [Fact]
    public void Remove_Component_NoLongerAvailable()
    {
        var entity = new Entity();
        entity.Add(new StatsComponent());

        entity.Remove<StatsComponent>();

        Assert.False(entity.Has<StatsComponent>());
        Assert.Null(entity.Get<StatsComponent>());
    }

    [Fact]
    public void Add_SameComponentType_OverwritesPrevious()
    {
        var entity = new Entity();
        entity.Add(new StatsComponent { Strength = 5 });
        entity.Add(new StatsComponent { Strength = 20 });

        Assert.Equal(20, entity.Get<StatsComponent>()!.Strength);
    }

    [Fact]
    public void AllComponents_ReturnsAllAdded()
    {
        var entity = new Entity();
        entity.Add(new StatsComponent());
        entity.Add(new ExerciseComponent());

        var components = entity.AllComponents.ToList();

        Assert.Equal(2, components.Count);
    }

    [Fact]
    public void NewEntity_HasUniqueId()
    {
        var e1 = new Entity();
        var e2 = new Entity();

        Assert.NotEqual(e1.Id, e2.Id);
    }
}
