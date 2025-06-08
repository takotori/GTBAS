using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace ProjectD.addons.gas.attributes;

[Tool]
[GlobalClass]
public partial class AttributeSet : Resource
{
    [Signal]
    public delegate void OnAttributeChangedEventHandler(
        Attribute attribute,
        float oldValue,
        float newValue
    );

    [Export]
    protected Array<Attribute> attributes = [];

    public void Init()
    {
        IsValid();
        ResetAttributes();

        foreach (var attribute in attributes)
        {
            attribute.AttributeChanged += AttributeChanged;
        }
    }

    public bool HasAttribute(Attribute attribute)
    {
        return attributes.Contains(attribute);
    }

    public bool HasAllAttributes(List<string> attributeNames)
    {
        var attributeNameSet = attributes.Select(a => a.attributeName).ToHashSet();
        return attributeNames.All(name => attributeNameSet.Contains(name));
    }

    public Attribute GetAttributeByName(string name)
    {
        return attributes.Single(a => a.attributeName == name);
    }

    public List<Attribute> GetAttributesByName(HashSet<string> attributeNames)
    {
        return attributeNames.Select(GetAttributeByName).ToList();
    }

    public Array<Attribute> GetAttributes()
    {
        return attributes;
    }

    private void ResetAttributes()
    {
        foreach (var attribute in attributes)
        {
            attribute.SetCurrentValue(attribute.baseValue);
        }
    }

    private void AttributeChanged(Attribute attribute, float oldValue, float newValue)
    {
        EmitSignal("OnAttributeChanged", attribute, oldValue, newValue);
    }

    private void IsValid()
    {
        if (attributes.Count == 0)
        {
            GD.PushError("AttributeSet has no attributes.");
        }

        var uniqueAttributes = attributes.Select(a => a.attributeName).ToHashSet();
        if (uniqueAttributes.Count != attributes.Count)
        {
            GD.PushError("AttributeSet contains duplicate attribute names.");
        }

        foreach (var attribute in attributes)
        {
            if (attribute.baseValue <= 0)
            {
                GD.PushError(
                    $"Attribute {attribute.attributeName} has a negative or zero base value."
                );
            }
        }
    }
}
