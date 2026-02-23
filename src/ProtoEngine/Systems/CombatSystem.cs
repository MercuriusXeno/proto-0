using ProtoEngine.Components;
using ProtoEngine.Core;
using ProtoEngine.Data;
using ProtoEngine.Events;

namespace ProtoEngine.Systems;

public class CombatSystem : IGameSystem, ITickable
{
    private readonly ContentManifest _content;
    private readonly IEventBus _eventBus;
    private readonly Random _rng = new();

    public CombatSystem(ContentManifest content, IEventBus eventBus)
    {
        _content = content;
        _eventBus = eventBus;
    }

    public void Initialize(GameState state) { }

    public void Tick(GameState state, GameClock clock)
    {
        // NPC retaliation during combat
        var combat = state.Player.Get<CombatComponent>();
        if (combat is null || !combat.InCombat || combat.TargetId is null) return;

        var target = state.GetEntity(combat.TargetId);
        if (target is null || target.Get<HealthComponent>()?.IsAlive != true)
        {
            combat.InCombat = false;
            combat.TargetId = null;
            return;
        }

        // Enemy attacks back
        var enemyCombat = target.Get<CombatComponent>();
        var playerHealth = state.Player.Get<HealthComponent>();
        var playerCombat = state.Player.Get<CombatComponent>();
        if (enemyCombat is not null && playerHealth is not null && playerCombat is not null)
        {
            var damage = Math.Max(1, enemyCombat.AttackPower - playerCombat.Defense);
            var variance = _rng.Next(-1, 2);
            damage = Math.Max(1, damage + variance);
            playerHealth.Current = Math.Max(0, playerHealth.Current - damage);
        }
    }

    public (int damage, bool targetDied, List<string> messages) Attack(GameState state, Entity target)
    {
        var messages = new List<string>();
        var playerStats = state.Player.Get<StatsComponent>();
        var playerCombat = state.Player.Get<CombatComponent>();
        var targetHealth = target.Get<HealthComponent>();
        var targetCombat = target.Get<CombatComponent>();
        var targetDesc = target.Get<DescriptionComponent>();

        if (playerCombat is null || targetHealth is null)
            return (0, false, new List<string> { "You can't attack that." });

        var baseDamage = playerCombat.AttackPower + (playerStats?.Strength ?? 0) / 3;
        var defense = targetCombat?.Defense ?? 0;
        var damage = Math.Max(1, baseDamage - defense);
        var variance = _rng.Next(-2, 3);
        damage = Math.Max(1, damage + variance);

        targetHealth.Current = Math.Max(0, targetHealth.Current - damage);
        var name = targetDesc?.Name ?? "the enemy";

        messages.Add($"You attack {name} for {damage} damage!");
        playerCombat.InCombat = true;
        playerCombat.TargetId = target.Id;

        if (!targetHealth.IsAlive)
        {
            messages.Add($"{name} has been defeated!");
            playerCombat.InCombat = false;
            playerCombat.TargetId = null;
            _eventBus.Publish(new EntityDiedEvent(target.Id, state.Player.Id));
            _eventBus.Publish(new CombatEndedEvent(state.Player.Id, target.Id));
            return (damage, true, messages);
        }

        messages.Add($"{name} has {targetHealth.Current}/{targetHealth.Max} HP remaining.");
        return (damage, false, messages);
    }

    public Entity? FindTargetInRoom(GameState state, string targetName)
    {
        var playerPos = state.Player.Get<PositionComponent>();
        if (playerPos is null) return null;

        return state.Entities.Values.FirstOrDefault(e =>
            e.Tag == "npc" &&
            e.Get<PositionComponent>()?.RoomId == playerPos.RoomId &&
            e.Get<HealthComponent>()?.IsAlive == true &&
            (e.Get<DescriptionComponent>()?.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase) ?? false));
    }
}
