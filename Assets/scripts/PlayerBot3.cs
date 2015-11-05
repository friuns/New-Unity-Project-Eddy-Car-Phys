//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Pathfinding;
//using UnityEngine;

//public partial class Player
//{

//    private uint botArea;
//    private IEnumerator StartBot3()
//    {
//        seeker = GetComponent<Seeker>();

//        var gridGraph = GridNode.GetGridGraph(0);
//        botArea = gridGraph.GetNearest(pos).node.Area;

//        if (cop)
//        {
//            while (true)
//            {
//                yield return null;
//                var nearestPlayer = GetNearestPlayer(false);
//                if (nearestPlayer) continue;
//                path = ABPath.Construct(pos, nearestPlayer.pos, null);
//                seeker.StartPath(path);
//                while (path.GetState() != PathState.Returned) yield return null;
//                vectorPath = path.vectorPath;
//            }
//        }
//        else
//            while (true)
//            {
//                yield return null;

//                UpdateRandomTarget();

//                while (true)
//                {
//                    var targetPos = (Vector3)targetNode.position;

//                    path = ABPath.Construct(GridNode.GetGridGraph(0).GetNearest(pos).clampedPosition, targetPos, null);
//                    seeker.StartPath(path);
//                    while (path.GetState() != PathState.Returned) yield return null;
//                    if (path.vectorPath.Count > 0)
//                        vectorPath = path.vectorPath;
//                    if ((pos - targetPos).magnitude < 5)
//                        break;
//                }
//            }
//    }
//    private GridNode targetNode;
//    private void UpdateRandomTarget()
//    {
//        while ((targetNode = GridNode.GetGridGraph(0).nodes.Random()).Area != botArea && _Game.bounds.Contains((Vector3)targetNode.position))
//        {
//        }
//    }
//    public void UpdateBot3()
//    {
//        Debug.DrawRay(pos, botDir, Color.blue);
//        if (vectorPath != null)
//        {
//            Vector3? o = null;
//            for (int i = 0; i < vectorPath.Count; i++)
//            {
//                if (o.HasValue)
//                    Debug.DrawLine(o.Value, vectorPath[i], i % 2 == 0 ? Color.black : Color.white);
//                o = vectorPath[i];
//            }
//        }


//        bool havePath = vectorPath != null && vectorPath.Count > 1;

//        if (averVel < 1 && Time.time - goBackTime > 5)
//        {
//            if (!cop)
//                UpdateRandomTarget();
//            goBackTime = Time.time;
//        }

//        averVel = Mathf.Lerp(averVel, rigidbody.velocity.magnitude, rigidbody.velocity.magnitude > averVel ? 1 : Time.deltaTime * 1);

//        var goback = Time.time - goBackTime < 2;
//        Vector3 dir = Vector3.zero;
//        if (havePath)
//        {
//            Vector3 nextPosition = vectorPath[0];
//            dir = (nextPosition - pos);
//            int next = new System.Random(viewId).Next(5, 15);
//            if (vectorPath.Count == 2 && distToPl < next)
//                dir = ((_Player.pos + _Player.rigidbody.velocity) - pos);
//            var normDir = ZeroY(dir).normalized;

//            Debug.DrawRay(pos, normDir, Color.blue);
//            float dot = Vector3.Dot(normDir, transform.right);
//            bool behind = Vector3.Dot(normDir, transform.forward) < 0;
//            var f = Mathf.Abs(dot) < 0.1f && !behind;
//            var rotLeft = dot < 0;
//            if (goback)
//                rotLeft = !rotLeft;
//            SetKey((int)KeyCode.A, rotLeft && !f);
//            SetKey((int)KeyCode.D, !rotLeft && !f);

//            //nitroDown = f && _Game.dif == Dif.Hard;
//            SetKey((int)KeyCode.Space, behind && velm > 10);

//            var stop = Mathf.Abs(dot) > .1f && velm > 10;
//            //var goback2 = gas ? goback : !goback;
//            SetKey((int)KeyCode.S, !stop && goback);
//            SetKey((int)KeyCode.W, !stop && !goback);
//        }


//    }
//}