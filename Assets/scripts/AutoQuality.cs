#define blur
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using gui = UnityEngine.GUILayout;
using System.Threading;
public class AutoQuality : GuiClasses
{
    internal float fps = 0;
    internal bool stop;
    public void OnLevelWasLoaded2()
    {
        if (autoQuality)
            SetQuality(Q2.Low);
        else
            UpdateQuality();
        //SetQuality(autoQuality ? Q2.Low : quality);
        stop = !autoQuality;
        enabled = true;
        fps = 0;
    }
    public void Awake()
    {
        print("AutoQUality Awake " + enabled);
        QualitySettings.vSyncCount = Application.isEditor ? 1 : -1;
    }
    //public new bool enabled { get { return base.enabled; } set { base.enabled = value; } }

    public bool? m_autoQuality;
    public bool autoQuality { get { return (m_autoQuality ?? (m_autoQuality = PlayerPrefsGetBool("autoQuality2", true)).Value); } set { PlayerPrefsSetBool("autoQuality2", (m_autoQuality = value).Value); } }


    public int globalQuality { get { return m_globalQuality ?? (m_globalQuality = PlayerPrefs.GetInt("globalQuality", 2)).Value; } set { PlayerPrefs.SetInt("globalQuality", (m_globalQuality = value).Value); } }
    public int? m_globalQuality;

    public int quality { get { return m_quality ?? (m_quality = PlayerPrefs.GetInt("quality3", android ? Q2.Low : Q2.High)).Value; } set { PlayerPrefs.SetInt("quality3", (int)(m_quality = value)); } }
    public int? m_quality;

    public int textureQuality { get { return m_textureQuality ?? (m_textureQuality = PlayerPrefs.GetInt("textureQuality", 2)).Value; } set { PlayerPrefs.SetInt("textureQuality", (m_textureQuality = value).Value); } }
    public int? m_textureQuality;
    public int graphQuality { get { return m_graphQuality ?? (m_graphQuality = PlayerPrefs.GetInt("graphQuality", 2)).Value; } set { PlayerPrefs.SetInt("graphQuality", (m_graphQuality = value).Value); } }
    public int? m_graphQuality;

    public int physQuality { get { return m_physQuality ?? (m_physQuality = PlayerPrefs.GetInt("physQuality", 2)).Value; } set { PlayerPrefs.SetInt("physQuality", (m_physQuality = value).Value); } }
    public int? m_physQuality;

    public int destrQuality { get { return m_destrQuality ?? (m_destrQuality = PlayerPrefs.GetInt("destrQuality", 2)).Value; } set { PlayerPrefs.SetInt("destrQuality", (m_destrQuality = value).Value); } }
    public int? m_destrQuality;

    //public int shadowQuality { get { return m_shadowDist ?? (m_shadowDist = PlayerPrefs.GetInt("shadowQuality", 2)).Value; } set { PlayerPrefs.SetInt("shadowQuality", (int)(m_shadowDist = value)); } }
    //public int? m_shadowDist;

    public bool? m_skidmarks;
    public bool skidmarks { get { return (m_skidmarks ?? (m_skidmarks = PlayerPrefsGetBool("skidmarks", true)).Value); } set { PlayerPrefsSetBool("skidmarks", (m_skidmarks = value).Value); } }

    //public bool? m_autoFullScreen;
    //public bool autoFullScreen { get { return (m_autoFullScreen ?? (m_autoFullScreen = PlayerPrefsGetBool("autoFullScreen", true)).Value); } set { PlayerPrefsSetBool("autoFullScreen", (m_autoFullScreen = value).Value); } }
    public bool? m_autoFullScreen;
    public bool autoFullScreen { get { return (m_autoFullScreen ?? (m_autoFullScreen = PlayerPrefsGetBool("autoFullScreen", true)).Value); } set { PlayerPrefsSetBool("autoFullScreen", (m_autoFullScreen = value).Value); } }

    public float drawDistance { get { return m_drawDistance ?? (m_drawDistance = PlayerPrefs.GetFloat("drawDistance", 600)).Value; } set { PlayerPrefs.SetFloat("drawDistance", (m_drawDistance = value).Value); } }
    public float? m_drawDistance;

    public bool? m_particles;
    public bool particles { get { return (m_particles ?? (m_particles = PlayerPrefsGetBool("particles", true)).Value); } set { PlayerPrefsSetBool("particles", (m_particles = value).Value); } }


    public bool? m_motionblur;
    public bool motionblur { get { return (m_motionblur ?? (m_motionblur = PlayerPrefsGetBool("motionblur", true)).Value); } set { PlayerPrefsSetBool("motionblur", (m_motionblur = value).Value); } }

