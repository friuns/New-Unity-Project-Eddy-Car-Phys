using ObscuredInt = System.Int32;
using ObscuredFloat = System.Single;
using UnityEngine;

public class WeaponBase : bsInput
{
    public bool slide;
    public Player pl;
    internal float bullets;
    public AudioClip draw;
    public Transform[] barrels;
    public Texture2D cursor;
    public Transform turretCannon;
    internal bool shooting;
    public virtual void SetShoot(bool b)
    {

    }
    public AudioClip[] bulletHitSound;
    public virtual void Shoot(int plId, Vector3 dist, Vector3 pos, Quaternion rot)
    {
        
    }

    internal float shootTm = MaxValue;
    public float shootInterval = .100111f;
    public virtual void Update()
    {
        
        if (pl.IsMine)
        {
            var mb = Input2.GetKey(KeyCode.Mouse1) && _Game.started && (bullets > 0||isDebug)&& !room .disableRockets && !win.active;
            if (mb != shooting && shootTm > shootInterval)
                pl.CallRPC(pl.SetShoot, mb);
        }
    }
    
    
    public Vector3 plPos { get { return pl.pos; } }

}