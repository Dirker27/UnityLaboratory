using System.Collections.Generic;

/**
* Implementation for given interface ILayeredAttributes - <see cref="ILayeredAttributes"/>
* 
* Technical Assessment for Wizards of the Coast [WotC] submitted by Dirk Hortensius.
*   (...Please pardon my Java-isms. Most of my C# experience is from Unity.)
*/
public class LayeredAttributesImpl : ILayeredAttributes {

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
        ;    }

    public void SetBaseAttribute(AttributeKey attribute, int value)
    {
        // invalidate cache
        cachedAttributeMap.Remove(attribute);

        // modify base
        baseAttributeMap.Add(attribute, value);
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
        // invalidate cache
        cachedAttributeMap.Remove(effect.Attribute);

        switch(effect.Operation)
        {
            case EffectOperation.Invalid:
                // TODO
                break;

            case EffectOperation.Set:
                // TODO
                break;

            case EffectOperation.Add:
                // TODO
                break;

            case EffectOperation.Subtract:
                // TODO
                break;

            case EffectOperation.Multiply:
                // TODO
                break;

            case EffectOperation.BitwiseOr:
                // TODO
                break;

            case EffectOperation.BitwiseAnd:
                // TODO
                break;

            case EffectOperation.BitwiseXor:
                // TODO
                break;
        }
    }

    public void ClearLayeredEffects()
    {
        attributeModifiers.Clear();
    }

    private int EvaluateLayeredAttributeValue(AttributeKey attribute)
    {
        int value = 0;
        if (baseAttributeMap.ContainsKey(attribute))
        {
            value = baseAttributeMap[attribute];
        }

        // Foreach Modifier...

        return value;
    }
}