    //public bool? m_fieldOfView;
    //public bool fieldOfView { get { return (m_fieldOfView ?? (m_fieldOfView = PlayerPrefsGetBool("fieldOfView", true)).Value); } set { PlayerPrefsSetBool("fieldOfView", (m_fieldOfView = value).Value); } }

    //public bool? m_disableLights;
    //public bool disableLights { get { return (m_disableLights ?? (m_disableLights = PlayerPrefsGetBool("disableLights", false)).Value); } set { PlayerPrefsSetBool("disableLights", (m_disableLights = value).Value); } }

    //public bool? m_disableSounds;
    //public bool disableSounds { get { return (m_disableSounds ?? (m_disableSounds = PlayerPrefsGetBool("disableSounds", true)).Value); } set { PlayerPrefsSetBool("disableSounds", (m_disableSounds = value).Value); } }

    public void Update()
    {

        if (KeyDebug(KeyCode.L))
        {
            QualitySettings.vSyncCount = -1;
            Application.targetFrameRate = 10;
        }
        //for (int i = 0; i < 999999999; i++)
        //{

        //}
        if (bs._Loader.deltaTime != 0)
            fps = Mathf.Lerp(fps, 1f / bs._Loader.deltaTime, bs._Loader.deltaTime * (1f / bs._Loader.deltaTime) > fps ? 1 : .01f);
        LogScreen(isDebug ? fps : Mathf.Round(1f / bs._Loader.deltaTime));

        if (!stop && fps > 40 && autoQuality && globalQuality < (android ? Q2.normal : Q2.High))
            SetQuality(globalQuality + 1);
        //if (KeyDebug(KeyCode.E, "Change Quality"))
        //    SetQuality(quality + 1);
        //KeyDebug(KeyCode.Q, "Change Quality") || 
        if (fps < 35 && globalQuality > Q2.Low && autoQuality)
        {
            SetQuality(globalQuality - 1);
            print("Auto quality stop");
            stop = true;
        }
    }
    private bool safeMode;
    public void DrawSetQuality()
    {
        win.SetupWindow(500, 500);
        Label("Graphics Quality", "label");
        safeMode = Toggle(safeMode, "Safe Mode");
        autoFullScreen = Toggle(autoFullScreen, "Auto Full Screen");
        GUI.enabled = !safeMode;
        Label("Quality Settings");
        gui.BeginHorizontal();
        bool updated = false;
        autoQuality = GlowButton("Auto", autoQuality) || autoQuality;
        int q = Toolbar(autoQuality ? -1 : globalQuality, android ? new string[] { "Normal", "High", "Ultra", null } : new string[] { "Lowest", "Low", "normal", "High" }, false, false, 99, -1, false);
        gui.EndHorizontal();
        graphQuality = Toolbar(graphQuality, new[] { "low", "Normal", "high" }, ref updated, "Shader Quality");
        quality = Toolbar(quality, new[] { "lowest", "low", "normal", "high" }, ref updated, "Game Quality");
        destrQuality = Toolbar(destrQuality, new[] { "Off", "normal", "high" }, title: "Destruction Quality");
        physQuality = Toolbar(physQuality, new[] { "low", "normal", "high" }, ref updated, "Physics Quality");
        textureQuality = Toolbar(textureQuality, new[] { "low", "normal", "high" }, ref updated, "Texture Quality");
        //disableLights = Toggle(disableLights, "disable Lights", ref updated);
        skidmarks = Toggle(skidmarks, "SkidMarks", ref updated);
        particles = Toggle(particles, "particles", ref updated);
        motionblur = Toggle(motionblur, "motionblur ", ref updated);

        gui.BeginHorizontal();
        if (Button("Smaller Resolution"))
            Screen.SetResolution(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, true);
        if (Button("Bigger Resolution"))
            Screen.SetResolution(Screen.currentResolution.width * 2, Screen.currentResolution.height * 2, true);
        gui.EndHorizontal();

        var horizontalSlider = HorizontalSlider("Draw Distance", drawDistance, 0, 1000);
        if (horizontalSlider != drawDistance)
            updated = true;

        //if (Button("Disable Sounds"))
        //{
        //    foreach (var a in Resources.FindObjectsOfTypeAll<AudioSource>())
        //    {
        //        a.clip = null;
        //        DestroyImmediate(a.clip);
        //    }
        //    foreach (var a in Resources.FindObjectsOfTypeAll<AudioClip>())
        //        DestroyImmediate(a);
        //    Resources.UnloadUnusedAssets();
        //}
        drawDistance = horizontalSlider;


        if (globalQuality != q && (int)q != -1)
        {
            autoQuality = false;
            SetQuality(q);
        }
        else if (updated)
            UpdateQuality();
        GUI.enabled = true;
    }


