using System;
using Godot;

namespace ProjectD.addons.gas.attributes;

[Tool]
[GlobalClass]
public partial class Attribute : Resource
{
    [Signal]
    public delegate void AttributeChangedEventHandler(Attribute attribute, float oldValue, float newValue);

    [Export] protected string attributeName;
    [Export] protected float baseValue;
    [Export] protected float currentValue;
    [Export] protected float minValue;
    [Export] protected float maxValue = -1;

    public string GetAttributeName()
    {
        return attributeName;
    }

    public float GetBaseValue()
    {
        return baseValue;
    }

    public float GetCurrentValue()
    {
        return currentValue;
    }

    public float GetMinValue()
    {
        return minValue;
    }

    public float GetMaxValue()
    {
        return maxValue;
    }

    public void SetAttributeName(string newName)
    {
        attributeName = newName;
    }

    public void SetCurrentValue(float newValue)
    {
        var oldValue = currentValue;
        currentValue = newValue;
        if (Math.Abs(oldValue - newValue) > 0.1f)
        {
            EmitSignal("AttributeChanged", this, oldValue, newValue);
        }
    }

    public void SetBaseValue(float NewValue)
    {
        baseValue = NewValue;
    }

    public void SetMinValue(float newValue)
    {
        minValue = newValue;
    }

    public void SetMaxValue(float newValue)
    {
        maxValue = newValue;
    }

    protected bool Equals(Attribute other)
    {
        return attributeName == other.attributeName;
    }
}