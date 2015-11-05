using System.Linq;
using UnityEngine;

public partial class Player
{
    private float botSpawnTime;
    private bool botSpawnUsed;
    private void UpdateBot()
    {
        NavMeshPath path = new NavMeshPath();


        //NavMesh.Raycast(_Player.pos + Vector3.up * 50, Vector3.down * 1000, out h, 0);
        if (KeyDebug(KeyCode.Q) || (_Game.dif >= Dif.Normal || isDebug) && (distToPl > 200) && Time.time - botSpawnTime > (isDebug ? .1f : _Game.hard ? 1 : 5))
        {

            botSpawnTime = Time.time;
            Spawn sp = _Game.spawns.Where(a => a != botSpawnUsed && (a.pos - _Player.pos).magnitude < 200 && inFrontInvisible(a.pos)).OrderBy(a => (a.pos - pos).magnitude).FirstOrDefault();
            if (sp != null)
            {
                botSpawnUsed = sp;
                pos = sp.pos;
                //transform.up = Vector3.up;

                CalculatePath(pos, _Player.pos, path);
                if (path.corners.Length > 1)
                    transform.forward = path.corners[1] - path.corners[0];
                rigidbody.angularVelocity = rigidbody.velocity = Vector3.zero;
                StartCoroutine(AddMethod(new WaitForFixedUpdate(), delegate { rigidbody.velocity = transform.forward * 13; }));
            }
        }
        CalculatePath(pos, _Player.pos, path);
        var c = path.corners;
        for (int i = 0; i < c.Length - 1; i += 2)
            Debug.DrawLine(c[i], c[i + 1], Color.red);
        bool havePath = path.corners.Length > 1;
        if (averVel < 1 && Time.time - goBackTime > 5)
            goBackTime = Time.time;

        averVel = Mathf.Lerp(averVel, rigidbody.velocity.magnitude, rigidbody.velocity.magnitude > averVel ? 1 : Time.deltaTime * 1);

        var goback = Time.time - goBackTime < 2;
        if (havePath)
        {
            Vector3 nextPosition = path.corners[1];
            Vector3 dir = (nextPosition - pos);
            int next = new System.Random(viewId).Next(5, 15);
            if (path.corners.Length == 2 && distToPl < next)
                dir = ((_Player.pos + _Player.rigidbody.velocity) - pos);
            dir = ZeroY(dir).normalized;

            Debug.DrawRay(pos, dir, Color.blue);
            float dot = Vector3.Dot(dir, transform.right);
            bool behind = Vector3.Dot(dir, transform.forward) < 0;
            var f = Mathf.Abs(dot) < 0.1f && !behind;
            var rotLeft = dot < 0;
            if (goback)
                rotLeft = !rotLeft;
            SetKey((int)KeyCode.A, rotLeft && !f);
            SetKey((int)KeyCode.D, !rotLeft && !f);
            nitroDown = f && _Game.dif == Dif.Hard;
            SetKey((int)KeyCode.Space, behind && velm > 10);
        }
        SetKey((int)KeyCode.S, havePath && goback);
        SetKey((int)KeyCode.W, havePath && !goback);
    }
    private void CalculatePath(Vector3 apos, Vector3 bpos, NavMeshPath path)
    {
        NavMesh.CalculatePath(apos, bpos, 1, path);
        //NavMeshHit h;
        //NavMeshHit h2;
        //if (NavMesh.SamplePosition(apos, out h, 50, 1) && NavMesh.SamplePosition(bpos, out h2, 50, 1))
        //    NavMesh.CalculatePath(h.position, h2.position, 1, path);
    }
}