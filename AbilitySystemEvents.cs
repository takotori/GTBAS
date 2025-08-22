using Godot;
using ProjectD.addons.gas.abilities;
using ProjectD.addons.gas.attributes;
using ProjectD.scripts.units;

namespace ProjectD.addons.gas;

public partial class AbilitySystemEvents : Node
{
    // Attribute System
    [Signal]
    public delegate void OnAttributeChangedEventHandler(Unit owner, Attribute attribute, float oldValue, float newValue);

    // Ability System
    [Signal]
    public delegate void OnAbilityActivatedEventHandler(Unit caster, AbilityData ability);
    
    [Signal]
    public delegate void OnAbilityEndedEventHandler();
    
    // Tracking
    [Signal]
    public delegate void OnAbilityKillEventHandler(Unit caster, Unit killedTarget, AbilityData ability);

    public static AbilitySystemEvents Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }
}