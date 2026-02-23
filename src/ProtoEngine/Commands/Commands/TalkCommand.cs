using ProtoEngine.Systems;

namespace ProtoEngine.Commands.Commands;

public class TalkCommand : ICommand
{
    private readonly NpcSystem _npc;

    public string Verb => "talk";
    public string[] Aliases => ["speak", "chat"];
    public string Description => "Talk to an NPC (talk <npc name>)";

    public TalkCommand(NpcSystem npc)
    {
        _npc = npc;
    }

    public CommandResult Execute(CommandContext context, string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Fail("Talk to whom? Specify a name.");

        var name = string.Join(" ", args);
        var npc = _npc.FindNpcInRoom(context.State, name);

        if (npc is null)
            return CommandResult.Fail($"There is no '{name}' here to talk to.");

        var lines = _npc.Talk(context.State, npc);
        return CommandResult.Ok(lines.ToArray());
    }
}
