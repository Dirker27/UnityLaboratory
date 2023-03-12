using System;
using System.Collections.Generic;

/**
* Implementation for given interface ILayeredAttributes - <see cref="ILayeredAttributes"/>
* 
* Technical Assessment for Wizards of the Coast [WotC] submitted by Dirk Hortensius.
*    (please pardon my Java-isms, most of my C# experience comes from Unity)
*     
* ------------------------------------------------------------------------------------------------
* 
* This approach utilizes a Sorted List of a custom IComparable composite key <see cref="LayeredEffectKey"/>
*   to group applied effects by their assigned layer and their chronological insertion time.
* 
* The trade-off with this approach is a penalty to insertion time (O(log n) -> O(n), depending on internal impl)
*   for new effects in exchange for a reduction in additional memory resources for indexing effects by layers
*   as well as creation time.
*   
* Additionally, this approach utilizes a simple cache in order to reduce unnecessary re-computes for attribute
*   values that have not received any changes to their base values or active modifiers. Values will be added to
*   the cache on first compute and invalidated on any update. This does add some overall memory footprint (up to
*   our total number of attributes - O(K)), but this can be tamed with TTL or LRU functionality if this size
*   becomes problematic. Doing so removes the need for an O(n) calculation for every attribute when we need a
*   fresh value, which could potentially be during every UI/Update tick if we're providing live attributes to a consumer.
*   
* K = total distinct attributes
* n = total layered effects for a given attribute
*/
public class LayeredAttributesImpl : ILayeredAttributes {

    private const int DEFAULT_ATTRIBUTE_VALUE = 0;

    // Base attributes affected 
    private Dictionary<AttributeKey, int> BaseAttributeMap;

    // An ordered list of modifiers for a given attribute.
    private Dictionary<AttributeKey, SortedList<LayeredEffectKey, LayeredEffectDefinition>> AttributeModifiers;

    // Cached values for attributes to avoid re-evaluating a current value until a layer is added/removed or base attribute is updated.
    private Dictionary<AttributeKey, int> CachedAttributeMap;


    public LayeredAttributesImpl()
    {
        BaseAttributeMap = new Dictionary<AttributeKey, int>();
        CachedAttributeMap = new Dictionary<AttributeKey, int>();
        AttributeModifiers = new Dictionary<AttributeKey, SortedList<LayeredEffectKey, LayeredEffectDefinition>>();
    }

    /**
     * Sets the base value for a given attribute.
     */
    public void SetBaseAttribute(AttributeKey attribute, int value)
    {
        // modify base
        BaseAttributeMap[attribute] = value;

        // invalidate cache
        CachedAttributeMap.Remove(attribute);
    }

    /**
     * Gets the current value of anattribute after all layer effects have been applied.
     * 
     * Values will be cached after their first evaluation to reduce compute time for future lookups.
     *   Cached values will be invalidated whenever a change is made to an attribute's modifiers
     *   or base value.
     */
    public int GetCurrentAttribute(AttributeKey attribute)
    {
        // If no attributes have been modified, use the cached values for quick-reads;
        if (CachedAttributeMap.ContainsKey(attribute))
        {
            return CachedAttributeMap[attribute];
        }

        int value = EvaluateLayeredAttributeValue(attribute);
        CachedAttributeMap.Add(attribute, value);
        return value;
    }

    /**
     * Adds a given effect to the list of modifiers to be applied to a given attribute.
     * 
     * All modifiers will be stored with a timestamp to determine precedence for Effects on the same Layer.
     */
    public void AddLayeredEffect(LayeredEffectDefinition effect)
    { 
        if (! AttributeModifiers.ContainsKey(effect.Attribute))
        {
            AttributeModifiers[effect.Attribute] = new SortedList<LayeredEffectKey, LayeredEffectDefinition>();
        }

        // Stores effect to attribute modifiers using a key that groups by LAYER-TIMESTAMP to preserve
        //   insertion order as well as layer precedence.
        LayeredEffectKey orderedKey = GenerateOrderedKeyForEffect(effect);
        AttributeModifiers[effect.Attribute].Add(orderedKey, effect);

        // invalidate cache
        CachedAttributeMap.Remove(effect.Attribute);
    }

    /**
     * Clears ALL layered effects for ALL attributes, as per our spec.
     *  Only base attributes will remain unaffected.
     */
    public void ClearLayeredEffects()
    {
        // Clear All Effects
        AttributeModifiers.Clear();

        // Invalidate Whole Cache
        CachedAttributeMap.Clear();
    }

    /**
     * Calculates the current value for a given attribute after all effects are applied to the base stat.
     * 
     * If no base value is set for a given attribute, we will assume a default value of zero[0] and apply any
     *   and all effects accordingly.
     *   
     * Effects have been stored in a precedence order of LAYER-TIMESTAMP, so Effects with lower Layer values will be
     *   applied first (with negative layer values taking further precedence). Effects with the same layer for a given
     *   attribute will be applied in order of insertion.
     */
    private int EvaluateLayeredAttributeValue(AttributeKey attribute)
    {
        int value = DEFAULT_ATTRIBUTE_VALUE;
        if (BaseAttributeMap.ContainsKey(attribute))
        {
            value = BaseAttributeMap[attribute];
        }

        if (AttributeModifiers.ContainsKey(attribute)) {
            foreach (LayeredEffectDefinition effect in AttributeModifiers[attribute].Values)
            {
                value = ApplyAttributeModifier(value, effect);
            }
        }

        return value;
    }

    /**
     * Applies attribute modifiers for a *single effect* to a given value as per our spec.
     */
    private int ApplyAttributeModifier(int originalValue, LayeredEffectDefinition modifier)
    {
        switch (modifier.Operation)
        {
            case EffectOperation.Invalid:
                // No spec for "Invalid" provided - opting to return original unaltered value.
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

    /**
     * Generates a key for a layered effect that uses both the given effect's layer *and* its insertion time.
     * 
     * Key has format "[LAYER]-[TIMESTAMP]", which will be sortable to preserve both layer precedence and
     *   insertion order. ie:
     *  
     *   [1]-[12:30]
     *   [2]-[14:56]
     *   [3]-[09:00]
     *   [3]-[09:45]
     *   [3]-[10:36]
     *   [5]-[08:00]
     *   ...
     *   
     * NOTE: I've opted to use an auto-incrementing static index key over standard TimeStamps simply due to
     *   simplicity and an admitted unfamiliarity with C#'s DateTime object. These operations will execute
     *   in sub-millisecond time, and I want to avoid creating duplicate composite keys for
     *   effects added in the same milli/nano/picosecond.
     *   
     * There's also something to be said about a potential exploit avenue for bad actors - generating this
     *   key based on a client's System clock can be open for exploitation, as system clocks could be overridden
     *   and players may be able to alter the computation order for a given series of effects on the same layer.
     */
    private static int _indexKey = 0;
    private static LayeredEffectKey GenerateOrderedKeyForEffect(LayeredEffectDefinition effect)
    {
        return new LayeredEffectKey
        {
            Layer = effect.Layer,
            Timestamp = _indexKey++
        };
    }

    /**
     * A Composite Key that will be used as a comparator to determine order of precedence for Layered Effects.
     */
    private class LayeredEffectKey : IComparable<LayeredEffectKey>
    {
        public int Layer;
        public int Timestamp;

        public int CompareTo(LayeredEffectKey other)
        {
            if (this.Layer.Equals(other.Layer))
            {
                return Timestamp.CompareTo(other.Timestamp);
            }
            return Layer.CompareTo(other.Layer);
        }
    }
}