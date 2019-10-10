using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class DelayInvokeConsumer : MonoBehaviour
{
    public delegate void VoidCallback();
    // https://stackoverflow.com/questions/44877500/register-c-sharp-delegate-to-c-callback-what-does-marshal-getfunctionpointerf
    [DllImport("DirksClientLibraryExample",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.StdCall)]
    private static extern void DelayInvoke(VoidCallback callback, int delayMillis);

    [DllImport("DirksClientLibraryExample")] private static extern void PollInvoke();


    //public PlayerRespawn respawn;

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ScheduleRespawn();
        }

        PollInvoke();
	}

    static void PerformRespawn()
    {
        Debug.LogWarning("RESPAWN TRIGGERED FROM THE OTHER SIDE.");

        PlayerRespawn respawn = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRespawn>();
        if (respawn != null)
        {
            respawn.Respawn();
        }
    }

    void ScheduleRespawn()
    {
        Debug.LogWarning("Respawn sheduled for 5s...");

        VoidCallback myCallback = new VoidCallback(PerformRespawn);
        DelayInvoke(myCallback, 5000);
    }
}
