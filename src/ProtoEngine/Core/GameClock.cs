namespace ProtoEngine.Core;

public class GameClock
{
    public int Tick { get; private set; }
    public int Hour => (Tick % 24);
    public int Day => (Tick / 24) + 1;

    public string TimeOfDay => Hour switch
    {
        >= 5 and < 8 => "dawn",
        >= 8 and < 12 => "morning",
        >= 12 and < 14 => "midday",
        >= 14 and < 18 => "afternoon",
        >= 18 and < 21 => "evening",
        _ => "night"
    };

    public void Advance(int ticks = 1) => Tick += ticks;

    public void Set(int tick) => Tick = tick;
}
