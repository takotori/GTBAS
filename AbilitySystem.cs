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
    public AttributeSet attributeSet { get; private set; }

    [Export]
    private AbilityData[] defaultAbilities = [];

    public List<AbilityData> abilities { get; } = [];
    public Unit owner { get; private set; }

    private NavigationController navigation;
    private List<Effect> activeEffects = [];
    
    private AbilityData activeAbility;
    private Vector2I activeAbilityIndex;
    private AbilitySystemEvents events;

    public override void _Ready()
    {
        events = AbilitySystemEvents.Instance;
        navigation = ServiceContainer.GetService<NavigationController>();
        owner = (Unit)GetParent();
        attributeSet.Init(owner);

        foreach (var defaultAbility in defaultAbilities)
        {
            AddAbility(defaultAbility);
        }
    }

    public bool CanActivateAbility(AbilityData abilityData)
    {
        if (!HasAbility(abilityData))
        {
            return false;
        }

        foreach (var cost in abilityData.costs)
        {
            var attributes = attributeSet.GetAttributesByName(cost.GetAffectedAttributeNames());
            if (attributes.Count != cost.GetAffectedAttributeNames().Count)
                return false;

            foreach (var effectCost in cost.effectModifiers)
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

        return ActivateAbility(targetIndex, abilityData);
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

    private bool ActivateAbility(Vector2I targetIndex, AbilityData abilityData)
    {
        CommitAbility(abilityData);
        var abilityNode = abilityData.ability.Instantiate();
        AddChild(abilityNode);

        if (abilityNode is Ability ability)
        {
            activeAbility = abilityData;
            activeAbilityIndex = targetIndex;
            ability.Init(abilityData, this);
            events.OnAbilityEnded += OnMainAbilityEnded;
            ability.ActivateAbility(targetIndex);
            events.EmitSignal(nameof(events.OnAbilityActivated));
            return true;
        }
        
        return false;
    }

    private void OnMainAbilityEnded()
    {
        foreach (var subAbilityData in activeAbility.subAbilities)
        {
            var subAbilityNode = subAbilityData.ability.Instantiate();
            AddChild(subAbilityNode);

            if (subAbilityNode is SubAbility subAbility && subAbility.CanActivateSubAbility())
            {
                subAbility.Init(subAbilityData, this);
                subAbility.ActivateAbility(activeAbilityIndex);
            }
            else
            {
                subAbilityNode.QueueFree();
            }
        }
    }

    private void CommitAbility(AbilityData ability)
    {
        foreach (var abilityCost in ability.costs)
        {
            var attributes = attributeSet.GetAttributesByName(
                abilityCost.GetAffectedAttributeNames()
            );
            if (attributes.Count != abilityCost.GetAffectedAttributeNames().Count)
                return;

            foreach (var effectCost in abilityCost.effectModifiers)
            {
                foreach (var attribute in attributes)
                {
                    effectCost.Operate(attribute);
                }
            }
        }
    }

    private bool IsValidTarget(Vector2I targetIndex, AbilityData abilityData)
    {
        switch (abilityData.pattern.CenterPoint)
        {
            case CenterPoint.Target:
                var opponentUnits = GetUnitOfTeam(
                    owner.Team == Team.Player ? Team.Enemy : Team.Player
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
                if (GetUnitOfTeam(owner.Team).Any(u => u.currentTileIndex == targetIndex))
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

    public void AddAbility(AbilityData abilityData)
    {
        if (abilities.Contains(abilityData))
        {
            throw new ArgumentException($"Ability {abilityData.abilityName} already exists");
        }

        abilities.Add(abilityData);
    }

    public void RemoveAbility(string abilityName)
    {
        abilities.RemoveAll(a => a.abilityName == abilityName);
    }

    public AbilityData GetAbility(string abilityName)
    {
        return abilities.First(a => a.abilityName == abilityName);
    }

    public AbilityData GetAbility(int index)
    {
        return abilities.ElementAtOrDefault(index)
            ?? throw new ArgumentOutOfRangeException(
                nameof(index),
                "No ability found at the given index."
            );
    }

    public bool HasAbility(AbilityData abilityData)
    {
        return abilities.Contains(abilityData);
    }
}
