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

    private AbilitySystemService abilitySystemService;

    private Node owner;

    public override void _Ready()
    {
        attributeSet.Init();
        abilitySystemService = new AbilitySystemService();

        foreach (var defaultAbility in defaultAbilities)
        {
            AddAbility(defaultAbility);
        }

        Init();
    }

    protected virtual void Init()
    {
        owner = GetParent();
    }

    public void AddAbility(AbilityData abilityData)
    {
        abilitySystemService.AddAbility(abilities, abilityData);
    }

    public void RemoveAbility(string abilityName)
    {
        abilitySystemService.RemoveAbility(abilities, abilityName);
    }

    public AbilityData GetAbility(string abilityName)
    {
        return abilitySystemService.GetAbility(abilities, abilityName);
    }

    public AbilityData GetAbility(int index)
    {
        return abilitySystemService.GetAbility(abilities, index);
    }

    public List<AbilityData> GetAvailableAbilities()
    {
        return abilities.Where(CanActivateAbility).ToList();
    }

    public bool CanActivateAbility(string abilityName)
    {
        return abilitySystemService.CanActivateAbility(attributeSet, abilities, abilityName);
    }

    public bool CanActivateAbility(AbilityData abilityData)
    {
        return abilitySystemService.CanActivateAbility(attributeSet, abilities, abilityData);
    }

    public bool TryActivateAbility(
        Vector3 targetPosition,
        List<AttributeSet> targetAttributeSets,
        AbilityData abilityData
    )
    {
        return abilitySystemService.TryActivateAbility(
            attributeSet,
            abilities,
            targetPosition,
            targetAttributeSets,
            abilityData,
            this
        );
    }

    public bool TryActivateAbilityWithoutAnimation(
        List<AttributeSet> targetAttributeSets,
        AbilityData abilityData
    )
    {
        return abilitySystemService.TryActivateAbilityWithoutAnimation(
            attributeSet,
            abilities,
            targetAttributeSets,
            abilityData
        );
    }

    public void ApplyEffectOnSelf(List<Effect> effects)
    {
        abilitySystemService.ApplyEffectOnSelf(attributeSet, effects);
    }

    public void ApplyEffectOnTarget(AttributeSet targetAttributeSet, List<Effect> effects)
    {
        abilitySystemService.ApplyEffectOnTarget(attributeSet, targetAttributeSet, effects);
    }

    public List<AbilityData> GetAbilities()
    {
        return abilities;
    }

    public AttributeSet GetAttributeSet()
    {
        return attributeSet;
    }

    public virtual Node GetOwnerActor()
    {
        return owner;
    }
}
