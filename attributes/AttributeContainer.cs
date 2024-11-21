using System.Collections.Generic;
using Godot;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas.attributes;

[Tool]
[GlobalClass]
public partial class AttributeContainer : Node
{
    [Signal]
    public delegate void OnAttributeChangedEventHandler(Attribute attribute, float oldValue, float newValue);

    [Signal]
    public delegate void OnEffectAppliedEventHandler(Effect effect);

    [Signal]
    public delegate void OnEffectRemovedEventHandler(Effect effect);

    [Export] protected AttributeSet attributeSet;
    protected Dictionary<string, Attribute> attributes;

    public override void _Ready()
    {
        Setup();
    }

    private void Setup()
    {
        attributes.Clear();
        if (attributeSet != null)
        {
            foreach (var attribute in attributeSet.GetAttributes())
            {
                AddAttribute(attribute);
            }
        }
    }

    private void AddAttribute(Attribute attribute)
    {
        if (attribute == null) return;

        if (!HasAttribute(attribute))
        {
            attribute.AttributeChanged += AttributeChanged;
            attribute.EffectApplied += EffectApplied;
            attribute.EffectRemoved += EffectRemoved;
            attributes.Add(attribute.GetAttributeName(), attribute);
        }
    }

    private void RemoveAttribute(Attribute attribute)
    {
        if (attribute == null) return;
        if (HasAttribute(attribute))
        {
            attribute.AttributeChanged -= AttributeChanged;
            attribute.EffectApplied -= EffectApplied;
            attribute.EffectRemoved -= EffectRemoved;
            attributes.Remove(attribute.GetAttributeName());
        }
    }

    private void ApplyEffect(Effect effect)
    {
        if (effect == null) return;
        var attribute = FindAttributeByName(effect.GetAffectedAttributeName());
        attribute?.AddEffect(effect);
    }
    
    private void RemoveEffect(Effect effect)
    {
        if (effect == null) return;
        foreach (var attribute in attributes.Values)
        {
            attribute.RemoveEffect(effect);
        }
    }

    private Attribute FindAttributeByName(string name)
    {
        return attributes.TryGetValue(name, out var attribute) ? attribute : null;
    }

    private void AttributeChanged(Attribute attribute, float oldValue, float newValue)
    {
        EmitSignal("OnAttributeChanged", attribute, oldValue, newValue);
    }

    private void EffectApplied(Effect effect)
    {
        EmitSignal("OnEffectApplied", effect);
    }

    private void EffectRemoved(Effect effect)
    {
        EmitSignal("OnEffectRemoved", effect);
    }

    private bool HasAttribute(Attribute attribute)
    {
        return attributes.ContainsKey(attribute.GetAttributeName());
    }
    
    public AttributeSet GetAttributeSet() => attributeSet;
}