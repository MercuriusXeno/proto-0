using System.Net.Http.Json;
using System.Text.Json;
using ProtoEngine.Commands;
using ProtoEngine.Commands.Commands;
using ProtoEngine.Core;
using ProtoEngine.Data;
using ProtoEngine.Events;
using ProtoEngine.Persistence;
using ProtoEngine.Session;
using ProtoEngine.Systems;

namespace ProtoMud.Services;

public class GameSessionHost
{
    private readonly HttpClient _http;
    private readonly ISaveLoadService _saveService;
    private GameSession? _session;
    private SaveDataMapper? _saveMapper;

    public WorldSystem? World { get; private set; }
    public PlayerSystem? Player { get; private set; }
    public InventorySystem? Inventory { get; private set; }
    public CombatSystem? Combat { get; private set; }
    public NpcSystem? Npc { get; private set; }
    public QuestSystem? Quest { get; private set; }
    public CraftingSystem? Crafting { get; private set; }
    public ActionLogSystem? ActionLog { get; private set; }
    public StatGrowthSystem? StatGrowth { get; private set; }

    public GameSession? Session => _session;
    public GameState? State => _session?.State;
    public bool IsInitialized => _session?.State.IsInitialized ?? false;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GameSessionHost(HttpClient http, ISaveLoadService saveService)
    {
        _http = http;
        _saveService = saveService;
    }

    public async Task InitializeAsync()
    {
        var content = await LoadContent();

        var state = new GameState();
        var eventBus = new EventBus();
        var parser = new CommandParser();
        var registry = new CommandRegistry();

        RegisterSystems(content, eventBus, state, parser, registry);
        RegisterCommands(registry);

        _session!.Initialize();
        _saveMapper = new SaveDataMapper(content.Items);
    }

    public async Task<CommandResult> ProcessCommandAsync(string input)
    {
        if (_session is null)
            return CommandResult.Fail("Game not initialized.");

        var result = _session.ProcessCommand(input);

        if (result.Success)
        {
            var saveData = _saveMapper!.CreateSaveData(_session.State);
            await _saveService.SaveAsync(saveData);
        }

        return result;
    }

    /// <summary>
    /// Creates a SaveData snapshot of the current game state.
    /// </summary>
    public SaveData CreateSaveData()
    {
        return _saveMapper!.CreateSaveData(_session!.State);
    }

    /// <summary>
    /// Restores game state from a SaveData snapshot.
    /// </summary>
    public void RestoreFromSave(SaveData save)
    {
        if (_session is null) return;
        _saveMapper!.RestoreFromSave(_session.State, save);
    }

    private async Task<ContentManifest> LoadContent()
    {
        return new ContentManifest
        {
            Rooms = await LoadJson<List<RoomData>>("data/rooms.json") ?? new(),
            Items = await LoadJson<List<ItemData>>("data/items.json") ?? new(),
            Npcs = await LoadJson<List<NpcData>>("data/npcs.json") ?? new(),
            Quests = await LoadJson<List<QuestData>>("data/quests.json") ?? new(),
            Recipes = await LoadJson<List<RecipeData>>("data/recipes.json") ?? new(),
            Dialogues = await LoadJson<List<DialogueData>>("data/dialogue.json") ?? new(),
            StartingRoomId = "town_square"
        };
    }

    private void RegisterSystems(ContentManifest content, EventBus eventBus, GameState state, CommandParser parser, CommandRegistry registry)
    {
        World = new WorldSystem(content, eventBus);
        Player = new PlayerSystem(eventBus);
        Inventory = new InventorySystem(content, eventBus);
        Combat = new CombatSystem(content, eventBus);
        Npc = new NpcSystem(content, eventBus);
        Quest = new QuestSystem(content, eventBus);
        Crafting = new CraftingSystem(content, eventBus);
        ActionLog = new ActionLogSystem();
        StatGrowth = new StatGrowthSystem(eventBus);
        var statusEffects = new StatusEffectSystem();
        var time = new TimeSystem();
        var events = new EventSystem(eventBus);

        _session = new GameSession(state, parser, registry, eventBus);

        _session.RegisterSystem(World);
        _session.RegisterSystem(Player);
        _session.RegisterSystem(Inventory);
        _session.RegisterSystem(Combat);
        _session.RegisterSystem(Npc);
        _session.RegisterSystem(Quest);
        _session.RegisterSystem(Crafting);
        _session.RegisterSystem(ActionLog);
        _session.RegisterSystem(StatGrowth);
        _session.RegisterSystem(statusEffects);
        _session.RegisterSystem(time);
        _session.RegisterSystem(events);
    }

    private void RegisterCommands(CommandRegistry registry)
    {
        registry.Register(new LookCommand(World!, Inventory!));
        registry.Register(new MoveCommand(World!, ActionLog!));
        registry.Register(new InventoryCommand(Inventory!));
        registry.Register(new TakeCommand(Inventory!, ActionLog!));
        registry.Register(new DropCommand(Inventory!));
        registry.Register(new UseCommand(Inventory!));
        registry.Register(new WearCommand(Inventory!));
        registry.Register(new WieldCommand(Inventory!));
        registry.Register(new UnwieldCommand());
        registry.Register(new AttackCommand(Combat!, Player!, ActionLog!));
        registry.Register(new TalkCommand(Npc!, World!));
        registry.Register(new StatusCommand());
        registry.Register(new CraftCommand(Crafting!));
        registry.Register(new QuestCommand(Quest!));
        registry.Register(new SaveCommand());
        registry.Register(new LoadCommand());
        registry.Register(new HelpCommand(registry));
    }

    private async Task<T?> LoadJson<T>(string path) where T : class
    {
        try
        {
            return await _http.GetFromJsonAsync<T>(path, JsonOptions);
        }
        catch
        {
            return null;
        }
    }
}
