using System;
using Godot;
using ProjectD.scripts;
using ProjectD.scripts.events;
using ProjectD.scripts.maps;

namespace ProjectD.addons.gas.abilities;

[GlobalClass]
public partial class Ability : Node3D
{
    public event EventHandler OnEffectTriggered;

    protected NavigationController navigation;
    protected AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        navigation = ServiceContainer.GetService<NavigationController>();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.AnimationFinished += EndAbility;
    }

    public virtual void ActivateAbility(Vector2I targetIndex) { }

    public virtual void EndAbility(StringName animationName)
    {
        Events.Instance.EmitSignal("OnAbilityEnded");
    }

    protected Vector3 GetGlobalPosition(Vector2I targetIndex)
    {
        return navigation.ToGlobalPosition(targetIndex);
    }

    protected virtual void TriggerEffect()
    {
        OnEffectTriggered?.Invoke(this, EventArgs.Empty);
    }
}
