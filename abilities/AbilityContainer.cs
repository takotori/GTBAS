using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectD.addons.gas.attributes;

namespace ProjectD.addons.gas.abilities;

public partial class AbilityContainer : Node
{
    [Signal]
    public delegate void OnAbilityActivatedEventHandler(Ability ability);

    [Signal]
    public delegate void OnAbilityEndedEventHandler(Ability ability);

    [Export] public AttributeContainer attributeContainer;

    [Export] protected Ability[] defaultAbilities = [];
    public List<Ability> abilities = [];

    public override void _Ready()
    {
        foreach (var defaultAbility in defaultAbilities)
        {
            if (defaultAbility == null) continue;
            AddAbility(defaultAbility);
        }
    }

    public bool ActivateAbility(string abilityName)
    {
        var ability = FindAbilityByName(abilityName);
        if (ability == null) return false;
        if (HasAbility(ability) && CanActivateAbility(ability))
        {
            ability.ActivateAbility();
            EmitSignal("OnAbilityActivated");
            return true;
        }

        return false;
    }

    private bool CanActivateAbility(Ability ability)
    {
        if (ability.GetCurrentCooldown() > 0) return false;

        foreach (var abilityCost in ability.GetCosts())
        {
            var attributes = attributeContainer.FindAttributesByName(abilityCost.GetAffectedAttributeNames());
            if (attributes.Count != abilityCost.GetAffectedAttributeNames().Count) return false;

            foreach (var effectCost in abilityCost.GetEffectModifiers())
            {
                foreach (var attribute in attributes)
                {
                    if (!effectCost.CanOperate(attribute)) return false;
                }
            }
        }

        return true;
    }

    public bool AddAbility(Ability ability)
    {
        if (ability == null) return false;
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
        if (ability == null) return false;
        if (HasAbility(ability))
        {
            ability.AbilityActivated -= AbilityActivated;
            ability.AbilityEnded -= AbilityEnded;
            abilities.Remove(ability);
            return true;
        }

        return false;
    }

    public Ability FindAbilityByName(string name)
    {
        return abilities.FirstOrDefault(a => a.GetAbilityName() == name);
    }

    private bool HasAbility(Ability ability)
    {
        return abilities.Any(a => a.Equals(ability));
    }

    private void AbilityActivated(Ability ability)
    {
        EmitSignal("OnAbilityActivated");
    }

    private void AbilityEnded(Ability ability)
    {
        EmitSignal("OnAbilityEnded");
    }

    public AttributeContainer GetAttributeContainer() => attributeContainer;

    public Ability[] GetAbilities() => abilities.ToArray();
}