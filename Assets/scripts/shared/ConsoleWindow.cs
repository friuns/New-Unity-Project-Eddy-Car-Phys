#define GA
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text.RegularExpressions;

using gui = UnityEngine.GUILayout;
#if !UNITY_FLASH || UNITY_EDITOR
using System.Linq;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Prefs = UnityEngine.PlayerPrefs;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public class ConsoleWindow:GuiClasses 
{

    private static string search = "";
    public static bs[] list;
    public void OnEnable()
    {
        //list = Resources.FindObjectsOfTypeAll<bs>();
        list = FindObjectsOfType<bs>();
    }
    static BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
    public void OnGUI()
    {
        GUI.depth = -999;
        gui.BeginArea(new Rect(0,0,Screen.width,Screen.height),skin.window);
        BeginScrollView();
        DrawConsole();
        if (gui.Button("close"))
            enabled = false;
        gui.EndScrollView();
        gui.EndArea();
    }
    public static void DrawConsole()
    {
        GUI.SetNextControlName("a");
        search = gui.TextField(search);
        //GUI.FocusControl("a");

        var ss = search.Split('.');
        if (ss[0].Length >= 2)
            foreach (var ago in list.Where(a => a != null && a.name.ToLower().Contains(ss[0])))
            {
                var q = new Queue<string>(ss);
                q.Dequeue();
                gui.Label(ago.name + "(" + ago.GetType() + ")" + "_________________________");
                if (q.Count > 0)
                    Mfsa(q, ago);
            }
        
    }
    private static MemberInfo selected;
    private static string value = "";
    private static object valueObject;
    private static void Mfsa(Queue<string> q, object m)
    {
        string ss1 = q.Dequeue();
        if (ss1.Length >= 2)
        {

            if (m == null) return;
            Type type = m.GetType();
            foreach (MemberInfo f in type.GetFields(flags).Cast<MemberInfo>().Concat(type.GetProperties(flags)))
            {
                if (f.Name.ToLower().Contains(ss1))
                {
                    if (q.Count > 0)
                        Mfsa(q, f.GetValue(m));
                    else
                    {
                        gui.BeginHorizontal();
                        gui.Label(f.Name);
                        if (selected == f)
                        {
                            value = gui.TextField(value);
                            if (Event.current.keyCode == KeyCode.Return)
                            {
                                f.SetValue(m, valueObject is int ? int.Parse(value) : valueObject is float ? float.Parse(value) : valueObject is bool ? bool.Parse(value) : (object)value);
                                selected = null;
                            }
                        }
                        else if (gui.Button(f.GetValue(m)+"", GUI.skin.textField))
                        {
                            selected = f;
                            value = (valueObject=f.GetValue(m)).ToString();
                        }
                        gui.EndHorizontal();
                    }
                }
            }

        }

    }

}
public static class EdExt
{
    public static object GetValue(this MemberInfo m, object o)
    {
        try
        {
            if (m is FieldInfo)
                return ((FieldInfo) m).GetValue(o);
            else
                return ((PropertyInfo) m).GetValue(o, null);
        } catch (Exception)
        {
            return null;
        }
    }
    public static void SetValue(this MemberInfo m, object o, object v)
    {
        try
        {
            if (m is FieldInfo)
                ((FieldInfo) m).SetValue(o, v);
            else
                ((PropertyInfo) m).SetValue(o, v, null);
        } catch (Exception ) { }
    }
}