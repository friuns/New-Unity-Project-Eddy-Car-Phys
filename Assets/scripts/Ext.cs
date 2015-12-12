using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ext
{

    public static void GetFields<T>(this object th, Action<string, T> act)
    {
        var type = th.GetType();
        var t2 = typeof(T);
        var fieldInfos = type.GetFields();
        foreach (var a in fieldInfos)
        {
            if (a.FieldType == t2)
            {
                var value = a.GetValue(th);
                if (value != null)
                    act(a.Name, (T)value);
            }
        }
    }

    public static string[] SplitString(this string text)
    {
        return text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }
    public static T Random<T>(this IList<T> ts)
    {
        if (ts.Count == 0) return default(T);
        return ts[UnityEngine.Random.Range(0, ts.Count)];
    }


    //public static bool Any<T>(this IEnumerable<T> replay)
    //{
    //    foreach (var a in replay)
    //        if (a.replay == replay) return true;
    //    return false;
    //}

    public static bool TryGetValue2<T, T2>(this Dictionary<T, T2> dict, T key, ref T2 value)
    {
        T2 v;
        if (dict.TryGetValue(key, out v))
        {
            value = v;
            return true;
        }
        return false;
    }
    public static T2 TryGet<T, T2>(this Dictionary<T, T2> dict, T key, T2 def)
    {
        T2 o;
        if (dict.TryGetValue(key, out o))
            return o;
        return dict[key] = def;
    }

    public static T2 TryGet<T, T2>(this Dictionary<T, T2> dict, T key)
    {
        T2 o;
        if (dict.TryGetValue(key, out o))
            return o;
        return default(T2);
    }

    public static void SetDirty(this MonoBehaviour g)
    {
#if (UNITY_EDITOR)
        UnityEditor.EditorUtility.SetDirty(g);
#endif
    }

    public static Vector3 ClampAngle(Vector3 angle)
    {
        return new Vector3(ClampAngle(angle.x), ClampAngle(angle.y), ClampAngle(angle.z));
    }
    public static float ClampAngle(float angle)
    {
        if (angle > 180) angle = angle - 360;
        return angle;
    }
    //public static void Hack(string s)
    //{
    //    SecureInt.detectedText = s;
    //    Block(s);
    //}
    public static void Block(string s = "")
    {

        if (SecureInt.detected || !string.IsNullOrEmpty(FieldCache.hackDetected))
        {
            if (!string.IsNullOrEmpty(bs.deviceUniqueIdentifier))
            {
                web.Download("report.php", null, null, false, "id", bs.deviceUniqueIdentifier, "msg", SecureInt.detectedText);
                var pl = bs._Player;
                web.Download("scripts/report.php", Debug.Log, Debug.LogWarning, false, new object[] { "ip", pl.owner.ip, "devid", pl.owner.deviceId, "plname", pl.owner.name, "version", bs.settings.version, "msg", s });
                web.LogEvent("Hack detected" + s);
            }
        }
        else
            web.LogEvent("Site Blocked" + s + Loader.GetHostName());

        Application.ExternalEval("window.top.location = '" + bs.settings.redirectUrl + "';");
        foreach (var a in Object.FindObjectsOfType<GameObject>().Where(a => a.transform.root == a.transform))
            Object.Destroy(a);
        Application.OpenURL(bs.settings.redirectUrl);
        GameObject gameObject = new GameObject();
        GUIText addComponent = gameObject.AddComponent<GUIText>();
        addComponent.transform.position = Vector3.one / 2;
        addComponent.anchor = TextAnchor.MiddleCenter;
        addComponent.text = string.Format(bs.settings.redirectText, bs.settings.redirectUrl);
        gameObject.AddComponent<Camera>();
        gameObject.AddComponent<GUILayer>();
        web.corObj = gameObject.AddComponent<MonoBehaviour>();        
    }

    public static T[] Fill<T>(this T[] ar, Func<T> ac)
    {
        for (int i = 0; i < ar.Length; i++)
        {
            ar[i] = ac();
        }
        return ar;
    }
}
