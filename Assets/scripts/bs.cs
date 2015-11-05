using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

public partial class bs : Base
{
    public static Administration _Administration;
    public static void LogScreen(object s)
    {
        if (Time.deltaTime == Time.fixedDeltaTime)
            sbuilder2.Append(s + "\r\n");
        else
            sbuilder.Append(s + "\r\n");
    }

    public static StringBuilder sbuilder = new StringBuilder();
    public static StringBuilder sbuilder2 = new StringBuilder();
    public static AutoQuality _AutoQuality;
    public static Pool _Pool;
    public static Settings m_settings;
    public static Settings settings { get { return m_settings ?? (m_settings = (Settings)Resources.Load<Settings>("Settings")); } }
    public static GuiSkins _GuiSkins;
    public static Res res;
    internal Transform m_transform;
    public Transform tr { get { return m_transform ? m_transform : (m_transform = base.transform); } }
    private Animation m_animation;
    public new Animation animation { get { return m_animation ? m_animation : (m_animation = base.animation); } }
    private Renderer m_renderer;
    public new Renderer renderer { get { return m_renderer ? m_renderer : (m_renderer = base.renderer); } }
    private Collider m_collider;
    public new Collider collider { get { return m_collider ? m_collider : (m_collider = base.collider); } }
    private Rigidbody m_rigidbody;
    public new Rigidbody rigidbody { get { return m_rigidbody ? m_rigidbody : (m_rigidbody = base.rigidbody); } }
    private AudioSource m_audio;
    public new AudioSource audio { get { return m_audio ? m_audio : (m_audio = base.audio); } }
    private Light m_light;
    public new Light light { get { return m_light ? m_light : (m_light = base.light); } }

    public PhotonPlayer photonPlayer { get { return PhotonNetwork.player; } }
    public static ResEditor resEditor
    {
        get { return (ResEditor)Resources.LoadAssetAtPath("Assets/!prefabs/resEditor.prefab", typeof(ResEditor)); }
    }
    public static bool android;
    public static Game _Game;
    public static Window win { get { return _Loader.m_win; } }
    public static LoaderMusic _LoaderMusic;
    public static Menu _LoaderGui;
    public static Menu _Menu;
    public static Hud _Hud;
    public static GameGui _GameGui;
    public static Player _Player;
    public static PlayerView _PlayerView;
    public static Loader _Loader;
    public static ChatGui _ChatGui;
    public static CameraControl _MainCamera;
    public static CarShop carShop;
    public static Camera CameraMain { get { return Camera.main; } }
    public static Transform CameraMainTransform { get { return CameraMain.transform; } }


    public static float Mod(float a, float n)
    {
        return ((a % n) + n) % n;
    }
    public static int Mod(int a, int n)
    {
        return ((a % n) + n) % n;
    }

    public const int MinValue = -99999;
    public const int MaxValue = 99999;
    public static string deviceUniqueIdentifier
    {
        get
        {
            //#if UNITY_ANDROID
            //            return "";
            //#else 
            return SystemInfo.deviceUniqueIdentifier;
            //#endif
        }
    }
    public Vector3 pos { get { return tr.position; } set { tr.position = value; } }
    public Quaternion rot { get { return tr.rotation; } set { tr.rotation = value; } }
    public static Vector3 ZeroY(Vector3 v, float a = 0)
    {
        v.y *= a;
        return v;
    }
    public static IEnumerator AddMethod(float seconds, Action act)
    {
        //StopAllCoroutines();
        yield return new WaitForSeconds(seconds);
        act();
    }
    public IEnumerator AddMethodCor(float seconds, Func<IEnumerator> act)
    {
        //StopAllCoroutines();
        yield return new WaitForSeconds(seconds);
        yield return StartCoroutine(act());
    }
    public static IEnumerator AddMethod(Action act)
    {
        //StopAllCoroutines();
        yield return null;
        act();
    }
    public static IEnumerator AddMethod(YieldInstruction y, Action act)
    {
        //StopAllCoroutines();
        yield return y;
        act();
    }
    public static IEnumerator AddMethod(Func<bool> y, Action act)
    {
        //StopAllCoroutines();
        while (!y())
            yield return null;
        act();
    }
    public static RoomInfo hostRoom;
    public static RoomInfo previousRoom;
    public static RoomInfo room { get { return PhotonNetwork.room ?? hostRoom; } }
    //public Room room { get { return PhotonNetwork.room ?? new Room("", null); } }
    public static bool isMaster
    {
        get
        {
            return PhotonNetwork.isMasterClient;
            //if (!m_IsMaster.HasValue)
            //    m_IsMaster = PhotonNetwork.isMasterClient;
            //return m_IsMaster.Value;
        }
    }
    public static bool isDebug { get { return settings.m_isDebug; } }
    public bool KeyDebug(KeyCode key, string desc = null)
    {
        if (isDebug || Application.isEditor)
            return Input.GetKeyDown(key) && Input.GetKey(KeyCode.LeftShift);
        return false;
    }
    public static string TimeToStr(float s, bool miliseconds = true, bool draw = false)
    {
        var f = Mathf.Abs(s);
        var s1 = new StringBuilder().Append(s < 0 ? "-" : draw ? "+" : "").Append((int)f / 60).Append(":").Append(((int)(f % 60)).ToString().PadLeft(2, '0'));
        if (miliseconds)
            s1.Append("." + ((int)((f % 1) * 100)).ToString().PadLeft(2, '0'));
        return s1.ToString();
    }
    public bool FramesElapsed(int tm, int random = 0)
    {
        return (Time.frameCount + random) % tm == 0 || Time.timeScale == 0;
    }


