using System;
using System.Collections.Generic;
using gui = UnityEngine.GUILayout;
using System.Linq;
using System.Reflection;
using System.Text;
using ExitGames.Client.Photon;
using UnityEngine;


public enum AccType { Guest, User, Mod, Dev }
[Field(recursive = true, save = true)]
public class PlayerStats
{
    public Gts[] games = new Gts[10].Fill(() => new Gts());
    [Field(save = false)]
    public Gts curGame = new Gts();
    public int moneyFound;
    public int reports;
    public int damageDeal;
    public int carIndex;
}
public partial class PhotonPlayer
{
    public string url { get { return (string)customProperties.TryGet("url", ""); } set { Set("url", value); } }
    public bool mute;

    public string deviceId { get { return (string)customProperties.TryGet("deviceId", ""); } set { Set("deviceId", value); } }
    public CarItem carSet { get { return bs.settings.cars[stats.carIndex]; } }
    private Hashtable propertiesToSet = new Hashtable();
    public AccType accType { get { return (AccType)customProperties.TryGet("accType", 0); } set { Set("accType", (int)value); } }

    public bool dev { get { return accType == AccType.Dev; } }


    public void Set(string key, object value)
    {
        if (/*isLocal && */!Equals(value, customProperties[key]))
            customProperties[key] = propertiesToSet[key] = value;
    }

    public VarParse varParse { get { return m_varParse ?? (m_varParse = new VarParse() { pl = this, root = stats }); } set { m_varParse = value; } }
    private VarParse m_varParse;
    public PlayerStats stats = new PlayerStats();
    public Gts curGame { get { return stats.curGame; } }
    public void Update()
    {
        varParse.UpdateValues();
        if (propertiesToSet.Count > 0)
        {
            SetCustomProperties(propertiesToSet);
            propertiesToSet.Clear();
        }
    }
    public void DrawStats()
    {
        bs.win.windowTitle = "Statics for " + name;
        GuiClasses.Label("ID".PadRight(20) + ID);
        if (GuiClasses.Button("ShowADvStats"))
            bs.win.ShowWindow(delegate { gui.Label(ToString2()); });
        var gameType = Enum.GetNames(typeof(GameTypeEnum));
        if (stats.moneyFound > 0)
            GuiClasses.TextField("found", stats.moneyFound.ToString());
        for (int i = 0; i < gameType.Length; i++)
        {
            gui.BeginVertical(gameType[i], GUI.skin.window);
            var gts = stats.games[i];
            gts.DrawStats();
            gui.EndVertical();
        }
        if (bs._Game)
        {
            gui.BeginVertical(bs.previousRoom.gameType.ToString(), GUI.skin.window);
            curGame.DrawStats();
            gui.EndVertical();
        }
    }



    public string ToString2()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("ID".PadRight(20) + ID);
        foreach (var a in customProperties)
        {
            var value = (ReferenceEquals(a.Key, "gameType") ? ((GameTypeEnum)a.Value) : a.Value);
            sb.AppendLine(a.Key.ToString().PadRight(40) + value);
        }
        return sb.ToString();
    }
    public int GetMoney()
    {
        var money = stats.games.Sum(a => a.GetMoney()) + stats.moneyFound;
        if (bs._Game)
            money += curGame.GetMoney();
        return money;
    }
    private void Constructor()
    {
    }

}
