using ProtoEngine.Components;

namespace ProtoEngine.Events;

public record RoomEnteredEvent(string EntityId, string RoomId) : IGameEvent;
public record EntityDiedEvent(string EntityId, string KillerId) : IGameEvent;
public record ItemPickedUpEvent(string EntityId, string ItemId) : IGameEvent;
public record ItemDroppedEvent(string EntityId, string ItemId) : IGameEvent;
public record ItemUsedEvent(string EntityId, string ItemId) : IGameEvent;
public record CombatStartedEvent(string AttackerId, string DefenderId) : IGameEvent;
public record CombatEndedEvent(string WinnerId, string LoserId) : IGameEvent;
public record QuestStartedEvent(string QuestId) : IGameEvent;
public record QuestCompletedEvent(string QuestId) : IGameEvent;
public record ObjectiveCompletedEvent(string QuestId, string ObjectiveId) : IGameEvent;
public record DialogueStartedEvent(string NpcId) : IGameEvent;
public record ItemCraftedEvent(string RecipeId) : IGameEvent;
public record LevelUpEvent(string EntityId, int NewLevel) : IGameEvent;
public record StatExercisedEvent(string EntityId, StatType Stat, double Amount) : IGameEvent;
public record StatIncreasedEvent(string EntityId, StatType Stat, int OldValue, int NewValue) : IGameEvent;
