//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class CustomWindow : GuiClasses
//{
//    public Vector2 size;
//    public Action act;
//    private Dictionary<Action, Action> backs = new Dictionary<Action, Action>();
//    public void ShowWindow(Action draw)
//    {
//        print("Show Window " + draw.Method.Name);
//        actStr = draw.Method.Name;
//        enabled = true;
//        //Setup();
//        scroll = Vector2.zero;
//        if (act != null)
//            backs[draw] = act;
//        act = draw;

//    }
//    public void Setup(float x = 400, float y = 300)
//    {
//        Setup(new Vector2(x, y));
//    }
//    public void Setup(Vector2 s)
//    {
//        style = skin.window;
//        addflexibleSpace = showBackButton = true;
//        size = s;
//    }
//    public bool showBackButton = true;
//    public bool addflexibleSpace = true;
//    internal GUIStyle style;
//    public virtual void OnGUI()
//    {
//        if (actStr == null)
//            return;

//        if (!_Loader.defSkin)
//            _Loader.defSkin = GUI.skin;
//        GUI.skin = res.necroSkin;

//        Vector2 screen = new Vector2(Screen.width, Screen.height);
//        Vector2 s = Vector2.Min(size, screen);
//        //s = new Vector2(Mathf.Min(size.x, screen.x), Mathf.Min(size.y, screen.y)); 
//        Vector2 a = screen / 2f - s / 2f;
//        Rect screenRect = new Rect(a.x, a.y, s.x, s.y);
//        GUILayout.BeginArea(screenRect, style ?? skin.window);
//        GUILayout.Space(50);
//        BeginScrollView();
//        if (act != null)
//            act();
//        else
//            SendMessage(actStr);

//        EndScrollView();
//        if (showBackButton)
//        {
//            if (addflexibleSpace)
//                GUILayout.FlexibleSpace();
//            if (BackButtonLeft())
//                Back();
//        }
//        GUILayout.EndArea();
//    }

//    public void CloseWindow()
//    {
//        print("Close Window");
//        act = null;
//        actStr = null;
//        enabled = false;
//    }
//    public void Back()
//    {
//        var action = act;
//        win.CloseWindow();
//        if (backs.ContainsKey(action))
//        {
//            win.ShowWindow(backs[action]);
//            backs.Remove(action);
//        }
//    }

//    internal string actStr;
//}