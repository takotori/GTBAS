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
    
    public string GetAttributeName() => attributeName;

    public void SetAttributeName(string newName) => attributeName = newName;

    public float GetCurrentValue() => currentValue;

    public void SetCurrentValue(float newValue)
    {
        var oldValue = currentValue;
        currentValue = newValue;
        if (Math.Abs(oldValue - newValue) > 0.1f)
        {
            EmitSignal("AttributeChanged", this, oldValue, newValue);
        }
    }

    public float GetBaseValue() => baseValue;

    public void SetBaseValue(float NewValue) => baseValue = NewValue;

    protected bool Equals(Attribute other)
    {
        return attributeName == other.attributeName;
    }
}