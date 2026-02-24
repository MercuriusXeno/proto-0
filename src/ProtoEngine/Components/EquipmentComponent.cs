using ProtoEngine.Core;

namespace ProtoEngine.Components;

/// <summary>
/// Simplified equipment slots
/// </summary>
public enum EquipmentSlot
{
    Head,
    Body,
    Arms,
    Belt,
    Legs,
    Feet,
    WieldLeft,
    WieldRight,
    Accessory1,
    Accessory2,
    Accessory3
}

/// <summary>
/// Represents a single equipped item
/// </summary>
public class EquippedItem
{
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
}

/// <summary>
/// Stores all equipped items on the player
/// </summary>
public class EquipmentComponent : IComponent
{
    public Dictionary<EquipmentSlot, EquippedItem?> Equipment { get; set; } = new();

    public EquipmentComponent()
    {
        // Initialize all slots as empty
        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
        {
            Equipment[slot] = null;
        }
    }

    /// <summary>
    /// Get item equipped in a specific slot
    /// </summary>
    public EquippedItem? GetSlotItem(EquipmentSlot slot)
    {
        return Equipment.ContainsKey(slot) ? Equipment[slot] : null;
    }

    /// <summary>
    /// Check if a slot has any item equipped
    /// </summary>
    public bool IsSlotEmpty(EquipmentSlot slot)
    {
        return !Equipment.ContainsKey(slot) || Equipment[slot] == null;
    }

    /// <summary>
    /// Add an item to a slot
    /// </summary>
    public void EquipItem(EquipmentSlot slot, string itemId, string itemName)
    {
        Equipment[slot] = new EquippedItem
        {
            ItemId = itemId,
            ItemName = itemName
        };
    }

    /// <summary>
    /// Remove an item from a slot
    /// </summary>
    public bool UnequipItem(EquipmentSlot slot)
    {
        if (!Equipment.ContainsKey(slot) || Equipment[slot] == null)
            return false;

        Equipment[slot] = null;
        return true;
    }
}
