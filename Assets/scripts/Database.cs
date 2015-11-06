using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using gui = UnityEngine.GUILayout;
public class Database
{

}
public static class Layer
{

    public static int def = LayerMask.NameToLayer("Default");
    public static int level = LayerMask.NameToLayer("level");
    public static int dragger = LayerMask.NameToLayer("dragger");
    public static int levelMask = 1 << def | 1 << level;
    public static int particles = 2;
    public static int dontSpawn = LayerMask.NameToLayer("DontSpawn");
    public static int car = LayerMask.NameToLayer("car");
    public static int allmask = levelMask | 1 << car;
    //public static Layer
}
public enum GameState { none, started, finnish }


public class AccessTimer<T>
{
    private T t;
    public int frameCount = 1;
    public T value
    {
        get { return needUpdate ? default(T) : t; }
        set
        {
            t = value;
            accessTime = Time.frameCount;
        }
    }

    private int accessTime = -999;
    public bool needUpdate
    {
        get
        {
            return Time.frameCount - accessTime >= frameCount;
        }
    }

    public override string ToString()
    {
        return value.ToString();
    }
}

public partial class CarSound
{
    //public AudioClip[] siren;
}

public enum Dif { Easy, Normal, Hard }

[Flags]
public enum WeaponEnum
{
    machinegun = 1, freezer = 2, fire = 4
}

public static class Tag
{
    public static string HitBox = "HitBox";
    public static string PCOnly = "PC Only";
}
public enum Contr
{
    keys, mouse, acel
}
//public static class Contr
//{
//    public static string[] names = new string[] { "Pad", "Keys", "Accelerometer" };
//    public static int mouse = 0;
//    public static int keys = 1;
//    public static int acel = 2;
//}

public class KeyHudBool
{
    public Touch? hitTest;
    public KeyHudBool LoadPos()
    {
        if (posx != 0 && posy != 0)
            archor.pos = new Vector3(posx, posy, 0);
        if (scale != 1)
            UpdateScale();
        return this;
    }
    public bool hold;
    public Archor archor;
    //public Color? m_hudColor;
    //public Color hudColor { get { return m_hudColor ?? (m_hudColor = guitext.color).Value; } set { if (value != m_hudColor)  m_hudColor = guitext.color = value; } }
    public Color hudColor { get { return archor.color; } set { archor.color = value; } }
    public KeyCode key;
    public bool down;
    public bool up;
    public bool secondPlayer;
    public float dist;
    public string prefix { get { return bs._Loader.playerName + archor.name + secondPlayer; } }
    public float? m_scale;
    public float scale { get { return m_scale ?? (m_scale = PlayerPrefs.GetFloat(prefix + "scale2", key == KeyCode.W ? 1.5f : 1)).Value; } set { PlayerPrefs.SetFloat(prefix + "scale2", (m_scale = value).Value); } }
    public float? m_posx;
    public float posx { get { return m_posx ?? (m_posx = PlayerPrefs.GetFloat(prefix + "posx2", 0)).Value; } set { PlayerPrefs.SetFloat(prefix + "posx2", (m_posx = value).Value); } }
    public float? m_posy;
    public float posy { get { return m_posy ?? (m_posy = PlayerPrefs.GetFloat(prefix + "posy2", 0)).Value; } set { PlayerPrefs.SetFloat(prefix + "posy2", (m_posy = value).Value); } }
    public void UpdateScale()
    {
        var last = this;
        last.archor.scale = last.scale;
    }
}

public static class MyDebug
{
    [Conditional("UNITY_EDITOR")]
    public static void LogWarning(string s)
    {
        Debug.LogWarning(s);
    }
    [Conditional("UNITY_EDITOR")]
    public static void LogError(string s)
    {
        Debug.LogError(s);
    }
    [Conditional("UNITY_EDITOR")]
    public static void Log(string s)
    {
        Debug.Log(s);
    }
}
public enum ToolType { Select, Delete }
public enum PacketType { Spawn, Version }
public enum LeadState
{
    Lost,
    Tie,
    Taken
}

public class web : bs { }


public class Score
{
    public string name;
    public int points;//{ get { return m_points; } set { m_points = value; } }
    public int pointsValue { get { return points * value; } }
    //private int m_points;
    public int value;
    public Score(int Points)
    {
        points = Points;
    }
    //public static Score operator ++(Score c2)
    //{
    //    c2.value++;
    //    return c2;
    //}
}
//[Serializable]
public class Gts
{
    internal List<Score> scores = new List<Score>();
    public Score kills = new Score(3);
    public Score deaths;
    public int timePlayed;
    public Score checkpoint = new Score(1);
    public Score escape = new Score(10);
    public Score arest = new Score(10);
    public Score wonRace = new Score(10);
    public Score teamWon = new Score(30);
    public Score killLeader = new Score(10);
    public Gts()
    {
        this.GetFields<Score>((a, b) => { scores.Add(b); b.name = a; });
    }

    public float GetRank()
    {
        return ((kills.value + arest.value) / (deaths.value + 50)) * 10;
    }
    public int GetMoney()
    {
        return scores.Sum(a => a.value * a.points);
    }

    public void DrawStats()
    {
        gui.BeginHorizontal();
        var GetValue = new Action<Func<Score, string>, string>((ac, title) =>
        {
            var h = gui.Height(20);
            gui.BeginVertical();
            gui.Label(GuiClasses.Tr(title), new GUIStyle(GUI.skin.label) { wordWrap = false });
            foreach (var a in scores.Where(a => a.value > 0 || bs.settings.showAllStats))
                gui.Label(ac(a), h);
            gui.EndVertical();
        });

        GetValue(a => a.name, "");
        GetValue(a => a.value + "x" + a.points, "");
        GetValue(a => (a.value * a.points) + "$", "");
        gui.EndHorizontal();
        if (GetMoney() > 0 || bs.settings.showAllStats)
            GuiClasses.TextField("total:", GetMoney() + "$");
    }





    public void Add(Gts g)
    {
        for (int i = 0; i < scores.Count; i++)
            scores[i].value += g.scores[i].value;
    }
}
[Serializable]
public class MapStat
{
    public int mapId;
    public string mapName = "mp";
    public string mapUrl = "mp";
    [Field(ignore = true)]
    public bool newMap;

}