using UnityEngine;
using System.Collections;

public class LogMessages : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Hello, World.");
        Debug.Log("You sexy devil.");

        Debug.LogWarning("The end is nigh.");

        Debug.LogError("And we're out of TP.");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
