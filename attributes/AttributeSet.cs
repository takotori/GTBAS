﻿using Godot;
using Godot.Collections;

namespace ProjectD.addons.gas.attributes;

[Tool]
[GlobalClass]
public partial class AttributeSet : Resource
{
    [Export]
    protected string attributeSetName;

    [Export]
    protected Array<Attribute> attributes;

    public AttributeSet()
    {
        attributes = [];
    }

    public AttributeSet(Array<Attribute> attributes, string name)
    {
        attributeSetName = name;
        this.attributes = attributes;
    }

    public bool AddAttribute(Attribute newAttribute)
    {
        if (attributes.Contains(newAttribute))
            return false;
        attributes.Add(newAttribute);

        return true;
    }

    public bool RemoveAttribute(Attribute attribute)
    {
        if (!attributes.Contains(attribute))
            return false;
        attributes.Remove(attribute);
        return true;
    }

    public bool HasAttribute(Attribute attribute)
    {
        return attributes.Contains(attribute);
    }

    public Attribute FindByName(string name)
    {
        foreach (var attribute in attributes)
        {
            if (attribute.GetAttributeName() == name)
            {
                return attribute;
            }
        }

        return null;
    }

    public Array<Attribute> GetAttributes()
    {
        return attributes;
    }

    public string GetAttributeSetName()
    {
        return attributeSetName;
    }
}
