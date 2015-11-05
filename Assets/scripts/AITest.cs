//using System.Collections;
//using System.Collections.Generic;
//using Pathfinding;
//using UnityEditor;
//using UnityEngine;

//public class AITest : bs
//{
//    internal Seeker seeker;
//    public Transform target;
//    public int randomMin = 1000;
//    public int randomMax = 500000;
//    private RandomPath fn;
//    private RichPath rp;
//    public bool move;
//    public IEnumerator Start()
//    {
//        yield return null;
//        rp = new RichPath();
//        print("test");
//        seeker = GetComponent<Seeker>();
//        while (true)
//        {
//            fn = RandomPath.Construct(pos, Random.Range(randomMin, randomMax));
            
//            fn.aimStrength = aimStrength;
//            fn.aim = transform.position + transform.forward * aimLen;
//            fn.spread = spread;
//            fn.uniform = false;
//            seeker.StartPath(fn);
//            yield return fn.WaitForPath();
//            rp.Initialize(seeker, fn, true, RichFunnel.FunnelSimplification.Iterative);

//            path = fn.vectorPath;
//            if (move && path.Count > 1)
//            {
//                transform.position = path[path.Count - 1];
//                transform.forward = path[path.Count - 1] - path[path.Count - 2];
//            }
//            break;
//        }

//    }
//    private List<Vector3> path;
//    public float aimStrength = .3f;
//    public int spread = 20000;
//    public float aimLen = 40;
//    public void Update()
//    {
//        Vector3? o = null;
//        if (path != null)
//            for (int j = 0; j < path.Count; j++)
//            {
//                if (o.HasValue)
//                {
//                    Debug.DrawLine(o.Value, path[j], j % 2 == 0 ? Color.black : Color.white);

//                    //Vector3 dir = o.Value - path[j];

//                    //Vector3 force = Vector3.zero;
                    
//                    //float wLeft = 0;
//                    //float wRight = 0;
//                    //(rp.GetCurrentPart() as RichFunnel).FindWalls();
//                    //for (int i = 0; i < wallBuffer.Count; i += 2)
//                    //{

//                    //    Vector3 closest = AstarMath.NearestPointStrict(wallBuffer[i], wallBuffer[i + 1], tr.position);
//                    //    float dist = (closest - position).sqrMagnitude;

//                    //    if (dist > wallDist * wallDist) continue;

//                    //    Vector3 tang = (wallBuffer[i + 1] - wallBuffer[i]).normalized;

//                    //    //Using the fact that all walls are laid out clockwise (seeing from inside)
//                    //    //Then left and right (ish) can be figured out like this
//                    //    float dot = Vector3.Dot(dir, tang) * (1 - System.Math.Max(0, (2 * (dist / (wallDist * wallDist)) - 1)));
//                    //    if (dot > 0) wRight = System.Math.Max(wRight, dot);
//                    //    else wLeft = System.Math.Max(wLeft, -dot);
//                    //}

//                    //Vector3 norm = Vector3.Cross(Vector3.up, dir);
//                    //force = norm * (wRight - wLeft);

//                    //Debug.DrawRay(tr.position, force, Color.cyan);


//                }
//                o = path[j];
//            }
//    }
//}