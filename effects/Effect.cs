using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using ProjectD.addons.gas.attributes;
using ProjectD.addons.gas.execution;

namespace ProjectD.addons.gas.effects;

[Tool]
[GlobalClass]
public partial class Effect : Resource
{
    private int currentDuration;
    private int currentStacks;

    [Export]
    public string effectName { get; private set; }

    [Export]
    public int maxDuration { get; private set; }

    [Export]
    public int maxStacks { get; private set; }

    [Export]
    public EffectTiming effectTiming { get; private set; }

    [Export]
    public EffectModifier[] effectModifiers { get; set; }

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

    public void ApplyEffect(AttributeSet casterAttributeSet, AttributeSet targetAttributeSet)
    {
        switch (effectCalculationType)
        {
            case EffectCalculationType.ScalableFloat:
                foreach (var effectModifier in effectModifiers)
                {
                    var attribute = targetAttributeSet.GetAttributeByName(
                        effectModifier.affectedAttributeName
                    );

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
                    effectCalculation.CalculateAndExecuteEffect(
                        casterAttributeSet,
                        targetAttributeSet,
                        effectModifier
                    );
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

    public HashSet<string> GetAffectedAttributeNames()
    {
        return effectModifiers.Select(e => e.affectedAttributeName).ToHashSet();
    }

    public override bool Equals(object other)
    {
        if (other is Effect otherAttribute)
        {
            return effectName == otherAttribute.effectName;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return effectName.GetHashCode();
    }
}
