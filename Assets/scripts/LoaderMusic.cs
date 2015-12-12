using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Security.Permissions;
using System.Text;
using ExitGames.Client.Photon;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;
using gui = UnityEngine.GUILayout;


public class LoaderMusic : bs
{
    public string music = "http://tmrace.net/cops/cops.mp3";
    public void Awake()
    {
        _LoaderMusic = this;
    }

    public void LoadMusic(string url, bool broadcast = false)
    {
        StartCoroutine(StartLoadMusic(url, broadcast));
    }
    private IEnumerator StartLoadMusic(string url, bool broadcast = false)
    {
        var s = http + "//tmrace.net/cops/music.php?url=" + WWW.EscapeURL(url);
        print(s);
        var w = new WWW(s);
        yield return w;
        print(w.text);
        w = new WWW(w.text);
        yield return w;
        var audioClip = w.GetAudioClip(false, true);
        if (audioClip.length == 0)
        {
            Debug.LogError(w.error);
            yield break;
        }
        if (broadcast && _Game && audioClip)
        {
            broadCastTime = Time.time;
            _Game.CallRPC(_ChatGui.Chat, _Player.playerName + " Set music to " + w.text);
            _Game.CallRPCTo(_Game.LoadMusic, PhotonTargets.Others, w.text);
        }

        audio.clip = audioClip;
        audio.Play();
    }
    public static float broadCastTime = -10;
}