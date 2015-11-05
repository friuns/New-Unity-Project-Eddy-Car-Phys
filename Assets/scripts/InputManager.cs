#pragma warning disable 162
#pragma warning disable 429
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;
public class InputManager : bs
{

    internal List<KeyValue> keys = new List<KeyValue>();
    internal KeyValue[] alternatives { get { return m_alternatives == null ? InitDict() : m_alternatives; } }
    private static KeyValue[] m_alternatives;
    public void Start()
    {
        if (m_alternatives == null)
            InitDict();
    }
    public KeyValue[] InitDict()
    {
        m_alternatives = new KeyValue[400];
        Add(KeyCode.Escape, "Menu", KeyCode.M);
        Add(KeyCode.R, "Reverse car");
        Add(KeyCode.LeftShift, "Nitro", KeyCode.RightControl);
        Add(KeyCode.S, "Down", KeyCode.DownArrow);
        Add(KeyCode.Space, "Brake", KeyCode.End);
        Add(KeyCode.W, "Up", KeyCode.UpArrow);
        Add(KeyCode.A, "Left", KeyCode.LeftArrow);
        Add(KeyCode.D, "Right", KeyCode.RightArrow);
        Add(KeyCode.Q, "Look Back");
        Add(KeyCode.Return, "Chat");
        Add(KeyCode.Tab, "Score Board");
        Add(KeyCode.F11, "FullScreen");
        Add(KeyCode.Mouse0, "Shoot", KeyCode.Alpha1);
        Add(KeyCode.Mouse1, "Rocket", KeyCode.Alpha2);
        return m_alternatives;
    }
    public void Add(KeyCode key, string descr, KeyCode alt = KeyCode.None)
    {
        var keyValue = new KeyValue() { descr = descr, keyCodeAlt = new[] { key, alt }, main = key };
        keyValue.Load();
        keys.Add(keyValue);
        m_alternatives[(int)key] = keyValue;
    }


    public bool GetKey(KeyCode key)
    {
        return GetKey(key, KeyAction.Key);
    }
    public enum KeyAction { KeyDown, KeyUp, Key }
    public bool GetKeyDown(KeyCode key, bool force = false)
    {
        return GetKey(key, KeyAction.KeyDown, force);
    }
    public bool GetKeyUp(KeyCode key)
    {
        return GetKey(key, KeyAction.KeyUp);
    }
    public const bool splitScreen = false;
    public const bool secondPlayer = false;

    public bool GetKey(KeyCode key, KeyAction GetKey, bool force = false)
    {
        if (win.active && !force) return false;
        KeyValue kv = alternatives[(int)key];
        if (kv == null) return ParseAction(GetKey, key);
        KeyCode[] keyCodeAlt = kv.keyCodeAlt;
        for (int i = 0; i < keyCodeAlt.Length; i++)
        {
            if (ParseAction(GetKey, keyCodeAlt[i]) && (!splitScreen || i % 2 == 0 && !secondPlayer || i % 2 == 1 && secondPlayer))
                return true;
        }
        return false;
    }
    public Dictionary<int, KeyHudBool> dict;
    bool ParseAction(KeyAction a, KeyCode key)
    {
        if (android)
        {
            KeyHudBool val;
            if (dict != null && dict.TryGetValue((int)key, out val))
            {
                KeyHudBool keyHudBool = val;
                if (keyHudBool.down && a == KeyAction.KeyDown)
                    return true;
                if (keyHudBool.up && a == KeyAction.KeyUp)
                    return true;
                if (keyHudBool.hold && a == KeyAction.Key)
                    return true;
            }
            if (key >= KeyCode.Mouse0 && key < KeyCode.Mouse6)
                return false;
        }
        return a == KeyAction.Key ? Input.GetKey(key) : a == KeyAction.KeyDown ? Input.GetKeyDown(key) : Input.GetKeyUp(key);
    }
    public static bool enableKeys = true;
    //static bool InputGetKey(KeyCode key)
    //{
    //    if (!enableKeys) return false;
    //    Axis s = Buttons[(int)key];
    //    if (s != null)
    //    {
    //        var axis = Input.GetAxis(s.s);
    //        if (axis < s.min && s.min < 0 || axis > s.min && s.min > 0)
    //        {
    //            return true;
    //        }
    //    }
    //    return Input.GetKey(key);
    //}
    public struct DR
    {
        public KeyCode key;
        public Func<KeyCode, bool> func;
        public DR(KeyCode a, Func<KeyCode, bool> b)
        {
            key = a;
            func = b;
        }
    }
    private static Axis[] m_buttons;
    public static Axis[] Buttons
    {
        get
        {
            if (m_buttons == null)
            {
                m_buttons = new Axis[459];
                //m_buttons[(int)KeyCode.A] = new Axis() { s = "Horizontal", min = -.1f };
                //m_buttons[(int)KeyCode.D] = new Axis() { s = "Horizontal", min = +.1f };
                //m_buttons[(int)KeyCode.LeftArrow] = new Axis() { s = "Horizontal2", min = -.1f };
                //m_buttons[(int)KeyCode.RightArrow] = new Axis() { s = "Horizontal2", min = +.1f };
            }
            return m_buttons;
        }
    }
    public class Axis
    {
        public string s;
        public float min;
    }

    public bool anyKeyDown { get { return Input.anyKeyDown; } }

    public bool GetMouseButtonUp(int I)
    {
        return !android && Input.GetKey(KeyCode.Mouse0);
    }
}
[Serializable]
public class KeyValue
{
    public string descr;
    public KeyCode main;
    public KeyCode[] keyCodeAlt;

    public void Load()
    {
        for (int i = 0; i < keyCodeAlt.Length; i++)
            keyCodeAlt[i] = (KeyCode)PlayerPrefs.GetInt(main + "" + i, (int)keyCodeAlt[i]);
    }
    public void Save()
    {
        for (int i = 0; i < keyCodeAlt.Length; i++)
            PlayerPrefs.SetInt(main + "" + i, (int)keyCodeAlt[i]);
    }
}
