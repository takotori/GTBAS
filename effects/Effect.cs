using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Attribute = ProjectD.addons.gas.attributes.Attribute;

namespace ProjectD.addons.gas.effects;

[Tool]
[GlobalClass]
public partial class Effect : Resource
{
    private EffectExecution _effectExecution;
    private int currentDuration;
    private int currentStacks;

    [Export]
    protected string effectName;

    [Export]
    protected int maxDuration;

    [Export]
    protected int maxStacks;

    [Export]
    protected EffectExecution effectExecution
    {
        get => _effectExecution;
        set
        {
            _effectExecution = value;
            NotifyPropertyListChanged();
        }
    }

    [Export]
    protected EffectModifier[] effectModifiers;

    public EffectCalculation EffectCalculation { get; set; }

    public void ApplyEffect(List<Attribute> attributes)
    {
        foreach (var effectModifier in effectModifiers)
        {
            var attribute = attributes.First(a =>
                a.GetAttributeName() == effectModifier.GetAffectedAttributeName()
            );
            effectModifier.Operate(attribute);
        }
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        var properties = new Array<Dictionary>();

        if (effectExecution == EffectExecution.Instant)
        {
            properties.Add(
                new Dictionary
                {
                    { "name", "EffectCalculation" },
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

    public EffectExecution GetEffectExecution()
    {
        return effectExecution;
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

    public void SetEffectExecution(EffectExecution newExecution)
    {
        effectExecution = newExecution;
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
