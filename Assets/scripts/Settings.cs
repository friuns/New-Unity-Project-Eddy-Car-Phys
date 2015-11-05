using System;
using System.Collections.Generic;
using UnityEngine;

public class Settings : bs
{
    //public float extraGravity=3;
    public bool fastLoad;
    public bool haveCar;
    public bool free;
    public bool release;
    public int version = 8;
    public int mpVersion = 8;
    public bool megaLife;
    public GameTypeEnum gameType;
    public float lerpMove = 3;
    public float lerpRot = 3;
    public bool offline;
    public float nitroForce = 40;
    public bool m_isDebug;
    public bool enableGuiEdit;
    public bool useLan;
    public bool disableTranslate;
    public bool androidTest;
    public void InitSettings()
    {
        //if (!Debug.isDebugBuild)

        if (!Application.isEditor)
            showAllStats = fastLoad = haveCar = disablePool = androidTest = disableTranslate = useLan = m_isDebug = false;

        if (!isDebug)
            megaLife = false;

        android = androidTest || Application.platform == RuntimePlatform.Android;

        if (!android && !Application.isEditor)
            free = false;
    }
    public TextAsset[] assetDictionaries;
    public int androidMapVersion;
    public int MapVersion;
    public string versionDate;
    public string[] appIds;
    public string settingsTxt;
    public bool disablePool;
    //public bool disPlayerPrefs2 { get { return fastLoad; } }
    //public string packageVersion;
    public float collDragSparks = 1;
    public bool bots;
    //public AISettings ai;
    public List<CarItem> cars = new List<CarItem>();
    public List<string> removeObjects = new List<string>() { "collision_wall" };
    public EditorSettings editor = new EditorSettings();
    [Serializable]
    public class EditorSettings
    {
        public float factorScale = 10;
        public float factorMove = 10;
        public float draggerSize = 0.001f;
    }
    public GUISkin unitySkin;
    public Font font;
    public bool showAllStats;
    public List<MapStat> maps = new List<MapStat>();
}

//[Serializable]
//public class AISettings
//{
//    public int size = 20000;
//    public int spread = 20000;
//    public float aimStrength = .3f;
//    public float dist = 1;
//    public float botSlowDownDist = 30;
//}
