using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectD.addons.gas.attributes;

namespace ProjectD.addons.gas.effects;

[Tool]
[GlobalClass]
public partial class Effect : Resource
{
    [Export] protected string effectName;

    private int currentDuration;
    private int currentStacks;

    [Export] protected int maxDuration;
    [Export] protected int maxStacks;
    [Export] protected EffectExecution effectExecution;
    [Export] protected EffectModifier[] effectModifiers;

    public void ApplyEffect(List<Attribute> attributes)
    {
        foreach (var effectModifier in effectModifiers)
        {
            var attribute = attributes.First(a => a.GetAttributeName() == effectModifier.GetAffectedAttributeName());
            effectModifier.Operate(attribute);
        }
    }

    public string GetEffectName() => effectName;

    public HashSet<string> GetAffectedAttributeNames() =>
        effectModifiers.Select(e => e.GetAffectedAttributeName()).ToHashSet();

    public int GetCurrentDuration() => currentDuration;

    public int GetMaxDuration() => maxDuration;

    public int GetCurrentStacks() => currentStacks;

    public int GetMaxStacks() => maxStacks;
    
    public EffectExecution GetEffectExecution() => effectExecution;

    public EffectModifier[] GetEffectModifiers() => effectModifiers;

    public void SetEffectName(string newName) => effectName = newName;

    public void SetCurrentDuration(int newDuration) => currentDuration = newDuration;

    public void SetMaxDuration(int newDuration) => maxDuration = newDuration;

    public void SetCurrentStacks(int newCurrentStacks) => currentStacks = newCurrentStacks;

    public void SetMaxStacks(int newMaxStacks) => maxStacks = newMaxStacks;
    
    public void SetEffectExecution(EffectExecution newExecution) => effectExecution = newExecution;

    public void SetEffectModifiers(EffectModifier[] newModifiers) => effectModifiers = newModifiers;

    public bool Equals(Effect other)
    {
        return effectName == other.effectName;
    }
}