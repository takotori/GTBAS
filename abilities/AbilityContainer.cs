using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectD.addons.gas.attributes;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas.abilities;

public partial class AbilityContainer : Node
{
    [Export]
    public AttributeContainer attributeContainer;

    [Export]
    private AbilityData[] defaultAbilities = [];

    public List<AbilityData> abilities = [];

    public override void _Ready()
    {
        foreach (var defaultAbility in defaultAbilities)
        {
            if (defaultAbility == null)
                continue;
            AddAbility(defaultAbility);
        }
    }

    public virtual bool TryActivateAbility(AbilityData abilityData)
    {
        return false;
    }

    public virtual void CommitAbility(AbilityData ability)
    {
        foreach (var abilityCost in ability.GetCosts())
        {
            var attributes = attributeContainer.GetAttributesByName(
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

    public virtual bool CanActivateAbility(AbilityData ability)
    {
        foreach (var abilityCost in ability.GetCosts())
        {
            var attributes = attributeContainer.GetAttributesByName(
                abilityCost.GetAffectedAttributeNames()
            );
            if (attributes.Count != abilityCost.GetAffectedAttributeNames().Count)
                return false;

            foreach (var effectCost in abilityCost.GetEffectModifiers())
            {
                foreach (var attribute in attributes)
                {
                    if (!effectCost.CanOperate(attribute))
                        return false;
                }
            }
        }

        return true;
    }

    public void ApplyEffectsToSelf(Effect[] effects)
    {
        attributeContainer.ApplyEffects(effects);
    }

    public void ApplyEffectsToTarget(AbilityContainer target, Effect[] effects)
    {
        target.GetAttributeContainer().ApplyEffects(effects);
    }

    public bool AddAbility(AbilityData ability)
    {
        if (ability == null)
            return false;
        if (!HasAbility(ability))
        {
            abilities.Add(ability);
            return true;
        }

        return false;
    }

    public bool RemoveAbility(AbilityData ability)
    {
        if (ability == null)
            return false;
        if (HasAbility(ability))
        {
            abilities.Remove(ability);
            return true;
        }

        return false;
    }

    public AbilityData GetAbilityByIndex(int index)
    {
        if (index < 0 || index >= abilities.Count)
            return null;
        return abilities[index];
    }

    protected bool HasAbility(AbilityData ability)
    {
        return abilities.Any(a => a.Equals(ability));
    }

    private void AbilityActivated(Ability ability) { }

    private void AbilityEnded(Ability ability) { }

    public AttributeContainer GetAttributeContainer()
    {
        return attributeContainer;
    }

    public virtual AbilityData[] GetAbilities()
    {
        return abilities.ToArray();
    }
}
