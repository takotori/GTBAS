using Godot;
using ProjectD.addons.gas.attributes;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas.execution;

[Tool]
[GlobalClass]
public partial class EffectCalculation : Resource
{
    public virtual EffectCalculationOutput CalculateEffect(
        AttributeSet casterAttributeSet,
        AttributeSet targetAttributeSet,
        EffectModifier effectModifier
    )
    {
        return new EffectCalculationOutput();
    }

    public virtual void CalculateAndExecuteEffect(
        AttributeSet casterAttributeSet,
        AttributeSet targetAttributeSet,
        EffectModifier effectModifier
    )
    {
        CalculateEffect(casterAttributeSet, targetAttributeSet, effectModifier);
    }
}
