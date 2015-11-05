using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Pool : bs
{
    readonly Dictionary<Transform, List<Transform>> gs = new Dictionary<Transform, List<Transform>>();
    public void Awake()
    {
        _Pool = this;
        tr.parent = null;
    }
    public Transform Load2(Object prefab, Vector3 p = default(Vector3), Quaternion q = default(Quaternion))
    {                
        return Load(((GameObject)prefab).transform, p, q);
    }
    public Transform Load(Transform prefab, Vector3 p, Quaternion q)
    {
        if (bs.settings.disablePool)
            return ((Transform)Instantiate(prefab, p, q));

        var list = gs.TryGet(prefab);
        if(list==null)
            list = gs[prefab] = new List<Transform>();

        var g = list.FirstOrDefault(a => a.parent == tr);
        if (g == null)
        {
            Profiler.BeginSample("Pool Instantiate");
            g = ((Transform)Instantiate(prefab, p, q));
            Profiler.EndSample();
            list.Add(g.transform);
            return g;
        }
        else
        {
            g.parent = null;
            g.position = p;
            g.rotation = q;
            g.gameObject.SetActive(true);
            return g;
        }
    }
    public void Save(GameObject g)
    {
        Save(g.transform);
    }
    public void Save(Transform g)
    {
        if (!this)   return;
        g.parent = tr;
        g.gameObject.SetActive(false);
    }
}