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
    public string name = "";
    public void UpdateValues()
    {
        id = 0;
        UpdateValues(root, name);
    }
    public int id;
    private static Dictionary<string, VarCache> superVars = new Dictionary<string, VarCache>();
    private void UpdateValues(object obs, string key = "", HashSet<object> antiloop = null, Field oldAtr = null)
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
            var varCache = values[id];
            if (varCache == null)
            {
                var atr2 = (Field)fi.GetCustomAttributes(true).Union(obsType.GetCustomAttributes(true)).FirstOrDefault(b => b is Field) ?? oldAtr;
                varCache = values[id] = new VarCache { atr = atr2, curKey = key + "/" + fi.Name, fi = fi, obs = obs };
                superVars[varCache.curKey] = varCache;
            }

            var atr = varCache.atr;
            if (atr == null || atr.ignore) continue;
            if (atr.Name == null || Equals(atr, oldAtr)) atr.Name = fi.Name;

            var curKey = varCache.curKey;
            var curValue = fi.GetValue(obs);



            if (!string.IsNullOrEmpty(fi.FieldType.Namespace))
            {
                atr.save2 = atr.save && (pl == null || pl.isLocal) && (bs.room == null || PhotonNetwork.isMasterClient);

                if (varCache.isSet)
                {
                    if (!Equals(varCache.value, curValue))
                        OnValueSet(curKey, curValue, atr.save2);
                }
                else if (atr.save2)
                    curValue = PlayerPrefGet(curValue, curKey);
                if (pl != null)
                    pl.customProperties.TryGetValue2(curKey, ref curValue);
                if (roomInfo != null)
                    roomInfo.customProperties.TryGetValue2(curKey, ref curValue);

                varCache.value = curValue;
                varCache.isSet = true;

                if (isgui && !atr.dontDraw && (filter == "" || atr.Name.ToLower().Contains(filter)))
                {
                    var drawValue = DrawValue(curValue, atr);
                    if ((roomInfo == null || PhotonNetwork.isMasterClient) && (pl == null || pl.isLocal))
                        curValue = drawValue;
                }
                fi.SetValue(obs, curValue);
            }
            else if (curValue != null && antiloop.Add(curValue))
            {
                if (isgui && !GuiClasses.BeginVertical(atr.Name, false)) continue;
                UpdateValues(curValue, curKey, antiloop, atr.recursive ? atr : null);
                if (isgui)
                    gui.EndVertical();
            }
        }
        if (isgui)
            GuiClasses.UnIndent();
    }
    public void OnValueSet(string curKey, object curValue, bool save)
    {
        if (pl != null)
            pl.Set(curKey, curValue);
        if (roomInfo != null)
            roomInfo.Set(curKey, curValue);
        if (save)
            PlayerPrefsSet(curKey, curValue);
    }

    public static void OnValueRead(string s, object value)
    {
        VarCache v;
        if (superVars.TryGetValue(s, out v))
            v.fi.SetValue(v.obs, v.value = value);
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
    public object obs;
    public Field atr;
    public FieldInfo fi;
    public object value;
    public bool isSet;
    public string curKey;
}