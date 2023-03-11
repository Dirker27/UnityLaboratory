using System.Collections.Generic;

/**
* Implementation for given interface ILayeredAttributes - <see cref="ILayeredAttributes"/>
* 
* Technical Assessment for Wizards of the Coast [WotC] submitted by Dirk Hortensius.
*   (...Please pardon my Java-isms. Most of my C# experience is from Unity.)
*/
public class LayeredAttributesImpl : ILayeredAttributes {

    private const int DEFAULT_ATTRIBUTE_VALUE = 0;

    // Base attributes affected 
    private Dictionary<AttributeKey, int> baseAttributeMap;

    // An ordered list of modifiers for a given attribute.
    private Dictionary<AttributeKey, List<LayeredEffectDefinition>> attributeModifiers;

    // Cached values for attributes to avoid re-evaluating a current value until a layer is added/removed or base attribute is updated.
    private Dictionary<AttributeKey, int> cachedAttributeMap;


    public LayeredAttributesImpl()
    {
        baseAttributeMap = new Dictionary<AttributeKey, int>();
        cachedAttributeMap = new Dictionary<AttributeKey, int>();
        attributeModifiers = new Dictionary<AttributeKey, List<LayeredEffectDefinition>>();
    }


    public void SetBaseAttribute(AttributeKey attribute, int value)
    {
        // modify base
        baseAttributeMap[attribute] = value;

        // invalidate cache
        cachedAttributeMap.Remove(attribute);
    }

    public int GetCurrentAttribute(AttributeKey attribute)
    {
        // If no attributes have been modified, use the cached values for quick-reads;
        if (cachedAttributeMap.ContainsKey(attribute))
        {
            return cachedAttributeMap[attribute];
        }

        int value = EvaluateLayeredAttributeValue(attribute);
        cachedAttributeMap.Add(attribute, value);
        return value;
    }

    public void AddLayeredEffect(LayeredEffectDefinition effect)
    { 
        if (! attributeModifiers.ContainsKey(effect.Attribute))
        {
            attributeModifiers[effect.Attribute] = new List<LayeredEffectDefinition>();
        }
        attributeModifiers[effect.Attribute].Add(effect);

        // invalidate cache
        cachedAttributeMap.Remove(effect.Attribute);
    }

    public void ClearLayeredEffects()
    {
        attributeModifiers.Clear();

        cachedAttributeMap.Clear();
    }


    private int EvaluateLayeredAttributeValue(AttributeKey attribute)
    {
        int value = DEFAULT_ATTRIBUTE_VALUE;
        if (baseAttributeMap.ContainsKey(attribute))
        {
            value = baseAttributeMap[attribute];
        }

        if (attributeModifiers.ContainsKey(attribute)) {
            foreach (LayeredEffectDefinition effect in attributeModifiers[attribute])
            {
                value = ApplyAttributeModifier(value, effect);
            }
        }

        return value;
    }

    private int ApplyAttributeModifier(int originalValue, LayeredEffectDefinition modifier)
    {
        switch (modifier.Operation)
        {
            case EffectOperation.Invalid:
                return originalValue;

            case EffectOperation.Set:
                return modifier.Modification;

            case EffectOperation.Add:
                return originalValue + modifier.Modification;

            case EffectOperation.Subtract:
                return originalValue - modifier.Modification;

            case EffectOperation.Multiply:
                return originalValue * modifier.Modification;

            case EffectOperation.BitwiseOr:
                return originalValue | modifier.Modification;

            case EffectOperation.BitwiseAnd:
                return originalValue & modifier.Modification;

            case EffectOperation.BitwiseXor:
                return originalValue ^ modifier.Modification;
        }

        return originalValue;
    }
}
