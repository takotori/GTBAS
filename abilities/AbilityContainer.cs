using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectD.addons.gas.attributes;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas.abilities;

public partial class AbilityContainer : Node
{
    [Signal]
    public delegate void OnAbilityActivatedEventHandler(Ability ability);

    [Signal]
    public delegate void OnAbilityEndedEventHandler(Ability ability);

    [Export]
    public AttributeContainer attributeContainer;

    [Export]
    protected Ability[] defaultAbilities = [];
    public List<Ability> abilities = [];

    public override void _Ready()
    {
        foreach (var defaultAbility in defaultAbilities)
        {
            if (defaultAbility == null)
                continue;
            AddAbility(defaultAbility);
        }
    }

    public virtual bool TryActivateAbility(int abilityIndex)
    {
        return TryActivateAbility(GetAbilityByIndex(abilityIndex));
    }

    public virtual bool TryActivateAbility(string abilityName)
    {
        return TryActivateAbility(GetAbilityByName(abilityName));
    }

    protected virtual bool TryActivateAbility(Ability ability)
    {
        if (ability == null)
            return false;
        if (HasAbility(ability) && CanActivateAbility(ability))
        {
            CommitAbility(ability);
            ability.ActivateAbility(this);
            EmitSignal("OnAbilityActivated");
            return true;
        }

        return false;
    }

    public virtual void CommitAbility(Ability ability)
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

    public virtual bool CanActivateAbility(Ability ability)
    {
        if (ability.GetCurrentCooldown() > 0)
            return false;

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

    public bool AddAbility(Ability ability)
    {
        if (ability == null)
            return false;
        if (!HasAbility(ability))
        {
            ability.AbilityActivated += AbilityActivated;
            ability.AbilityEnded += AbilityEnded;
            abilities.Add(ability);
            return true;
        }

        return false;
    }

    public bool RemoveAbility(Ability ability)
    {
        if (ability == null)
            return false;
        if (HasAbility(ability))
        {
            ability.AbilityActivated -= AbilityActivated;
            ability.AbilityEnded -= AbilityEnded;
            abilities.Remove(ability);
            return true;
        }

        return false;
    }

    public Ability GetAbilityByName(string name)
    {
        return abilities.FirstOrDefault(a => a.GetAbilityName() == name);
    }

    public Ability GetAbilityByIndex(int index)
    {
        if (index < 0 || index >= abilities.Count)
            return null;
        return abilities[index];
    }

    protected bool HasAbility(Ability ability)
    {
        return abilities.Any(a => a.Equals(ability));
    }

    private void AbilityActivated(Ability ability)
    {
        EmitSignal("OnAbilityActivated", ability);
    }

    private void AbilityEnded(Ability ability)
    {
        EmitSignal("OnAbilityEnded", ability);
    }

    public AttributeContainer GetAttributeContainer()
    {
        return attributeContainer;
    }

    public virtual Ability[] GetAbilities()
    {
        return abilities.ToArray();
    }
}
