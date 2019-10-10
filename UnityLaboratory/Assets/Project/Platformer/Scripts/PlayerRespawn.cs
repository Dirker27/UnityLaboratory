using UnityEngine;
using System.Collections;

public class PlayerLifecycle : MonoBehaviour
{
    private const string TAG = "[PlayerLifecycle] ";

    public Vector3 spawnLocation;
    public Quaternion spawnRotation;

	// Use this for initialization
	void Start ()
    {
        spawnLocation = transform.position;
        spawnRotation = transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Respawn()
    {
        Debug.Log(TAG + "Respawning to pos[" + spawnLocation + "] rot<" + spawnRotation + ">.");

    transform.position = spawnLocation;
        transform.rotation = spawnRotation;
    }
}
