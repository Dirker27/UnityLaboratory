using System;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;

/**
 * see: https://docs.unity3d.com/540/Documentation/Manual/NativePlugins.html
 */
public class NativeLibraryConsumer : MonoBehaviour
{

    public delegate void Callback(StringBuilder sb);

    static void LogCallback(StringBuilder sb) {
        Debug.LogWarning("FROM THE OTHER SIDE: " + sb.ToString());
    }

    #if UNITY_IPHONE
        // On iOS plugins are statically linked into
        //   the executable, so we have to use __Internal as the
        // library name.
        DLLIMPORT [DllImport ("__Internal")]
    #else
        // Other platforms load plugins dynamically, so pass the name
        // of the plugin's dynamic library.
        [DllImport("DirksClientLibraryExample")] private static extern byte GenerateRandom8Bit();
        [DllImport("DirksClientLibraryExample")] private static extern ushort GenerateRandom16Bit();
        [DllImport("DirksClientLibraryExample")] private static extern int GenerateRandom32Bit();
        [DllImport("DirksClientLibraryExample")] private static extern long GenerateRandom64Bit();

        // https://www.mono-project.com/docs/advanced/pinvoke/
        [DllImport("DirksClientLibraryExample", 
            CharSet=CharSet.Ansi, 
            CallingConvention=CallingConvention.StdCall)]
        private static extern void GenerateRandomUUID(StringBuilder dest);

        [DllImport("DirksClientLibraryExample")] private static extern void GenerateRandomString(StringBuilder dest, int length);
        [DllImport("DirksClientLibraryExample")] private static extern char GenerateRandomChar();
        [DllImport("DirksClientLibraryExample")] private static extern char ToUpperChar(char c);
        [DllImport("DirksClientLibraryExample")] private static extern void ToUpperString(StringBuilder dest, string source, int length);

        // https://stackoverflow.com/questions/44877500/register-c-sharp-delegate-to-c-callback-what-does-marshal-getfunctionpointerf
        [DllImport("DirksClientLibraryExample",
            CharSet=CharSet.Ansi,
            CallingConvention=CallingConvention.StdCall)]
        private static extern void InvokeMe(Callback callback, string message, int length);
    #endif


    //~ ========================================================= ~//
    //  UNITY LAND
    //~ ========================================================= ~//

    // Use this for initialization
    void Start() {
        
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("Rando__8: " + GenerateRandom8Bit());
        //Debug.Log("Rando_16: " + GenerateRandom16Bit());
        //Debug.Log("Rando_32: " + GenerateRandom32Bit());
        //Debug.Log("Rando_64: " + GenerateRandom64Bit());
        //Debug.Log("Rando_UUID: " + GenerateRandomUUID());

        char c = GenerateRandomChar();
        Debug.Log("Rando_Char: " + c);
        Debug.Log("Upper'd: " + ToUpperChar(c));

        // Use StringBuilders as receivers of out parameters -------=
        // https://stackoverflow.com/questions/20752001/passing-strings-from-c-sharp-to-c-dll-and-back-minimal-example
        //
        StringBuilder randomStringBuilder = new StringBuilder(42);
        GenerateRandomString(randomStringBuilder, randomStringBuilder.Capacity);
        Debug.Log("Rando_String: " + randomStringBuilder.ToString());
        //
        StringBuilder upperStringBuilder = new StringBuilder(42);
        ToUpperString(upperStringBuilder, randomStringBuilder.ToString(), upperStringBuilder.Capacity);
        Debug.Log("Upper'd String: " + upperStringBuilder.ToString());
        //
        StringBuilder uuidBuilder = new StringBuilder(36);
        GenerateRandomUUID(uuidBuilder);
        Debug.Log("Rando_UUID: " + uuidBuilder.ToString());

        // Function pointers as callbacks --------------------------=
        // https://stackoverflow.com/questions/44877500/register-c-sharp-delegate-to-c-callback-what-does-marshal-getfunctionpointerf
        //
        Callback myCallback = new Callback(LogCallback);
        InvokeMe(myCallback, "asdfff", 5);
    }
}
