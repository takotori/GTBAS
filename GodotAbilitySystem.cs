#if TOOLS
using Godot;

[Tool]
public partial class GodotAbilitySystem : EditorPlugin
{
    public override void _EnterTree()
    {
        base._EnterTree();
        var icon = GD.Load<Texture2D>("res://addons/gas/Node.png");

        var attributeContainerScript = GD.Load<Script>(
            "res://addons/gas/attributes/AttributeContainer.cs"
        );
        AddCustomType("AttributeContainer", "Node", attributeContainerScript, icon);

        var abilityContainerScript = GD.Load<Script>(
            "res://addons/gas/abilities/AbilityContainer.cs"
        );
        AddCustomType("AbilityContainer", "Node", abilityContainerScript, icon);

        var abilitySystemScript = GD.Load<Script>("res://addons/gas/abilities/AbilitySystem.cs");
        AddCustomType("AbilitySystem", "Node", abilitySystemScript, icon);
    }

    public override void _ExitTree()
    {
        RemoveCustomType("AttributeContainer");
        RemoveCustomType("AbilityContainer");
        RemoveCustomType("AbilitySystem");
    }
}
#endif
