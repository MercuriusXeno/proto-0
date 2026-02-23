using ProtoEngine.Components;
using ProtoEngine.Core;
using ProtoEngine.Data;
using ProtoEngine.Events;

namespace ProtoEngine.Systems;

public class WorldSystem : IGameSystem
{
    private readonly ContentManifest _content;
    private readonly IEventBus _eventBus;
    private readonly Dictionary<string, Entity> _rooms = new();

    public WorldSystem(ContentManifest content, IEventBus eventBus)
    {
        _content = content;
        _eventBus = eventBus;
    }

    public void Initialize(GameState state)
    {
        foreach (var roomData in _content.Rooms)
        {
            var room = new Entity { Id = roomData.Id, Tag = "room" };
            room.Add(new DescriptionComponent
            {
                Name = roomData.Name,
                ShortDescription = roomData.Description,
                LongDescription = roomData.Description
            });
            _rooms[room.Id] = room;
            state.AddEntity(room);
        }

        // Place player in starting room
        var pos = state.Player.Get<PositionComponent>() ?? state.Player.Add(new PositionComponent());
        pos.RoomId = _content.StartingRoomId;
    }

    public RoomData? GetRoom(string roomId)
        => _content.Rooms.FirstOrDefault(r => r.Id == roomId);

    public Entity? GetRoomEntity(string roomId)
        => _rooms.TryGetValue(roomId, out var r) ? r : null;

    public RoomData? GetPlayerRoom(GameState state)
    {
        var pos = state.Player.Get<PositionComponent>();
        return pos is not null ? GetRoom(pos.RoomId) : null;
    }

    public bool TryMove(GameState state, string direction, out string? newRoomId)
    {
        newRoomId = null;
        var room = GetPlayerRoom(state);
        if (room is null) return false;

        if (!room.Exits.TryGetValue(direction.ToLowerInvariant(), out var targetId))
            return false;

        var pos = state.Player.Get<PositionComponent>()!;
        pos.RoomId = targetId;
        newRoomId = targetId;
        _eventBus.Publish(new RoomEnteredEvent(state.Player.Id, targetId));
        return true;
    }

    public IEnumerable<Entity> GetEntitiesInRoom(GameState state, string roomId)
        => state.Entities.Values.Where(e =>
            e.Get<PositionComponent>()?.RoomId == roomId && e.Tag != "room" && e.Id != state.Player.Id);
}
