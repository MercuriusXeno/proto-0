using ProtoEngine.Core;

namespace ProtoEngine.Components;

/// <summary>
/// Equipment slots on the player's body
/// </summary>
public enum EquipmentSlot
{
    // Head region
    Head,
    Face,
    Neck,

    // Torso region
    Body,
    Waist,

    // Arm region (with laterality)
    LeftArm,
    RightArm,
    LeftHand,
    RightHand,

    // Leg region (no laterality)
    Legs,
    Feet,

    // Accessory slots
    Relic1,
    Relic2,
    Relic3,
    Relic4,

    // Wielding slots (weapons, shields, etc.)
    WieldLeft,
    WieldRight
}

/// <summary>
/// Represents a single equipped item with layer support
/// </summary>
public class EquippedItem
{
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int Layer { get; set; } = 0; // 0 = innermost, higher = outer layers
}

/// <summary>
/// Stores all equipped items on the player
/// Supports multiple items per slot via layering
/// </summary>
public class EquipmentComponent : IComponent
{
    // Each slot can have multiple items (layers)
    public Dictionary<EquipmentSlot, List<EquippedItem>> Equipment { get; set; } = new();

    public EquipmentComponent()
    {
        // Initialize all slots with empty lists
        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
        {
            Equipment[slot] = new List<EquippedItem>();
        }
    }

    /// <summary>
    /// Get all items equipped in a specific slot, ordered by layer
    /// </summary>
    public List<EquippedItem> GetSlotItems(EquipmentSlot slot)
    {
        return Equipment.ContainsKey(slot)
            ? Equipment[slot].OrderBy(e => e.Layer).ToList()
            : new List<EquippedItem>();
    }

    /// <summary>
    /// Check if a slot has any items equipped
    /// </summary>
    public bool IsSlotEmpty(EquipmentSlot slot)
    {
        return !Equipment.ContainsKey(slot) || Equipment[slot].Count == 0;
    }

    /// <summary>
    /// Add an item to a slot at a specific layer
    /// </summary>
    public void EquipItem(EquipmentSlot slot, string itemId, string itemName, int layer = 0)
    {
        if (!Equipment.ContainsKey(slot))
            Equipment[slot] = new List<EquippedItem>();

        Equipment[slot].Add(new EquippedItem
        {
            ItemId = itemId,
            ItemName = itemName,
            Layer = layer
        });
    }

    /// <summary>
    /// Remove an item from a slot
    /// </summary>
    public bool UnequipItem(EquipmentSlot slot, string itemId)
    {
        if (!Equipment.ContainsKey(slot))
            return false;

        var item = Equipment[slot].FirstOrDefault(e => e.ItemId == itemId);
        if (item != null)
        {
            Equipment[slot].Remove(item);
            return true;
        }

        return false;
    }
}

