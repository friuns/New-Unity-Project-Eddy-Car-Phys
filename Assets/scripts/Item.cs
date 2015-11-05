using System;
using System.Linq;
using UnityEngine;

public enum ItemEnum { None,Ammo, Rocket, Nitro, Life }
public class Item : ItemBase
{    
    public GameObject ammo;
    public GameObject rocket;
    public GameObject nitro;
    public GameObject life;
    public ItemEnum itemEnum;
    [RPC]
    public void SetItem(int item2)
    {
        var item = (ItemEnum)item2;
        itemEnum = item;
        ammo.SetActive(item == ItemEnum.Ammo);
        rocket.SetActive(item == ItemEnum.Rocket);
        nitro.SetActive(item == ItemEnum.Nitro);
        life.SetActive(item == ItemEnum.Life);
        if (item == ItemEnum.None)
        {
            _Game.disabledItems.Add(this);
        }
        else
        {
            _Game.disabledItems.Remove(this);
        }
    }
    

    public void OnTriggerEnter(Collider other)
    {
        var cc = other.transform.root.GetComponent<CarControl>();
        if (cc && cc.pl.IsMine)
        {
            
            var pl = cc.pl;
            Action a = () =>
            {
                var f = _Game.disabledItems.FirstOrDefault();
                if (f != null)
                    f.CallRPC(f.SetItem, UnityEngine.Random.Range(0, 3));
                pl.PlayOneShot(res.pickup.Random(), 2);
                PlayOneShotGui(res.item[(int)itemEnum], 4);
                CallRPC(SetItem, 0);
            };

            var maxLife = room.lifeDef + 100;
            if (itemEnum == ItemEnum.Life && pl.life < maxLife)
            {
                pl.life = Mathf.Max(100, pl.life) + 50;
                pl.life = Mathf.Min(maxLife, pl.life);
                a();
                pl.meshTest.Reset(pl==_Player);
            }

            var wep = pl.machineGun;
            var maxBullets = wep.maxBullets * 3;
            if (itemEnum == ItemEnum.Ammo && wep.bullets < maxBullets)
            {
                wep.bullets = Mathf.Max(wep.bullets, wep.maxBullets) + 100;
                wep.bullets = Mathf.Min(maxBullets, wep.bullets);
                a();
            }

            var maxNitro = 4;
            if (itemEnum == ItemEnum.Nitro && pl.nitro < maxNitro)
            {
                pl.nitro += 2;
                pl.nitro = Mathf.Min(pl.nitro, maxNitro);
                a();
            }

            if (itemEnum == ItemEnum.Rocket && pl.rocketLauncher.bullets < 1)
            {
                if (bs.settings.free && bs._Loader.daysElapsed > 1)
                {
                    _Hud.centerText2("RocketLauncher Requied");
                }
                else
                {
                    pl.CallRPC(pl.TakeRocket);
                    a();
                }
            }
        }
    }
    public override void OnPlConnected()
    {
        CallRPC(SetItem, (int)itemEnum);
        base.OnPlConnected();
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawIcon(tr.position, "bag.psd", true);
    }
}