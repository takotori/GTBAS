using System;
using Godot;

namespace ProjectD.addons.gas.abilities;

public interface IAbility
{
    public event EventHandler OnEffectTriggered;

    public void ActivateAbility(Vector3 position);

    protected void TriggerEffect();

    public void EndAbility();
}
