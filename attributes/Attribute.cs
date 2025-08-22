using System;
using Godot;

namespace ProjectD.addons.gas.attributes;

[Tool]
[GlobalClass]
public partial class Attribute : Resource
{
    [Signal]
    public delegate void AttributeChangedEventHandler(
        Attribute attribute,
        float oldValue,
        float newValue
    );

    [Export]
    public string attributeName { get; protected set; }

    [Export]
    public float baseValue { get; set; }

    [Export]
    public float currentValue { get; set; }

    [Export]
    public float minValue { get; set; }

    [Export]
    public float maxValue { get; set; } = -1;

    public Attribute()
    {
        SetLocalToScene(true);
    }

    public void SetCurrentValue(float newValue)
    {
        var oldValue = currentValue;
        currentValue = newValue;
        if (Math.Abs(oldValue - newValue) > 0.1f)
        {
            EmitSignal(nameof(AttributeChanged), this, oldValue, newValue);
        }
    }

    public override bool Equals(object other)
    {
        if (other is Attribute otherAttribute)
        {
            return attributeName == otherAttribute.attributeName;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return attributeName.GetHashCode();
    }
}
