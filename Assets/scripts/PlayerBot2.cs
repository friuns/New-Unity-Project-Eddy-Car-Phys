//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Pathfinding;
//using UnityEngine;

//public partial class Player
//{
//    private Seeker seeker;
//    private List<Vector3> vectorPath;
//    private Vector3 botDir;
//    private Path path;
//    private IEnumerator StartBot()
//    {
//        seeker = GetComponent<Seeker>();
//        while (true)
//        {
//            yield return null;
//            createPath:
//            ConstructPath();
//            while (path.GetState() != PathState.Returned) yield return null;
//            vectorPath = path.vectorPath;
//            foreach (Vector3 nextPosition in path.vectorPath)
//            {

//                while (true)
//                {
//                    yield return null;

//                    if (averVel < 1 && Time.time - goBackTime > 5)
//                    {
//                        goBackTime = Time.time;
//                        goto createPath;
//                    }
                    
//                    botDir = ZeroY(nextPosition - pos);
//                    if (botDir.magnitude < settings.ai.dist) break;
//                    var goback = Time.time - goBackTime < 2;
//                    averVel = Mathf.Lerp(averVel, rigidbody.velocity.magnitude, rigidbody.velocity.magnitude > averVel ? 1 : Time.deltaTime * 1);

//                    var botDirNormalized = ZeroY(botDir).normalized;

//                    float dot = Vector3.Dot(botDirNormalized, transform.right);
//                    bool behind = Vector3.Dot(botDirNormalized, transform.forward) < 0;
//                    var f = Mathf.Abs(dot) < 0.1f && !behind;
//                    var rotLeft = dot < 0;
//                    if (goback)
//                        rotLeft = !rotLeft;
//                    SetKey((int)KeyCode.A, rotLeft && !f);
//                    SetKey((int)KeyCode.D, !rotLeft && !f);
//                    //nitroDown = f && _Game.dif == Dif.Hard;
//                    SetKey((int)KeyCode.Space, behind && velm > 10);
//                    var gas = velm < (botDir.magnitude > settings.ai.botSlowDownDist ? 30 : 10);

//                    var goback2 = gas ? goback : !goback;
//                    SetKey((int)KeyCode.S, goback2);
//                    SetKey((int)KeyCode.W, !goback2);
//                    if (cop)
//                        goto createPath;
//                }
//            }
//        }
//    }
//    private void ConstructPath()
//    {
//        var nearest = GetNearestPlayer(!cop);
//        var vector3 = nearest ? nearest.pos : pos - transform.forward * 40;

//        if (cop && nearest)
//            path = ABPath.Construct(pos, vector3, null);
//        else
//        {
//            FleePath fleePath = FleePath.Construct(pos, (vector3 - pos).normalized * 40, settings.ai.size);
//            fleePath.aimStrength = settings.ai.aimStrength;
//            fleePath.spread = settings.ai.spread;
//            path = fleePath;
//        }
//        seeker.StartPath(path);
//    }
//    private Player GetNearestPlayer(bool cop)
//    {
//        return _Game.listOfPlayersActive.Where(a => a.cop == cop).OrderBy(a => (a.pos - pos).magnitude).FirstOrDefault();
//    }

//    private void UpdateBot2()
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
//    }

//}