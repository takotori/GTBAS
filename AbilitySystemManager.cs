using System.Collections.Generic;
using Godot;
using ProjectD.addons.gas.abilities;
using ProjectD.addons.gas.attributes;
using ProjectD.scripts.events;
using ProjectD.scripts.units;

namespace ProjectD.addons.gas;

public partial class AbilitySystemManager : Node
{
    public static AbilitySystemManager Instance { get; private set; }
    
    private AbilitySystemEvents abilitySystemEvents;
    private Events events;
    
    public Stack<AttributeChanged> attributesChangedThisTurn = new();
    public Stack<AbilityActivated> abilitiesActivatedThisTurn = new();

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        abilitySystemEvents = AbilitySystemEvents.Instance;
        events = Events.Instance;
        
        events.OnTurnEnded += OnTurnEnded;
        abilitySystemEvents.OnAttributeChanged += OnAttributeChanged;
        abilitySystemEvents.OnAbilityActivated += OnAbilityActivated;
    }

    private void OnAbilityActivated(Unit caster, AbilityData ability)
    {
        abilitiesActivatedThisTurn.Push(new AbilityActivated(caster, ability));
    }

    private void OnAttributeChanged(Unit owner, Attribute attribute, float oldValue, float newValue)
    {
        // todo calculate some stuff based on attribute changed like percentage damage etc.
        attributesChangedThisTurn.Push(new AttributeChanged(owner, attribute, oldValue, newValue));
    }
    
    private void OnTurnEnded()
    {
        attributesChangedThisTurn.Clear();
        abilitiesActivatedThisTurn.Clear();
    }
}

public record AttributeChanged(Unit owner, Attribute attribute, float oldValue, float newValue);
public record AbilityActivated(Unit caster, AbilityData ability);