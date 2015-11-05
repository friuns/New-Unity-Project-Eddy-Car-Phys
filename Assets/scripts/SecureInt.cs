using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;




[Serializable]
public class SecureInt //: IComparable<int>, IEquatable<int>
{
    public static bool started;
    public static string detectedText = "";
    public static bool detected { get { return !string.IsNullOrEmpty(detectedText); } }
    public int unEnc;
    private int enc;
    private int? rand;
    private int time;
    public static Random Random = new Random();
    private string name;
    public SecureInt(string n, int value = 0)
    {
        name = n;
        Set(value);
    }

    public void Set(int value)
    {
        unEnc = value;
        rand = Random.Next(0, 9999);
        time = Environment.TickCount;
        enc = value ^ rand.Value ^ time;
    }
    public int Get()
    {
        if (!rand.HasValue)
        {
            Set(unEnc);
            return unEnc;
        }
        var v = enc ^ rand.Value ^ time;
        if (v != unEnc)
            detectedText = string.Format("{0} from {1} to {2}", name, v, unEnc);
        return v;
    }

    
}