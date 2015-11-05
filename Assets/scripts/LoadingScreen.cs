using gui = UnityEngine.GUILayout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;
using MonoBehaviour = Photon.MonoBehaviour;
using Random = UnityEngine.Random;
public class LoadingScreen { }
public partial class Loader
{
    internal string[] linksButtons = new string[0];

    public List<MapStat> maps { get { return settings.maps; } }


    public IEnumerator LoadSettings()
    {
        var www = new WWW(web.mainSite + "cops.txt" + (isDebug ? "?" + Random.value : ""));
        yield return www;

        if (string.IsNullOrEmpty(www.error))
            bs.settings.settingsTxt = www.text;

        string[] ss = SplitSpecial(bs.settings.settingsTxt);
        foreach (var s in ss)
        {
            var sp = SplitOnce(s);
            if (sp.Length > 1)
                webSettings[sp[0].Trim()] = sp[1].Trim();
        }
        if (tryGet("minVer" + platformPrefix) && bs.settings.version < int.Parse(parsedText))
            ext.Block(" min Vel");



        if (tryGet("buttons_" + platformPrefix))
        {
            print("Prased buttons");
            linksButtons = parsedTextSplit;
        }

        if (!isDebug)
            yield return new WaitForSeconds(3);

        if (tryGet("blocksites") && ContainsSite())
            ext.Block(" block Sites");

        if (tryGet("warntext"))
            warnSite.text = parsedText;


        if (tryGet("runJs") && ContainsSite())
        {
            StartCoroutine(RunJs());
        }
        if (tryGet("music"))
            _LoaderMusic.music = parsedText;

        if (tryGet("demo") && ContainsSite())
            bs.settings.free = true;

        if (tryGet("warnsites") && ContainsSite())
        {
            warnSite.enabled = true;
            warnSite.text = string.Format(warnSite.text, url2.Host);
        }


        if (tryGet("redirectUrl"))
            redirectUrl = parsedText.Trim();

        if (tryGet("redirectText"))
            redirectText = parsedText;

        if (tryGet("ads"))
            bs.settings.appIds = parsedText.SplitString();

        print("site: " + bs._Loader.url);
        if (tryGet("ownSites") && ContainsSite())
            ownSite = true;

        if (android)
        {
            var genuineCheckAvailable = Application.genuineCheckAvailable;
            var genuine = Application.genuine;
            print("genuine: " + genuine);
            print("genuineCheckAvailable: " + genuineCheckAvailable);
            print("deviceUniqueIdentifier:" + deviceUniqueIdentifier);
            if (genuineCheckAvailable && genuine && !Application.isEditor || bs.settings.release)
                ownSite = true;
            if (!ownSite)
                yield return web.Download("scripts/playcount.php", delegate (string s, bool suc)
                {
                    if (suc) playCount = int.Parse(s);
                }, false, "id", deviceUniqueIdentifier, "version", bs.settings.version);
        }


        if (!ownSite && playCount > 20 && !Application.isEditor)
            ext.Block(" trial");


        if (startDay == 0)
            startDay = curDay;
        daysElapsed = curDay - startDay;
        if (android && daysElapsed > 1)
            win.ShowWindow(DoYouLikeThisGame);
        playCount++;
        settingsLoaded = true;
    }
    public static bool settingsLoaded;
    public int daysElapsed;// { get { return curDay - startDay; } }
    public void DoYouLikeThisGame()
    {
        win.SetupWindow(249.83f, 163.7f);
        win.showBackButton = false;
        Label("Poll", "label");
        Label("Do you like this game?");
        gui.FlexibleSpace();
        gui.BeginHorizontal();
        if (Button("Yes"))
        {
            win.ShowWindow(delegate
            {
                win.SetupWindow(249.83f, 163.7f);
                win.showBackButton = false;
                gui.Label(Tr("Please rate our game 5 star, and we will make game even better!"), new GUIStyle(GUI.skin.label) { wordWrap = true });
                if (Button("Ok"))
                {
                    startDay = curDay + 30;
                    Application.OpenURL("https://play.google.com/store/apps/details?id=com.dm.race" + (bs.settings.free ? "free" : ""));
                    win.CloseWindow();
                }
            });
        }
        gui.FlexibleSpace();
        if (Button("No"))
        {
            startDay = curDay + 5;
            win.Back();
        }
        gui.EndHorizontal();
    }
    public static string redirectUrl = "http://tmrace.net";
    public static string redirectText = "New game version available! Please run game from {0}";
    public int startDay { get { return PlayerPrefs.GetInt("startDay"); } set { PlayerPrefs.SetInt("startDay", value); } }
    private static int curDay { get { return (int)(DateTime.Now.Ticks / TimeSpan.TicksPerDay); } }
    public int playCount { get { return PlayerPrefs.GetInt("playCount" + (android ? bs.settings.version + "" : "")); } set { PlayerPrefs.SetInt("playCount" + (android ? bs.settings.version + "" : ""), value); } }
    internal bool ownSite;

    public IEnumerator RunJs()
    {
        print("Running js ");
        WWW www = new WWW(web.mainSite + "runJs.js");
        yield return www;
        Application.ExternalEval(www.text);
    }
    private bool ContainsSite()
    {
        foreach (var b in parsedTextSplit)
        {
            if (bs._Loader.url != null && bs._Loader.url.ToLower().Contains(b.ToLower().Trim()))
            {
                return true;
            }
        }
        return false;
    }
    private static string parsedText = "";
    private static string[] parsedTextSplit = new string[0];
    public bool tryGet(string s)
    {
        var containsKey = webSettings.ContainsKey(s);
        if (containsKey)
        {
            parsedText = webSettings[s];
            parsedTextSplit = parsedText.SplitString();
        }
        return containsKey;
    }
    public GUIText warnSite;
    public static string[] SplitOnce(string s)
    {
        var a = s.IndexOf("\r\n", System.StringComparison.Ordinal);
        var b = a + 2;
        if (a == -1)
        {
            a = s.IndexOf("\n", System.StringComparison.Ordinal);
            b = a + 1;
        }
        if (a == -1) return new string[] { s };
        return new string[] { s.Substring(0, a), s.Substring(b, s.Length - b) };
    }
    public static Dictionary<string, string> webSettings = new Dictionary<string, string>();




    public static string[] SplitSpecial(string s)
    {

        List<string> ss = new List<string>();
        int io = 0;
        for (int i = 0; i < 100; i++)
        {
            var indexOf = s.IndexOf("##", io, System.StringComparison.Ordinal);
            if (indexOf == -1)
            {
                ss.Add(s.Substring(io, s.Length - io));
                break;
            }
            ss.Add(s.Substring(io, indexOf - io));
            io = Math.Min(s.Length - 1, indexOf + 2);
        }

        return ss.ToArray();
    }
}