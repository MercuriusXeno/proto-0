using ProtoEngine.Core;

namespace ProtoEngine.Components;

public class ExerciseComponent : IComponent
{
    public Dictionary<StatType, double> Progress { get; set; } = new();

    public double GetProgress(StatType stat)
        => Progress.TryGetValue(stat, out var value) ? value : 0.0;

    public void AddProgress(StatType stat, double amount)
    {
        if (!Progress.ContainsKey(stat))
            Progress[stat] = 0.0;
        Progress[stat] += amount;
    }
}
