using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectD.addons.gas.attributes;
using ProjectD.scripts;
using ProjectD.scripts.maps;
using ProjectD.scripts.units;

namespace ProjectD.addons.gas.abilities;

[GlobalClass]
public partial class Ability : Node3D
{
    protected NavigationController navigation;
    protected AbilitySystemEvents events;
    protected AbilitySystemManager manager;
    protected AnimationPlayer animationPlayer;
    protected AbilitySystem casterAbilitySystem;
    protected AbilityData abilityData;
    protected List<AttributeSet> targets = [];

    public override void _Ready()
    {
        events = AbilitySystemEvents.Instance;
        manager = AbilitySystemManager.Instance;
        navigation = ServiceContainer.GetService<NavigationController>();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.AnimationFinished += EndAbility;
    }

    public void Init(AbilityData abilityData, AbilitySystem casterAbilitySystem)
    {
        this.abilityData = abilityData;
        this.casterAbilitySystem = casterAbilitySystem;
    }

    public virtual void ActivateAbility(Vector2I targetIndex)
    {
        events.EmitSignal(nameof(events.OnAbilityActivated), GetOwnerActor(), abilityData);
    }

    public virtual void EndAbility(StringName animationName)
    {
        events.EmitSignal(nameof(events.OnAbilityEnded));
    }

    protected virtual void TriggerEffect() { }

    protected Vector3 GetGlobalPosition(Vector2I targetIndex)
    {
        return navigation.ToGlobalPosition(targetIndex);
    }

    protected List<AttributeSet> GetTargetsInRange(AbilityData ability, Vector2I targetTile)
    {
        var opponentUnits = GetUnitOfTeam(
            GetOwnerActor().Team == Team.Player ? Team.Enemy : Team.Player
        );

        var abilityAoeReachablePositions = PatternCalculator.GetAoeAbilityRange(
            GetOwnerActor().currentTileIndex,
            targetTile,
            ability.pattern.AoePattern,
            ability.pattern.MinMaxAoeRange
        );

        var abilityReachableTiles = navigation
            .GetAbilityReachableTiles(abilityAoeReachablePositions)
            .Select(a => a.GetTileIndex())
            .ToList();

        return opponentUnits
            .Where(u => abilityReachableTiles.Contains(u.currentTileIndex))
            .Select(u => u.abilitySystem.attributeSet)
            .ToList();
    }

    protected List<Unit> GetUnitOfTeam(Team team)
    {
        return GetTree()
            .GetNodesInGroup(team == Team.Player ? Constants.PlayerUnits : Constants.EnemyUnits)
            .Select(u => u as Unit)
            .ToList();
    }

    protected Unit GetOwnerActor()
    {
        return casterAbilitySystem.owner;
    }
}
