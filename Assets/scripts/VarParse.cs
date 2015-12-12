//using gui = UnityEngine.GUILayout;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Boo.Lang;
//using ExitGames.Client.Photon.Lite;
//using UnityEditor;
//using UnityEngine;
//using Vexe.Runtime.Extensions;

//[FieldAtr(ignore = true)]
//public class VarParse
//{
//    public PhotonPlayer pl;
//    public RoomInfo roomInfo;
//    public string filter = "";


//    public FieldCache[] values = new FieldCache[999];

//    public object root;
//    public string name = "";
//    public void UpdateValues()
//    {
//        id = 0;
//        UpdateValues(root, name);
//    }
//    public int id;
//    private static Dictionary<string, FieldCache> fieldDict = new Dictionary<string, FieldCache>();
//    private void UpdateValues<T>(T obs, string key = "", HashSet<object> antiloop = null, FieldAtr oldAtr = null)
//    {
//        bool isgui = Event.current != null;
//        if (isgui)
//            GuiClasses.Indent();

//        if (antiloop == null)
//            antiloop = new HashSet<object>();

//        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

//        if (obs == null) throw new NullReferenceException();
//        Type obsType = obs.GetType();
//        var fieldInfos = obsType.GetFields(flags);
//        foreach (FieldInfo fi in fieldInfos)
//        {
//            id++;

//            var fieldCache = (FieldCache<T>)values[id];
//            if (fieldCache == null)
//            {
//                var atr2 = (FieldAtr)fi.GetCustomAttributes(true).Union(obsType.GetCustomAttributes(true)).FirstOrDefault(b => b is FieldAtr) ?? oldAtr;
//                fieldCache = values[id] = new FieldCache<T> { atr = atr2, curKey = key + "/" + fi.Name, fi = fi, obs = obs, getter = fi.DelegateForGet(), setter = fi.DelegateForSet() };
//                fieldDict[fieldCache.curKey] = fieldCache;
//            }

//            var atr = fieldCache.atr;
//            if (atr == null || atr.ignore) continue;
//            if (atr.Name == null || Equals(atr, oldAtr)) atr.Name = fi.Name;

//            string curKey = fieldCache.curKey;

//            object curValue = fieldCache.getter(obs);


//            var superVar = curValue as SuperVar;
//            if (superVar != null)
//            {
//                superVar.cache = values[id];
//                superVar.varparse = this;
//            }

//            if (!string.IsNullOrEmpty(fi.FieldType.Namespace))
//            {
//                atr.save2 = atr.save && (pl == null || pl.isLocal) && (bs.room == null || PhotonNetwork.isMasterClient);

//                if (fieldCache.isSet)
//                {
//                    if (!Equals(fieldCache.value, curValue))
//                        OnValueSet(curKey, curValue, atr.save2);
//                }
//                else if (atr.save2)
//                    curValue = PlayerPrefGet(curValue, curKey);
//                if (pl != null)
//                    pl.customProperties.TryGetValue2(curKey, ref curValue);
//                if (roomInfo != null)
//                    roomInfo.customProperties.TryGetValue2(curKey, ref curValue);

//                fieldCache.value = curValue;
//                fieldCache.isSet = true;

//                if (isgui && !atr.dontDraw && (filter == "" || atr.Name.ToLower().Contains(filter)))
//                {
//                    var drawValue = DrawValue(curValue, atr);
//                    if ((roomInfo == null || PhotonNetwork.isMasterClient) && (pl == null || pl.isLocal))
//                        curValue = drawValue;
//                }
//                fieldCache.setter(ref obs, curValue);
//            }
//            else if (curValue != null && antiloop.Add(curValue))
//            {
//                if (isgui && !GuiClasses.BeginVertical(atr.Name, false)) continue;
//                UpdateValues(curValue, curKey, antiloop, atr.recursive ? atr : null);
//                if (isgui)
//                    gui.EndVertical();
//            }
//        }
//        if (isgui)
//            GuiClasses.UnIndent();
//    }
//    public void OnValueSet(string curKey, object curValue, bool save)
//    {
//        if (pl != null)
//            pl.Set(curKey, curValue);
//        if (roomInfo != null)
//            roomInfo.Set(curKey, curValue);
//        if (save)
//            PlayerPrefsSet(curKey, curValue);
//    }

//    public static void OnValueRead(string s, object value)
//    {
//        FieldCache v;
//        if (fieldDict.TryGetValue(s, out v))
//            v.fi.SetValue(v.obs, v.value = value);
//    }


//    public static object DrawValue(object v, FieldAtr a)
//    {
//        var name = a.Name;

//        if (v is int)
//            v = (int)HorizontalSlider2(name, (int)v, a.left, a.right, a.scrollbar);
//        else if (v is float)
//            v = HorizontalSlider2(name, (float)v, a.left, a.right, a.scrollbar);
//        else if (v is bool)
//            v = GuiClasses.Toggle((bool)v, name);
//        else if (v is string)
//            v = GuiClasses.TextField(name, (string)v);
//        return v;
//    }
//    public static float HorizontalSlider2(string Name, float P1, float Left, float Right, bool scrollbar)
//    {
//        if (scrollbar)
//            return GuiClasses.HorizontalSlider(Name, P1, Left, Right);
//        return float.Parse(GuiClasses.TextField(Name, P1.ToString()));
//    }
//    public static object PlayerPrefGet(object value, string key)
//    {
//        if (!PlayerPrefs.HasKey(key)) return value;
//        if (value is int)
//            value = PlayerPrefs.GetInt(key);
//        else if (value is float)
//            value = PlayerPrefs.GetFloat(key);
//        else if (value is string)
//            value = PlayerPrefs.GetString(key);
//        else if (value is bool)
//            value = PlayerPrefs.GetInt(key) > 0;
//        MonoBehaviour.print("Get " + key + ":" + value);
//        return value;
//    }
//    public static void PlayerPrefsSet(string key, object value)
//    {
//        MonoBehaviour.print("Set " + key + ":" + value);
//        if (value is int)
//            PlayerPrefs.SetInt(key, (int)value);
//        else if (value is float)
//            PlayerPrefs.SetFloat(key, (float)value);
//        else if (value is bool)
//            PlayerPrefs.SetInt(key, (bool)value ? 1 : 0);
//        else if (value is string)
//            PlayerPrefs.SetString(key, (string)value);
//    }
//}
