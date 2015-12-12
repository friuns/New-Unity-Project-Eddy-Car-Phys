






using System;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using LitJson;
using UnityEngine;

[FieldAtr]
public class RoomSettings
{
    [FieldAtr(recursive = true)]
    public MapStat mapStats = new MapStat();
}
[Serializable]
public class MapStat
{
    public int id;
    public int version;
    public string mapName = "mp";
    public string mapJson = "";
    public string mapIcon = "";
    public string mapUrl = "mp";
    [FieldAtr(ignore = true)]
    public bool newMap;
    internal bool liked;
    public int hidden;

    private Texture2D m_texture;
    private WWW www;
    internal Texture2D texture
    {
        get
        {
            if (www == null && !string.IsNullOrEmpty(mapIcon))
                www = bs.Download(bs.mainSite + mapIcon, obj => m_texture = www.texture);
            return m_texture;
        }
    }
}

public partial class RoomInfo
{
    private GameTypeEnum? m_gameType;
    public GameTypeEnum gameType { get { return m_gameType ?? (m_gameType = (GameTypeEnum)customProperties.TryGet("gameType", GameTypeEnum.pursuit)).Value; } set { Set("gameType", (m_gameType = value).Value); } }
    public float matchTime { get { return (float)customProperties.TryGet("matchTime", 3 * 60f); } set { Set("matchTime", value); } }
    public float lifeDef { get { return (float)customProperties.TryGet("lifeDef", 100f); } set { Set("lifeDef", value); } }
    public bool autoLifeRecovery { get { return (bool)customProperties.TryGet("autoLifeRecovery", false); } set { Set("autoLifeRecovery", value); } }
    public int version { get { return (int)customProperties.TryGet("version", 0); } set { Set("version", value); } }
    public bool android { get { return (bool)customProperties.TryGet("android", false); } set { Set("android", value); } }
    public bool privateRoom { get { return (bool)customProperties.TryGet("privateRoom", false); } set { Set("privateRoom", value); } }
    public float gravity { get { return (float)customProperties.TryGet("gravity", Physics.gravity.y); } set { Set("gravity", value); } }

    public float gameSpeed { get { return (float)customProperties.TryGet("gameSpeed", 1f); } set { Set("gameSpeed", value); } }

    public bool disableRockets { get { return (bool)customProperties.TryGet("disableRockets", false); } set { Set("disableRockets", value); } }
    public bool disableMachineGun { get { return (bool)customProperties.TryGet("disableMachineGun", false); } set { Set("disableMachineGun", value); } }
    public bool disableVoiceChat { get { return (bool)customProperties.TryGet("disableVoiceChat", false); } set { Set("disableVoiceChat", value); } }
    public bool bots { get { return (bool)customProperties.TryGet("bots", false); } set { Set("bots", value); } }

    private RoomSettings m_sets;
    public RoomSettings sets { get { return m_sets ?? (m_sets = JsonMapper.ToObject<RoomSettings>((string)customProperties.TryGet("sets", "{}"))); } set { Set("sets", JsonMapper.ToJson(value)); } }


    private PlayerStats m_playerStats;
    public PlayerStats playerStats { get { return m_playerStats ?? (m_playerStats = JsonMapper.ToObject<PlayerStats>((string)customProperties.TryGet("playerStats", "{}"))); } set { Set("playerStats", JsonMapper.ToJson(value)); } }


    public string country { get { return (string)customProperties.TryGet("country", ""); } set { Set("country", value); } }

    private bool? m_collFix;
    public bool collFix { get { return m_collFix ?? (m_collFix = (bool)customProperties.TryGet("collFix", true)).Value; } set { Set("collFix", (m_collFix = value).Value); } }
    private Hashtable propertiesToSet = new Hashtable();
    public void Set(string Flags, object value)
    {
        if (!Equals(value, customProperties[Flags]) && (PhotonNetwork.isMasterClient || PhotonNetwork.room == null))
        {
            customProperties[Flags] = propertiesToSet[Flags] = value;
            Update();
        }
    }
    public void Update()
    {
        if (propertiesToSet.Count > 0 && this is Room && PhotonNetwork.room != null)
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

    //public VarParse<RoomSettings> varParse { get { return m_varParse ?? (m_varParse = new VarParse<RoomSettings>() { roomInfo = this, root = sets }); } set { m_varParse = value; } }
    //private VarParse<RoomSettings> m_varParse;
    public void UpdateValues()
    {
        //varParse.UpdateValues();
        sets = sets;
    }
}

