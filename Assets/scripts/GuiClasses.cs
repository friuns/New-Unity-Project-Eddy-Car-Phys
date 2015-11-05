using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using gui = UnityEngine.GUILayout;

using UnityEngine;

public class GuiClasses : bsNetwork
{
    public static bool BeginVertical(string name, bool def = true)
    {
        var r = tf.ContainsKey(name) ? tf[name] : def;
        tf[name] = gui.Toggle(r, Tr(name), settings.unitySkin.GetStyle("ToolbarDropDown"));
        if (r)
            gui.BeginVertical(settings.unitySkin.GetStyle("CN Box"));
        return r;
    }
    private static Dictionary<string, bool> tf = new Dictionary<string, bool>();
    protected Vector2 scroll;
    public static bool Indent()
    {
        gui.BeginHorizontal();
        gui.Space(20);
        gui.BeginVertical();
        return true;
    }

    public static bool UnIndent()
    {
        gui.EndVertical();
        gui.EndHorizontal();
        return true;
    }
    public void BeginScrollView(GUIStyle style = null, bool showHorizontal = false, bool showVertical = true, bool showScrollAlways = false, float width = 0)
    {
        if (style == null) style = skin.scrollView;
        skin.scrollView.fixedWidth = width;
        Vector2 bsv = gui.BeginScrollView(scroll, false, showScrollAlways, showHorizontal ? skin.horizontalScrollbar : GUIStyle.none, showVertical ? skin.verticalScrollbar : GUIStyle.none, style);
        if (bsv == scroll)
        {
            Vector2 mouseDelta = bs._Loader.getMouseDelta(false);
            bsv += new Vector2(-mouseDelta.x, mouseDelta.y) * 3;
        }
        scroll = bsv;
    }
    public void EndScrollView()
    {
        GUILayout.EndScrollView();
    }


    public static void Label(string s)
    {
        gui.Label(Tr(s));
    }
    public void Label(string s, string s2)
    {
        gui.Label(Tr(s), s2);
    }

    public static void SelectAll(int len)
    {
        TextEditor te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
        te.pos = 0; //set cursor position
        te.selectPos = len; //se
    }

    public static string TextField(string Name, string input)
    {
        gui.BeginHorizontal();
        Label(Name);
        var a = gui.TextField(input);
        gui.EndHorizontal();
        return a;
    }
    public string TextArea(string Name, string input)
    {
        gui.BeginHorizontal();
        Label(Name);
        var a = gui.TextArea(input);
        gui.EndHorizontal();
        return a;
    }
    public static float HorizontalSlider(string label, float value, float leftValue, float rightValue, bool clamp = false, bool time = false)
    {
        gui.BeginVertical();

        float round = (float)Math.Round(Mathf.Clamp(value, leftValue, rightValue), 2);
        gui.Label(Tr(label) + " " + (time ? TimeToStr(round, false) : round.ToString()));

        var horizontalSlider = GUILayout.HorizontalSlider(value, leftValue, rightValue);
        gui.EndVertical();
        return clamp ? Mathf.Clamp(horizontalSlider, leftValue, rightValue) : horizontalSlider;//;
    }
    public static GUISkin skin { get { return GUI.skin; } }
    public int Toolbar(int id, IList<string> getNames, ref bool changed, string name2 = null)
    {
        var toolbar = Toolbar(id, getNames, title: name2);
        if (id != toolbar)
            changed = true;
        return toolbar;
    }
    public static bool Toggle(bool b, string s)
    {
        return gui.Toggle(b, Tr(s));
    }
    public static bool Toggle(bool b, string s, ref bool changed)
    {
        var toggle = gui.Toggle(b, Tr(s));
        if (toggle != b)
            changed = true;
        return toggle;
    }

    public void SetupWindow(float x = 400, float y = 300)
    {
        win.SetupWindow(x, y);
    }
    public static void CloseWindow()
    {
        win.CloseWindow();
    }
    public static void ShowWindow(Action draw)
    {
        win.ShowWindow(draw);
    }
    public IEnumerator ShowWindow2(Action func)
    {
        win.ShowWindow(func);
        while (win.act != null && win.act == func)
            yield return null;
    }
    public void Back()
    {
        win.Back();
    }

