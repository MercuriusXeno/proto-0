using ProtoEngine.Commands;
using ProtoEngine.Commands.Commands;
using ProtoEngine.Components;
using ProtoEngine.Core;
using ProtoEngine.Data;

namespace ProtoEngine.Tests.Commands;

public class LookCommandTests
{
    [Fact]
    public void BuildItemLines_EmptyList_ReturnsEmpty()
    {
        var result = LookCommand.BuildItemLines(new List<ItemData>());

        Assert.Empty(result);
    }

    [Fact]
    public void BuildItemLines_SingleItem_ReturnsBlankLineAndItemLine()
    {
        var items = new List<ItemData>
        {
            new() { Id = "sword", Name = "rusty sword" }
        };

        var result = LookCommand.BuildItemLines(items);

        Assert.Equal(2, result.Count);
        Assert.Equal("", result[0].Text);
        Assert.Contains("rusty sword", result[1].Text);
    }

    [Fact]
    public void BuildItemLines_ItemEntity_HasTakeAndExamineActions()
    {
        var items = new List<ItemData>
        {
            new() { Id = "sword", Name = "rusty sword" }
        };

        var result = LookCommand.BuildItemLines(items);

        var entity = Assert.Single(result[1].Entities);
        Assert.Equal("sword", entity.EntityId);
        Assert.Equal(EntityType.Item, entity.Type);
        Assert.Contains(EntityAction.Take, entity.AvailableActions);
        Assert.Contains(EntityAction.Examine, entity.AvailableActions);
    }

    [Fact]
    public void BuildItemLines_MultipleItems_ReturnsLinePerItem()
    {
        var items = new List<ItemData>
        {
            new() { Id = "sword", Name = "rusty sword" },
            new() { Id = "shield", Name = "wooden shield" }
        };

        var result = LookCommand.BuildItemLines(items);

        Assert.Equal(3, result.Count);
        Assert.Contains("rusty sword", result[1].Text);
        Assert.Contains("wooden shield", result[2].Text);
    }

    [Theory]
    [InlineData("apple", "an ")]
    [InlineData("emerald", "an ")]
    [InlineData("rusty sword", "a ")]
    [InlineData("Orb", "an ")]
    [InlineData("Shield", "a ")]
    public void GetArticle_ReturnsCorrectArticle(string word, string expected)
    {
        Assert.Equal(expected, LookCommand.GetArticle(word));
    }

    [Fact]
    public void GetArticle_EmptyString_ReturnsEmpty()
    {
        Assert.Equal("", LookCommand.GetArticle(""));
    }

    [Fact]
    public void BuildNpcLines_NoNpcs_ReturnsEmpty()
    {
        var result = LookCommand.BuildNpcLines(new List<Entity>(), new List<Entity>());

        Assert.Empty(result);
    }

    [Fact]
    public void BuildNpcLines_AliveNpc_ShowsTalkAttackExamineActions()
    {
        var npc = CreateNpc("guard_1", "Aldric", "guard", isAlive: true);

        var result = LookCommand.BuildNpcLines(new List<Entity> { npc }, new List<Entity>());

        Assert.Equal(2, result.Count);
        var entity = Assert.Single(result[1].Entities);
        Assert.Equal(EntityType.Npc, entity.Type);
        Assert.Contains(EntityAction.Talk, entity.AvailableActions);
        Assert.Contains(EntityAction.Attack, entity.AvailableActions);
        Assert.Contains(EntityAction.Examine, entity.AvailableActions);
    }

    [Fact]
    public void BuildNpcLines_DeadNpc_ShowsExamineOnly()
    {
        var npc = CreateNpc("goblin_1", "Grik", "thief", isAlive: false);

        var result = LookCommand.BuildNpcLines(new List<Entity>(), new List<Entity> { npc });

        var entity = Assert.Single(result[1].Entities);
        Assert.Equal(new List<EntityAction> { EntityAction.Examine }, entity.AvailableActions);
    }

    [Fact]
    public void BuildNpcLines_DeadNpc_TextContainsCorpse()
    {
        var npc = CreateNpc("goblin_1", "Grik", "thief", isAlive: false);

        var result = LookCommand.BuildNpcLines(new List<Entity>(), new List<Entity> { npc });

        Assert.Contains("corpse", result[1].Text);
    }

