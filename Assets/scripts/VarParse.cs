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
    public Dictionary<string, object> values = new Dictionary<string, object>();
    public PhotonPlayer pl;
    public RoomInfo roomInfo;
    public string filter = "";
    //public VarParse(object o)
    //{
    //    if (o is PhotonPlayer)
    //        pl = o as PhotonPlayer;
    //    if (o is RoomInfo)
    //        roomInfo = o as RoomInfo;
    //    root = o;
    //}
    public object root;
    public void UpdateValues()
    {
        UpdateValues(root);
    }
    public void UpdateValues(object obs, string key = "", HashSet<object> antiloop = null, Field oldf = null, bool forceSave = false)
    {
        var isgui = Event.current != null;
        if (isgui)
            GuiClasses.Indent();
        string curKey;

        if (antiloop == null)
            antiloop = new HashSet<object>();

        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

        if (obs == null) throw new NullReferenceException();
        var obsType = obs.GetType();
        foreach (FieldInfo fi in obsType.GetFields(flags))
        {

            var fd = (Field)fi.GetCustomAttributes(true).Union(obsType.GetCustomAttributes(true)).FirstOrDefault(b => b is Field) ?? oldf;
            if (fd == null || fd.ignore) continue;
            if (fd.Name == null || Equals(fd, oldf)) fd.Name = fi.Name;

            var curValue = fi.GetValue(obs);

            curKey = key + "/" + fi.Name;


            if (curValue is IList)
            {
                if (!isgui || GuiClasses.BeginVertical(fd.Name, false))
                {

                    for (int i = 0; i < (curValue as IList).Count; i++)
                    {
                        var value = (curValue as IList)[i];
                        fd.Name = string.Format("{0}[{1}]", fi.Name, i);

                        if (!isgui || GuiClasses.BeginVertical(fd.Name, false))
                        {
                            UpdateValues(value, string.Format("{0}/[{1}]", curKey, i), antiloop, fd.recursive ? fd : null, forceSave);
                            if (isgui)
                                gui.EndVertical();
                        }
                    }
                    if (isgui)
                        gui.EndVertical();
                }
            }
            else if (!string.IsNullOrEmpty(fi.FieldType.Namespace))
            {
                object o;
                var save = fd.save && (pl == null || pl.isLocal) && (bs.room == null || PhotonNetwork.isMasterClient);
                if (values.TryGetValue(curKey, out o))
                {
                    if (forceSave || !Equals(o, curValue))
                    {
                        if (pl != null)
                            pl.Set(curKey, curValue);
                        if (roomInfo != null)
                            roomInfo.Set(curKey, curValue);
                        if (save)
                            PlayerPrefsSet(curKey, curValue);
                    }
                }
                else if (save)
                    curValue = PlayerPrefGet(curValue, curKey);
                if (pl != null)
                    pl.customProperties.TryGetValue2(curKey, ref curValue);
                if (roomInfo != null)
                    roomInfo.customProperties.TryGetValue2(curKey, ref curValue);

                values[curKey] = curValue;

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
                if (!isgui || GuiClasses.BeginVertical(fd.Name, false))
                {
                    UpdateValues(curValue, curKey, antiloop, fd.recursive ? fd : null, forceSave);
                    if (isgui)
                        gui.EndVertical();
                }
            }
        }
        if (isgui)
            GuiClasses.UnIndent();
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
        if (PlayerPrefs.HasKey(key))
        {
            MonoBehaviour.print("Get " + key + ":" + value);
            if (value is int)
                value = PlayerPrefs.GetInt(key);
            else if (value is float)
                value = PlayerPrefs.GetFloat(key);
            else if (value is string)
                value = PlayerPrefs.GetString(key);
            else if (value is bool)
                value = PlayerPrefs.GetInt(key) > 0;
        }
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
}
