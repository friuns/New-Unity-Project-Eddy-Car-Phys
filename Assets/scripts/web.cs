using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public partial class bs
{
    private const string m_mainSite = "://tmrace.net/cops/";
    internal static string mainSite { get { return http + m_mainSite; } }
    internal static string http
    {
        get { return (Application.absoluteURL == null || !Application.absoluteURL.ToLower().StartsWith("https") ? "http" : "https"); }
    }


    public static MonoBehaviour corObj;

    public static WWW Download2(string url, Action<WWW> a = null, bool post = false, params object[] prms)
    {
        if (bs.settings.offline && isDebug)
            return null;
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID

        var fileName = Path.GetFileName(url);
        if (File.Exists(fileName))
            return new WWW("file://" + Path.GetFullPath(fileName));
#endif
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
                    else
                        form.AddField(prms[i].ToString(), prms[i + 1].ToString());
                }
                query.Append(i != 0 ? "&" : "?");
                query.Append(prms[i] + "=" + WWW.EscapeURL(prms[i + 1].ToString()));
            }
            w = post ? new WWW(url, form) : new WWW(url + query);
        }
        else
            w = new WWW(url);
        print(post ? w.url + query : w.url);
        if (a != null)
            corObj.StartCoroutine(DownloadCor(a, w));
        return w;
    }
    private static IEnumerator DownloadCor(Action<WWW> a, WWW w)
    {
        yield return w;
        a(w);
    }


    public static WWW Download(string url, Action<string, bool> a = null, bool post = false, params object[] prms)
    {
        if (bs.settings.offline && isDebug)
            return null;
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID

        var fileName = Path.GetFileName(url);
        if (File.Exists(fileName))
            return new WWW("file://" + Path.GetFullPath(fileName));
#endif
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
                    else
                        form.AddField(prms[i].ToString(), prms[i + 1].ToString());
                }
                query.Append(i != 0 ? "&" : "?");
                query.Append(prms[i] + "=" + WWW.EscapeURL(prms[i + 1].ToString()));
            }
            w = post ? new WWW(url, form) : new WWW(url + query);
        }
        else
            w = new WWW(url);
        print(post ? w.url + query : w.url);
        if (a != null)
            corObj.StartCoroutine(DownloadCor(a, w));
        return w;
    }
    //private static  string url;
    private static IEnumerator DownloadCor(Action<string, bool> a, WWW w)
    {
        yield return w;
        string trim;
        if (String.IsNullOrEmpty(w.error) && !((trim = w.text.Trim()).StartsWith("<") && trim.EndsWith(">")))
            a(w.text, true);
        else
        {
            a(w.error == null ? "Failed to Parse" + w.text : w.error, false);
            Debug.LogWarning(w.error + w.text + "\n" + w.url);
        }
    }

    private static HashSet<string> eventsCommited = new HashSet<string>();
    private static void LogEvent(string gr, string name)
    {
        if (!eventsCommited.Contains(name))
        {
            var prms = gr == "Site" ? gr + "/" + platformPrefix + "/" + name : platformPrefix + bs.settings.version + "/" + gr + "/" + name;
            Download(mainSite + "scripts/count.php", null, false, "submit", prms);
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
    Shop
}