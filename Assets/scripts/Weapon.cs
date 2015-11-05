using System.Globalization;
#if !UNITY_WP8
#else
using ObscuredInt = System.Int32;
using ObscuredFloat = System.Single;
#endif
#if !UNITY_FLASH || UNITY_EDITOR
using System.Linq;
#endif
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
public class Weapon : WeaponBase
{
    public Bullet bullet;
    public float accuracy = .04f;
    public float damage = 11;
    public Light muzzleFlashLight;
    public Renderer muzzleFlash;
    public ParticleEmitter[] capsule;
    public AudioClip[] shootSound;
    internal Renderer[] renderers;
    private float lastShoot;
    public void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }
    public override void Update()
    {
        if (pl.dead) return;
        base.Update();

        if (muzzleFlash != null)
        {
            muzzleFlash.enabled = Time.time - lastShoot < .03f;
            muzzleFlashLight.enabled = Time.time - lastShoot < .05f;
        }
        if (shooting)
        {
            if (shootTm >= shootInterval)
            {
                shootTm = shootTm % shootInterval;
                lastShoot = Time.time;
                if (shootSound.Length > 0)
                    pl.audio.PlayOneShot(shootSound[Random.Range(0, shootSound.Length)], 1);
                foreach (var a in capsule)
                    a.Emit();
                if (pl.IsMine)
                    pl.CallRPC(pl.Shoot, pl.TargetPlayerId, pl.distanceToCursor2, turretCannon.position, turretCannon.rotation);
            }
        }
        shootTm += Time.deltaTime;
        foreach (var a in renderers)
            a.enabled = bullets > 0;
    }
    public override void Shoot(int plId, Vector3 dist, Vector3 pos, Quaternion rot)
    {
        if (plId != -1 && !_Game.players.TryGet(plId)) return;
        if (bullet != null)
        {
            bullets--;

            //var t = barrels[shootCount % barrels.Length];
            //pl.distanceToCursor2 = dist;
            //pl.TargetPlayerId = plId;
            //pl.UpdateTurretEuler(true);
            //var b = ((Bullet)Instantiate(bullet, t.position, Quaternion.LookRotation(pl.shootDirection.normalized)));

            var b = ((Bullet)Instantiate(bullet, pos, rot));
            b.targetPlayer = pl.TargetPlayerId;
            b.pl = pl;
            b.wep = this;
            //b.extraTime = (float)(PhotonNetwork.time - info.timestamp);
        }
    }
    public override void SetShoot(bool b)
    {
        shootTm = shootInterval;
        shooting = b;
        
    }
}
