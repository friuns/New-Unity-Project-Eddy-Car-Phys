using System.Collections.Generic;
using UnityEngine;

public class bsInput : bs
{
    private Dictionary<string, SecureInt> secureDict;
    public void SetSecure(string key, int value)
    {
        if (secureDict == null) secureDict = new Dictionary<string, SecureInt>();
        SecureInt o;
        if (secureDict.TryGetValue(key, out o))
        {
            o.Get();
            o.Set(value);
        }
        else
            secureDict[key] = new SecureInt(key, value);
    }
    public int GetSecure(string key, int def = 0)
    {
        if (secureDict == null) secureDict = new Dictionary<string, SecureInt>();
        SecureInt o;
        if (secureDict.TryGetValue(key, out o))
            return o.Get();
        else
        {
            secureDict[key] = new SecureInt(key, def);
            return def;
        }
    }
    public static InputManager Input2;
    public static InputManager inputManger { get { return Input2; } }
}