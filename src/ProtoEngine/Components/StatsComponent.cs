using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class StatsComponent : IComponent
{
    public int Level { get; set; } = 1;
    public int Experience { get; set; }
    public int ExperienceToNextLevel => Level * 100;
    public int Gold { get; set; }

    // Simplified 8 Core Attributes
    public int Strength { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Fortitude { get; set; } = 10;
    public int Agility { get; set; } = 10;
    public int Willpower { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Perception { get; set; } = 10;
    public int Charisma { get; set; } = 10;

    public int GetStat(StatType stat) => stat switch
    {
        StatType.Strength => Strength,
        StatType.Dexterity => Dexterity,
        StatType.Fortitude => Fortitude,
        StatType.Agility => Agility,
        StatType.Willpower => Willpower,
        StatType.Intelligence => Intelligence,
        StatType.Perception => Perception,
        StatType.Charisma => Charisma,
        _ => throw new ArgumentOutOfRangeException(nameof(stat))
    };

    public void SetStat(StatType stat, int value)
    {
        switch (stat)
        {
            case StatType.Strength: Strength = value; break;
            case StatType.Dexterity: Dexterity = value; break;
            case StatType.Fortitude: Fortitude = value; break;
            case StatType.Agility: Agility = value; break;
            case StatType.Willpower: Willpower = value; break;
            case StatType.Intelligence: Intelligence = value; break;
            case StatType.Perception: Perception = value; break;
            case StatType.Charisma: Charisma = value; break;
            default: throw new ArgumentOutOfRangeException(nameof(stat));
        }
    }
}
