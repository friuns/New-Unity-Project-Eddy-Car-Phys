using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using UnityEngine;
public class web : bs { }
public partial class bs
{
    private const string m_mainSite = "//tmrace.net/cops/";
    internal static string mainSite { get { return http + m_mainSite; } }
    internal static string http
    {
        get { return (Application.absoluteURL == null || !Application.absoluteURL.ToLower().StartsWith("https") ? "http:" : "https:"); }
    }

    public static bool isHttps { get { return Application.absoluteURL.ToLower().StartsWith("https"); } }

    public static MonoBehaviour corObj;

    //public static WWW Download(string url, params object[] prms)
    //{
    //    return Download(url, null, null, false, prms);
    //}


    //public static WWW Download(string url, Action<string, bool> a = null, bool post = false, params object[] prms)
    //{
    //    return Download2(url, a == null ? (Action<WWW>)null : w => a(w.text, true), a == null ? (Action<string>)null : s => a(s, false), post, prms);
    //}

    public static WWW Download(string url, Action<WWW> a = null, Action<string> b = null, bool post = false, params object[] prms)
    {
        if (bs.settings.offline && isDebug)
            return null;

        if (!url.StartsWith("http"))
            url = mainSite + url;
        //if (bs.settings.disPlayerPrefs2)
        //{
        //    //if (a != null)
        //    //    a("", false);
        //    return null;
        //}
        url = Uri.EscapeUriString(url);

        WWW w;
        StringBuilder query = new StringBuilder();
        if (prms.Length > 0)
        {
            WWWForm form = new WWWForm();
            for (int i = 0; i < prms.Length; i += 2)
            {
                if (post)
                {
                    if (prms[i + 1] is byte[])
                        form.AddBinaryData(prms[i].ToString(), (byte[])prms[i + 1]);
                    //else if (prms[i + 1] is IConvertible)
                    else
                        form.AddField(prms[i].ToString(), prms[i + 1].ToString());
                    //else
                    //    form.AddField(prms[i].ToString(), JsonMapper.ToJson(prms[i + 1]));
                }
                query.Append(i != 0 ? "&" : "?");
                query.Append(prms[i] + "=" + WWW.EscapeURL(prms[i + 1].ToString()));
            }
            w = post ? new WWW(url, form) : new WWW(url + query);
        }
        else
            w = new WWW(url);

        corObj.StartCoroutine(DownloadCor(a, b, w, UnityEngine.StackTraceUtility.ExtractStackTrace()));

        return w;
    }
    //private static  string url;
    private static IEnumerator DownloadCor(Action<WWW> a, Action<string> b, WWW w, string stack)
    {

        yield return w;
        string text;
        if (string.IsNullOrEmpty(w.error) && (text = w.url.EndsWith(".jpg") ? "" : w.text.Trim()) == text.Trim('<', '>'))
        {
            Debug.Log(w.url + "\n\n" + text + "\n\n" + stack);
            if (a != null)
                a(w);
        }
        else
        {
            var s = w.error ?? "Failed to Parse\n" + w.text;
            if (b != null)
                b(s);
            Debug.LogError(w.url + "\n" + s + "\n" + stack);
        }
    }

    private static HashSet<string> eventsCommited = new HashSet<string>();
    private static void LogEvent(string gr, string name)
    {
        if (!eventsCommited.Contains(name))
        {
            var prms = gr == "Site" ? gr + "/" + platformPrefix + "/" + name : platformPrefix + bs.settings.version + "/" + gr + "/" + name;
            Download(mainSite + "scripts/count.php", null, null, false, "submit", prms);
            eventsCommited.Add(name);
        }
        //GA.API.Design.NewEvent(String.Format("{0}:{1}", @group, name.Replace(':', ' ')));
    }

    public static void LogEvent(string name)
    {
        LogEvent(EventGroup.Other.ToString(), name);
    }
    public static void LogEvent(EventGroup eg, string name)
    {
        LogEvent(eg.ToString(), name);
    }
}

public enum EventGroup
{
    Other, SiteOld,
    Maps,
    Fps,
    playedTime,
    LoadedIn,
    Site,
    GameType,
    LevelEditor,
    Debug,
    Shop,
    Ban
}