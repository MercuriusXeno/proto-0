using ProtoEngine.Components;
using ProtoEngine.Core;
using ProtoEngine.Events;

namespace ProtoEngine.Systems;

public class MemorySystem : IGameSystem
{
    private readonly IEventBus _eventBus;

    public MemorySystem(IEventBus eventBus)
    {
        _eventBus = eventBus;
        _eventBus.Subscribe<RoomEnteredEvent>(OnRoomEntered);
    }

    public void Initialize(GameState state)
    {
        // Ensure player has memory component
        if (state.Player.Get<RoomMemoryComponent>() is null)
            state.Player.Add(new RoomMemoryComponent());
    }

    private void OnRoomEntered(RoomEnteredEvent evt)
    {
        // Handled via AddRoomVisit method
    }

    public void AddRoomVisit(GameState state, string roomId)
    {
        var memory = state.Player.Get<RoomMemoryComponent>();
        if (memory is null) return;

        memory.VisitedRoomIds.Add(roomId);

        // Increment visit count when entering a different room
        if (memory.CurrentRoomId != roomId)
        {
            memory.CurrentRoomId = roomId;
            if (!memory.RoomVisitCounts.ContainsKey(roomId))
                memory.RoomVisitCounts[roomId] = 0;
            memory.RoomVisitCounts[roomId]++;
        }
    }

    public int GetCurrentVisitNumber(GameState state, string roomId)
    {
        var memory = state.Player.Get<RoomMemoryComponent>();
        if (memory is null || !memory.RoomVisitCounts.ContainsKey(roomId))
            return 0;
        return memory.RoomVisitCounts[roomId];
    }

    public void RecordItemFound(GameState state, string roomId, string itemId, string itemName)
    {
        var memory = state.Player.Get<RoomMemoryComponent>();
        if (memory is null) return;

        if (!memory.RoomMemories.ContainsKey(roomId))
            memory.RoomMemories[roomId] = new List<RoomMemory>();

        // Only record if not already recorded
        if (!memory.RoomMemories[roomId].Any(m => m.Type == RoomMemoryType.ItemFound && m.EntityId == itemId))
        {
            var visitNumber = GetCurrentVisitNumber(state, roomId);
            memory.RoomMemories[roomId].Add(new RoomMemory
            {
                Type = RoomMemoryType.ItemFound,
                EntityId = itemId,
                EntityName = itemName,
                GameTick = state.Clock.Tick,
                RoomVisitNumber = visitNumber
            });
        }
    }

    public void RecordItemTaken(GameState state, string roomId, string itemId, string itemName)
    {
        var memory = state.Player.Get<RoomMemoryComponent>();
        if (memory is null) return;

        if (!memory.RoomMemories.ContainsKey(roomId))
            memory.RoomMemories[roomId] = new List<RoomMemory>();

        var visitNumber = GetCurrentVisitNumber(state, roomId);
        memory.RoomMemories[roomId].Add(new RoomMemory
        {
            Type = RoomMemoryType.ItemTaken,
            EntityId = itemId,
            EntityName = itemName,
            GameTick = state.Clock.Tick,
            RoomVisitNumber = visitNumber
        });
    }

    public void RecordNpcMet(GameState state, string roomId, string npcId, string npcName)
    {
        var memory = state.Player.Get<RoomMemoryComponent>();
        if (memory is null) return;

        if (!memory.RoomMemories.ContainsKey(roomId))
            memory.RoomMemories[roomId] = new List<RoomMemory>();

        // Only record first meeting
        if (!memory.RoomMemories[roomId].Any(m =>
            (m.Type == RoomMemoryType.NpcMet || m.Type == RoomMemoryType.NpcKilled) && m.EntityId == npcId))
        {
            var visitNumber = GetCurrentVisitNumber(state, roomId);
            memory.RoomMemories[roomId].Add(new RoomMemory
            {
                Type = RoomMemoryType.NpcMet,
                EntityId = npcId,
                EntityName = npcName,
                GameTick = state.Clock.Tick,
                RoomVisitNumber = visitNumber
            });
        }
    }

    public void RecordNpcKilled(GameState state, string roomId, string npcId, string npcName)
    {
        var memory = state.Player.Get<RoomMemoryComponent>();
        if (memory is null) return;

        if (!memory.RoomMemories.ContainsKey(roomId))
            memory.RoomMemories[roomId] = new List<RoomMemory>();

        memory.RoomMemories[roomId].Add(new RoomMemory
        {
            Type = RoomMemoryType.NpcKilled,
            EntityId = npcId,
            EntityName = npcName,
            GameTick = state.Clock.Tick
        });
    }

    public void RecordExitExplored(GameState state, string fromRoomId, string direction, string toRoomId)
    {
        var memory = state.Player.Get<RoomMemoryComponent>();
        if (memory is null) return;

        if (!memory.RoomMemories.ContainsKey(fromRoomId))
            memory.RoomMemories[fromRoomId] = new List<RoomMemory>();

        // Only record once per direction
        if (!memory.RoomMemories[fromRoomId].Any(m =>
            m.Type == RoomMemoryType.ExitExplored && m.AdditionalInfo == direction))
        {
            memory.RoomMemories[fromRoomId].Add(new RoomMemory
            {
                Type = RoomMemoryType.ExitExplored,
                EntityId = toRoomId,
                EntityName = direction,
                GameTick = state.Clock.Tick,
                AdditionalInfo = direction
            });
        }
    }

    public bool HasVisited(GameState state, string roomId)
    {
        var memory = state.Player.Get<RoomMemoryComponent>();
        return memory?.VisitedRoomIds.Contains(roomId) ?? false;
    }

    public List<RoomMemory> GetRoomMemories(GameState state, string roomId)
    {
        var memory = state.Player.Get<RoomMemoryComponent>();
        if (memory is null || !memory.RoomMemories.ContainsKey(roomId))
            return new List<RoomMemory>();

        return memory.RoomMemories[roomId];
    }

    public bool HasExploredExit(GameState state, string roomId, string direction)
    {
        var memories = GetRoomMemories(state, roomId);
        return memories.Any(m => m.Type == RoomMemoryType.ExitExplored && m.AdditionalInfo == direction);
    }

    public int GetMemoryStrength(GameState state)
    {
        var stats = state.Player.Get<StatsComponent>();
        // For now, return a default value. Will be updated when Memory stat is added
        return stats?.Intelligence ?? 10;
    }
}
