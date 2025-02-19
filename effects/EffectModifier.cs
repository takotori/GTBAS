using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using Godot.Collections;
using Attribute = ProjectD.addons.gas.attributes.Attribute;

namespace ProjectD.addons.gas.effects;

[Tool]
[GlobalClass]
public partial class EffectModifier : Resource
{
    // todo value is currently hard coded, make it scale by something

    [Export] protected string affectedAttributeName;
    [Export] protected float value;
    [Export] protected OperationType operand;

    public void Operate(Attribute attribute)
    {
        switch (operand)
        {
            case OperationType.Add:
                attribute.SetCurrentValue(attribute.GetCurrentValue() + value);
                break;
            case OperationType.Multiply:
                attribute.SetCurrentValue(attribute.GetCurrentValue() * value);
                break;
            case OperationType.Divide:
                attribute.SetCurrentValue(Math.Abs(value) < float.Epsilon ? 0 : attribute.GetCurrentValue() / value);
                break;
            case OperationType.Percentage:
                attribute.SetCurrentValue(attribute.GetCurrentValue() + attribute.GetCurrentValue() / 100 * value);
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
                newValue = attribute.GetCurrentValue() + value;
                break;
            case OperationType.Multiply:
                newValue = attribute.GetCurrentValue() * value;
                break;
            case OperationType.Divide:
                newValue = Math.Abs(value) < float.Epsilon ? 0 : attribute.GetCurrentValue() / value;
                break;
            case OperationType.Percentage:
                newValue = attribute.GetCurrentValue() + attribute.GetCurrentValue() / 100 * value;
                break;
            case OperationType.Override:
                newValue = value;
                break;
        }

        if (Math.Abs(attribute.GetMaxValue() - -1) < 0.1f)
        {
            return newValue > attribute.GetMinValue();
        }

        return newValue > attribute.GetMinValue() && newValue <= attribute.GetMaxValue();
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
            // todo maybe do this in an autoload, so we don't have to do it every time
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(ass => ass.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Attribute)))
                .ToList();

            HashSet<string> attributeNames = [];
            foreach (var type in types)
            {
                attributeNames.Add(type
                    .GetField("attributeName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    ?.GetValue(Activator.CreateInstance(type)) as string);
            }

            property["hint"] = (int)PropertyHint.Enum;
            property["hint_string"] = string.Join(",", attributeNames);
        }
    }
}