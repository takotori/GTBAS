using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectD.addons.gas.abilities;
using ProjectD.addons.gas.attributes;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas;

public class AbilitySystemService
{
    public void AddAbility(List<AbilityData> abilities, AbilityData abilityData)
    {
        if (abilities.Contains(abilityData))
        {
            throw new ArgumentException($"Ability {abilityData.GetAbilityName()} already exists");
        }

        abilities.Add(abilityData);
    }

    public void RemoveAbility(List<AbilityData> abilities, string abilityName)
    {
        abilities.RemoveAll(a => a.GetAbilityName() == abilityName);
    }

    public AbilityData GetAbility(List<AbilityData> abilities, string abilityName)
    {
        return abilities.First(a => a.GetAbilityName() == abilityName);
    }

    public AbilityData GetAbility(List<AbilityData> abilities, int index)
    {
        return abilities.ElementAtOrDefault(index)
            ?? throw new ArgumentOutOfRangeException(
                nameof(index),
                "No ability found at the given index."
            );
    }

    public bool CanActivateAbility(
        AttributeSet attributeSet,
        List<AbilityData> abilities,
        string abilityName
    )
    {
        var ability = GetAbility(abilities, abilityName);
        return CanActivateAbility(attributeSet, abilities, ability);
    }

    public bool CanActivateAbility(
        AttributeSet attributeSet,
        List<AbilityData> abilities,
        AbilityData abilityData
    )
    {
        if (!HasAbility(abilities, abilityData))
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
        AttributeSet casterAttributeSet,
        List<AbilityData> casterAbilities,
        Vector3 targetPosition,
        List<AttributeSet> targetAttributeSets,
        AbilityData abilityData,
        AbilitySystem node
    )
    {
        // todo is in range should be handled somewhere else
        // && IsInRange(abilityData, caster, targetTile)
        // is valid target as well

        if (
            !CanActivateAbility(casterAttributeSet, casterAbilities, abilityData)
            || targetAttributeSets.Count == 0
        )
            return false;

        var ability = ActivateAbility(
            casterAttributeSet,
            targetPosition,
            abilityData,
            targetAttributeSets,
            node
        );
        return ability is not null;
    }

    private Ability ActivateAbility(
        AttributeSet casterAttributeSet,
        Vector3 targetPosition,
        AbilityData abilityData,
        List<AttributeSet> targetAttributeSets,
        AbilitySystem node
    )
    {
        CommitAbility(casterAttributeSet, abilityData);
        var abilityNode = abilityData.GetAbilityScene().Instantiate();
        node.AddChild(abilityNode);

        if (abilityNode is Ability ability)
        {
            ability.OnEffectTriggered += OnEffectTriggered(
                casterAttributeSet,
                abilityData,
                targetAttributeSets
            );
            ability.ActivateAbility(targetPosition);
            // events.EmitSignal("OnAbilityActivated");
            return ability;
        }

        return null;
    }

    private EventHandler OnEffectTriggered(
        AttributeSet casterAttributeSet,
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
                    ApplyEffectOnTarget(
                        casterAttributeSet,
                        target,
                        abilityData.GetEffects().ToList()
                    );
                }
            }
        };
    }

    public bool TryActivateAbilityWithoutAnimation(
        AttributeSet casterAttributeSet,
        List<AbilityData> casterAbilities,
        List<AttributeSet> targetAttributeSets,
        AbilityData abilityData
    )
    {
        // todo is in range should be handled somewhere else
        // && IsInRange(abilityData, caster, targetTile)
        // is valid target as well

        if (
            !CanActivateAbility(casterAttributeSet, casterAbilities, abilityData)
            || targetAttributeSets.Count == 0
        )
            return false;

        return ActivateAbilityWithoutAnimation(
            casterAttributeSet,
            abilityData,
            targetAttributeSets
        );
    }

    public void ApplyEffectOnSelf(AttributeSet caster, List<Effect> effects)
    {
        foreach (var effect in effects)
        {
            effect.ApplyEffect(caster, caster);
        }
    }

    public void ApplyEffectOnTarget(
        AttributeSet casterAttributeSet,
        AttributeSet targetAttributeSet,
        List<Effect> effects
    )
    {
        if (targetAttributeSet is null || !AreEffectsValid(casterAttributeSet, effects))
        {
            return;
        }

        foreach (var effect in effects)
        {
            effect.ApplyEffect(casterAttributeSet, targetAttributeSet);
        }
    }

    private bool AreEffectsValid(AttributeSet casterAttributeSet, List<Effect> effects)
    {
        var affectedAttributes = effects.SelectMany(e => e.GetAffectedAttributeNames()).ToList();
        if (!casterAttributeSet.HasAllAttributes(affectedAttributes))
        {
            return false;
        }

        return true;
    }

    private bool ActivateAbilityWithoutAnimation(
        AttributeSet casterAttributeSet,
        AbilityData abilityData,
        List<AttributeSet> targetAttributeSets
    )
    {
        CommitAbility(casterAttributeSet, abilityData);
        foreach (var targetAttributeSet in targetAttributeSets)
        {
            if (targetAttributeSet is not null)
            {
                ApplyEffectOnTarget(
                    casterAttributeSet,
                    targetAttributeSet,
                    abilityData.GetEffects().ToList()
                );
            }
        }
        return true;
    }

    private void CommitAbility(AttributeSet attributeSet, AbilityData ability)
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

    private bool HasAbility(List<AbilityData> abilities, AbilityData abilityData)
    {
        return abilities.Contains(abilityData);
    }
}
