//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Pathfinding;
//using UnityEditor;
//using UnityEngine;

//public class AITest2 : bs
//{
//    internal Seeker seeker;
//    public Transform target;
//    private ABPath fn;
//    public IEnumerator Start()
//    {
//        yield return null;
//        print("test");
//        seeker = GetComponent<Seeker>();
//        while (true)
//        {
//            fn = ABPath.Construct(pos, target.position);

//            seeker.StartPath(fn);
//            yield return fn.WaitForPath();
//            path = fn.vectorPath;
//        }

//    }
//    private List<Vector3> path;
//    public float offset = 1;
//    public void Update()
//    {
//        Vector3? o = null;
//        if (path != null)
//            for (int i = 0; i < path.Count; i++)
//            {
//                //var v = GetNodeOffset(fn.path[i]);
//                var v = (Vector3)fn.path[i].position;
//                var vector3 = path[i] - v * offset;
//                if (o.HasValue)
//                    Debug.DrawLine(o.Value, vector3, i % 2 == 0 ? Color.black : Color.white);
//                o = vector3;
//            }
//    }
//    private Vector3 GetNodeOffset(GraphNode GraphNode)
//    {
//        var v = Int3.zero;
//        GraphNode.GetConnections(delegate (GraphNode Node) { v += (GraphNode.position - Node.position); });
//        return (Vector3)v;
//    }
//}
