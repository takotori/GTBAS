using System;
using Godot;
using ProjectD.scripts.events;

namespace ProjectD.addons.gas.abilities;

[GlobalClass]
public partial class Ability : Node3D
{
    public event EventHandler OnEffectTriggered;

    protected AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.AnimationFinished += EndAbility;
    }

    public virtual void ActivateAbility(Vector3 position) { }

    virtual protected void TriggerEffect()
    {
        OnEffectTriggered?.Invoke(this, EventArgs.Empty);
    }

    public virtual void EndAbility(StringName animationName)
    {
        Events.Instance.EmitSignal("OnAbilityEnded");
    }
}
