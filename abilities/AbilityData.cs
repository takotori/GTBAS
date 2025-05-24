using Godot;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas.abilities;

[GlobalClass]
public partial class AbilityData : Resource
{
    [Export]
    private string abilityName;

    [Export]
    private Effect[] effects = [];

    [Export]
    private Effect[] costs = [];

    [Export]
    private AbilityPattern pattern;

    [Export]
    private PackedScene ability;

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

    public AbilityPattern GetPattern()
    {
        return pattern;
    }

    public PackedScene GetAbilityScene()
    {
        return ability;
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

    public void SetPattern(AbilityPattern newPattern)
    {
        pattern = newPattern;
    }
}
