using gui = UnityEngine.GUILayout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExitGames.Client.Photon.Lite;
using UnityEditor;
using UnityEngine;

[Field(ignore = true)]
public class VarParse
{
    public PhotonPlayer pl;
    public RoomInfo roomInfo;
    public string filter = "";


    VarCache[] values = new VarCache[999];

    public object root;
    public string name="";
    public void UpdateValues()
    {
        id = 0;
        UpdateValues(root,name);
    }
    public int id;
    private Dictionary<string, SuperVar> superVars = new Dictionary<string, SuperVar>();
    private void UpdateValues(object obs, string key = "", HashSet<object> antiloop = null, Field oldf = null)
    {
        var isgui = Event.current != null;
        if (isgui)
            GuiClasses.Indent();



        if (antiloop == null)
            antiloop = new HashSet<object>();

        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

        if (obs == null) throw new NullReferenceException();
        var obsType = obs.GetType();
        foreach (FieldInfo fi in obsType.GetFields(flags))
        {

            id++;
            if (values[id] == null)
            {
                var fd2 = (Field)fi.GetCustomAttributes(true).Union(obsType.GetCustomAttributes(true)).FirstOrDefault(b => b is Field) ?? oldf;
                values[id] = new VarCache { fd = fd2, curKey = key + "/" + fi.Name };
            }

            var fd = values[id].fd;
            if (fd == null || fd.ignore) continue;
            if (fd.Name == null || Equals(fd, oldf)) fd.Name = fi.Name;

            var curKey = values[id].curKey;
            var curValue = fi.GetValue(obs);

            var superVar = curValue as SuperVar;
            if (superVar != null)
            {
                superVar.cache = values[id];
                superVars[curKey] = superVar;
            }

            if (!string.IsNullOrEmpty(fi.FieldType.Namespace))
            {
                object o = values[id].value;
                fd.save2 = fd.save && (pl == null || pl.isLocal) && (bs.room == null || PhotonNetwork.isMasterClient);

                if (values[id].isSet)
                {
                    if (!Equals(o, curValue))
                        OnValueSet(curKey, curValue, fd);
                }
                else if (fd.save2)
                    curValue = PlayerPrefGet(curValue, curKey);
                if (pl != null)
                    pl.customProperties.TryGetValue2(curKey, ref curValue);
                if (roomInfo != null)
                    roomInfo.customProperties.TryGetValue2(curKey, ref curValue);

                values[id].value = curValue;
                values[id].isSet = true;

                if (isgui && !fd.dontDraw && (filter == "" || fd.Name.ToLower().Contains(filter)))
                {
                    var drawValue = DrawValue(curValue, fd);
                    if ((roomInfo == null || PhotonNetwork.isMasterClient) && (pl == null || pl.isLocal))
                        curValue = drawValue;
                }
                fi.SetValue(obs, curValue);
            }
            else if (curValue != null && antiloop.Add(curValue))
            {
                if (isgui && !GuiClasses.BeginVertical(fd.Name, false)) continue;
                UpdateValues(curValue, curKey, antiloop, fd.recursive ? fd : null);
                if (isgui)
                    gui.EndVertical();
            }
        }
        if (isgui)
            GuiClasses.UnIndent();
    }
    public void OnValueSet(string curKey, object curValue, Field fd)
    {
        if (pl != null)
            pl.Set(curKey, curValue);
        if (roomInfo != null)
            roomInfo.Set(curKey, curValue);
        if (fd.save2)
            PlayerPrefsSet(curKey, curValue);
    }

    public void OnVariableChanged(string s, int value)
    {
        SuperVar v;
        if (superVars.TryGetValue(s, out v))
            v.m_value = value;
    }


    public static object DrawValue(object v, Field a)
    {
        var name = a.Name;

        if (v is int)
            v = (int)HorizontalSlider2(name, (int)v, a.left, a.right, a.scrollbar);
        else if (v is float)
            v = HorizontalSlider2(name, (float)v, a.left, a.right, a.scrollbar);
        else if (v is bool)
            v = GuiClasses.Toggle((bool)v, name);
        else if (v is string)
            v = GuiClasses.TextField(name, (string)v);
        return v;
    }
    public static float HorizontalSlider2(string Name, float P1, float Left, float Right, bool scrollbar)
    {
        if (scrollbar)
            return GuiClasses.HorizontalSlider(Name, P1, Left, Right);
        return float.Parse(GuiClasses.TextField(Name, P1.ToString()));
    }
    private static object PlayerPrefGet(object value, string key)
    {
        if (!PlayerPrefs.HasKey(key)) return value;
        MonoBehaviour.print("Get " + key + ":" + value);
        if (value is int)
            value = PlayerPrefs.GetInt(key);
        else if (value is float)
            value = PlayerPrefs.GetFloat(key);
        else if (value is string)
            value = PlayerPrefs.GetString(key);
        else if (value is bool)
            value = PlayerPrefs.GetInt(key) > 0;
        return value;
    }
    private static void PlayerPrefsSet(string key, object value)
    {
        MonoBehaviour.print("Set " + key + ":" + value);
        if (value is int)
            PlayerPrefs.SetInt(key, (int)value);
        else if (value is float)
            PlayerPrefs.SetFloat(key, (float)value);
        else if (value is bool)
            PlayerPrefs.SetInt(key, (bool)value ? 1 : 0);
        else if (value is string)
            PlayerPrefs.SetString(key, (string)value);
    }
}

public class Field : Attribute
{
    public bool recursive;
    public string Name;
    public bool save;
    public float left;
    public float right = 100;
    public bool scrollbar;
    public bool ignore;
    public bool dontDraw;
    public bool save2;
}

public class VarCache
{
    public Field fd;
    public object value;
    public bool isSet;
    public string curKey;
}