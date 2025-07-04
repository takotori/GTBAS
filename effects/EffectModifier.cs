﻿using System;
using Godot;
using Godot.Collections;
using ProjectD.scripts;
using Attribute = ProjectD.addons.gas.attributes.Attribute;

namespace ProjectD.addons.gas.effects;

[Tool]
[GlobalClass]
public partial class EffectModifier : Resource
{
    // todo value is currently hard coded, make it scale by something

    [Export]
    protected string affectedAttributeName;

    [Export]
    protected float value;

    [Export]
    protected OperationType operand;

    public void Operate(Attribute attribute)
    {
        switch (operand)
        {
            case OperationType.Add:
                attribute.SetCurrentValue(attribute.currentValue + value);
                break;
            case OperationType.Multiply:
                attribute.SetCurrentValue(attribute.currentValue * value);
                break;
            case OperationType.Divide:
                attribute.SetCurrentValue(
                    Math.Abs(value) < float.Epsilon ? 0 : attribute.currentValue / value
                );
                break;
            case OperationType.Percentage:
                attribute.SetCurrentValue(
                    attribute.currentValue + attribute.currentValue / 100 * value
                );
                break;
            case OperationType.Override:
                attribute.SetCurrentValue(value);
                break;
        }
    }

    public bool CanOperate(Attribute attribute)
    {
        var newValue = 0f;
        switch (operand)
        {
            case OperationType.Add:
                newValue = attribute.currentValue + value;
                break;
            case OperationType.Multiply:
                newValue = attribute.currentValue * value;
                break;
            case OperationType.Divide:
                newValue = Math.Abs(value) < float.Epsilon ? 0 : attribute.currentValue / value;
                break;
            case OperationType.Percentage:
                newValue = attribute.currentValue + attribute.currentValue / 100 * value;
                break;
            case OperationType.Override:
                newValue = value;
                break;
        }

        if (Math.Abs(attribute.maxValue - -1) < 0.1f)
        {
            return newValue > attribute.minValue;
        }

        return newValue > attribute.minValue && newValue <= attribute.maxValue;
    }

    public string GetAffectedAttributeName()
    {
        return affectedAttributeName;
    }

    public OperationType GetOperand()
    {
        return operand;
    }

    public float GetValue()
    {
        return value;
    }

    public void SetAffectedAttributeName(string newAttribute)
    {
        affectedAttributeName = newAttribute;
    }

    public void SetOperand(OperationType newOperand)
    {
        operand = newOperand;
    }

    public void SetValue(float newValue)
    {
        value = newValue;
    }

    public override void _ValidateProperty(Dictionary property)
    {
        if (property["name"].AsStringName() == PropertyName.affectedAttributeName)
        {
            property["hint"] = (int)PropertyHint.Enum;
            property["hint_string"] = string.Join(",", Constants.Attributes);
        }
    }
}

public enum OperationType
{
    Add,
    Multiply,
    Divide,
    Percentage,
    Override,
}
