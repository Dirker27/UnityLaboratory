using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    private const string TAG = "[PlayerRespawn] ";

    // Bad dependency pattern. repalce with event emission.
    public DeathTextProvider notification;

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

        // Replace with event emission.
        notification.Display();
    }
}
