using UnityEngine;
using System.Collections;

public class TrackingCameraMovement : MonoBehaviour
{
    private const string TAG = "[TrackingCameraMovement] ";

    public Vector3 offset = Vector3.zero;

    private GameObject player;

	// Use this for initialization
	void Start ()
    {
	    player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning(TAG + "No player found for camera mount.");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (player)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, 0.3f);
        }
    }
}
