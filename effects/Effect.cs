using Godot;

namespace ProjectD.addons.gas.effects;

[Tool]
[GlobalClass]
public partial class Effect : Resource
{
    protected string effectName;
    protected string affectedAttribute;
    protected int duration;
    protected int maxStacks;
    protected bool isUnique;
    protected EffectOperation effectOperation;
    protected EffectExecution effectExecution;

    public float ApplyEffect(float baseValue)
    {
        return effectOperation?.Operate(baseValue) ?? 0f;
    }
    
    public string GetEffectName() => effectName;
    
    public string GetAffectedAttributeName() => affectedAttribute;
    
    public int GetDuration() => duration;
    
    public int GetMaxStacks() => maxStacks;
    
    public bool GetIsUnique() => isUnique;
    
    public EffectOperation GetEffectOperation() => effectOperation;
    
    public EffectExecution GetEffectExecution() => effectExecution;
    
    public void SetEffectName(string newName) => effectName = newName;
    
    public void SetAffectedAttribute(string newAttribute) => affectedAttribute = newAttribute;
    
    public void SetDuration(int newDuration) => duration = newDuration;
    
    public void SetMaxStacks(int newMaxStacks) => maxStacks = newMaxStacks;
    
    public void SetIsUnique(bool newIsUnique) => isUnique = newIsUnique;
    
    public void SetEffectOperation(EffectOperation newOperation) => effectOperation = newOperation;
    
    public void SetEffectExecution(EffectExecution newExecution) => effectExecution = newExecution;

    public bool Equals(Effect other)
    {
        return effectName == other.effectName;
    }
}