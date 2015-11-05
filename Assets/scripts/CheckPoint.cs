using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckPoint : ItemBase
{
    public void Awake()
    {

    }
    public override void Start()
    {
        if (_Game && !GameType.pursuitOrRace && !isDebug)
            gameObject.SetActive(false);

        base.Start();
    }
    public static List<CheckPoint> lastCheckPoint = new List<CheckPoint>();
    public void OnTriggerEnter(Collider other)
    {
        var pl = other.transform.root.GetComponent<CarControl>();

        if (pl == _Player.m_Car && !lastCheckPoint.Contains(this) && !_Player.cop) //
        {
            lastCheckPoint.Add(this);
            if (lastCheckPoint.Count > _Game.checkPoints.Count / 3)
                lastCheckPoint.RemoveAt(0);
            _Game.checkPoints.Remove(this);
            if (_Game.checkPoints.Count == 0)
                _Game.ResetCheckPoints();
            PlayerView pv = pl.pl.pv;
            pl.pl.stats.checkpoint.value++;
            pv.AddScore(1);
            _Hud.centerText("CheckPoint\n+" + _Hud.MoneyDif + "$");
            PlayOneShotGui(res.checkPoint);
            //_Loader.PlayOneShot(res.kill1.Random());
            //PlayOneShotGui(res.checkpoint2.Random());
        }
    }

}