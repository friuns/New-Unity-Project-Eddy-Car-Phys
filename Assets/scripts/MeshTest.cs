using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
public class MeshTest : bs
{
    public MeshFilter mf;
    public List<Renderer> meshList;
    public new class Tr
    {
        public int[] ind;
        public int[] pointers;
        public int group;
        public bool went;
    }
    Dictionary<int, List<Tr>> lab;
    internal Vector3[] vertices;
    internal Vector3[] oldVertices;
    internal Vector3[] normals;
    internal Vector2[] uvs;
    internal Vector4[] tangents;
    List<Tr> flat = new List<Tr>();
    internal List<Element> elements = new List<Element>();
    //private bool localPlayer;
    public IEnumerator Start()
    {
        Start2();
        yield return null;
        yield return null;
        if (mf != null && res != null)
        {
            ResetColor();
        }
    }
    public void Reset(bool local)
    {
        if (!changed)
            return;
        changed = false;
        if (oldVertices != null)
        {
            vertices = mf.mesh.vertices = (Vector3[])oldVertices.Clone();
            foreach (var a in elements)
                a.detached = false;
        }
        //localPlayer = local;
        ResetColor();
    }
    public void Damage(Vector3 nwPoint)
    {
        changed = true;
        nwPoint = mf.transform.InverseTransformPoint(nwPoint);
        if (color32s == null)
            color32s = mf.mesh.colors32;
        for (int j = 0; j < 3; j++)
        {
            var nextSearch = nwPoint + Random.insideUnitSphere;
            Vector3 r = Random.onUnitSphere;
            float oldM = MaxValue;
            var curPoint = nwPoint;
            for (int i = 0; i < vertices.Length; i++)
            {
                if ((vertices[i] - nextSearch).magnitude < oldM)
                {
                    nwPoint = vertices[i];
                    oldM = (vertices[i] - nextSearch).magnitude;
                }
                float d = (.7f - (vertices[i] - curPoint).magnitude * lossyScale.x);
                if (d > 0 && oldVertices[i] == vertices[i])
                    vertices[i] += Mathf.Sqrt(d) * r * .2f / lossyScale.x;
                if (d > 0 && i < color32s.Length)
                {
                    byte b = (byte)Mathf.Max(color32s[i].r - 10000 * d, 0);
                    color32s[i] = new Color32(b, b, b, b);
                }
            }
        }
        mf.mesh.colors32 = color32s;
        mf.mesh.vertices = vertices;
    }
    private Color32[] color32s;
    private Vector3 lossyScale;
    public void Hit(Vector3 point, Vector3 vel = default(Vector3), float cnt = 10, int max = 20)
    {
        if (cnt < 0) return;
        changed = true;
        var po = mf.transform.InverseTransformPoint(point);
        foreach (Element element in elements.OrderBy(a => (a.b.center - po).magnitude).Take(max))
        {
            if (!element.detached)
            {
                if (element.list.Count > 10)
                {
                    element.GenerateVertex();
                    Mesh m = new Mesh();
                    m.vertices = element.vertex.ToArray();
                    m.triangles = element.nwlist.ToArray();
                    m.uv = element.uvs.ToArray();
                    m.tangents = element.tangents.ToArray();
                    m.normals = element.normals.ToArray();
                    m.RecalculateBounds();
                    float size = m.bounds.size.magnitude * lossyScale.x;
                    cnt -= size;
                    if (cnt < 0) break;
                    var g = new GameObject();
                    g.transform.position = mf.transform.position;
                    g.transform.rotation = mf.transform.rotation;
                    var r = g.AddComponent<MeshRenderer>();
                    r.sharedMaterial = mf.renderer.sharedMaterials[element.materialGroup % mf.renderer.sharedMaterials.Length];
                    g.AddComponent<MeshFilter>().mesh = m;
                    g.transform.localScale = lossyScale;
                    var c = g.AddComponent<BoxCollider>();
                    g.collider.material = new PhysicMaterial();
                    g.gameObject.layer = Layer.particles;
                    var g2 = new GameObject("oskolok");
                    g2.hideFlags =  HideFlags.HideInHierarchy;
                    g2.AddComponent<Oskolok2>();
                    var g2transform = g2.transform;
                    g2transform.position = c.bounds.center;
                    g.transform.parent = g2transform;
                    g2.AddComponent<Rigidbody>();
                    SetRandomVel(point, vel, g2);
                    if (size < 1 && _AutoQuality.destrQuality >= 2)
                        for (int i = 0; i < (size < .3f ? 2 : 1); i++)
                        {
                            var instantiate = (GameObject)Instantiate(g2, g2transform.position, g2transform.rotation);
                            instantiate.hideFlags= HideFlags.HideInHierarchy;
                            SetRandomVel(point, vel, instantiate);
                        }
                    //if (size < 2)
                    //{
                    foreach (var b in element.list)
                        vertices[b] = Vector3.zero;
                    //}
                    //if(localPlayer)
                    element.detached = true;
                    //else
                    //{
                    //    foreach (var b in element.list)
                    //        uvs[b] = Vector2.zero;
                    //}
                }
            }
        }
        //mf.mesh.uv = uvs;
        mf.mesh.vertices = vertices;
    }
    private void SetRandomVel(Vector3 point, Vector3 vel, GameObject g2)
    {
        var r = g2.rigidbody;
        r.velocity = (ZeroY(point - pos).normalized + Vector3.up + Random.insideUnitSphere)*5 + vel;
        r.angularVelocity = Random.insideUnitSphere*3;
    }
    private void ResetColor()
    {
        color32s = new Color32[mf.mesh.colors32.Length];
        for (int i = 0; i < color32s.Length; i++)
            color32s[i] = new Color32(255, 255, 255, 255);
        mf.mesh.colors32 = color32s;
    }
    bool changed;
    public void Start2()
    {
        if (mf == null)
            mf = GetComponent<MeshFilter>();
        if (meshList.Count > 0)
        {
            var list = new List<CombineInstance>();
            var r = mf.GetComponent<MeshRenderer>();
            var enumerable = new[] { mf.renderer }.Concat(meshList);
            r.sharedMaterials = enumerable.SelectMany(a => a.sharedMaterials).ToArray();
            foreach (var a in enumerable)
            {
                var sharedMesh = a.GetComponent<MeshFilter>().sharedMesh;
                for (int i = 0; i < sharedMesh.subMeshCount; i++)
                {
                    var ci = new CombineInstance();
                    ci.transform = tr.worldToLocalMatrix * a.transform.localToWorldMatrix;
                    ci.mesh = sharedMesh;
                    ci.subMeshIndex = i;
                    list.Add(ci);    
                }
            }
            mf.mesh.CombineMeshes(list.ToArray(), false);
            foreach (var a in meshList)
                Destroy(a.gameObject);
        }
        lossyScale = mf.transform.lossyScale;
        elements.Clear();
        lab = new Dictionary<int, List<Tr>>();
        vertices = mf.mesh.vertices;
        oldVertices = (Vector3[])vertices.Clone();
        normals = mf.mesh.normals;
        tangents = mf.mesh.tangents;
        uvs = mf.mesh.uv;
        for (int x = 0; x < mf.mesh.subMeshCount; x++)
        {
            var trs = mf.mesh.GetIndices(x);
            for (int i = 0; i < trs.Length; i += 3)
            {
                var tr = new Tr() { ind = new[] { trs[i], trs[i + 1], trs[i + 2] }, pointers = new[] { i, i + 1, i + 2 }, group = x };
                flat.Add(tr);
                for (int j = 0; j < 3; j++)
                {
                    List<Tr> lt;
                    if (!lab.TryGetValue(trs[i + j], out lt))
                        lab[trs[i + j]] = lt = new List<Tr>();
                    lt.Add(tr);
                }
            }
        }
        foreach (Tr t in flat)
        {
            if (!t.went)
            {
                Element element = new Element();
                element.MeshTest = this;
                elements.Add(element);
                var color32 = new Color32((byte)(Random.value * 255), (byte)(Random.value * 255), (byte)(Random.value * 255), 255);
                Go(t, color32, element, 0);
            }
        }
    }
    private void Go(Tr tr, Color32 color, Element element, int depth)
    {
        if (tr.went) return;
        tr.went = true;
        element.materialGroup = tr.group;
        element.pointers.AddRange(tr.pointers);
        element.list.AddRange(tr.ind);
        foreach (int a in tr.ind)
        {
            element.b.Encapsulate(vertices[a]);
        }
        foreach (var i in tr.ind)
        {
            List<Tr> lt;
            if (lab.TryGetValue(i, out lt))
                foreach (Tr t in lt)
                    Go(t, color, element, depth + 1);
        }
    }
#if UNITY_EDITOR2
    public void Update()
    {
        if (_Game)
            return;
        var r = CameraMain.ScreenPointToRay(Input.mousePosition);
        RaycastHit h;
        if (Input.GetMouseButtonDown(1))
            Reset();
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(r, out h))
        {
                Damage(h.point, r.direction);
            Debug.DrawRay(h.point, h.normal, Color.white, 10);
        }
    }
#endif
}