    public int Toolbar(int id, IList<string> getNames, bool expand = true, bool center = false, int limit = 99, int hor = -1, bool useSkin = true, string title = null)
    {
        //GUI.skin = _Loader.defSkin;
        if (getNames[Mod(id, getNames.Count)] == null)
            for (int i = 0; i < getNames.Count && i < limit && getNames[i] == null; i++)
                id = i;
        if (useSkin)
            gui.BeginVertical(skin.box);
        if (title != null)
            Label(title, "Label");
        gui.BeginHorizontal();
        if (center)
            gui.FlexibleSpace();
        for (int i = 0, j = 0; i < getNames.Count && i < limit; i++)
        {
            if (hor != -1 && j % hor == 0 && j != 0)
            {
                gui.EndHorizontal();
                gui.BeginHorizontal();
            }
            if (getNames[i] == null)
                continue;
            j++;
            //var guiStyle = skin.GetStyle("ButtonIcon");
            //Texture2D old = guiStyle.normal.background;
            //guiStyle.normal.background = id == i ? guiStyle.active.background : guiStyle.normal.background;
            //if (SoundButton(gui.Button(Tr(getNames[i]), gui.ExpandWidth(expand), gui.MinWidth(50))))
            //    id = i;
            //guiStyle.normal.background = old;
            if (GlowButton(getNames[i], id == i, expand))
                id = i;
        }
        if (center)
            gui.FlexibleSpace();
        gui.EndHorizontal();
        if (useSkin)
            gui.EndVertical();
        //GUI.skin = res.necroSkin;
        return id;
    }

    public static bool SoundButton(bool button)
    {
        //if (Input.GetMouseButtonUp(0) && button)
        //    PlayPushButton();
        return button;
    }
    public static void PlayPushButton()
    {
        PlayOneShotGui(res.pushButton.Random(), res.volumeFactor);
    }
    public bool GlowButton(string s, bool glow, bool expand = true, Color c = default(Color))
    {
        GUIStyle guiStyle = skin == res.necroSkin ? skin.GetStyle("ButtonIcon") : skin.button;
        Texture2D old = guiStyle.normal.background;
        var oldc = guiStyle.normal.textColor;
        if (c != default(Color))
            guiStyle.normal.textColor = c;
        guiStyle.normal.background = glow ? guiStyle.active.background : guiStyle.normal.background;
        bool pressed = SoundButton(gui.Button(Tr(s), guiStyle, gui.MinWidth(50), gui.ExpandWidth(expand)));
        guiStyle.normal.background = old;
        guiStyle.normal.textColor = oldc;
        return pressed;
    }

    public bool BackButton(string s = "Back")
    {
        return Button(s);
    }
    public static bool Button(string s)
    {
        var button = SoundButton(gui.Button(new GUIContent(Tr(s), s)));
        return button;
    }
    public bool ButtonLeft(string s)
    {
        gui.BeginHorizontal();
        gui.FlexibleSpace();
        var button = Button(s);
        gui.EndHorizontal();
        return button;
    }
    public bool BackButtonLeft()
    {
        return ButtonLeft("Back");
    }

    public void ToggleWindow(Action playersWindow)
    {
        if (win.act == playersWindow)
            win.Back();
        else
            win.ShowWindow(playersWindow);
    }
    public IEnumerator ShowPopup2(string s)
    {
        return ShowWindow2(delegate
        {
            Label(s);
        });
    }
    public void ShowPopup(string s)
    {
        win.ShowWindow(delegate
        {
            win.showBackButton = false;
            Label(s);
            if (gui.Button("Ok")) Back();
        });
        //StartCoroutine(AddMethod(1, delegate { Back(); }));
    }
    public static string CreateTable(string source)
    {
        string table = "";
        MatchCollection m = Regex.Matches(source, @"\w*\s*");
        for (int i = 0; i < m.Count - 1; i++)
            table += "{" + i + ",-" + m[i].Length + "}";
        return table;
    }
    //public void ShowPopupQuick(string s)
    //{
    //    ShowWindow(delegate
    //    {
    //        Label(s);
    //    });
    //    StartCoroutine(AddMethod(1, delegate { Back(); }));
    //}

}





