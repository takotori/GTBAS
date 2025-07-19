using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectD.addons.gas.abilities;
using ProjectD.addons.gas.attributes;
using ProjectD.addons.gas.effects;
using ProjectD.scripts;
using ProjectD.scripts.maps;
using ProjectD.scripts.units;

namespace ProjectD.addons.gas;

[GlobalClass]
public partial class AbilitySystem : Node
{
    [Export]
    protected AttributeSet attributeSet;

    [Export]
    protected AbilityData[] defaultAbilities = [];

    private NavigationController navigation;

    private List<AbilityData> abilities = [];
    private List<Effect> activeEffects = [];

    private Unit owner;

    public override void _Ready()
    {
        navigation = ServiceContainer.GetService<NavigationController>();
        attributeSet.Init();
        owner = (Unit)GetParent();

        foreach (var defaultAbility in defaultAbilities)
        {
            AddAbility(defaultAbility);
        }
    }

    public void AddAbility(AbilityData abilityData)
    {
        if (abilities.Contains(abilityData))
        {
            throw new ArgumentException($"Ability {abilityData.GetAbilityName()} already exists");
        }

        abilities.Add(abilityData);
    }

    public void RemoveAbility(string abilityName)
    {
        abilities.RemoveAll(a => a.GetAbilityName() == abilityName);
    }

    public AbilityData GetAbility(string abilityName)
    {
        return abilities.First(a => a.GetAbilityName() == abilityName);
    }

    public AbilityData GetAbility(int index)
    {
        return abilities.ElementAtOrDefault(index)
            ?? throw new ArgumentOutOfRangeException(
                nameof(index),
                "No ability found at the given index."
            );
    }

    public bool CanActivateAbility(string abilityName)
    {
        var ability = GetAbility(abilityName);
        return CanActivateAbility(ability);
    }

    public bool CanActivateAbility(AbilityData abilityData)
    {
        if (!HasAbility(abilityData))
        {
            return false;
        }

        foreach (var cost in abilityData.GetCosts())
        {
            var attributes = attributeSet.GetAttributesByName(cost.GetAffectedAttributeNames());
            if (attributes.Count != cost.GetAffectedAttributeNames().Count)
                return false;

            foreach (var effectCost in cost.GetEffectModifiers())
            {
                if (attributes.Any(attribute => !effectCost.CanOperate(attribute)))
                {
                    // todo emit signal that cost are too high
                    return false;
                }
            }
        }

        return true;
    }

    public bool TryActivateAbility(Vector2I targetIndex, AbilityData abilityData)
    {
        if (!CanActivateAbility(abilityData) || !IsValidTarget(targetIndex, abilityData))
            return false;

        var targetsInRange = GetTargetsInRange(abilityData, targetIndex);

        var ability = ActivateAbility(targetIndex, abilityData, targetsInRange);
        return ability is not null;
    }

    protected Ability ActivateAbility(
        Vector2I targetIndex,
        AbilityData abilityData,
        List<AttributeSet> targetAttributeSets
    )
    {
        CommitAbility(abilityData);
        var abilityNode = abilityData.GetAbilityScene().Instantiate();
        AddChild(abilityNode);

        if (abilityNode is Ability ability)
        {
            ability.OnEffectTriggered += OnEffectTriggered(abilityData, targetAttributeSets);
            ability.ActivateAbility(targetIndex);
            // events.EmitSignal("OnAbilityActivated");
            return ability;
        }

        return null;
    }

    protected EventHandler OnEffectTriggered(
        AbilityData abilityData,
        List<AttributeSet> targetAttributeSets
    )
    {
        return (_, _) =>
        {
            foreach (var target in targetAttributeSets)
            {
                if (target is not null)
                {
                    ApplyEffectOnTarget(target, abilityData.GetEffects().ToList());
                }
            }
        };
    }

    protected void CommitAbility(AbilityData ability)
    {
        foreach (var abilityCost in ability.GetCosts())
        {
            var attributes = attributeSet.GetAttributesByName(
                abilityCost.GetAffectedAttributeNames()
            );
            if (attributes.Count != abilityCost.GetAffectedAttributeNames().Count)
                return;

            foreach (var effectCost in abilityCost.GetEffectModifiers())
            {
                foreach (var attribute in attributes)
                {
                    effectCost.Operate(attribute);
                }
            }
        }
    }

    public void ApplyEffectOnSelf(List<Effect> effects)
    {
        foreach (var effect in effects)
        {
            effect.ApplyEffect(attributeSet, attributeSet);
        }
    }

    public void ApplyEffectOnTarget(AttributeSet targetAttributeSet, List<Effect> effects)
    {
        if (targetAttributeSet is null || !AreEffectsValid(effects))
        {
            return;
        }

        foreach (var effect in effects)
        {
            effect.ApplyEffect(attributeSet, targetAttributeSet);
        }
    }

    public bool HasAbility(AbilityData abilityData)
    {
        return abilities.Contains(abilityData);
    }

    public List<AbilityData> GetAbilities()
    {
        return abilities;
    }

    public AttributeSet GetAttributeSet()
    {
        return attributeSet;
    }

    protected virtual Unit GetOwnerActor()
    {
        return owner;
    }

    private bool IsValidTarget(Vector2I targetIndex, AbilityData abilityData)
    {
        switch (abilityData.GetPattern().CenterPoint)
        {
            case CenterPoint.Target:
                var opponentUnits = GetUnitOfTeam(
                    GetOwnerActor().Team == Team.Player ? Team.Enemy : Team.Player
                );
                if (opponentUnits.Any(u => u.currentTileIndex == targetIndex))
                {
                    return true;
                }

                break;
            case CenterPoint.Self:
                if (owner.currentTileIndex == targetIndex)
                {
                    return true;
                }

                break;
            case CenterPoint.Ally:
                if (GetUnitOfTeam(GetOwnerActor().Team).Any(u => u.currentTileIndex == targetIndex))
                {
                    return true;
                }

                break;
            case CenterPoint.Area:
                return true;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }

    private List<AttributeSet> GetTargetsInRange(AbilityData ability, Vector2I targetTile)
    {
        var opponentUnits = GetUnitOfTeam(
            GetOwnerActor().Team == Team.Player ? Team.Enemy : Team.Player
        );

        var abilityAoeReachablePositions = PatternCalculator.GetAoeAbilityRange(
            GetOwnerActor().currentTileIndex,
            targetTile,
            ability.GetPattern().AoePattern,
            ability.GetPattern().MinMaxAoeRange
        );

        var abilityReachableTiles = navigation
            .GetAbilityReachableTiles(abilityAoeReachablePositions)
            .Select(a => a.GetTileIndex())
            .ToList();

        return opponentUnits
            .Where(u => abilityReachableTiles.Contains(u.currentTileIndex))
            .Select(u => u.abilitySystem.GetAttributeSet())
            .ToList();
    }

    private List<Unit> GetUnitOfTeam(Team team)
    {
        return GetTree()
            .GetNodesInGroup(team == Team.Player ? Constants.PlayerUnits : Constants.EnemyUnits)
            .Select(u => u as Unit)
            .ToList();
    }

    private bool AreEffectsValid(List<Effect> effects)
    {
        var affectedAttributes = effects.SelectMany(e => e.GetAffectedAttributeNames()).ToList();
        if (!attributeSet.HasAllAttributes(affectedAttributes))
        {
            return false;
        }

        return true;
    }
}
