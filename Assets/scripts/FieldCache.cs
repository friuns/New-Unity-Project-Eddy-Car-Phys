using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using Vexe.Runtime.Extensions;






public class FieldAtr : Attribute
{
    public bool recursive;
    public string Name;
    public bool save;
    public float left;
    public float right = 100;
    public bool scrollbar;
    public bool ignore;
    public bool dontDraw;
    //public bool save2;

    public FieldAtr Clone()
    {
        return (FieldAtr)MemberwiseClone();
    }
}

public class FieldCache
{
    public static string hackDetected;

    public VarParse varparse;

    public FieldAtr atr;
    public FieldInfo fi;
    public bool isSet;
    public string curKey;
    internal List<FieldCache> fieldCaches;
    //public int id;
    //public Type obsType;
    //public Type type;
    //protected virtual SuperVar superVar { get; set; }

    public virtual void Init()
    {

    }

    public virtual void Update(object o)
    {

    }

    public virtual void SetValue(object o)
    {

    }
    public PhotonPlayer pl { get { return varparse.pl; } }
    public RoomInfo roomInfo { get { return varparse.roomInfo; } }
    public void OnValueChanged(object curValue, bool save)
    {
        if (pl != null)
            pl.Set(curKey, curValue);
        if (roomInfo != null)
            roomInfo.Set(curKey, curValue);
        if (save)
            PlayerPrefsSet(curKey, curValue);
    }
    public static void PlayerPrefsSet(string key, object value)
    {
        MonoBehaviour.print("Set " + key + ":" + value);
        if (value is int)
            PlayerPrefs.SetInt(key, (int)(object)value);
        else if (value is float)
            PlayerPrefs.SetFloat(key, (float)(object)value);
        else if (value is bool)
            PlayerPrefs.SetInt(key, (bool)(object)value ? 1 : 0);
        else if (value is string)
            PlayerPrefs.SetString(key, (string)(object)value);
    }

}

public class FieldCache<T, T2> : FieldCache
{
    public T obs;
    public MemberGetter<T, T2> getter;
    public MemberSetter<T, T2> setter;
    public T2 value;
    public T2 fakeValue;
    public int fakeValueEnc;
    public void HackCheck()
    {
        if (fakeValueEnc != 0 && (fakeValue.GetHashCode() ^ 234) != fakeValueEnc)
        {
            hackDetected = "hack detected";
            if (bs.isDebug)
                UnityEngine.Debug.LogWarning(hackDetected);
            ext.Block(hackDetected);
        }

        fakeValue = value;
        if (fakeValue != null)
            fakeValueEnc = fakeValue.GetHashCode() ^ 234;
    }
    //protected override SuperVar superVar { get { return getter(obs) as SuperVar; } }
    public override void Init()
    {
        getter = fi.DelegateForGet<T, T2>();
        setter = fi.DelegateForSet<T, T2>();
        typeValue = !string.IsNullOrEmpty(fi.FieldType.Namespace);
        if (value is SuperVar)
            (getter(obs) as SuperVar).cache = this;
    }

    private bool typeValue;

    public override void Update(object o)
    {
        obs = (T)o;
        T2 curValue = getter(obs);

        if (typeValue)
        {
            var save = atr.save && varparse.isLocal;

            if (!EqualityComparer<T2>.Default.Equals(curValue, value) && isSet)
                OnValueChanged(curValue, save && isSet);

            if (save && !isSet)
                curValue = PlayerPrefGet(curValue, curKey);
            value = curValue;
            isSet = true;
            HackCheck();


            if (varparse.isgui && !atr.dontDraw && (varparse.filter == "" || atr.Name.ToLower().Contains(varparse.filter)))
            {
                var drawValue = DrawValue(curValue, atr);
                if (varparse.isLocal)
                    curValue = (T2)drawValue;
            }
            setter(ref obs, curValue);
        }
        else if (curValue != null)
        {

            if (varparse.isgui && !GuiClasses.BeginVertical(atr.Name, false)) return;
            if (varparse.isgui) GuiClasses.Indent();
            varparse.UpdateValues(curValue, this);
            if (varparse.isgui)
            {
                GuiClasses.UnIndent();
                GUILayout.EndVertical();
            }
        }
    }
    public static object DrawValue(object v, FieldAtr a)
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
    public override void SetValue(object o)
    {
        setter(ref obs, (T2)o);
    }
    public static T2 PlayerPrefGet(T2 value, string key)
    {
        if (!PlayerPrefs.HasKey(key)) return value;
        if (value is int)
            value = (T2)(object)PlayerPrefs.GetInt(key);
        else if (value is float)
            value = (T2)(object)PlayerPrefs.GetFloat(key);
        else if (value is string)
            value = (T2)(object)PlayerPrefs.GetString(key);
        else if (value is bool)
            value = (T2)(object)(PlayerPrefs.GetInt(key) > 0);
        MonoBehaviour.print("Get " + key + ":" + value);
        return value;
    }

}