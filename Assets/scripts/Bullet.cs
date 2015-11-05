using System.Globalization;
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
class Rocket { }
public class Bullet : bs
{
    public float speedUp;
    private int maxSpeed2 = 150;
    public float bulletSpeed = 1500;
    private float bulletSpeed2;
    public int maxSpeed;
    public GameObject explosion;
    public Weapon wep;
    public float randomMax = 0;
    public Player pl;
    private Vector3 vel;
    internal float extraTime;
    internal float dist;
    //public bool rocket;
    public float explosionForce = 10000;
    public float explosionRadius = 25;
    public bool remote2 = false;
    public int targetPlayer;
    public void Start()
    {
        pl.rocket = this;
        bulletSpeed2 = bulletSpeed + pl.velm;
        maxSpeed2 = maxSpeed + (int)pl.velm;
        var r = Random.Range(0, randomMax);
        foreach (Transform a in tr)
            a.position += tr.forward * r;

        var tryGet = _Game.players.TryGet(targetPlayer);
        if (tryGet == _Player)
            PlayOneShotGui(res.missileDetected, 2);
    }
    public void OnEnable()
    {
        vel = Vector3.zero;
        extraTime = 0;
        dist = 0;
    }
    public void Destroy2(GameObject obj)
    {
        Destroy(obj);
    }
    public void Update()
    {
        var time = Mathf.Min(extraTime, Time.deltaTime * 3);
        extraTime -= time;
        var deltaTime = Time.deltaTime + time;
        if (deltaTime == 0) return;

        var targetPl = _Game.players.TryGet(targetPlayer);
        if (targetPl)
        {
            rot = Quaternion.RotateTowards(rot, Quaternion.LookRotation(targetPl.pos - pos), Time.deltaTime * 20);
        }

        vel = tr.forward * bulletSpeed2 * deltaTime;
        if (dist > 1000 && !exploded)
            Destroy2(gameObject);
        dist += vel.magnitude;
        if (speedUp != 0)
        {
            bulletSpeed2 += speedUp * deltaTime;
            bulletSpeed2 = Mathf.Min(bulletSpeed2, maxSpeed2);
        }
        if (!exploded)
            foreach (RaycastHit h in Physics.RaycastAll(tr.position, tr.forward, vel.magnitude + 3, Layer.allmask).OrderBy(a => a.distance))
            {
                MonoBehaviour bs = h.transform.root.GetComponent<MonoBehaviour>();
                CarControl cc = bs as CarControl;
                Player hitPl = cc != null ? cc.pl : null;
                var IsMine = wep.pl == _Player;

                //if (hitPl != null && !explosion)
                //{                
                //    var b = remote2 ? !IsMine && hitPl.IsMine : IsMine;
                //    if (b && hitPl != wep.pl && !hitPl.dead)
                //    {                    
                //        if (wep.pl != null && wep.pl.IsEnemy(hitPl))
                //            hitPl.CallRPC(hitPl.SetLife, hitPl.life - wep.damage, wep.pl.viewId);
                //    }
                //    if (hitPl.IsEnemy(wep.pl) && checkVisible(h.point))
                //    {
                //        hitPl.OnCollision(h.point, 20);
                //        _Game.Emit(h.point, h.normal, _Game.sparks);
                //    }
                //}
                if ((hitPl != wep.pl || hitPl == null))
                {
                    if (explosion != null && IsMine)
                        pl.CallRPC(pl.Explode, h.point);

                    if (wep.bulletHitSound.Length > 0 && checkVisible(pos) && Vector3.Distance(_Player.pos, pos) < 100)
                    {
                        var audioClip = wep.bulletHitSound[Random.Range(0, wep.bulletHitSound.Length)];
                        PlayAtPosition(h.point, audioClip, hitPl && hitPl.IsMine ? 0 : 200);
                    }
                }

                //if (bs == null && !rocket)
                //{
                //    if (!lowQuality)
                //        _Game.Emit(h.point, h.normal, _Game.concrete);
                //    Destroy2(gameObject);
                //    break;
                //}
            }

        if (wep.slide)
        {
            RaycastHit h2;
            if (Physics.Raycast(pos, -tr.up, out h2, 2, Layer.levelMask))
            {
                pos += Vector3.up * Time.deltaTime * 2;
            }
        }
        tr.position += vel;
    }
    public void Explode(Vector3 hpoint)
    {
        _Game.StartCoroutine(AddMethod(.1f - (float)(PhotonNetwork.time - NetworkingPeer.photonMessageInfo.timestamp), delegate
         {
             foreach (var a in _Game.listOfPlayersActive)
             {
                 a.collisionTime = Time.time;
                 a.rigidbody.AddExplosionForce(explosionForce * a.rigidbody.mass, hpoint + Vector3.down * 1, explosionRadius);

                 float damage = (explosionRadius - (hpoint - a.pos).magnitude) / explosionRadius * wep.damage;
                 if (damage > 0)
                 {
                     if (a.IsMine)
                         a.AddLife(-damage, wep.pl.viewId);
                     a.OnCollision(hpoint, damage);
                 }
             }
             Destroy(Instantiate(explosion, hpoint, Quaternion.identity), 2);
             Destroy2(gameObject);
         }
         ));

        //_Player.rigidbody.AddExplosionForce(explosionForce * _Player.rigidbody.mass, hpoint + Vector3.down * 1, explosionRadius);

        //_Player.StartCoroutine(AddMethod(new WaitForFixedUpdate(), delegate
        //{
        //    _Player.ZeroY();
        //}));



        exploded = true;
    }
    private bool exploded;
    //public static void Hole(GameObject original, Vector3 position, Vector3 vector3)
    //{
    //    var g = (GameObject)Instantiate(original, position, Quaternion.LookRotation(vector3) * Quaternion.Euler(0, 0, Random.Range(0, 360)));
    //    g.hideFlags = HideFlags.HideInHierarchy;
    //    Destroy(g, highQuality ? 100 : 20);
    //}
    //public GameObject hole;
    private static void PlayAtPosition(Vector3 position, AudioClip audioClip, int priority = 200)
    {
        GameObject o = new GameObject();
        o.hideFlags = HideFlags.HideInHierarchy;
        var au = o.AddComponent<AudioSource>();
        au.transform.position = position;
        au.clip = audioClip;
        au.priority = priority;
        au.volume = res.volumeFactor;
        au.Play();
        Destroy(o, audioClip.length * Time.timeScale);
    }
}
