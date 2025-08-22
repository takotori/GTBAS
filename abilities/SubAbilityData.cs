using Godot;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas.abilities;

[GlobalClass]
public partial class SubAbilityData : Resource
{
    [Export]
    public string abilityName { get; private set; }

    [Export]
    public Effect[] effects { get; set; } = [];
    
    [Export]
    public AbilityPattern pattern { get; set; }

    [Export]
    public PackedScene ability { get; private set; }
    
    public override bool Equals(object other)
    {
        if (other is AbilityData otherAttribute)
        {
            return abilityName == otherAttribute.abilityName;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return abilityName.GetHashCode();
    }
}