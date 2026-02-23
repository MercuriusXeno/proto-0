using ProtoEngine.Components;
using ProtoEngine.Core;
using ProtoEngine.Data;
using ProtoEngine.Events;

namespace ProtoEngine.Systems;

public class InventorySystem : IGameSystem
{
    private readonly ContentManifest _content;
    private readonly IEventBus _eventBus;
    private readonly Dictionary<string, Entity> _itemEntities = new();

    public InventorySystem(ContentManifest content, IEventBus eventBus)
    {
        _content = content;
        _eventBus = eventBus;
    }

    public void Initialize(GameState state)
    {
        foreach (var itemData in _content.Items)
        {
            var item = new Entity { Id = itemData.Id, Tag = "item" };
            item.Add(new DescriptionComponent
            {
                Name = itemData.Name,
                ShortDescription = itemData.Description,
                LongDescription = itemData.Description
            });
            _itemEntities[item.Id] = item;
            state.AddEntity(item);
        }

        // Place items in rooms
        foreach (var roomData in _content.Rooms)
        {
            foreach (var itemId in roomData.ItemIds)
            {
                if (_itemEntities.TryGetValue(itemId, out var item))
                {
                    var pos = item.Get<PositionComponent>() ?? item.Add(new PositionComponent());
                    pos.RoomId = roomData.Id;
                }
            }
        }
    }

    public ItemData? GetItemData(string itemId)
        => _content.Items.FirstOrDefault(i => i.Id == itemId);

    public ItemData? FindItemByName(string name)
        => _content.Items.FirstOrDefault(i =>
            i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public bool TryPickUp(GameState state, string itemName, out string message)
    {
        message = string.Empty;
        var playerPos = state.Player.Get<PositionComponent>();
        var inventory = state.Player.Get<InventoryComponent>();
        if (playerPos is null || inventory is null)
        {
            message = "You can't do that.";
            return false;
        }

        if (inventory.IsFull)
        {
            message = "Your inventory is full.";
            return false;
        }

        // Find item in current room by name
        var item = _content.Items.FirstOrDefault(i =>
            i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        if (item is null)
        {
            message = $"There is no '{itemName}' here.";
            return false;
        }

        var entity = _itemEntities.GetValueOrDefault(item.Id);
        if (entity?.Get<PositionComponent>()?.RoomId != playerPos.RoomId)
        {
            message = $"There is no '{itemName}' here.";
            return false;
        }

        if (!item.IsPickable)
        {
            message = "You can't pick that up.";
            return false;
        }

        entity.Remove<PositionComponent>();
        inventory.ItemIds.Add(item.Id);
        _eventBus.Publish(new ItemPickedUpEvent(state.Player.Id, item.Id));
        message = $"You pick up the {item.Name}.";
        return true;
    }

    public bool TryDrop(GameState state, string itemName, out string message)
    {
        message = string.Empty;
        var playerPos = state.Player.Get<PositionComponent>();
        var inventory = state.Player.Get<InventoryComponent>();
        if (playerPos is null || inventory is null)
        {
            message = "You can't do that.";
            return false;
        }

        var item = _content.Items.FirstOrDefault(i =>
            i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase) &&
            inventory.ItemIds.Contains(i.Id));
        if (item is null)
        {
            message = $"You don't have a '{itemName}'.";
            return false;
        }

        inventory.ItemIds.Remove(item.Id);
        var entity = _itemEntities.GetValueOrDefault(item.Id);
        if (entity is not null)
        {
            var pos = entity.Get<PositionComponent>() ?? entity.Add(new PositionComponent());
            pos.RoomId = playerPos.RoomId;
        }
        _eventBus.Publish(new ItemDroppedEvent(state.Player.Id, item.Id));
        message = $"You drop the {item.Name}.";
        return true;
    }

    public List<ItemData> GetInventoryItems(GameState state)
    {
        var inventory = state.Player.Get<InventoryComponent>();
        if (inventory is null) return new();
        return inventory.ItemIds
            .Select(id => GetItemData(id))
            .Where(i => i is not null)
            .Cast<ItemData>()
            .ToList();
    }

    public List<ItemData> GetItemsInRoom(GameState state, string roomId)
    {
        return _itemEntities.Values
            .Where(e => e.Get<PositionComponent>()?.RoomId == roomId)
            .Select(e => GetItemData(e.Id))
            .Where(i => i is not null)
            .Cast<ItemData>()
            .ToList();
    }
}
