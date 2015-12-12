using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExitGames.Client.Photon.Lite;
using UnityEditor;
using UnityEngine;


public class VarParse<T> : VarParse
{
    public T root;
    public override void UpdateValues()
    {
        //id = 0;
        isgui = Event.current != null;
        isLocal = (pl == null || pl.isLocal) && (PhotonNetwork.room == null || PhotonNetwork.isMasterClient);
        UpdateValues(root, rootFIeld);
    }
}
[FieldAtr(ignore = true)]
public class VarParse
{
    public PhotonPlayer pl;
    public RoomInfo roomInfo;
    public string filter = "";
    public string name { set { rootFIeld.curKey = value; } }
    public FieldCache rootFIeld = new FieldCache();
    public virtual void UpdateValues() { }
    public bool isgui;
    public static Dictionary<string, FieldCache> fieldDict = new Dictionary<string, FieldCache>();

    const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
    HashSet<object> antilock = new HashSet<object>();
    public bool isLocal;

    public void UpdateValues<T>(T obs, FieldCache parentField)
    {
        if (parentField.fieldCaches == null)
        {
            parentField.fieldCaches = new List<FieldCache>();
            if (!antilock.Add(obs))
                return;
            var obsType = obs.GetType();
            foreach (FieldInfo fi in obsType.GetFields(flags))
            {
                FieldCache fieldCache = CreateFieldCache(fi, obs);
                if (fieldCache == null)
                    continue;
                fieldCache.atr = (FieldAtr)fi.GetCustomAttributes(true).Union(obsType.GetCustomAttributes(true)).FirstOrDefault(b => b is FieldAtr) ?? (parentField.atr.recursive ? parentField.atr.Clone() : null);
                if (fieldCache.atr == null || fieldCache.atr.ignore)
                    continue;
                //id++;
                //values[id] = fieldCache;
                //fieldCache.id = id;
                if (fieldCache.atr.Name == null || Equals(fieldCache.atr, parentField.atr))
                    fieldCache.atr.Name = fi.Name;
                fieldCache.varparse = this;
                fieldCache.fi = fi;
                fieldCache.curKey = parentField.curKey + "/" + fi.Name;
                fieldCache.Init();
                fieldDict[fieldCache.curKey] = fieldCache;
                parentField.fieldCaches.Add(fieldCache);

            }
        }

        for (var i = 0; i < parentField.fieldCaches.Count; i++)
            parentField.fieldCaches[i].Update(obs);
    }

    public static void SetValue(string s, object value)
    {
        FieldCache v;
        if (fieldDict.TryGetValue(s, out v))
            v.SetValue(value);
    }
    private FieldCache CreateFieldCache<T>(FieldInfo fi, T obs)
    {
        var t = fi.GetValue(obs);
        if (t is int)
            return new FieldCache<T, int>() { obs = obs };
        if (t is string)
            return new FieldCache<T, string>() { obs = obs };
        if (t is bool)
            return new FieldCache<T, bool>() { obs = obs };
        if (t is float)
            return new FieldCache<T, float>() { obs = obs };
        if (t is Gts)
            return new FieldCache<T, Gts>() { obs = obs };
        if (t is Score)
            return new FieldCache<T, Score>() { obs = obs };
        if (t is MapStat)
            return new FieldCache<T, MapStat>() { obs = obs };

        if (t is CarVisuals)
            return new FieldCache<T, CarVisuals>() { obs = obs };
        if (t is CarWheel)
            return new FieldCache<T, CarWheel>() { obs = obs };
        if (t is CarFrictionCurve)
            return new FieldCache<T, CarFrictionCurve>() { obs = obs };
        if (t is CarAntiRollBar)
            return new FieldCache<T, CarAntiRollBar>() { obs = obs };
        if (t is CarDamage)
            return new FieldCache<T, CarDamage>() { obs = obs };
        return null;
    }
}







