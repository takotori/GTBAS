using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas.attributes;

[Tool]
[GlobalClass]
public partial class Attribute : Resource
{
    [Signal]
    public delegate void AttributeChangedEventHandler(Attribute attribute, float oldValue, float newValue);

    [Signal]
    public delegate void EffectAppliedEventHandler(Effect effect);

    [Signal]
    public delegate void EffectRemovedEventHandler(Effect effect);

    [Export] protected string attributeName;
    [Export] protected float baseValue;
    [Export] protected float currentValue;
    private List<Effect> effects;

    public Attribute()
    {
        effects = [];
    }
    
    public Attribute(string attributeName, float defaultValue) : this()
    {
        this.attributeName = attributeName;
        baseValue = defaultValue;
        currentValue = defaultValue;
    }

    public void AddEffect(Effect effect)
    {
        if (!CanReceiveEffect(effect)) return;
        switch (effect.GetEffectExecution())
        {
            case EffectExecution.Instant:
                currentValue = effect.ApplyEffect(currentValue);
                break;
            case EffectExecution.EndOfPlayerTurn:
            case EffectExecution.StartOfPlayerTurn:
                effects.Add(effect);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        EmitSignal("EffectApplied", effect);
    }

    public void RemoveEffect(Effect effect)
    {
        if (!HasEffect(effect)) return;

        // todo check if this works
        foreach (var effect1 in effects.ToList())
        {
            if (!effect1.Equals(effect)) continue;
            effects.Remove(effect);
            EmitSignal("EffectRemoved", effect);
        }
    }

    private bool CanReceiveEffect(Effect effect)
    {
        if (effect.GetIsUnique() && HasEffect(effect)) return false;

        var effectCount = effects.Count(effect1 => effect1.Equals(effect));
        if (effectCount >= effect.GetMaxStacks()) return false;

        return true;
    }

    public bool HasEffect(Effect effect)
    {
        return effects.Any(effect1 => effect1.Equals(effect));
    }

    public string GetAttributeName() => attributeName;

    public void SetAttributeName(string newName) => attributeName = newName;

    public float GetCurrentValue() => currentValue;

    public void SetCurrentValue(float newValue) => currentValue = newValue;

    public float GetBaseValue() => baseValue;

    public void SetBaseValue(float NewValue) => baseValue = NewValue;

    public List<Effect> GetEffects() => effects;

    public void SetEffects(List<Effect> newEffects) => effects = newEffects;

    protected bool Equals(Attribute other)
    {
        return attributeName == other.attributeName;
    }
}