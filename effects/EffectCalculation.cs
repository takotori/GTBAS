using Godot;
using ProjectD.addons.gas.abilities;
using ProjectD.addons.gas.attributes;

namespace ProjectD.addons.gas.effects;

[GlobalClass]
public partial class EffectCalculation : Resource
{
    public virtual void CalculateEffect(
        AttributeContainer caster,
        AttributeContainer target,
        AbilityData ability
    )
    {
        // This method should be overridden in derived classes to implement specific effect calculations.
        GD.Print("EffectCalculation: CalculateEffect called, but not implemented.");
    }

    public virtual void CalculateAndExecuteEffect(
        AttributeContainer caster,
        AttributeContainer target,
        AbilityData ability
    )
    {
        CalculateEffect(caster, target, ability);
        // This method should be overridden in derived classes to implement specific effect calculations and execution.
        GD.Print("EffectCalculation: CalculateAndExecuteEffect called, but not implemented.");
    }
}
