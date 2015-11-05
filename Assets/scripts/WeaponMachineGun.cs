







using UnityEngine;

public class WeaponMachineGun : WeaponBase
{
    internal int maxBullets = 100;
    internal int bulletDecline = 35;
    //public AudioClip[] shoot;
    public ParticleSystem[] particles;
    public Animation[] anims;
    internal new AudioSource audio;
    private float shootTime = -10;
    public void Start()
    {
        audio = base.audio;
    }
    public override void Update()
    {
        if (!pl.IsMine)
        {
            if (pl.turretDirection != Vector3.zero)
                turretCannon.forward = pl.turretDirection;
        }
        else
            turretCannon.rotation = tr.rotation;
        if (_Game.players.TryGet(pl.TargetPlayerId))
        {
            var quaternion = turretCannon.rotation;
            var player = _Game.players[pl.TargetPlayerId];
            var lookRotation = Quaternion.LookRotation(player.posUp - turretCannon.position);
            //turretCannon.rotation = Quaternion.RotateTowards(quaternion, lookRotation, 5);
            if (pl.IsMine && android && Quaternion.Angle(quaternion, lookRotation) < (player.distToPl < 10 ? 60 : 10))
                Player.pressedKey.Add(KeyCode.Mouse0);
        }

        if ((bullets <= 0 || pl.dead) && pl.IsMine)
            Player.unpressKey.Add(KeyCode.Mouse0);

        var inputGetKey = pl.InputGetKey(KeyCode.Mouse0)&&!room.disableMachineGun && !win.active;
        foreach (var a in anims)
        {
            if (inputGetKey)
                if (!a.isPlaying)
                    a.Play();
            if (!inputGetKey)
                if (a.isPlaying)
                {
                    a.Rewind();
                    a.Sample();
                    a.Stop();
                }
        }

        foreach (var a in particles)
            a.enableEmission = inputGetKey;

        if (inputGetKey)
        {
            shootTime = Time.time;
            if (isMine && bullets > 0 && bullets - Time.deltaTime * bulletDecline < 0)
                PlayOneShotGui(res.reload, .4f);
            bullets -= Time.deltaTime * bulletDecline;
        }
        audio.enabled = inputGetKey;
        //if (inputGetKey && !audio.isPlaying)
        //    audio.Play();
        //if (!inputGetKey)
        //    audio.Pause();
        var max = Mathf.Max(bullets, maxBullets);
        if (Time.time - shootTime > 3 && bullets != max)
        {
            //PlayOneShotGui(res.reloadEnd);
            bullets = max;
        }

        //if (bulletGrow > 10 && bullets < res.maxBullets)
        //{
        //    bullets = Mathf.Min(bullets + bulletGrow*Time.deltaTime, res.maxBullets);
        //    //if (bullets == res.maxBullets && isMine)
        //        //PlayOneShotGui(res.noAmmo, .3f);
        //}
    }
    public bool isMine { get { return _Player == pl; } }
    //public float bulletGrow;
    public float lastTimeHit;
    public void OnCollision(ParticleSystem.CollisionEvent h, CarControl cc)
    {
        if (cc == pl.cc) return;
        Debug.DrawRay(h.intersection, h.normal, Color.red, 10);

        _Game.Hole(h.intersection, h.normal);

        if (_AutoQuality.particles || cc)
            _Game.Emit(h.intersection, h.normal, cc ? _Game.sparks : _Game.concrete);

        if (cc)
            cc.pl.PlayOneShot(res.damageSound.Random(), 1, true, Random.Range(.5f, 1));
        else if (Random.Range(1, 20) == 1)
            PlayClipAtPoint(res.ric.Random(), h.intersection);
        if (Time.time - lastTimeHit > .3f && cc)
        {
            lastTimeHit = Time.time;

            if (cc.pl.IsMine && cc.pl.IsEnemy(pl))
                cc.pl.AddLife(-(pl.pcVsAndroid ? 5 : 7), pl.viewId);

            if (_AutoQuality.destrQuality >= 2 || (pl.IsMine || cc.pl.IsMine) && _AutoQuality.destrQuality >= 1 && Random.Range(1, 3) == 1)
                cc.pl.OnCollision(h.intersection, 3, 10, true);
        }
    }





}