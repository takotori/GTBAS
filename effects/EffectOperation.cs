using System;
using Godot;

namespace ProjectD.addons.gas.effects;

[Tool]
[GlobalClass]
public partial class EffectOperation : Resource
{
    protected OperationType operand;
    protected float value;

    public EffectOperation(OperationType operand, float value)
    {
        this.operand = operand;
        this.value = value;
    }

    public float Operate(float baseValue)
    {
        switch (operand)
        {
            case OperationType.Add:
                return baseValue + value;
            case OperationType.Multiply:
                return baseValue * value;
            case OperationType.Divide:
                return Math.Abs(value) < float.Epsilon ? 0 : baseValue / value;
            case OperationType.Percentage:
                return baseValue + baseValue / 100 * value;
            case OperationType.Override:
                return value;
            default:
                return baseValue;
        }
    }

    public OperationType GetOperand() => operand;

    public float GetValue() => value;

    public void SetOperand(OperationType newOperand) => operand = newOperand;

    public void SetValue(float newValue) => value = newValue;

    public static EffectOperation Add(float value) => new EffectOperation(OperationType.Add, value);
}