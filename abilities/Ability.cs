using Godot;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas.abilities;

[Tool]
[GlobalClass]
public partial class Ability : Resource
{
    [Signal]
    public delegate void AbilityActivatedEventHandler(Ability ability);

    [Signal]
    public delegate void AbilityEndedEventHandler(Ability ability);

    [Export] protected string abilityName;
    [Export] protected Effect[] effects = [];
    [Export] protected Effect[] costs = [];
    [Export] protected int cooldown;
    [Export] protected AbilityPattern pattern;

    private int currentCooldown;

    public virtual void ActivateAbility(AbilityContainer abilityContainer)
    {
        EmitSignal("AbilityActivated", this);
    }

    public virtual void EndAbility()
    {
        EmitSignal("AbilityEnded", this);
    }

    public string GetAbilityName() => abilityName;

    public Effect[] GetEffects() => effects;

    public Effect[] GetCosts() => costs;

    public int GetCooldown() => cooldown;

    public int GetCurrentCooldown() => currentCooldown;
    
    public AbilityPattern GetPattern() => pattern;

    public void SetAbilityName(string newName) => abilityName = newName;

    public void SetEffects(Effect[] newEffects) => effects = newEffects;

    public void SetCosts(Effect[] newCosts) => costs = newCosts;

    public void SetCooldown(int newCooldown) => cooldown = newCooldown;

    public void SetCurrentCooldown(int newCooldown) => currentCooldown = newCooldown;
    
    public void SetPattern(AbilityPattern newPattern) => pattern = newPattern;

    public bool Equals(Ability other)
    {
        return abilityName == other.abilityName;
    }
}