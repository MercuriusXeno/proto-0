using ProtoEngine.Components;
using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class TalkCommand : ICommand
{
    private readonly NpcSystem _npc;
    private readonly WorldSystem _world;

    public string Verb => "talk";
    public string[] Aliases => ["speak", "chat"];
    public string Description => "Talk to an NPC (talk <npc name>)";

    public TalkCommand(NpcSystem npc, WorldSystem world)
    {
        _npc = npc;
        _world = world;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        var room = _world.GetPlayerRoom(context.State);
        if (room is null)
            return CommandResult.Fail("You are nowhere.");

        if (args.Length == 0)
            return CommandResult.Fail("Talk to whom? Specify an NPC name.");

        var name = string.Join(" ", args);
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

        var lines = _npc.Talk(context.State, npc);

        return CommandResult.Ok(lines.ToArray());
    }
}
