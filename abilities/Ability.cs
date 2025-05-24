using Godot;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas.abilities;

[Tool]
[GlobalClass]
public partial class Ability : Resource
{
    [Export]
    protected string abilityName;

    [Export]
    protected Effect[] effects = [];

    [Export]
    protected Effect[] costs = [];

    [Export]
    protected int cooldown;

    private int currentCooldown;

    public virtual void ActivateAbility() { }

    public virtual void EndAbility() { }

    public string GetAbilityName()
    {
        return abilityName;
    }

    public Effect[] GetEffects()
    {
        return effects;
    }

    public Effect[] GetCosts()
    {
        return costs;
    }

    public int GetCooldown()
    {
        return cooldown;
    }

    public int GetCurrentCooldown()
    {
        return currentCooldown;
    }

    public void SetAbilityName(string newName)
    {
        abilityName = newName;
    }

    public void SetEffects(Effect[] newEffects)
    {
        effects = newEffects;
    }

    public void SetCosts(Effect[] newCosts)
    {
        costs = newCosts;
    }

    public void SetCooldown(int newCooldown)
    {
        cooldown = newCooldown;
    }

    public void SetCurrentCooldown(int newCooldown)
    {
        currentCooldown = newCooldown;
    }

    public bool Equals(Ability other)
    {
        return abilityName == other.abilityName;
    }
}
