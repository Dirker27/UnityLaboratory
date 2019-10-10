using System.CodeDom;
using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour
{
    private const string TAG = "[Hazard] ";

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(TAG + "Collision detected at pos[" + collision.transform.position + "].");

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(TAG + "Trigger detected at pos[" + other.transform.position + "].");

        PlayerRespawn lifecycle = other.gameObject.GetComponent<PlayerRespawn>();
        if (lifecycle)
        {
            lifecycle.Respawn();
        }
    }
}
