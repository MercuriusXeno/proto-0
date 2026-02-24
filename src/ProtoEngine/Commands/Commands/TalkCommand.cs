using ProtoEngine.Components;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class TalkCommand : ICommand
{
    private readonly NpcSystem _npc;
    private readonly WorldSystem _world;
    private readonly MemorySystem _memory;

    public string Verb => "talk";
    public string[] Aliases => ["speak", "chat"];
    public string Description => "Talk to an NPC (talk <npc name> or just 'talk' to talk to someone)";

    public TalkCommand(NpcSystem npc, WorldSystem world, MemorySystem memory)
    {
        _npc = npc;
        _world = world;
        _memory = memory;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        var room = _world.GetPlayerRoom(context.State);
        if (room is null)
            return CommandResult.Fail("You are nowhere.");

        // If no name provided, try to talk to "someone" (the unknown NPC)
        var name = args.Length > 0 ? string.Join(" ", args) : "someone";
        var npc = _npc.FindNpcInRoom(context.State, name);

        if (npc is null)
            return CommandResult.Fail($"There is no '{name}' here to talk to.");

        var desc = npc.Get<DescriptionComponent>();
        var npcComp = npc.Get<NpcComponent>();

        if (desc is null)
            return CommandResult.Fail("This entity cannot speak.");

        // Check if this is a creature without a name (can't talk)
        if (string.IsNullOrEmpty(desc.Name))
            return CommandResult.Fail($"The {npcComp?.Title ?? "creature"} cannot speak.");

        // Record that we've met this NPC (learned their name)
        _memory.RecordNpcMet(context.State, room.Id, npc.Id, desc.Name);

        var lines = _npc.Talk(context.State, npc);

        // Add introduction if this is first time meeting
        var memories = _memory.GetRoomMemories(context.State, room.Id);
        var isFirstMeeting = !memories.Any(m =>
            m.Type == Components.RoomMemoryType.NpcMet &&
            m.EntityId == npc.Id &&
            m.GameTick < context.State.Clock.Tick);

        if (isFirstMeeting && npcComp is not null)
        {
            var introduction = "";

            // Format introduction based on disposition and whether they have a title
            if (npcComp.Disposition.OnlyGivesNameOnIntroduction || string.IsNullOrEmpty(npcComp.Title))
            {
                introduction = $"They introduce themselves as {desc.Name}.";
            }
            else
            {
                introduction = $"They introduce themselves as {desc.Name} the {npcComp.Title}.";
            }

            var introLines = new List<string> { introduction, "" };
            introLines.AddRange(lines);

            // Return with refresh flag since room state changed (we learned NPC's name)
            return new CommandResult
            {
                Success = true,
                Output = introLines,
                RefreshRoomDescription = true
            };
        }

        return CommandResult.Ok(lines.ToArray());
    }
}
