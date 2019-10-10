using UnityEngine;
using System.Collections;

public class DeathTextProvider : UITextProvider
{
    public float displayTimeSeconds = 1f; // [s]

    private const string message = "[ YOU DIED ]";

    private bool display = false;
    private float timeDisplayed;

	void Start ()
    {
        timeDisplayed = displayTimeSeconds;
    }
	
	// Update is called once per frame
	void Update ()
    {
        timeDisplayed += Time.deltaTime;
    }

    public void Display()
    {
        timeDisplayed = 0f;
    }

    public override string ProvideText()
    {
        display = timeDisplayed < displayTimeSeconds;

        return (display) ? message : null;
    }
}
