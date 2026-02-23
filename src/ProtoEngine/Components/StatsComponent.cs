using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class StatsComponent : IComponent
{
    public int Level { get; set; } = 1;
    public int Experience { get; set; }
    public int ExperienceToNextLevel => Level * 100;
    public int Strength { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Gold { get; set; }
}
