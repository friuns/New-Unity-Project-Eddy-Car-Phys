using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolTest : bs
{
    public GameObject[] prefabs;

    public void Start()
    {
        List<CombineInstance> list = new List<CombineInstance>();
        var r = gameObject.GetComponent<MeshRenderer>();
        r.materials = prefabs.Select(a => a.renderer.material).ToArray();
        foreach (var a in prefabs)
        {
            var ci = new CombineInstance();
            ci.transform = tr.worldToLocalMatrix*a.transform.localToWorldMatrix;
            ci.mesh = a.GetComponent<MeshFilter>().sharedMesh;
            list.Add(ci);
        }

        var mf = gameObject.GetComponent<MeshFilter>();
        mf.mesh.CombineMeshes(list.ToArray(),false);
    }
}