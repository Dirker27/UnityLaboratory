/**
 * Unit / Functional Tests for <see cref="LayeredAttributesImpl"/>
 * 
 * Please pardon the lack of C# test framework integration - I ran these tests in Unity using a shim actor.
 */
public class LayeredAttributeImplTest
{
    private bool testSuiteSuccess;

    public void SetUp()
    {
        testSuiteSuccess = true;
    }

    public void Test_SetGetBaseAttribute_AssignsAllAttributess()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        instance.SetBaseAttribute(AttributeKey.NotAssessed, 0);
        instance.SetBaseAttribute(AttributeKey.Power, 1);
        instance.SetBaseAttribute(AttributeKey.Toughness, 2);
        instance.SetBaseAttribute(AttributeKey.Loyalty, 3);
        instance.SetBaseAttribute(AttributeKey.Color, 4);
        instance.SetBaseAttribute(AttributeKey.Types, 5);
        instance.SetBaseAttribute(AttributeKey.Subtypes, 6);
        instance.SetBaseAttribute(AttributeKey.Supertypes, 7);
        instance.SetBaseAttribute(AttributeKey.ManaValue, 8);
        instance.SetBaseAttribute(AttributeKey.Controller, 9);

        AssertExpectedValueForKey(instance, AttributeKey.NotAssessed, 0);
        AssertExpectedValueForKey(instance, AttributeKey.Power, 1);
        AssertExpectedValueForKey(instance, AttributeKey.Toughness, 2);
        AssertExpectedValueForKey(instance, AttributeKey.Loyalty, 3);
        AssertExpectedValueForKey(instance, AttributeKey.Color, 4);
        AssertExpectedValueForKey(instance, AttributeKey.Types, 5);
        AssertExpectedValueForKey(instance, AttributeKey.Subtypes, 6);
        AssertExpectedValueForKey(instance, AttributeKey.Supertypes, 7);
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 8);
        AssertExpectedValueForKey(instance, AttributeKey.Controller, 9);
    }

    public void Test_SetGetBaseAttribute_ProvidesDefaultValue()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        AssertExpectedValueForKey(instance, AttributeKey.Power, 0);
        AssertExpectedValueForKey(instance, AttributeKey.Loyalty, 0);
        AssertExpectedValueForKey(instance, AttributeKey.Toughness, 0);
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 0);
    }

    public void Test_SetGetBaseAttribute_UpdatesExistingValues()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        instance.SetBaseAttribute(AttributeKey.Power, 1);
        AssertExpectedValueForKey(instance, AttributeKey.Power, 1);

        instance.SetBaseAttribute(AttributeKey.Power, 5);
        AssertExpectedValueForKey(instance, AttributeKey.Power, 5);

        instance.SetBaseAttribute(AttributeKey.Power, 7);
        AssertExpectedValueForKey(instance, AttributeKey.Power, 7);
    }

    public void Test_AddLayeredEffect_ModifiesValues_Addition()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        instance.SetBaseAttribute(AttributeKey.Loyalty, 2);
        instance.AddLayeredEffect(new LayeredEffectDefinition {
            Attribute = AttributeKey.Loyalty,
            Operation = EffectOperation.Add,
            Modification = 3,
            Layer = 0
        });

        // 2 + 3 == 5
        AssertExpectedValueForKey(instance, AttributeKey.Loyalty, 5);
    }

    public void Test_AddLayeredEffect_ModifiesValues_Subtraction()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        instance.SetBaseAttribute(AttributeKey.Loyalty, 10);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Loyalty,
            Operation = EffectOperation.Subtract,
            Modification = 3,
            Layer = 0
        });

        // 10 - 3 == 7
        AssertExpectedValueForKey(instance, AttributeKey.Loyalty, 7);
    }

    public void Test_AddLayeredEffect_ModifiesValues_Multiplication()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        instance.SetBaseAttribute(AttributeKey.Toughness, 4);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Toughness,
            Operation = EffectOperation.Multiply,
            Modification = 6,
            Layer = 0
        });

        // 4 * 6 == 24
        AssertExpectedValueForKey(instance, AttributeKey.Toughness, 24);
    }

    public void Test_AddLayeredEffect_ModifiesValues_BitwiseOR()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        //    00101000-10101111-10100001-01101011 [682598763]
        // OR 01010111-01010000-01011110-10010100 [1464884884]
        // -> 01111111-11111111-11111111-11111111 [2147483647]
        instance.SetBaseAttribute(AttributeKey.Toughness, 682598763);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Toughness,
            Operation = EffectOperation.BitwiseOr,
            Modification = 1464884884,
            Layer = 0
        });
        AssertExpectedValueForKey(instance, AttributeKey.Toughness, 2147483647);

        //    00101000-10101111-10100001-01101011 [682598763]
        // OR 00000000-00000000-00000000-00000000 [0]
        // -> 00101000-10101111-10100001-01101011 [682598763]
        instance.SetBaseAttribute(AttributeKey.Power, 682598763);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Power,
            Operation = EffectOperation.BitwiseOr,
            Modification = 0,
            Layer = 0
        });
        AssertExpectedValueForKey(instance, AttributeKey.Power, 682598763);

        //    00000000-00000000-00000000-00000000 [0]
        // OR 01010111-01010000-01011110-10010100 [1464884884]
        // -> 01010111-01010000-01011110-10010100 [1464884884]
        instance.SetBaseAttribute(AttributeKey.Color, 0);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Color,
            Operation = EffectOperation.BitwiseOr,
            Modification = 1464884884,
            Layer = 0
        });
        AssertExpectedValueForKey(instance, AttributeKey.Color, 1464884884);
    }

    public void Test_AddLayeredEffect_ModifiesValues_BitwiseAND()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        //     00101000-10101111-10100001-01101011 [682598763]
        // AND 01010111-01010000-01011110-10010100 [1464884884]
        //  -> 00000000-00000000-00000000-00000000 [0]
        instance.SetBaseAttribute(AttributeKey.Toughness, 682598763);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Toughness,
            Operation = EffectOperation.BitwiseAnd,
            Modification = 1464884884,
            Layer = 0
        });
        AssertExpectedValueForKey(instance, AttributeKey.Toughness, 0);

        //     00101000-10101111-10100001-01101011 [682598763]
        // AND 11111111-11111111-11111111-11111111 [-1]
        //  -> 00101000-10101111-10100001-01101011 [682598763]
        instance.SetBaseAttribute(AttributeKey.Power, 682598763);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Power,
            Operation = EffectOperation.BitwiseAnd,
            Modification = -1,
            Layer = 0
        });
        AssertExpectedValueForKey(instance, AttributeKey.Power, 682598763);
    }

    public void Test_AddLayeredEffect_ModifiesValues_BitwiseXOR()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        //     00101000-10101111-10100001-01101011 [682598763]
        // XOR 01010111-01010000-01011110-10010100 [1464884884]
        //  -> 01111111-11111111-11111111-11111111 [2147483647]
        instance.SetBaseAttribute(AttributeKey.Toughness, 682598763);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Toughness,
            Operation = EffectOperation.BitwiseXor,
            Modification = 1464884884,
            Layer = 0
        });
        AssertExpectedValueForKey(instance, AttributeKey.Toughness, 2147483647);

        //     00101000-10101111-10100001-01101011 [682598763]
        // XOR 01111111-11111111-11111111-11111111 [2147483647]
        //  -> 01010111-01010000-01011110-10010100 [1464884884]
        instance.SetBaseAttribute(AttributeKey.Color, 682598763);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Color,
            Operation = EffectOperation.BitwiseXor,
            Modification = 2147483647,
            Layer = 0
        });
        AssertExpectedValueForKey(instance, AttributeKey.Color, 1464884884);
    }

    public void Test_AddLayeredEffect_IgnoresInvalid()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        instance.SetBaseAttribute(AttributeKey.Color, 3);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Color,
            Operation = EffectOperation.Invalid,
            Modification = 5,
            Layer = 1
        });
        AssertExpectedValueForKey(instance, AttributeKey.Color, 3);
    }

    public void Test_AddLayeredEffect_SetValue()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();
        instance.SetBaseAttribute(AttributeKey.ManaValue, 10);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.ManaValue,
            Operation = EffectOperation.Set,
            Modification = 5,
            Layer = 0
        });
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 5);
    }

    public void Test_AddLayeredEffect_SetValue_OverridesPreviousLayer()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        instance.SetBaseAttribute(AttributeKey.ManaValue, 10);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.ManaValue,
            Operation = EffectOperation.Multiply,
            Modification = 5,
            Layer = 1
        });
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 50);


        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.ManaValue,
            Operation = EffectOperation.Set,
            Modification = 5,
            Layer = 2
        });
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 5);
    }

    public void Test_LayeredEffects_AppliedInLayerOrder()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        instance.SetBaseAttribute(AttributeKey.ManaValue, 1);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.ManaValue,
            Operation = EffectOperation.Multiply,
            Modification = 5,
            Layer = 1
        });
        // 5 * 1 == 5
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 5);


        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.ManaValue,
            Operation = EffectOperation.Subtract,
            Modification = 1,
            Layer = 2
        });
        // (5 * 1) - 1 == 4
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 4);

        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.ManaValue,
            Operation = EffectOperation.Multiply,
            Modification = 4,
            Layer = 3
        });
        // ((5 * 1) - 1) * 4 == 16
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 16);

        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.ManaValue,
            Operation = EffectOperation.Subtract,
            Modification = 1,
            Layer = -1
        });
        // ((5 * (1 - 1)) - 1) * 4 == -4
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, -4);
    }

    public void Test_LayeredEffects_AppliedInInsertionOrderForSameLayer()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        instance.SetBaseAttribute(AttributeKey.ManaValue, 1);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.ManaValue,
            Operation = EffectOperation.Multiply,
            Modification = 5,
            Layer = 1
        });
        // 5 * 1 == 5
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 5);


        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.ManaValue,
            Operation = EffectOperation.Subtract,
            Modification = 1,
            Layer = 1
        });
        // (5 * 1) - 1 == 4
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 4);

        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.ManaValue,
            Operation = EffectOperation.Multiply,
            Modification = 4,
            Layer = 1
        });
        // ((5 * 1) - 1) * 4 == 16
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 16);
    }

    public void Test_ClearEffects_ClearsAllModifiers()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        instance.SetBaseAttribute(AttributeKey.ManaValue, 1);
        instance.SetBaseAttribute(AttributeKey.Power, 10);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.ManaValue,
            Operation = EffectOperation.Multiply,
            Modification = 5,
            Layer = 1
        });
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Power,
            Operation = EffectOperation.Subtract,
            Modification = 2,
            Layer = 2
        });
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Power,
            Operation = EffectOperation.Multiply,
            Modification = 4,
            Layer = 3
        });
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 5); // 5 * 1 == 5
        AssertExpectedValueForKey(instance, AttributeKey.Power, 32);    // (10 - 2) * 4 == 32

        instance.ClearLayeredEffects();
        AssertExpectedValueForKey(instance, AttributeKey.ManaValue, 1);
        AssertExpectedValueForKey(instance, AttributeKey.Power, 10);
    }

    public void Test_ClearEffects_AllowsModifiersAfterClear()
    {
        LayeredAttributesImpl instance = new LayeredAttributesImpl();

        instance.SetBaseAttribute(AttributeKey.Power, 10);
        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Power,
            Operation = EffectOperation.Subtract,
            Modification = 1,
            Layer = 2
        });
        instance.ClearLayeredEffects();
        AssertExpectedValueForKey(instance, AttributeKey.Power, 10);

        instance.AddLayeredEffect(new LayeredEffectDefinition
        {
            Attribute = AttributeKey.Power,
            Operation = EffectOperation.Multiply,
            Modification = 4,
            Layer = 3
        });
        AssertExpectedValueForKey(instance, AttributeKey.Power, 40);
    }

    private void AssertExpectedValueForKey(LayeredAttributesImpl instance, AttributeKey key, int expectedValue)
    {
        int actualValue = instance.GetCurrentAttribute(key);
        if (! Equals(expectedValue, actualValue))
        {
            Debug.LogError("[ASSERT FAILED] Expected Key '" + key + "' to be Value '" + expectedValue + "' - was '" + actualValue + "'.");
            testSuiteSuccess = false;
        }
    }

    public bool GetTestSuiteSuccess()
    {
        return testSuiteSuccess;
    }
}
