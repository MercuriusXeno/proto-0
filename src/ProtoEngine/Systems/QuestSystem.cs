using ProtoEngine.Core;
using ProtoEngine.Data;
using ProtoEngine.Events;

namespace ProtoEngine.Systems;

public class ActiveQuest
{
    public string QuestId { get; set; } = string.Empty;
    public Dictionary<string, int> Progress { get; set; } = new(); // objectiveId → current count
    public bool IsComplete { get; set; }
}

public class QuestSystem : IGameSystem
{
    private readonly ContentManifest _content;
    private readonly IEventBus _eventBus;
    private readonly List<ActiveQuest> _activeQuests = new();
    private readonly List<string> _completedQuestIds = new();

    public IReadOnlyList<ActiveQuest> ActiveQuests => _activeQuests;
    public IReadOnlyList<string> CompletedQuestIds => _completedQuestIds;

    public QuestSystem(ContentManifest content, IEventBus eventBus)
    {
        _content = content;
        _eventBus = eventBus;
    }

    public void Initialize(GameState state)
    {
        _eventBus.Subscribe<EntityDiedEvent>(e => UpdateObjectives("kill", e.EntityId));
        _eventBus.Subscribe<ItemPickedUpEvent>(e => UpdateObjectives("collect", e.ItemId));
        _eventBus.Subscribe<RoomEnteredEvent>(e => UpdateObjectives("visit", e.RoomId));
        _eventBus.Subscribe<DialogueStartedEvent>(e => UpdateObjectives("talk", e.NpcId));
    }

    public bool TryStartQuest(string questId, out string message)
    {
        message = string.Empty;
        if (_activeQuests.Any(q => q.QuestId == questId))
        {
            message = "You already have this quest.";
            return false;
        }
        if (_completedQuestIds.Contains(questId))
        {
            message = "You've already completed this quest.";
            return false;
        }

        var quest = _content.Quests.FirstOrDefault(q => q.Id == questId);
        if (quest is null)
        {
            message = "Quest not found.";
            return false;
        }

        var active = new ActiveQuest { QuestId = questId };
        foreach (var obj in quest.Objectives)
            active.Progress[obj.Id] = 0;
        _activeQuests.Add(active);
        _eventBus.Publish(new QuestStartedEvent(questId));
        message = $"Quest started: {quest.Name}";
        return true;
    }

    private void UpdateObjectives(string type, string targetId)
    {
        foreach (var active in _activeQuests.Where(q => !q.IsComplete))
        {
            var questData = _content.Quests.FirstOrDefault(q => q.Id == active.QuestId);
            if (questData is null) continue;

            foreach (var obj in questData.Objectives.Where(o => o.Type == type && o.TargetId == targetId))
            {
                if (active.Progress.ContainsKey(obj.Id) && active.Progress[obj.Id] < obj.RequiredCount)
                {
                    active.Progress[obj.Id]++;
                    if (active.Progress[obj.Id] >= obj.RequiredCount)
                        _eventBus.Publish(new ObjectiveCompletedEvent(active.QuestId, obj.Id));
                }
            }

            if (questData.Objectives.All(o => active.Progress.GetValueOrDefault(o.Id) >= o.RequiredCount))
            {
                active.IsComplete = true;
                _completedQuestIds.Add(active.QuestId);
                _activeQuests.Remove(active);
                _eventBus.Publish(new QuestCompletedEvent(active.QuestId));
                break;
            }
        }
    }

    public QuestData? GetQuestData(string questId)
        => _content.Quests.FirstOrDefault(q => q.Id == questId);

    public List<(QuestData quest, ActiveQuest active)> GetActiveQuestDetails()
    {
        return _activeQuests
            .Select(a => (quest: GetQuestData(a.QuestId)!, active: a))
            .Where(x => x.quest is not null)
            .ToList();
    }
}
