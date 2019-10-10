using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

public class RandomUUIDProvider : UITextProvider {

    // https://www.mono-project.com/docs/advanced/pinvoke/
    [DllImport("DirksClientLibraryExample",
        CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.StdCall)]
    private static extern void GenerateRandomUUID(StringBuilder dest);
    private static string _GenerateRandomUUID()
    {
        StringBuilder sb = new StringBuilder(36);
        GenerateRandomUUID(sb);
        return sb.ToString();
    }

    // Update is called once per frame
    void Update () {
	
	}

    public override string ProvideText()
    {
        return "Generated UUID: " + _GenerateRandomUUID();
    }
}
