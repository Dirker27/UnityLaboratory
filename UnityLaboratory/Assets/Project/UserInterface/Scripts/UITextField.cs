using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITextField : MonoBehaviour
{
    public string message = "";
    public string prefix = "";

    public UITextProvider provider = null;

    private Text text;

	// Use this for initialization
	void Start ()
    {
        text = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (provider)
        {
            message = provider.ProvideText();
        }

        text.text = prefix + message;
    }
}
