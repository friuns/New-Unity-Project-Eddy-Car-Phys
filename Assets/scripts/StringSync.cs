//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using gui = UnityEngine.GUILayout;
//using UnityEngine;





//public class Sync
//{
//    public string name;
//    public PhotonPlayer pl;
//    public static void InitNames(object _this, string s, PhotonPlayer pl = null, bool? save = null)
//    {

//        Type syncType = typeof(Sync);
//        //if (_this.GetType().BaseType == type || _this.GetType().BaseType == type)
//        var sync = _this as Sync;
//        if (sync != null)
//        {
//            sync.name = s;
//            sync.pl = pl;
//            sync.OnInit();
//            if (save.HasValue)
//                sync.save = save.Value;
//            save = sync.save;
//        }

//        foreach (FieldInfo a in _this.GetType().GetFields())
//        {
//            if (a.FieldType.IsArray)
//            {
//            }
//            if (a.FieldType.BaseType == syncType || a.FieldType == syncType)
//            {
//                var child = a.GetValue(_this);
//                InitNames(child, s + "/" + a.Name, pl, save);
//            }
//        }
//    }
//    public virtual void OnInit()
//    {

//    }

//    public bool save = true;
//}

//public class SyncInt : Sync
//{
//    private int? v;
//    private int def;

//    public SyncInt Set(int i)
//    {
//        if (name != null)
//        {
//            v = i;
//            PlayerPrefsSetInt(name, i);
//            if (pl != null) pl.Set(name, i);
//        }
//        else
//            def = i;
//        return this;
//    }
//    private void PlayerPrefsSetInt(string n, int I)
//    {
//        if (pl != null && !pl.isLocal || !save)
//            return;
//        PlayerPrefs.SetInt(n, I);

//    }
//    private int PlayerPrefsGetInt(string n)
//    {
//        if (pl != null && !pl.isLocal || !save)
//            return def;
//        Debug.Log("Get " + n);
//        return PlayerPrefs.GetInt(n, def);
//    }
//    public int Get()
//    {
//        if (name == null)
//            throw new Exception("Init names first");

//        object o;
//        if (pl != null && pl.customProperties.TryGetValue(name, out o))
//            return (int)o;

//        return v ?? Set(PlayerPrefsGetInt(name)).v.Value;
//    }

//    public override void OnInit()
//    {
//        if (v.HasValue)
//            Set(v.Value);
//    }

//    public static implicit operator int (SyncInt rhs)
//    {
//        return rhs.Get();
//    }
//}



//public class Sync<T> : Sync where T : new()
//{
//    private T value;
//    private bool isSet;

//    public void Set(T t)
//    {
//        var o = (object)t;
//        if (o is int)
//            PlayerPrefs.SetInt(name, (int)o);
//        isSet = true;
//        value = t;
//    }
//    public T Get()
//    {

//        if (!isSet)
//        {
//            T t = default(T);
//            if (t is int)
//                Set((T)(object)PlayerPrefs.GetInt(name));
//        }
//        return value;
//    }
//}



