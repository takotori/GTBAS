using Godot;
using ProjectD.addons.gas.abilities;
using ProjectD.addons.gas.attributes;

namespace ProjectD.addons.gas;

[GlobalClass]
public partial class AbilitySystem : Node
{
    [Export]
    private AttributeContainer attributeContainer;
    
    [Export]
    private AbilityContainer abilityContainer;
}