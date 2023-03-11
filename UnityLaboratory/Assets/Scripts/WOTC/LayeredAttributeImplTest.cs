using UnityEngine;
using System.Collections;

public class LayeredAttributeImplTest : MonoBehaviour
{
    private LayeredAttributesImpl instance;
    private bool testSuiteSuccess;

    public void SetUp()
    {
        instance = new LayeredAttributesImpl();
        testSuiteSuccess = true;
    }

    public void Test_SetGetBaseAttribute_HappyPath()
    {
        instance.SetBaseAttribute(AttributeKey.Power, 1);
        instance.SetBaseAttribute(AttributeKey.Loyalty, 2);
        instance.SetBaseAttribute(AttributeKey.Color, 3);

        AssertExpectedValueForKey(AttributeKey.Power, 1);
        AssertExpectedValueForKey(AttributeKey.Loyalty, 2);
        AssertExpectedValueForKey(AttributeKey.Color, 3);
    }

    private void AssertExpectedValueForKey(AttributeKey key, int expectedValue)
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
