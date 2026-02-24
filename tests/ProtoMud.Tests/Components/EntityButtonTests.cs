using ProtoEngine.Commands;
using ProtoMud.Components;

namespace ProtoMud.Tests.Components;

public class EntityButtonTests : TestContext
{
    private static EntityReference CreateItemRef(string name = "sword", params EntityAction[] actions) =>
        new("item_01", name, EntityType.Item,
            actions.Length > 0 ? actions.ToList() : new List<EntityAction> { EntityAction.Take, EntityAction.Examine });

    private static EntityReference CreateExitRef(string direction = "north") =>
        new("exit_north", direction, EntityType.Exit, new List<EntityAction> { EntityAction.Move });

    private static EntityReference CreateNpcRef(string name = "merchant", params EntityAction[] actions) =>
        new("npc_01", name, EntityType.Npc,
            actions.Length > 0 ? actions.ToList() : new List<EntityAction> { EntityAction.Talk, EntityAction.Attack });

    [Fact]
    public void EntityButton_RendersDisplayName()
    {
        var entity = CreateItemRef("magic sword");

        var cut = RenderComponent<EntityButton>(parameters => parameters
            .Add(p => p.Entity, entity));

        cut.MarkupMatches(@"<span class=""entity-btn entity-item"" title="""">magic sword</span>");
    }

    [Fact]
    public void EntityButton_Exit_RendersWithExitClass()
    {
        var entity = CreateExitRef("north");

        var cut = RenderComponent<EntityButton>(parameters => parameters
            .Add(p => p.Entity, entity));

        var span = cut.Find(".entity-btn");
        Assert.Contains("entity-exit", span.ClassList);
    }

    [Fact]
    public void EntityButton_Npc_RendersWithNpcClass()
    {
        var entity = CreateNpcRef("guard");

        var cut = RenderComponent<EntityButton>(parameters => parameters
            .Add(p => p.Entity, entity));

        var span = cut.Find(".entity-btn");
        Assert.Contains("entity-npc", span.ClassList);
    }

    [Fact]
    public void EntityButton_WithTooltip_SetsTitle()
    {
        var entity = new EntityReference("item_01", "potion", EntityType.Item,
            new List<EntityAction> { EntityAction.Use }, "A healing potion");

        var cut = RenderComponent<EntityButton>(parameters => parameters
            .Add(p => p.Entity, entity));

        var span = cut.Find(".entity-btn");
        Assert.Equal("A healing potion", span.GetAttribute("title"));
    }

    [Fact]
    public void EntityButton_ExitClick_SendsDirectionAsCommand()
    {
        var entity = CreateExitRef("north");
        string? sentCommand = null;

        var cut = RenderComponent<EntityButton>(parameters => parameters
            .Add(p => p.Entity, entity)
            .Add(p => p.OnCommand, EventCallback.Factory.Create<string>(this, cmd => sentCommand = cmd)));

        cut.Find(".entity-btn").Click();

        Assert.Equal("north", sentCommand);
    }

    [Fact]
    public void EntityButton_ItemClick_ShowsContextMenu()
    {
        var entity = CreateItemRef("sword", EntityAction.Take, EntityAction.Examine);

        var cut = RenderComponent<EntityButton>(parameters => parameters
            .Add(p => p.Entity, entity));

        cut.Find(".entity-btn").Click();

        var menu = cut.Find(".entity-menu");
        var buttons = menu.QuerySelectorAll(".entity-action");
        Assert.Equal(2, buttons.Length);
    }

    [Fact]
    public void EntityButton_ContextMenuAction_SendsCorrectCommand()
    {
        var entity = CreateItemRef("sword", EntityAction.Take);
        string? sentCommand = null;

        var cut = RenderComponent<EntityButton>(parameters => parameters
            .Add(p => p.Entity, entity)
            .Add(p => p.OnCommand, EventCallback.Factory.Create<string>(this, cmd => sentCommand = cmd)));

        cut.Find(".entity-btn").Click();
        cut.Find(".entity-action").Click();

        Assert.Equal("take sword", sentCommand);
    }

    [Fact]
    public void EntityButton_NpcTalkAction_SendsTalkCommand()
    {
        var entity = CreateNpcRef("Aldric", EntityAction.Talk);
        string? sentCommand = null;

        var cut = RenderComponent<EntityButton>(parameters => parameters
            .Add(p => p.Entity, entity)
            .Add(p => p.OnCommand, EventCallback.Factory.Create<string>(this, cmd => sentCommand = cmd)));

        cut.Find(".entity-btn").Click();
        cut.Find(".entity-action").Click();

        Assert.Equal("talk Aldric", sentCommand);
    }
}
