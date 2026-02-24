using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class StatsComponent : IComponent
{
    public int Level { get; set; } = 1;
    public int Experience { get; set; }
    public int ExperienceToNextLevel => Level * 100;
    public int Gold { get; set; }

    // Core Attributes (13 diverse stats that grow through actions)
    public int Strength { get; set; } = 10;
    public int Agility { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Perception { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Willpower { get; set; } = 10;
    public int Vitality { get; set; } = 10;
    public int Charisma { get; set; } = 10;
    public int Luck { get; set; } = 10;
    public int Memory { get; set; } = 10;
    public int Fate { get; set; } = 10;
    public int Eldritch { get; set; } = 10;
    public int Racial { get; set; } = 10;
}
