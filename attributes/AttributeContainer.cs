using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectD.addons.gas.effects;

namespace ProjectD.addons.gas.attributes;

[GlobalClass]
public partial class AttributeContainer : Node
{
    [Signal]
    public delegate void OnAttributeChangedEventHandler(
        Attribute attribute,
        float oldValue,
        float newValue
    );

    [Signal]
    public delegate void OnEffectAppliedEventHandler(Effect effect);

    [Signal]
    public delegate void OnEffectRemovedEventHandler(Effect effect);

    [Export]
    protected AttributeSet attributeSet;
    private Dictionary<string, Attribute> attributes = new();
    protected List<Effect> effects;

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
        if (attribute == null)
            return;

        if (!HasAttribute(attribute))
        {
            attribute.AttributeChanged += AttributeChanged;
            attributes.Add(attribute.GetAttributeName(), attribute);
        }
    }

    private void RemoveAttribute(Attribute attribute)
    {
        if (attribute == null)
            return;
        if (HasAttribute(attribute))
        {
            attribute.AttributeChanged -= AttributeChanged;
            attributes.Remove(attribute.GetAttributeName());
        }
    }

    public void ApplyEffect(Effect effect)
    {
        switch (effect.GetEffectExecution())
        {
            case EffectExecution.Instant:
                var list = effect.GetAffectedAttributeNames().Select(GetAttributeByName).ToList();
                effect.ApplyEffect(list);
                break;
            case EffectExecution.EndOfPlayerTurn:
            case EffectExecution.StartOfPlayerTurn:
                if (HasEffect(effect))
                {
                    var existingEffect = effects.First(e => e.Equals(effect));
                    existingEffect.SetCurrentDuration(existingEffect.GetMaxDuration());
                }
                else
                {
                    effects.Add(effect);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        EmitSignal("OnEffectApplied", effect);
    }

    public void ApplyEffects(Effect[] effects)
    {
        foreach (var effect in effects)
        {
            ApplyEffect(effect);
        }
    }

    public void RemoveEffect(Effect effect)
    {
        // todo create function to reduce duration
        if (!HasEffect(effect))
            return;

        effects.Remove(effect);

        // todo check if this works
        foreach (var effect1 in effects.ToList())
        {
            if (!effect1.Equals(effect))
                continue;
            effects.Remove(effect);
            EmitSignal("EffectRemoved", effect);
        }
    }

    public Attribute GetAttributeByName(string name)
    {
        return attributes.TryGetValue(name, out var attribute) ? attribute : null;
    }

    public List<Attribute> GetAttributesByName(HashSet<string> name)
    {
        return name.Select(GetAttributeByName).ToList();
    }

    private bool HasEffect(Effect effect)
    {
        return effects.Any(effect1 => effect1.Equals(effect));
    }

    private void AttributeChanged(Attribute attribute, float oldValue, float newValue)
    {
        EmitSignal("OnAttributeChanged", attribute, oldValue, newValue);
    }

    private void EffectRemoved(Effect effect)
    {
        EmitSignal("OnEffectRemoved", effect);
    }

    private bool HasAttribute(Attribute attribute)
    {
        return attributes.ContainsKey(attribute.GetAttributeName());
    }

    public AttributeSet GetAttributeSet()
    {
        return attributeSet;
    }
}
