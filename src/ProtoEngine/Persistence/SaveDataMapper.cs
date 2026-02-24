using ProtoEngine.Components;
using ProtoEngine.Core;
using ProtoEngine.Data;

namespace ProtoEngine.Persistence;

/// <summary>
/// Maps between live GameState and SaveData for persistence.
/// </summary>
public class SaveDataMapper
{
    private readonly List<ItemData> _items;

    public SaveDataMapper(List<ItemData> items)
    {
        _items = items;
    }

    /// <summary>
    /// Creates a SaveData snapshot from the current game state.
    /// </summary>
    public SaveData CreateSaveData(GameState state)
    {
        var pos = state.Player.Get<PositionComponent>();
        var health = state.Player.Get<HealthComponent>();
        var stats = state.Player.Get<StatsComponent>();
        var inv = state.Player.Get<InventoryComponent>();
        var equip = state.Player.Get<EquipmentComponent>();
        var exercise = state.Player.Get<ExerciseComponent>();

        return new SaveData
        {
            ClockTick = state.Clock.Tick,
            PlayerRoomId = pos?.RoomId ?? "town_square",
            PlayerHealth = health?.Current ?? 100,
            PlayerMaxHealth = health?.Max ?? 100,
            PlayerLevel = stats?.Level ?? 1,
            PlayerExperience = stats?.Experience ?? 0,
            PlayerStrength = stats?.Strength ?? 10,
            PlayerDexterity = stats?.Dexterity ?? 10,
            PlayerIntelligence = stats?.Intelligence ?? 10,
            PlayerFortitude = stats?.Fortitude ?? 10,
            PlayerAgility = stats?.Agility ?? 10,
            PlayerWillpower = stats?.Willpower ?? 10,
            PlayerPerception = stats?.Perception ?? 10,
            PlayerCharisma = stats?.Charisma ?? 10,
            PlayerGold = stats?.Gold ?? 0,
            ExerciseProgress = exercise?.Progress != null
                ? new Dictionary<StatType, double>(exercise.Progress)
                : new(),
            InventoryItemIds = inv?.ItemIds.ToList() ?? new(),
            WeaponId = equip?.GetSlotItem(EquipmentSlot.WieldRight)?.ItemId,
            ArmorId = equip?.GetSlotItem(EquipmentSlot.Body)?.ItemId,
            SavedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Restores game state from a SaveData snapshot.
    /// </summary>
    public void RestoreFromSave(GameState state, SaveData save)
    {
        state.Clock.Set(save.ClockTick);

        RestorePosition(state, save);
        RestoreHealth(state, save);
        RestoreStats(state, save);
        RestoreExercise(state, save);
        RestoreInventory(state, save);
        RestoreEquipment(state, save);
    }

    private static void RestorePosition(GameState state, SaveData save)
    {
        var pos = state.Player.Get<PositionComponent>();
        if (pos is not null) pos.RoomId = save.PlayerRoomId;
    }

    private static void RestoreHealth(GameState state, SaveData save)
    {
        var health = state.Player.Get<HealthComponent>();
        if (health is null) return;
        health.Current = save.PlayerHealth;
        health.Max = save.PlayerMaxHealth;
    }

    private static void RestoreStats(GameState state, SaveData save)
    {
        var stats = state.Player.Get<StatsComponent>();
        if (stats is null) return;
        stats.Level = save.PlayerLevel;
        stats.Experience = save.PlayerExperience;
        stats.Strength = save.PlayerStrength;
        stats.Dexterity = save.PlayerDexterity;
        stats.Intelligence = save.PlayerIntelligence;
        stats.Fortitude = save.PlayerFortitude;
        stats.Agility = save.PlayerAgility;
        stats.Willpower = save.PlayerWillpower;
        stats.Perception = save.PlayerPerception;
        stats.Charisma = save.PlayerCharisma;
        stats.Gold = save.PlayerGold;
    }

    private static void RestoreExercise(GameState state, SaveData save)
    {
        var exercise = state.Player.Get<ExerciseComponent>();
        if (exercise is not null && save.ExerciseProgress.Count > 0)
            exercise.Progress = new Dictionary<StatType, double>(save.ExerciseProgress);
    }

    private static void RestoreInventory(GameState state, SaveData save)
    {
        var inv = state.Player.Get<InventoryComponent>();
        if (inv is not null)
            inv.ItemIds = save.InventoryItemIds.ToList();
    }

    private void RestoreEquipment(GameState state, SaveData save)
    {
        var equip = state.Player.Get<EquipmentComponent>();
        if (equip is null) return;

        RestoreSlot(equip, EquipmentSlot.WieldRight, save.WeaponId);
        RestoreSlot(equip, EquipmentSlot.Body, save.ArmorId);
    }

    private void RestoreSlot(EquipmentComponent equip, EquipmentSlot slot, string? itemId)
    {
        if (itemId is null) return;
        var itemData = _items.FirstOrDefault(i => i.Id == itemId);
        if (itemData is not null)
            equip.EquipItem(slot, itemId, itemData.Name);
    }
}
