using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public partial class bs
{
    public static bool skipNext;
    public static Dictionary<string, string> trcache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public static string Tr(string s, bool sk = false, bool save = true)
    {

        if (!settings || settings.disableTranslate || string.IsNullOrEmpty(s) || !Application.isPlaying)
            return s;
        if (skipNext)
        {
            skipNext = sk;
            return s;
        }
        skipNext = sk;
        if (string.IsNullOrEmpty(s) || s.Length > 300) return s;
        string h;
        if (trcache.TryGetValue(s, out h)) { return h; }
        string trim = s;//.Replace("\r", "\\r").Replace("\n", "\\n");
        string end;
        string start;
        trim = Trim(trim, out start, out end);
        if (string.IsNullOrEmpty(trim)) return s;
        string d;
        dict.TryGetValue(trim, out d);

        return trcache[s] = string.IsNullOrEmpty(d) ? s : start + unescape(d) + end;
    }
    public string Trs(string s)
    {
        return Tr(s, true);
    }
    public string Trn(string s)
    {
        return Tr(s, false, false);
    }
    public static string unescape(string d)
    {
        return d;
        //return d.Replace("\\r", "\r").Replace("\\n", "\n");
    }
    private static string Trim(string s, out string start, out string end)
    {
        var m = Regex.Match(s, @"[\w ]+");
        start = s.Substring(0, m.Index);
        var startIndex = m.Index + m.Length;
        end = s.Substring(startIndex, s.Length - startIndex);
        return m.Value;
    }

    private static Dictionary<string, string> m_dict;
    private static Dictionary<string, string> dict
    {
        get
        {
            if (bs.m_dict == null)
                LoadTranslate();
            return bs.m_dict;
        }
        set { bs.m_dict = value; }
    }
    public static void LoadTranslate()
    {
        dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        tooltips = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (Application.isEditor ? settings.disableTranslate : curDict == 0)
            return;
        Profiler.BeginSample("LoadTranslate");
        //LoadDict(setting.translations[0]);
        LoadDict(translations);
        Profiler.EndSample();
    }
    private static void LoadDict(TextAsset AssetDictionary)
    {
        var lines = AssetDictionary.text.Split(new[] { ";\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        print("LoadTranslate " + AssetDictionary.name + lines.Length);
        foreach (var a in lines)
        {

            var ss = a.Replace("\\n", "\n").Split(';');
            string key = ss[0];
            if (ss.Length == 1)
                dict[key] = key;
            if (ss.Length >= 2)
                dict[key] = ss[1];
            if (ss.Length >= 3)
            {
                tooltips[key] = ss[2];
            }
        }
    }
    private static Dictionary<string, string> tooltips = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public static TextAsset translations { get { return settings.translations[Mathf.Max(0, curDict)]; } }
    protected static int curDict
    {
        get
        {
            return PlayerPrefs.GetInt("Dict", 0);
        }
        set
        {
            if (value != bs.curDict)
            {
                PlayerPrefs.SetInt("Dict", value);
                dict = null;
                trcache.Clear();
                foreach (var a in EasyFontTextMesh.textMeshes)
                    a.isDirty = true;
            }
        }
    }
}