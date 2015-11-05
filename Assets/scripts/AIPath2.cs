//using System.Linq;
//using UnityEngine;

//public class AIPath2 : bs
//{
//    public void Start()
//    {
//    }
//    Vector3 lastPoint;
//    public void Update()
//    {
//        if ((pos - lastPoint).magnitude > 50)
//        {
//            if (lastPoint != Vector3.zero)
//                Debug.DrawLine(lastPoint, pos, Color.yellow, 10);
//            lastPoint = pos;
//        }
//        transform.forward += ZeroY(Random.insideUnitSphere) * .3f;
//        var dIr = GetDIr();
//        if (dIr.HasValue)
//        {
//            var vector3 = dIr.Value;
//            transform.forward = Vector3.Lerp(transform.forward, dIr.Value, 1);
//            Debug.DrawRay(pos, vector3, Color.blue, 10);
//            pos += vector3;
//        }

//    }
//    Vector3? GetDIr()
//    {
//        var pos = this.pos;

//        var r = new Ray(pos, transform.forward);
//        var d = 1;
//        RaycastHit h = new RaycastHit(), h2;

//        var rays = new[] { (transform.forward + transform.right).normalized, transform.forward * 3, (transform.forward - transform.right).normalized };
//        if (rays.Any(a => Physics.Raycast(new Ray(pos, a), out h, a.magnitude * d, Layer.levelMask)))
//        {
//            Debug.DrawRay(h.point, h.normal, Color.red, 10);
//            r = new Ray(pos, ZeroY((h.point + h.normal) - pos).normalized * d);
//            if (Physics.Raycast(r, out h2, d, Layer.levelMask)) //tupik
//                r.direction = ZeroY(h2.normal + h.normal).normalized * d;
//        }

//        if (!Physics.Raycast(pos + r.direction, Vector3.down, out h, 100, Layer.levelMask))
//            return null;

//        return h.point - pos + Vector3.up;
//    }

//    public struct Ray
//    {
//        public Vector3 direction;
//        public Vector3 position;
//        public Ray(Vector3 pos, Vector3 dir)
//        {
//            direction = dir;
//            position = pos;
//        }
//        public static implicit operator UnityEngine.Ray(Ray r)
//        {
//            return new UnityEngine.Ray(r.position, r.direction);
//        }
//    }
//}