    public bool TimeElapsed(float seconds, float offset = 0)
    {
        var deltaTime = Time.deltaTime + offset;

        if (deltaTime > seconds || seconds == 0) return true;
        if (Time.time % seconds < (Time.time - deltaTime) % seconds)
            return true;
        return false;
    }
    public bool online { get { return !PhotonNetwork.offlineMode; } }
    public static bool checkVisible(Vector3 pos)
    {
        Camera cam = CameraMain;
        if ((cam.transform.position - pos).magnitude < 30)
            return true;
        var wp = cam.WorldToViewportPoint(pos);
        bool b = new Rect(0, 0, 1, 1).Contains(wp) && wp.z > 0 && !Physics.Linecast(cam.transform.position, pos + Vector3.up * .2f, Layer.levelMask);
        //if (!b)
        //    Debug.DrawLine(cam.transform.position, pos, Color.white, 1);
        return b;
    }
    public static bool highQuality
    {
        get { return _AutoQuality.quality >= Q2.High; }
    }
    //public static bool normalQuality
    //{
    //    get { return _AutoQuality.quality >= Q2.normal; }
    //}
    public static bool lowQuality
    {
        get { return _AutoQuality.quality <= Q2.Low; }
    }
    public static bool lowestQuality
    {
        get { return _AutoQuality.quality == Q2.Lowest; }
    }

    public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 rhs = point - lineStart;
        Vector3 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector3 lhs = vector2;
        if (magnitude > 1E-06f)
        {
            lhs = (Vector3)(lhs / magnitude);
        }
        float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
        return (lineStart + ((Vector3)(lhs * num2)));
    }


    public static string m_platformPrefix;


    public static string platformPrefix2
    {
        get { return platformPrefix + (platformPrefix == "android" ? settings.androidMapVersion : settings.MapVersion); }
    }

    public static string platformPrefix
    {
        get
        {

#if UNITY_EDITOR
            if (Application.isEditor)
            {
                //if (isDebug && Application.isPlaying) return "dm" + "web";
                var bt = EditorUserBuildSettings.activeBuildTarget;
                return "dm" + (bt == BuildTarget.WP8Player ? "wp8" : bt == BuildTarget.iPhone ? "ios" : bt == BuildTarget.Android ? "android" : bt == BuildTarget.FlashPlayer ? "flash" : "web");
            }
#endif
            return "dm" + (m_platformPrefix != null ? m_platformPrefix : (m_platformPrefix = Application.platform == RuntimePlatform.WP8Player ? "wp8" : Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.IPhonePlayer ? "ios" : Application.platform == RuntimePlatform.Android ? "android" : Application.platform == RuntimePlatform.FlashPlayer ? "flash" : "web"));
        }
    }
    public static bool repaint;
#if UNITY_4_6
    public static T Instantiate<T>(T t) where T : Object
    {
        return (T)Object.Instantiate(t);
    }
#endif
}