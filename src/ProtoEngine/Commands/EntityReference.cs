namespace ProtoEngine.Commands;

public enum EntityType { Item, Npc, Exit, Room }

public enum EntityAction
{
    // Items
    Take, Examine, Drop, Use, Equip,
    // NPCs
    Talk, Attack,
    // Exits
    Move
}

public record EntityReference(
    string EntityId,
    string DisplayName,
    EntityType Type,
    List<EntityAction> AvailableActions,
    string? TooltipText = null
);
