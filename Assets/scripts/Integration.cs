
using System;
using System.IO;
using System.Net;
using UnityEngine;

public partial class Loader
{
    public string url;
    public void InitUrl()
    {
        //var unity = Application.absoluteURL.Contains("kongregate") ? "kongregateUnitySupport.getUnityObject()" : "u.getUnity()";
        Url(isDebug ? "http://test.com" : Application.absoluteURL);

        ExternalEval("u.getUnity().SendMessage('Loader', 'Url',typeof GetUrl==='undefined'?document.location.toString(): GetUrl());");
        //ExternalEval("u.getUnity().SendMessage('Integration', 'UrlOdnoklasniki',document.location.toString());");
        
        if (Application.isWebPlayer || isDebug)
            StartCoroutine(AddMethod(1, delegate { web.LogEvent(EventGroup.Site, GetHostName()); }));
        
    }
    public static string GetHostName()
    {
        var names = url2.Host.Split('.');
        return names.Length > 2 ? names[names.Length - 2] : names[0];
    }
    private void ExternalEval(string P0)
    {
        Application.ExternalEval(P0);
    }
    public bool vkSite;
    public bool kongregate;
    public static Uri url2 = new Uri("http://Unkonwn.com");
    public void Url(string s)
    {
        url = s.ToLower();
        vkSite = s.Contains("vk.com");
        kongregate = s.Contains("kongregate.com");
        bs._Loader.RefreshAds();
        //print("Url received " + s);
        //curDict = vkSite ? 1 : 0;
        if (vkSite)
            curDict = 1;
        Uri.TryCreate(url, UriKind.Absolute, out url2);
    }
    
}