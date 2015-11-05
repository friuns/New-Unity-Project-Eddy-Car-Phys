using System.Linq;
using System.Text;
using UnityEngine;

public partial class PlayerView : bsNetwork
{
    internal int kills;
    internal int deaths;
    internal float score;
    internal float totalScore;

    public Team team { get { return _Game.teams[(int)teamEnum]; } }
    internal TeamEnum teamEnum = TeamEnum.Red;
    public int scoreInt { get { return (int)score; } }
    public Player pl;
    public string playerName;// { get { return ownerName; } }
    public new string name { get { return playerName; } }
    public int version;
    public void Awake()
    {
        OnValidate();
    }
    private void OnValidate()
    {
        if (!Application.isPlaying || !gameObject.activeInHierarchy) return;
        _Game.playerViews[viewId] = this;
        if (!_PlayerView)
            _PlayerView = this;
    }
    public void Start()
    {
        base.name = "PlayerView " + ownerName;
    }
    public void InitNetwork()
    {
        //CallRPC(SetPl, pl.photonView.viewID);
        CallRPC(SetVersion, version);
        CallRPC(SetName, playerName);
        CallRPC(SetDeaths, deaths);
        CallRPC(SetKills, kills);
        CallRPC(SetScore, score, totalScore);
    }

    [RPC]
    private void SetName(string s)
    {
        playerName = s;
        pl.RefreshText();
    }
    [RPC]
    private void SetPl(int Obj)
    {
        if (online)
            pl = PhotonView.Find(Obj).GetComponent<Player>();
    }
    public void SetPlID()
    {
        if (online)
            photonView.RPC("SetPl", PhotonTargets.AllBufferedViaServer, pl.photonView.viewID);
    }
    //[RPC]
    //public void SetTeam( int t)
    //{
    //    Debug.Log(IsMine + " SetTeam", gameObject);
    //    teamEnum = (TeamEnum)t;

    //}
    [RPC]
    public void SetDeaths(int Obj)
    {
        deaths = Obj;
    }

    [RPC]
    public void SetVersion(int Obj)
    {
        version = Obj;
    }

    [RPC]
    public void SetKills(int Obj)
    {
        kills = Obj;
    }
    public void AddDeaths(int Obj)
    {
        CallRPC(SetDeaths, deaths + Obj);
    }
    public void AddKills(int I)
    {
        CallRPC(SetKills, kills + I);
    }
    public void AddScore(float Obj)
    {
        CallRPC(SetScore, score + Obj, totalScore + Obj);
        //_Loader.playerScore += Obj;        
    }

    public void ResetScore()
    {
        kills = 0;
        deaths = 0;
        score = 0;
    }


    public static LeadState leadState;
    public bool ally;
    [RPC]
    public void SetScore(float Obj, float total)
    {
        score = Obj;
        totalScore = total;
        pl.distWent = 0;

        if (_PlayerView.score > 0 && _Game.playersList.Count > 1)
        {
            var max = _Game.playersList.Where(a => a != _Player).Select(a => a.pv.score).Max();

            if (max == _Player.pv.score && leadState != LeadState.Tie)
            {
                leadState = LeadState.Tie;
                //_Game.PlayOneShot(res.tieLead);
            }
            else if (max > _Player.pv.score && leadState != LeadState.Lost)
            {
                leadState = LeadState.Lost;
                _Game.PlayOneShot(res.lostLead);
            }
            else if (max < _Player.pv.score && leadState != LeadState.Taken)
            {
                leadState = LeadState.Taken;
                _Game.PlayOneShot(res.takenLead);
            }
        }

    }
    public override void OnPlConnected()
    {
        InitNetwork();
        base.OnPlConnected();
    }
    public string getText(bool B = false)
    {
        StringBuilder sb = new StringBuilder();
        if (pl.voiceChatting)
            sb.Append("ö ");
        sb.Append("<color=").Append(ally ? "green" : teamEnum == TeamEnum.Red ? "red" : "blue").Append(">").Append(playerName).Append("</color>");
        return sb.ToString();
    }

}

//if (_PlayerView.scoreInt > 0 && Time.time - _Game.stateChangeTime > 1 && Time.timeSinceLevelLoad > 1)
//        {
//            int max = _Game.playersList.Where(a => a != _Player).Select(a => a.pv.scoreInt).Max();
//            print("max:"+max + " pl:" + _PlayerView.scoreInt);
//            if (max == _PlayerView.scoreInt && leadState != LeadState.Tie)
//            {
//                leadState = LeadState.Tie;
//                _Loader.PlayOneShot(res.tieLead);
//            }
//            else if (max > _PlayerView.kills && leadState != LeadState.Lost)
//            {
//                leadState = LeadState.Lost;
//                _Loader.PlayOneShot(res.lostLead);
//            }
//            else if (max < _PlayerView.scoreInt && leadState != LeadState.Taken)
//            {
//                leadState = LeadState.Taken;
//                _Loader.PlayOneShot(res.takeLead);
//            }
//        }