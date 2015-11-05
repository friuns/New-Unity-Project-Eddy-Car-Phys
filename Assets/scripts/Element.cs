using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Element
{
    public MeshTest MeshTest;
    public List<int> list = new List<int>();
    public List<int> pointers = new List<int>();

    public List<int> nwlist = new List<int>();
    public List<Vector3> vertex = new List<Vector3>();
    public List<Vector2> uvs = new List<Vector2>();
    public List<Vector4> tangents = new List<Vector4>();
    public List<Vector3> normals = new List<Vector3>();

    public Bounds b = new Bounds();
    public bool detached;
    public int materialGroup;
    bool generated;
    public void GenerateVertex()
    {
        if (generated) return;
        generated = true;

        nwlist = new List<int>(new int[list.Count]);
        foreach (var a in list.Distinct())
        {
            vertex.Add(MeshTest.vertices[a]);
            uvs.Add(MeshTest.uvs[a]);
            normals.Add(MeshTest.normals[a]);
            if (a < MeshTest.tangents.Length)
                tangents.Add(MeshTest.tangents[a]);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == a)
                {
                    nwlist[i] = vertex.Count - 1;
                }
            }
        }
    }
    
}