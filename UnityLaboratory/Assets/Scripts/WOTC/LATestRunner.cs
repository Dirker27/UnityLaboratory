using UnityEngine;
using System.Collections;

public class LATestRunner : MonoBehaviour {

	private LayeredAttributeImplTest testHarness = new LayeredAttributeImplTest();

	// Use this for initialization
	void Start () {
		testHarness.SetUp();
		Debug.Log("Started LA Test...");

		Debug.Log("Test: Get/Set Base Attribute");
		testHarness.Test_SetGetBaseAttribute_AssignsAllAttributess();

        Debug.Log("Test: Get/Set Default Value");
        testHarness.Test_SetGetBaseAttribute_ProvidesDefaultValue();

        Debug.Log("Test: Get/Set Updates Existing Values");
        testHarness.Test_SetGetBaseAttribute_UpdatesExistingValues();

        Debug.Log("Test: AddLayeredEffect-Addition");
        testHarness.Test_AddLayeredEffect_ModifiesValues_Addition();

        Debug.Log("Test: AddLayeredEffect-Subtraction");
        testHarness.Test_AddLayeredEffect_ModifiesValues_Subtraction();

        Debug.Log("Test: AddLayeredEffect-Multiplication");
        testHarness.Test_AddLayeredEffect_ModifiesValues_Multiplication();

        Debug.Log("Test: AddLayeredEffect-BitwiseOR");
        testHarness.Test_AddLayeredEffect_ModifiesValues_BitwiseOR();

        Debug.Log("Test: AddLayeredEffect-BitwiseAND");
        testHarness.Test_AddLayeredEffect_ModifiesValues_BitwiseAND();

        Debug.Log("Test: AddLayeredEffect-BitwiseXOR");
        testHarness.Test_AddLayeredEffect_ModifiesValues_BitwiseXOR();

        Debug.Log("Test: AddLayeredEffect-Invalid");
        testHarness.Test_AddLayeredEffect_IgnoresInvalid();

        Debug.Log("Test: AddLayeredEffect-SetValue");
        testHarness.Test_AddLayeredEffect_SetValue();

        Debug.Log("Test: AddLayeredEffect-SetValue-PreviousLayer");
        testHarness.Test_AddLayeredEffect_SetValue_OverridesPreviousLayer();

        Debug.Log("Test: LayeredEffects-LayerOrder");
        testHarness.Test_LayeredEffects_AppliedInLayerOrder();

        Debug.Log("Test: LayeredEffects-InsertionOrder");
        testHarness.Test_LayeredEffects_AppliedInInsertionOrderForSameLayer();

        Debug.Log("Test: ClearEffects-ClearsAllModifiers");
        testHarness.Test_ClearEffects_ClearsAllModifiers();

        Debug.Log("Test: ClearEffects-ClearsAllModifiers");
        testHarness.Test_ClearEffects_AllowsModifiersAfterClear();


        if (testHarness.GetTestSuiteSuccess())
		{
			Debug.Log("TEST SUCCESS!");
		}
		else
		{
			Debug.LogError("TEST FAILURE!");
		}
	}
}
