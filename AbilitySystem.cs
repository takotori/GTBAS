using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectD.addons.gas.abilities;
using ProjectD.addons.gas.attributes;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas;

[GlobalClass]
public partial class AbilitySystem : Node
{
    [Export]
    protected AttributeSet attributeSet;

    [Export]
    protected AbilityData[] defaultAbilities = [];

    private List<AbilityData> abilities = [];
    private List<Effect> activeEffects = [];

    public override void _Ready()
    {
        attributeSet.Init();
        foreach (var defaultAbility in defaultAbilities)
        {
            AddAbility(defaultAbility);
        }
    }

    #region Abilities

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

    public void SetAbilities(List<AbilityData> newAbilities)
    {
        abilities = newAbilities;
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

    public List<AbilityData> GetAbilities()
    {
        return abilities;
    }

    public List<AbilityData> GetAvailableAbilities()
    {
        return abilities.Where(CanActivateAbility).ToList();
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

    public bool TryActivateAbility(
        AbilitySystem caster,
        Vector3 targetPosition,
        List<AbilitySystem> targets,
        AbilityData abilityData
    )
    {
        // todo is in range should be handled somewhere else
        // && IsInRange(abilityData, caster, targetTile)
        // is valid target as well

        if (!CanActivateAbility(abilityData) || targets.Count == 0)
            return false;

        return ActivateAbility(caster, targetPosition, abilityData, targets);
    }

    private bool ActivateAbility(
        AbilitySystem caster,
        Vector3 targetPosition,
        AbilityData abilityData,
        List<AbilitySystem> targets
    )
    {
        CommitAbility(abilityData);
        var abilityNode = abilityData.GetAbilityScene().Instantiate();
        AddChild(abilityNode);

        if (abilityNode is Ability ability)
        {
            ability.OnEffectTriggered += OnEffectTriggered(abilityData, targets);
            ability.ActivateAbility(targetPosition);
            // events.EmitSignal("OnAbilityActivated");
            return true;
        }

        return false;
    }

    private EventHandler OnEffectTriggered(AbilityData abilityData, List<AbilitySystem> targets)
    {
        return (_, _) =>
        {
            foreach (var target in targets)
            {
                if (target is not null)
                {
                    ApplyEffectOnTarget(target, abilityData.GetEffects().ToList());
                }
            }
        };
    }

    private bool HasAbility(AbilityData abilityData)
    {
        return defaultAbilities.Contains(abilityData);
    }

    private void CommitAbility(AbilityData ability)
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

    #endregion


    #region Effects

    public void ApplyEffectOnSelf(List<Effect> effects)
    {
        foreach (var effect in effects)
        {
            effect.ApplyEffect(this, this);
        }
    }

    public void ApplyEffectOnTarget(AbilitySystem target, List<Effect> effects)
    {
        if (target is null || !target.AreEffectsValid(effects))
        {
            return;
        }

        foreach (var effect in effects)
        {
            effect.ApplyEffect(this, target);
        }
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

    #endregion

    public AttributeSet GetAttributeSet()
    {
        return attributeSet;
    }

    public void SetAttributeSet(AttributeSet newAttributeSet)
    {
        attributeSet = newAttributeSet;
    }
}
