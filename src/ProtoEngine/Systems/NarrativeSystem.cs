using ProtoEngine.Core;
using ProtoEngine.Events;

namespace ProtoEngine.Systems;

public class NarrativeSystem : IGameSystem
{
    private readonly IEventBus _eventBus;

    public NarrativeSystem(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void Initialize(GameState state) { }

    public string GetTimeDescription(GameClock clock)
    {
        return clock.TimeOfDay switch
        {
            "dawn" => "The first light of dawn creeps across the sky.",
            "morning" => "The morning sun casts long shadows.",
            "midday" => "The sun hangs high overhead.",
            "afternoon" => "The afternoon light warms the air.",
            "evening" => "Shadows lengthen as evening approaches.",
            "night" => "Darkness blankets the land. Stars glitter above.",
            _ => ""
        };
    }
}
