






using System;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using UnityEngine;

[Field]
public class RoomSettings
{
    [Field(Name = "Auto Repair")]
    public bool autoLifeRecovery;// { get { return (bool)customProperties.TryGet("autoLifeRecovery", false); } set { Set("autoLifeRecovery", value); } }
    [Field(dontDraw = true)]
    public MapStat mapStats = new MapStat();

}
public partial class RoomInfo
{
    private GameTypeEnum? m_gameType;
    public GameTypeEnum gameType { get { return m_gameType ?? (m_gameType = (GameTypeEnum)customProperties.TryGet("gameType", GameTypeEnum.pursuit)).Value; } set { Set("gameType", (m_gameType = value).Value); } }
    public float matchTime { get { return (float)customProperties.TryGet("matchTime", 3 * 60f); } set { Set("matchTime", value); } }
    public float lifeDef { get { return (float)customProperties.TryGet("lifeDef", 100f); } set { Set("lifeDef", value); } }
    public int version { get { return (int)customProperties.TryGet("version", 0); } set { Set("version", value); } }
    public bool android { get { return (bool)customProperties.TryGet("android", false); } set { Set("android", value); } }
    public bool privateRoom { get { return (bool)customProperties.TryGet("privateRoom", false); } set { Set("privateRoom", value); } }
    public float gravity { get { return (float)customProperties.TryGet("gravity", Physics.gravity.y); } set { Set("gravity", value); } }

    public float gameSpeed { get { return (float)customProperties.TryGet("gameSpeed", 1f); } set { Set("gameSpeed", value); } }

    public bool disableRockets { get { return (bool)customProperties.TryGet("disableRockets", false); } set { Set("disableRockets", value); } }
    public bool disableMachineGun { get { return (bool)customProperties.TryGet("disableMachineGun", false); } set { Set("machineGun", value); } }
    public bool disableVoiceChat { get { return (bool)customProperties.TryGet("disableVoiceChat", false); } set { Set("disableVoiceChat", value); } }
    public bool bots { get { return (bool)customProperties.TryGet("bots", false); } set { Set("bots", value); } }

    public string mapUrl { get { return sets.mapStats.mapUrl; } }
    public RoomSettings sets = new RoomSettings();
    //public string music { get { return (string)customProperties.TryGet("music"); } set { Set("music", value); } }


    private bool? m_collFix;
    public bool collFix { get { return m_collFix ?? (m_collFix = (bool)customProperties.TryGet("collFix", true)).Value; } set { Set("collFix", (m_collFix = value).Value); } }
    private Hashtable propertiesToSet = new Hashtable();
    public void Set(string Flags, object value)
    {

        if (!Equals(value, customProperties[Flags]) && (PhotonNetwork.isMasterClient || PhotonNetwork.room == null))
            customProperties[Flags] = propertiesToSet[Flags] = value;
    }
    public void Update()
    {
        if (propertiesToSet.Count > 0)
        {
            ((Room)this).SetCustomProperties(propertiesToSet);
            propertiesToSet.Clear();
        }
    }
    public int killScore { get { return (int)customProperties.TryGet("killScore", 10); } set { Set("killScore", value); } }
    public int checkPointScore { get { return (int)customProperties.TryGet("checkPointScore", 10); } set { Set("checkPointScore", value); } }

    public bool noKillScore { get { return (bool)customProperties.TryGet("noKillScore", false); } set { Set("noKillScore", value); } }

#if UNITY_ANDROID || UNITY_EDITOR
    public int maxPlayers2 = 4;
#else
    public int maxPlayers2 = 10;
#endif

    public VarParse varParse { get { return m_varParse ?? (m_varParse = new VarParse() { roomInfo = this, root = sets }); } set { m_varParse = value; } }
    private VarParse m_varParse;
}

