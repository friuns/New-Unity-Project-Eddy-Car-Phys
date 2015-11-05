
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public partial class Player
{
    public Weapon rocketLauncher;
    public Weapon curWeapon { get { return rocketLauncher; } }
    public WeaponMachineGun machineGun;



    internal Vector3 distanceToCursor2;
    private Vector3 distanceToCursor;
    internal Vector3 turretDirection;
    internal Vector3 shootDirection;
    public int TargetPlayerId;

    //public void PlayOneShot(AudioClip AudioClip, float volume = 1)
    //{
    //    if (checkVisible(pos))
    //        audio.PlayOneShot(AudioClip, volume);
    //}
    public void UpdateDm()
    {
        if (!GameType.weapons) return;

        if (this.IsMine)
        {
            var targetPlayer = FirstOrDefault();
            this.TargetPlayerId = targetPlayer != null ? targetPlayer.viewId : -1;
            this.distanceToCursor2 = targetPlayer != null ? targetPlayer.distanceToCursor : this.curWeapon.turretCannon.forward;
        }
        else
        {
            var plPos = pos;
            float dist;
            Plane plane = new Plane((_Player.curWeapon.turretCannon.position - plPos).normalized, plPos);
            tempTr.position = plPos;
            tempTr.forward = (_Player.curWeapon.turretCannon.position - plPos);


            Ray ray = new Ray(_Player.curWeapon.turretCannon.position, _Player.curWeapon.turretCannon.forward);

            if (!plane.Raycast(ray, out dist))
            {
                this.distanceToCursor = _Player.curWeapon.turretCannon.forward * 99999;
            }
            else
            {
                Vector3 p = ray.GetPoint(dist);
                Debug.DrawLine(p, plPos);
                this.distanceToCursor = tempTr.InverseTransformPoint(p);
            }
            UpdateTurretEuler();
        }

    }
    private static Player FirstOrDefault()
    {
        Player b = null;
        foreach (Player a in _Game.listOfPlayers)
            if (a.active && !Physics.Linecast(CameraMainTransform.position, a.posUp, Layer.levelMask) && _Player.IsEnemy(a) && !a.dead && a.enabled && a.distanceToCursor.magnitude < 50)
                if (b == null || a.distanceToCursor.magnitude < b.distanceToCursor.magnitude)
                    b = a;
        return b;
        //return _Game.listOfPlayers.Slinq().Where(a => _Player.IsEnemy(a) && !a.dead && a.enabled && a.distanceToCursor.magnitude < 50).OrderBy(a => a.distanceToCursor.magnitude).FirstOrDefault();
    }
    public Bullet rocket;
    public void ZeroY(int max = 5)
    {
        var r = rigidbody;
        var v = r.velocity;
        r.velocity = new Vector3(v.x, Mathf.Min(v.y, max), v.z);
    }
    [RPC]
    public void Explode(Vector3 pos)
    {
        if (rocket != null)
            rocket.Explode(pos);
    }
    public void UpdateTurretEuler(bool shooting = false)
    {
        var turretCannon = rocketLauncher.turretCannon;
        if (distanceToCursor2 != Vector3.zero)
        {
            if (!_Game.players.TryGet(TargetPlayerId))
                turretDirection = shootDirection = distanceToCursor2;
            else
            {

                tempTr.position = _Game.players[TargetPlayerId].pos;
                tempTr.forward = turretCannon.position - tempTr.position;
                turretDirection = shootDirection = tempTr.TransformPoint(distanceToCursor2) - turretCannon.position;
                if (shooting)
                    Debug.DrawRay(turretCannon.position, shootDirection, Color.red, 5);
                else
                    Debug.DrawRay(turretCannon.position, shootDirection);
            }
        }
    }

    private static Transform m_tempTr;
    public static Transform tempTr { get { return m_tempTr ? m_tempTr : m_tempTr = new GameObject("tempTr").transform; } }

    [RPC]
    public void Shoot(int plId, Vector3 dist, Vector3 pos, Quaternion rot)
    {
        //print(PhotonNetwork.time - NetworkingPeer.photonMessageInfo.timestamp);
        rocketLauncher.Shoot(plId, dist, pos, rot);
    }
    [RPC]
    public void SetShoot(bool Obj)
    {
        //print(PhotonNetwork.time - NetworkingPeer.photonMessageInfo.timestamp);
        rocketLauncher.SetShoot(Obj);
    }
    [RPC]
    public void TakeRocket()
    {
        rocketLauncher.bullets = 1;
    }
}






































