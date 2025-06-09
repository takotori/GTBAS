using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using ProjectD.addons.gas.execution;

namespace ProjectD.addons.gas.effects;

[Tool]
[GlobalClass]
public partial class Effect : Resource
{
    private int currentDuration;
    private int currentStacks;

    [Export]
    protected string effectName;

    [Export]
    protected int maxDuration;

    [Export]
    protected int maxStacks;

    [Export]
    protected EffectTiming effectTiming;

    [Export]
    protected EffectModifier[] effectModifiers;

    [Export]
    public EffectCalculationType effectCalculationType
    {
        get => _effectCalculationType;
        set
        {
            _effectCalculationType = value;
            NotifyPropertyListChanged();
        }
    }

    public EffectCalculation effectCalculation { get; set; }

    private EffectCalculationType _effectCalculationType;

    public void ApplyEffect(AbilitySystem caster, AbilitySystem target)
    {
        switch (effectCalculationType)
        {
            case EffectCalculationType.ScalableFloat:
                foreach (var effectModifier in effectModifiers)
                {
                    var attribute = target
                        .GetAttributeSet()
                        .GetAttributeByName(effectModifier.GetAffectedAttributeName());

                    effectModifier.Operate(attribute);
                }
                break;
            case EffectCalculationType.CustomCalculationClass:
                if (effectCalculation is null)
                {
                    return;
                }

                foreach (var effectModifier in effectModifiers)
                {
                    effectCalculation.CalculateAndExecuteEffect(caster, target, effectModifier);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        var properties = new Array<Dictionary>();

        if (effectCalculationType == EffectCalculationType.CustomCalculationClass)
        {
            properties.Add(
                new Dictionary
                {
                    { "name", nameof(effectCalculation) },
                    { "type", (int)Variant.Type.Object },
                    { "usage", (int)PropertyUsageFlags.Default },
                    { "hint", (int)PropertyHint.ResourceType },
                    { "hint_string", nameof(EffectCalculation) },
                }
            );
        }

        return properties;
    }

    public string GetEffectName()
    {
        return effectName;
    }

    public HashSet<string> GetAffectedAttributeNames()
    {
        return effectModifiers.Select(e => e.GetAffectedAttributeName()).ToHashSet();
    }

    public int GetCurrentDuration()
    {
        return currentDuration;
    }

    public int GetMaxDuration()
    {
        return maxDuration;
    }

    public int GetCurrentStacks()
    {
        return currentStacks;
    }

    public int GetMaxStacks()
    {
        return maxStacks;
    }

    public EffectTiming GetEffectExecution()
    {
        return effectTiming;
    }

    public EffectModifier[] GetEffectModifiers()
    {
        return effectModifiers;
    }

    public void SetEffectName(string newName)
    {
        effectName = newName;
    }

    public void SetCurrentDuration(int newDuration)
    {
        currentDuration = newDuration;
    }

    public void SetMaxDuration(int newDuration)
    {
        maxDuration = newDuration;
    }

    public void SetCurrentStacks(int newCurrentStacks)
    {
        currentStacks = newCurrentStacks;
    }

    public void SetMaxStacks(int newMaxStacks)
    {
        maxStacks = newMaxStacks;
    }

    public void SetEffectExecution(EffectTiming newTiming)
    {
        effectTiming = newTiming;
    }

    public void SetEffectModifiers(EffectModifier[] newModifiers)
    {
        effectModifiers = newModifiers;
    }

    public bool Equals(Effect other)
    {
        return effectName == other.effectName;
    }
}
