using System.Collections;
using gui = UnityEngine.GUILayout;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class Window : GuiClasses
{

    //public void OnValidate()
    //{
    //    if (!win)
    //        win = this;
    //}
    public Vector2 size;
    public Action act;
    public Action act2;
    private Dictionary<Action, Action> backs = new Dictionary<Action, Action>();

    public void OnLevelWasLoaded(int level)
    {
        CloseWindow();
    }
    public new void SetupWindow(float x = 400, float y = 300)
    {
        SetupWindow(new Vector2(x, y));
    }
    public void Reset()
    {
        showBackButton = true;
        showScrollAlways = showVertical = addflexibleSpace = false;
        windowSkin = res.necroSkin;

        dock = Dock.Center;
    }
    public bool showScrollAlways;
    public bool showVertical;
    public void SetupWindow(Vector2 s)
    {
        if (!inited)
        {
            size = s;
            //size.y = 0;
        }
    }
    public bool showBackButton = true;
    public bool addflexibleSpace = true;
    internal string windowTitle = "";
    internal GUIStyle style;
    internal Vector2 scale;
    public static float WindowScale = 1.5f;
    public Dock dock = Dock.Center;
    public enum Dock { Left, Right, UP, Down, Center }

    internal GUISkin windowSkin;
    public void OnGUI()
    {


        if (actStr == null)
            return;

        //if (!bs._Loader.defSkin)
        //    bs._Loader.defSkin = GUI.skin;
        GUI.skin = windowSkin;


        scale = GUIMatrix(WindowScale);


        var c = Vector3.zero;
        float w = Screen.width / scale.x;
        float h = Screen.height / scale.y;
        var s = new Vector3(Mathf.Min(w, size.x), Mathf.Min(h, size.y)) / 2f;
        if (dock == Dock.Right)
            c = new Vector3(w - s.x, h / 2f);
        else if (dock == Dock.Left)
            c = new Vector3(s.x, h / 2f);//+ w * .05f
        else if (dock == Dock.Down)
            c = new Vector3(w / 2f, h - s.y);//+ w * .05f
        else
            c = new Vector3(w, h) / 2f;
        var v1 = c - s;
        var v2 = c + s;

        windowRect = Rect.MinMaxRect(v1.x, v1.y, v2.x, v2.y);

        //Vector2 screen = new Vector2(Screen.width / scale.x, Screen.height / scale.y);
        //Vector2 s = Vector2.Min(size, screen);
        //Vector2 a = screen / 2f - s / 2f;
        //Rect screenRect = new Rect(a.x, a.y, s.x, s.y);
        GUILayout.BeginArea(windowRect, new GUIContent(windowTitle), style ?? skin.window);
        if (res.necroSkin.name == "NecromancerGUI - Copy - Copy")
            GUILayout.Space(50);

        BeginScrollView(null, showVertical, true, showScrollAlways, windowRect.width - windowSkin.window.border.horizontal);
        if (act != null)
            act();
        else if (g != null && !string.IsNullOrEmpty(actStr))
            g.SendMessage(actStr, SendMessageOptions.DontRequireReceiver);

        Rect lastRect = GUILayoutUtility.GetLastRect();

        if (!inited && Event.current.type == EventType.repaint)
        {
            size.y = lastRect.yMax + (showBackButton ? 30 : 0) + 110;
            inited = true;
        }
        EndScrollView();
        if (addflexibleSpace)
            GUILayout.FlexibleSpace();
        GUI.enabled = true;
        if (showBackButton && BackButtonLeft())
            Back();

        GUILayout.EndArea();

        //Sprite s;s.textureRect
        if (Event.current.type == EventType.repaint)
            foreach (var a in GameObject.FindGameObjectsWithTag("GUICam"))
                a.GetComponent<Camera>().Render();
    }

    public new void CloseWindow()
    {
        print("Close Window");
        PlayPushButton();
        act2 = act = null;
        actStr = null;
        enabled = false;
    }
    private bool inited;
    //public bool save = false;
    private MonoBehaviour g;
    public void ShowWindow(Action draw, bool save = true)
    {
        PlayPushButton();
        inited = false;
        print("Show Window " + draw.Method.Name);
        windowTitle = actStr = draw.Method.Name;
        g = draw.Target as MonoBehaviour;
        enabled = true;
        Reset();
        SetupWindow();
        scroll = Vector2.zero;
        if (act2 != null)
            backs[draw] = act2;
        act = draw;
        if (save)
            act2 = draw;
        try
        {
            draw();
        }
        catch (Exception) { }
    }
    public new IEnumerator ShowWindow2(Action func)
    {
        ShowWindow(func);
        while (act != null && act == func)
            yield return null;
    }
    public new void Back()
    {
        var action = act;
        win.CloseWindow();
        if (action != null && backs.ContainsKey(action))
        {
            win.ShowWindow(backs[action]);
            backs.Remove(action);
        }
    }

    private string actStr;
    public new bool active { get { return !string.IsNullOrEmpty(actStr) && actStr != "ScoreBoard"; } }
    public Vector3 GUIMatrix(float sc = 1)
    {
        var x = ((float)Screen.height / Screen.width) / (originalHeight / originalWidth);
        //var mt = GUI.matrix;
        guiscale = android && Screen.orientation != ScreenOrientation.AutoRotation ? new Vector3(Screen.width / originalWidth * x, Screen.height / originalHeight, 1) * sc : Vector3.one;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, guiscale);
        return guiscale;
    }
    static float originalWidth = 950;
    static float originalHeight = 450;
    public static Vector3 guiscale;


    public Rect windowRect;
    public bool WindowHit
    {
        get
        {
            if (string.IsNullOrEmpty(actStr)) return false;
            var m = Input.mousePosition;
            return windowRect.Contains(new Vector3(m.x / guiscale.x, m.y / guiscale.y));
        }
    }


}