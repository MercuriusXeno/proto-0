using System.Net.Http.Json;
using System.Text.Json;
using ProtoEngine.Commands;
using ProtoEngine.Commands.Commands;
using ProtoEngine.Components;
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
    private ContentManifest? _content;

    // Systems exposed for UI panels
    public WorldSystem? World { get; private set; }
    public PlayerSystem? Player { get; private set; }
    public InventorySystem? Inventory { get; private set; }
    public CombatSystem? Combat { get; private set; }
    public NpcSystem? Npc { get; private set; }
    public QuestSystem? Quest { get; private set; }
    public CraftingSystem? Crafting { get; private set; }
    public NarrativeSystem? Narrative { get; private set; }
    public MemorySystem? Memory { get; private set; }
    public ActionLogSystem? ActionLog { get; private set; }

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
        _content = new ContentManifest
        {
            Rooms = await LoadJson<List<RoomData>>("data/rooms.json") ?? new(),
            Items = await LoadJson<List<ItemData>>("data/items.json") ?? new(),
            Npcs = await LoadJson<List<NpcData>>("data/npcs.json") ?? new(),
            Quests = await LoadJson<List<QuestData>>("data/quests.json") ?? new(),
            Recipes = await LoadJson<List<RecipeData>>("data/recipes.json") ?? new(),
            Dialogues = await LoadJson<List<DialogueData>>("data/dialogue.json") ?? new(),
            StartingRoomId = "town_square"
        };

        var state = new GameState();
        var eventBus = new EventBus();
        var parser = new CommandParser();
        var registry = new CommandRegistry();

        // Create systems
        World = new WorldSystem(_content, eventBus);
        Player = new PlayerSystem(eventBus);
        Inventory = new InventorySystem(_content, eventBus);
        Combat = new CombatSystem(_content, eventBus);
        Npc = new NpcSystem(_content, eventBus);
        Quest = new QuestSystem(_content, eventBus);
        Crafting = new CraftingSystem(_content, eventBus);
        Narrative = new NarrativeSystem(eventBus);
        Memory = new MemorySystem(eventBus);
        ActionLog = new ActionLogSystem();
        var statusEffects = new StatusEffectSystem();
        var time = new TimeSystem();
        var events = new EventSystem(eventBus);

        _session = new GameSession(state, parser, registry, eventBus);

        // Register systems
        _session.RegisterSystem(World);
        _session.RegisterSystem(Player);
        _session.RegisterSystem(Inventory);
        _session.RegisterSystem(Combat);
        _session.RegisterSystem(Npc);
        _session.RegisterSystem(Quest);
        _session.RegisterSystem(Crafting);
        _session.RegisterSystem(Narrative);
        _session.RegisterSystem(Memory);
        _session.RegisterSystem(ActionLog);
        _session.RegisterSystem(statusEffects);
        _session.RegisterSystem(time);
        _session.RegisterSystem(events);

        // Register commands
        registry.Register(new LookCommand(World, Inventory, Narrative, Memory));
        registry.Register(new MoveCommand(World, Memory, ActionLog));
        registry.Register(new InventoryCommand(Inventory));
        registry.Register(new TakeCommand(Inventory, World, Memory, ActionLog));
        registry.Register(new DropCommand(Inventory));
        registry.Register(new UseCommand(Inventory));
        registry.Register(new WearCommand(Inventory));
        registry.Register(new WieldCommand(Inventory));
        registry.Register(new UnwieldCommand());
        registry.Register(new AttackCommand(Combat, Player, World, Memory, ActionLog));
        registry.Register(new TalkCommand(Npc, World, Memory));
        registry.Register(new StatusCommand());
        registry.Register(new CraftCommand(Crafting));
        registry.Register(new QuestCommand(Quest));
        registry.Register(new SaveCommand());
        registry.Register(new LoadCommand());
        registry.Register(new HelpCommand(registry));

        _session.Initialize();
    }

    public async Task<CommandResult> ProcessCommandAsync(string input)
    {
        if (_session is null)
            return CommandResult.Fail("Game not initialized.");

        var result = _session.ProcessCommand(input);

        // Auto-save after each command
        if (result.Success)
        {
            var saveData = CreateSaveData();
            await _saveService.SaveAsync(saveData);
        }

        return result;
    }

    public SaveData CreateSaveData()
    {
        var state = _session!.State;
        var pos = state.Player.Get<PositionComponent>();
        var health = state.Player.Get<HealthComponent>();
        var stats = state.Player.Get<StatsComponent>();
        var inv = state.Player.Get<InventoryComponent>();
        var equip = state.Player.Get<EquipmentComponent>();

        return new SaveData
        {
            ClockTick = state.Clock.Tick,
            PlayerRoomId = pos?.RoomId ?? "town_square",
            PlayerHealth = health?.Current ?? 100,
            PlayerMaxHealth = health?.Max ?? 100,
            PlayerLevel = stats?.Level ?? 1,
            PlayerExperience = stats?.Experience ?? 0,
            PlayerStrength = stats?.Strength ?? 10,
            PlayerDexterity = stats?.Dexterity ?? 10,
            PlayerIntelligence = stats?.Intelligence ?? 10,
            PlayerGold = stats?.Gold ?? 0,
            InventoryItemIds = inv?.ItemIds.ToList() ?? new(),
            WeaponId = equip?.GetSlotItems(EquipmentSlot.WieldRight).FirstOrDefault()?.ItemId, // Save right hand wield for backwards compat
            ArmorId = equip?.GetSlotItems(EquipmentSlot.Body).FirstOrDefault()?.ItemId,
            SavedAt = DateTime.UtcNow
        };
    }

    public void RestoreFromSave(SaveData save)
    {
        if (_session is null) return;
        var state = _session.State;

        state.Clock.Set(save.ClockTick);
        var pos = state.Player.Get<PositionComponent>();
        if (pos is not null) pos.RoomId = save.PlayerRoomId;

        var health = state.Player.Get<HealthComponent>();
        if (health is not null) { health.Current = save.PlayerHealth; health.Max = save.PlayerMaxHealth; }

        var stats = state.Player.Get<StatsComponent>();
        if (stats is not null)
        {
            stats.Level = save.PlayerLevel;
            stats.Experience = save.PlayerExperience;
            stats.Strength = save.PlayerStrength;
            stats.Dexterity = save.PlayerDexterity;
            stats.Intelligence = save.PlayerIntelligence;
            stats.Gold = save.PlayerGold;
        }

        var inv = state.Player.Get<InventoryComponent>();
        if (inv is not null) inv.ItemIds = save.InventoryItemIds.ToList();

        var equip = state.Player.Get<EquipmentComponent>();
        if (equip is not null)
        {
            // Restore weapon to WieldRight slot
            if (save.WeaponId is not null)
            {
                var weaponData = _content?.Items.FirstOrDefault(i => i.Id == save.WeaponId);
                if (weaponData is not null)
                    equip.EquipItem(EquipmentSlot.WieldRight, save.WeaponId, weaponData.Name);
            }

            // Restore armor to Body slot
            if (save.ArmorId is not null)
            {
                var armorData = _content?.Items.FirstOrDefault(i => i.Id == save.ArmorId);
                if (armorData is not null)
                    equip.EquipItem(EquipmentSlot.Body, save.ArmorId, armorData.Name);
            }
        }
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
