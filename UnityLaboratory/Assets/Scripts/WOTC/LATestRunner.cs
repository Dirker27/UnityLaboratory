using UnityEngine;
using System.Collections;

public class LATestRunner : MonoBehaviour {

	private LayeredAttributeImplTest testHarness = new LayeredAttributeImplTest();

	// Use this for initialization
	void Start () {
		testHarness.SetUp();
		Debug.Log("Started LA Test...");

		Debug.Log("Test: Get/Set Base Attribute");
		testHarness.Test_SetGetBaseAttribute_HappyPath();


		if (testHarness.GetTestSuiteSuccess())
		{
			Debug.Log("TEST SUCCESS!");
		}
	}
}
