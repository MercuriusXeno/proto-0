using ProtoEngine.Components;
using ProtoEngine.Core;
using ProtoEngine.Data;
using ProtoEngine.Events;

namespace ProtoEngine.Systems;

public class NpcSystem : IGameSystem
{
    private readonly ContentManifest _content;
    private readonly IEventBus _eventBus;

    public NpcSystem(ContentManifest content, IEventBus eventBus)
    {
        _content = content;
        _eventBus = eventBus;
    }

    public void Initialize(GameState state)
    {
        foreach (var npcData in _content.Npcs)
        {
            var npc = new Entity { Id = npcData.Id, Tag = "npc" };
            npc.Add(new DescriptionComponent
            {
                Name = npcData.Name,
                ShortDescription = npcData.Description,
                LongDescription = npcData.Description
            });
            npc.Add(new HealthComponent { Current = npcData.Health, Max = npcData.Health });
            npc.Add(new CombatComponent
            {
                AttackPower = npcData.AttackPower,
                Defense = npcData.Defense
            });
            npc.Add(new NpcComponent
            {
                Title = npcData.Title,
                DialogueId = npcData.DialogueId,
                Behavior = npcData.Behavior,
                IsHostile = npcData.IsHostile,
                Disposition = npcData.Disposition
            });
            state.AddEntity(npc);
        }

        // Place NPCs in rooms
        foreach (var roomData in _content.Rooms)
        {
            foreach (var npcId in roomData.NpcIds)
            {
                var npc = state.GetEntity(npcId);
                if (npc is not null)
                {
                    var pos = npc.Get<PositionComponent>() ?? npc.Add(new PositionComponent());
                    pos.RoomId = roomData.Id;
                }
            }
        }
    }

    public DialogueData? GetDialogue(string dialogueId)
        => _content.Dialogues.FirstOrDefault(d => d.Id == dialogueId);

    public NpcData? GetNpcData(string npcId)
        => _content.Npcs.FirstOrDefault(n => n.Id == npcId);

    public Entity? FindNpcInRoom(GameState state, string name)
    {
        var playerPos = state.Player.Get<PositionComponent>();
        if (playerPos is null) return null;

        // Special case: "someone" means any NPC we haven't met yet (or just the first one)
        if (name.Equals("someone", StringComparison.OrdinalIgnoreCase))
        {
            return state.Entities.Values.FirstOrDefault(e =>
                e.Tag == "npc" &&
                e.Get<PositionComponent>()?.RoomId == playerPos.RoomId &&
                e.Get<HealthComponent>()?.IsAlive == true);
        }

        // Try to find by name first, then by title (for NPCs you haven't met yet)
        return state.Entities.Values.FirstOrDefault(e =>
            e.Tag == "npc" &&
            e.Get<PositionComponent>()?.RoomId == playerPos.RoomId &&
            e.Get<HealthComponent>()?.IsAlive == true &&
            ((e.Get<DescriptionComponent>()?.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false) ||
             (e.Get<NpcComponent>()?.Title.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false)));
    }

    public List<string> Talk(GameState state, Entity npc)
    {
        var lines = new List<string>();
        var npcComp = npc.Get<NpcComponent>();
        var desc = npc.Get<DescriptionComponent>();
        if (npcComp is null || desc is null) return lines;

        var dialogue = GetDialogue(npcComp.DialogueId);
        if (dialogue is null || dialogue.Nodes.Count == 0)
        {
            lines.Add($"{desc.Name} has nothing to say.");
            return lines;
        }

        _eventBus.Publish(new DialogueStartedEvent(npc.Id));
        var node = dialogue.Nodes[0];
        lines.Add($"{desc.Name} says: \"{node.Text}\"");

        if (node.Choices.Count > 0)
        {
            lines.Add("");
            for (int i = 0; i < node.Choices.Count; i++)
                lines.Add($"  [{i + 1}] {node.Choices[i].Text}");
        }

        return lines;
    }
}