    internal float fixedDeltaTime = 0.005f;
    public void SetQuality(int q)
    {

        //if (q < Q2.Low) q = Q2.Lowest;
        //if (q > Q2.High) q = Q2.High;
        globalQuality = Mathf.Clamp(q, Q2.min, Q2.max);
        var highQuality = globalQuality >= Q2.High;
        var normalOrHigher = globalQuality >= Q2.normal;

        var lowQuality = globalQuality <= Q2.Low;
        var lowestQuality = globalQuality == Q2.Lowest;
        quality = Mathf.Clamp(android ? globalQuality - 1 : globalQuality + 1, Q2.min, Q2.max);
        physQuality = android ? (lowestQuality ? 0 : lowQuality ? 1 : 2) : lowestQuality ? 1 : 2;
        destrQuality = android ? (lowestQuality ? 0 : globalQuality <= Q2.normal ? 1 : 2) : lowestQuality ? 1 : 2;
        graphQuality = highQuality ? 2 : normalOrHigher ? android ? 2 : 1 : 0;
        textureQuality = 2;//lowestQuality ? 0 : lowQuality ? 1 : 2;
        skidmarks = !lowestQuality;
        particles = !lowestQuality;
        motionblur = !(android ? lowQuality : lowestQuality);
        drawDistance = 600;
        UpdateQuality();

    }



    private void UpdateQuality()
    {
        if (safeMode) return;
        RenderSettings.fog = particles;
        if (android)
        {
            QualitySettings.anisotropicFiltering = textureQuality == 2 ? AnisotropicFiltering.ForceEnable : textureQuality == 1 ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;
            QualitySettings.masterTextureLimit = textureQuality >= 1 ? 0 : 1;
            QualitySettings.shadowDistance = graphQuality > 1 ? 30 : 0;
            //QualitySettings.shadowDistance = shadowQuality == 0 ? 0 : shadowQuality == 1 ? 10 : shadowQuality == 2 ? 30 : 50;
            //QualitySettings.shadowCascades = shadowQuality <= 1 ? 0 : shadowQuality <= 2 ? 2 : 4;
            //foreach (var a in Light.GetLights(LightType.Directional, 0))
            //    a.shadows = shadowQuality <= 0 ? LightShadows.None : shadowQuality <= 2 ? LightShadows.Hard : LightShadows.Soft;

        }
        else
            QualitySettings.SetQualityLevel(textureQuality == 2 ? 5 : textureQuality == 1 ? (android ? 2 : 3) : 0, _Game == null);

        //if (disableLights && !Application.isEditor)
        //    foreach (var a in Resources.FindObjectsOfTypeAll<Light>())
        //        if (a.type != LightType.Directional)
        //            DestroyImmediate(a, true);

        foreach (var a in FindObjectsOfType<bs>())//Resources.FindObjectsOfTypeAll<bs>()
            a.OnQualityChanged();


        foreach (MonoBehaviour a in FindObjectsOfType(typeof(AmplifyMotionEffect)))
            a.enabled = motionblur;

        if (_Game)
        {
            _Game.MainCamera.enableImageEffects = graphQuality >= 1;
            _Game.skidmarks.gameObject.SetActive(skidmarks);
            foreach (Camera a in Camera.allCameras)
            {
                a.renderingPath = graphQuality >= 1 ? RenderingPath.DeferredLighting : RenderingPath.VertexLit;
                if (a == Camera.main)
                    a.farClipPlane = drawDistance;
            }
        }
        Time.fixedDeltaTime = physQuality >= 2 ? 0.02f : physQuality == 1 ? 0.04f : 0.07f;
    }
    private bool PlayerPrefsGetBool(string a, bool b)
    {
        return PlayerPrefs.GetInt(a, b ? 1 : 0) == 1;
    }
    private void PlayerPrefsSetBool(string a, bool b)
    {
        PlayerPrefs.SetInt(a, b ? 1 : 0);
    }

}

//public static class Q3
//{
//    public static int Off = 0;
//    public static int Low = 0;
//    public static int High = 1;
//}

//public static class Q4
//{
//    public static string[] strs = new string[] { "Low", "Normal", "High" };
//    public static int Low = 0;
//    public static int Normal = 1;
//    public static int High = 2;
//}

public static class Q2
{
    public static int min = 0;
    public static int max = 3;
    public static int Lowest = 0;
    public static int Low = 1;
    public static int normal = 2;
    public static int High = 3;
}
