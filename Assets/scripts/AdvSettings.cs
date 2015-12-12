//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Net.Mime;
//using System.Reflection;
//using System.Security.Permissions;
//using System.Text;
//using ExitGames.Client.Photon;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif
//using UnityEngine;
//using Random = UnityEngine.Random;
//using gui = UnityEngine.GUILayout;
//using Bs = bs;

//public class AdvSettings : GuiClasses
//{

//    public class FieldAtr : Attribute
//    {
//        public Type[] ignore;
//        public Type[] include;
//        public string name;
//        public float min;
//        public float max = 100;
//        public bool customOnly;
//        public bool cspOnly;
//        public Action act;
//        public bool androidOnly;
//        public bool tdmOnly;
//        public GameType[] gameType = new GameType[0];
//        public string[] dependsOn = new string[0];
//        //public string[] dependsOnField = new string[0];
//    }

//    public Dictionary<KeyValuePair<string, string>, float> customValues = new Dictionary<KeyValuePair<string, string>, float>();
//    public Dictionary<string, Bs> objects2 = new Dictionary<string, Bs>();
//    public Dictionary<string, float> customValuesText = new Dictionary<string, float>();
//    private Dictionary<KeyValuePair<string, string>, float> customValuesDef = new Dictionary<KeyValuePair<string, string>, float>();


//    private void SettingsFor(Bs cur)
//    {
//        foreach (MemberInfo memberInfo in cur.GetType().GetProperties().Cast<MemberInfo>().Concat(cur.GetType().GetFields()))
//            if (memberInfo is FieldInfo || memberInfo is PropertyInfo)
//            {
//                var atr = memberInfo.GetCustomAttributes(false).FirstOrDefault() as FieldAtr;
//                if (atr != null)
//                {
//                    var custom = true;//;
//                    if (
//                        (atr.include == null || atr.include.Contains(cur.GetType())) &&
//                        (atr.ignore == null || !atr.ignore.Contains(cur.GetType())) &&
//                        (!(!custom && atr.customOnly) || atr.gameType.Contains(_Loader.gameType)) &&
//                        (atr.gameType.Contains(_Loader.gameType) || atr.gameType.Length == 0 || custom && atr.customOnly) &&
//                        !(!android && atr.androidOnly) &&
//                        atr.dependsOn.All(Properties) &&
//                        !(!_Loader.TDM && atr.tdmOnly) //&&
//                                                       //!(atr.cspOnly)
//                        )
//                    {
//                        Type t = memberInfo is FieldInfo ? ((FieldInfo)memberInfo).FieldType : ((PropertyInfo)memberInfo).PropertyType;
//                        object o = memberInfo is FieldInfo ? ((FieldInfo)memberInfo).GetValue(cur) : ((PropertyInfo)memberInfo).GetValue(cur, null);
//                        if (t == typeof(bool))
//                        {
//                            var v = ((bool)o);
//                            var sc = Toggle(v, atr.name);
//                            if (v != sc && mod) //todo2 GameGui.SettingsFor:644 
//                                _Game.CallRPC(_Game.SetCustomValue, cur.name, memberInfo.Name, sc ? 1f : 0);
//                        }
//                        if (t == typeof(int) || t == typeof(float))
//                        {
//                            float v = t == typeof(int) ? (int)o : (float)o;
//                            //gui.BeginHorizontal();
//                            var sc = Scroll(atr.name, v, atr.min, atr.max, 1);
//                            //gui.EndHorizontal();
//                            if (v != sc && mod)
//                                _Game.CallRPC(_Game.SetCustomValue, cur.name, memberInfo.Name, sc);
//                        }
//                        if (t == typeof(GameObject))
//                        {
//                            if (Button(atr.name))
//                            {
//                                PrefabSettings = ((GameObject)o).GetComponent<Bs>();
//                                return;
//                            }
//                        }
//                    }
//                }
//            }
//    }

//    [RPC]
//    public void SetCustomValue(string gameObjectName, string fieldName, float value)
//    {
//        var keyValuePair = new KeyValuePair<string, string>(gameObjectName, fieldName);
//        customValues[keyValuePair] = value;
//        if (Application.isEditor) print("Settings: " + gameObjectName + "." + fieldName + ":" + value);
//        if (!objects2.ContainsKey(gameObjectName))
//            throw new Exception("SetCustomValue key not exists " + gameObjectName);
//        var bs = objects2[gameObjectName];
//        MemberInfo memberInfo = bs.GetType().GetMember(fieldName).FirstOrDefault();
//        if (memberInfo == null)
//        {
//            Debug.LogError("Member not found: " + gameObjectName + ":" + fieldName + ":" + value);
//            return;
//        }
//        var obj = memberInfo.GetCustomAttributes(true).FirstOrDefault();
//        var txt = (bs.name != "!Game" ? bs.name + " " : "") + ((FieldAtr)obj).name + ": ";
//        _ChatGui.Chat(txt + value);
//        customValuesText[txt] = value;
//        Type t = memberInfo is FieldInfo ? ((FieldInfo)memberInfo).FieldType : ((PropertyInfo)memberInfo).PropertyType;
//        object o = memberInfo is FieldInfo ? ((FieldInfo)memberInfo).GetValue(bs) : ((PropertyInfo)memberInfo).GetValue(bs, null);
//        if (!customValuesDef.ContainsKey(keyValuePair))
//        {
//            float v = 0;
//            if (t == typeof(bool))
//                v = ((bool)o) ? 1 : 0;
//            if (t == typeof(float))
//                v = (float)o;
//            if (t == typeof(int))
//                v = (int)o;
//            customValuesDef.Add(keyValuePair, v);
//        }
//        foreach (Bs a in new[] { bs }.Union(FindObjectsOfType(bs.GetType()).Where(b => b.name == bs.name)).ToArray())
//        {
//            try
//            {
//                if (t == typeof(bool))
//                {
//                    if (memberInfo is FieldInfo)
//                        ((FieldInfo)memberInfo).SetValue(a, value == 1);
//                    else
//                        ((PropertyInfo)memberInfo).SetValue(a, value == 1, null);
//                }
//                else if (t == typeof(int))
//                {
//                    if (memberInfo is FieldInfo)
//                        ((FieldInfo)memberInfo).SetValue(a, (int)value);
//                    else
//                        ((PropertyInfo)memberInfo).SetValue(a, (int)value, null);
//                }
//                else
//                {
//                    if (memberInfo is FieldInfo)
//                        ((FieldInfo)memberInfo).SetValue(a, value);
//                    else
//                        ((PropertyInfo)memberInfo).SetValue(a, value, null);
//                }
//            }
//            catch (Exception e)
//            {
//                Debug.LogError(e);
//            }
//        }
//    }
//}