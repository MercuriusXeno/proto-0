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
}
