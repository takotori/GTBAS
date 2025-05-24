using System;
using Godot;
using ProjectD.scripts.events;

namespace ProjectD.addons.gas.abilities;

[Tool]
[GlobalClass]
public partial class Ability : Node3D, IAbility
{
    public event EventHandler OnEffectTriggered;

    public void ActivateAbility(Vector3 position) { }

    public void TriggerEffect() { }

    public void EndAbility()
    {
        Events.Instance.EmitSignal("OnAbilityEnded");
    }
}
