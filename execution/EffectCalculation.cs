using Godot;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas.execution;

[Tool]
[GlobalClass]
public partial class EffectCalculation : Resource
{
    public virtual EffectCalculationOutput CalculateEffect(
        AbilitySystem caster,
        AbilitySystem target,
        EffectModifier effectModifier
    )
    {
        return new EffectCalculationOutput();
    }

    public virtual void CalculateAndExecuteEffect(
        AbilitySystem caster,
        AbilitySystem target,
        EffectModifier effectModifier
    )
    {
        CalculateEffect(caster, target, effectModifier);
    }
}
