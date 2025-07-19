using System.Collections.Generic;
using System.Linq;
using ProjectD.addons.gas.abilities;
using ProjectD.addons.gas.attributes;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas;

// todo remove this class and use AbilitySystem directly
public class AbilitySystemService
{
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
