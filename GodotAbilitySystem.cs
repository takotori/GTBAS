#if TOOLS
using Godot;

[Tool]
public partial class GodotAbilitySystem : EditorPlugin
{
    public override void _EnterTree()
    {
        base._EnterTree();

        var attributeContainerScript = GD.Load<Script>("res://addons/gas/AttributeContainer.cs");
        var attributeContainerIcon = GD.Load<Texture2D>("res://addons/gas/Node.png");
        AddCustomType("AttributeContainer", "Node", attributeContainerScript, attributeContainerIcon);
    }

    public override void _ExitTree()
    {
        RemoveCustomType("AttributeContainer");
    }
}
#endif