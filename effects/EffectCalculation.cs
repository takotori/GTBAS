using Godot;

namespace ProjectD.addons.gas.effects;

[GlobalClass]
public partial class EffectCalculation : Resource
{
    public virtual void CalculateEffect(
        AbilitySystem caster,
        AbilitySystem target,
        EffectModifier effectModifier
    )
    {
        GD.Print("EffectCalculation: CalculateEffect called, but not implemented.");
    }

    public virtual void CalculateAndExecuteEffect(
        AbilitySystem caster,
        AbilitySystem target,
        EffectModifier effectModifier
    ) { }
}
