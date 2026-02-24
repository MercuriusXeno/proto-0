using System.Text.Json;
using ProtoEngine.Components;
using ProtoEngine.Persistence;

namespace ProtoEngine.Tests.Persistence;

public class SaveDataRoundTripTests
{
    [Fact]
    public void SaveData_SerializeDeserialize_PreservesAllFields()
    {
        var original = new SaveData
        {
            ClockTick = 42,
            PlayerRoomId = "dungeon_entrance",
            PlayerHealth = 85,
            PlayerMaxHealth = 100,
            PlayerLevel = 3,
            PlayerExperience = 250,
            PlayerStrength = 14,
            PlayerDexterity = 12,
            PlayerIntelligence = 11,
            PlayerFortitude = 13,
            PlayerAgility = 10,
            PlayerWillpower = 15,
            PlayerPerception = 12,
            PlayerCharisma = 9,
            PlayerGold = 500,
            ExerciseProgress = new Dictionary<StatType, double>
            {
                [StatType.Strength] = 45.5,
                [StatType.Dexterity] = 20.0,
                [StatType.Perception] = 0.5
            },
            InventoryItemIds = new List<string> { "sword_01", "potion_hp_01" },
            WeaponId = "sword_01",
            ArmorId = "leather_armor_01"
        };

        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<SaveData>(json)!;

        Assert.Equal(original.ClockTick, restored.ClockTick);
        Assert.Equal(original.PlayerRoomId, restored.PlayerRoomId);
        Assert.Equal(original.PlayerHealth, restored.PlayerHealth);
        Assert.Equal(original.PlayerMaxHealth, restored.PlayerMaxHealth);
        Assert.Equal(original.PlayerLevel, restored.PlayerLevel);
        Assert.Equal(original.PlayerExperience, restored.PlayerExperience);
        Assert.Equal(original.PlayerStrength, restored.PlayerStrength);
        Assert.Equal(original.PlayerDexterity, restored.PlayerDexterity);
        Assert.Equal(original.PlayerIntelligence, restored.PlayerIntelligence);
        Assert.Equal(original.PlayerFortitude, restored.PlayerFortitude);
        Assert.Equal(original.PlayerAgility, restored.PlayerAgility);
        Assert.Equal(original.PlayerWillpower, restored.PlayerWillpower);
        Assert.Equal(original.PlayerPerception, restored.PlayerPerception);
        Assert.Equal(original.PlayerCharisma, restored.PlayerCharisma);
        Assert.Equal(original.PlayerGold, restored.PlayerGold);
        Assert.Equal(original.WeaponId, restored.WeaponId);
        Assert.Equal(original.ArmorId, restored.ArmorId);
        Assert.Equal(original.InventoryItemIds, restored.InventoryItemIds);
    }

    [Fact]
    public void SaveData_SerializeDeserialize_PreservesExerciseProgress()
    {
        var original = new SaveData
        {
            ExerciseProgress = new Dictionary<StatType, double>
            {
                [StatType.Strength] = 75.3,
                [StatType.Intelligence] = 12.8
            }
        };

        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<SaveData>(json)!;

        Assert.Equal(2, restored.ExerciseProgress.Count);
        Assert.Equal(75.3, restored.ExerciseProgress[StatType.Strength]);
        Assert.Equal(12.8, restored.ExerciseProgress[StatType.Intelligence]);
    }

    [Fact]
    public void SaveData_DefaultValues_SerializeCorrectly()
    {
        var original = new SaveData();

        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<SaveData>(json)!;

        Assert.Equal(0, restored.ClockTick);
        Assert.Empty(restored.PlayerRoomId);
        Assert.Empty(restored.ExerciseProgress);
        Assert.Empty(restored.InventoryItemIds);
        Assert.Null(restored.WeaponId);
        Assert.Null(restored.ArmorId);
    }
}