    [Fact]
    public void BuildNpcLines_NpcWithTitle_IncludesTitleInText()
    {
        var npc = CreateNpc("guard_1", "Aldric", "guard", isAlive: true);

        var result = LookCommand.BuildNpcLines(new List<Entity> { npc }, new List<Entity>());

        Assert.Contains("the guard", result[1].Text);
    }

    [Fact]
    public void BuildNpcLines_NpcWithoutTitle_OmitsTitle()
    {
        var npc = CreateNpc("stranger_1", "Mysterious Figure", "", isAlive: true);

        var result = LookCommand.BuildNpcLines(new List<Entity> { npc }, new List<Entity>());

        Assert.DoesNotContain("the ,", result[1].Text);
    }

    [Fact]
    public void BuildExitLines_NoExits_ReturnsEmpty()
    {
        var result = LookCommand.BuildExitLines(
            new Dictionary<string, string>(), null, null);

        Assert.Empty(result);
    }

    [Fact]
    public void BuildExitLines_UnexploredExit_GroupedUnderUnexplored()
    {
        var exits = new Dictionary<string, string> { ["north"] = "tavern" };

        var result = LookCommand.BuildExitLines(exits, null, null);

        var exitLine = result.First(l => l.Entities.Count > 0);
        Assert.Contains("Unexplored", exitLine.Text);
    }

    [Fact]
    public void BuildExitLines_ExploredExit_GroupedUnderExplored()
    {
        var exits = new Dictionary<string, string> { ["north"] = "tavern" };
        var explored = new ExploredRoomsComponent();
        explored.VisitedRoomIds.Add("tavern");

        var result = LookCommand.BuildExitLines(exits, null, explored);

        var exitLine = result.First(l => l.Entities.Count > 0);
        Assert.Contains("Explored", exitLine.Text);
    }

    [Fact]
    public void BuildExitLines_MixedExits_SplitsIntoTwoGroups()
    {
        var exits = new Dictionary<string, string>
        {
            ["north"] = "tavern",
            ["south"] = "forest"
        };
        var explored = new ExploredRoomsComponent();
        explored.VisitedRoomIds.Add("tavern");

        var result = LookCommand.BuildExitLines(exits, null, explored);

        var entityLines = result.Where(l => l.Entities.Count > 0).ToList();
        Assert.Equal(2, entityLines.Count);
        Assert.Contains("Explored", entityLines[0].Text);
        Assert.Contains("Unexplored", entityLines[1].Text);
    }

    [Fact]
    public void BuildExitLines_WithPreview_IncludesTooltipText()
    {
        var exits = new Dictionary<string, string> { ["north"] = "tavern" };
        var previews = new Dictionary<string, string>
        {
            ["north"] = "Warm light spills from the tavern windows."
        };

        var result = LookCommand.BuildExitLines(exits, previews, null);

        var entity = result.SelectMany(l => l.Entities).First();
        Assert.Equal("Warm light spills from the tavern windows.", entity.TooltipText);
    }

    [Fact]
    public void BuildExitLines_WithoutPreview_TooltipIsNull()
    {
        var exits = new Dictionary<string, string> { ["north"] = "tavern" };

        var result = LookCommand.BuildExitLines(exits, null, null);

        var entity = result.SelectMany(l => l.Entities).First();
        Assert.Null(entity.TooltipText);
    }

    [Fact]
    public void BuildExitLines_ExitEntity_HasMoveAction()
    {
        var exits = new Dictionary<string, string> { ["north"] = "tavern" };

        var result = LookCommand.BuildExitLines(exits, null, null);

        var entity = result.SelectMany(l => l.Entities).First();
        Assert.Equal(EntityType.Exit, entity.Type);
        Assert.Contains(EntityAction.Move, entity.AvailableActions);
    }

    private static Entity CreateNpc(string id, string name, string title, bool isAlive)
    {
        var npc = new Entity { Id = id, Tag = "npc" };
        npc.Add(new DescriptionComponent { Name = name });
        npc.Add(new NpcComponent { Title = title });
        npc.Add(new HealthComponent { Current = isAlive ? 100 : 0, Max = 100 });
        return npc;
    }
}
