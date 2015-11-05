using System;
using System.Linq;
using System.Net;
using UnityEngine;

public class CarTextureChanger : GuiClasses
{
    //private bool inited;
    private Texture[] textures;
    private Texture[] originalTextures;

    private string[] urls;
    public Renderer r;
    public void OnValidate()
    {
        //inited = false;
    }
    private bool started;
    public void Start()
    {
        started = true;
        Init();


    }
    public void Init()
    {
        //if (inited) return;
        print("ChangeCarTexture Init");
        //inited = true;
        textures = r.sharedMaterials.Select(a => a.mainTexture).ToArray();
        originalTextures = textures.ToArray();
        if (urls == null)
        {
            urls = new string[textures.Length];
            for (int i = 0; i < textures.Length; i++)
                if (textures[i])
                    urls[i] = Def(textures[i].name);
        }

        for (int i = 0; i < textures.Length; i++)
            DownloadTexture(i, urls[i]);
    }
    private string Def(string Name)
    {
        return "http://tmrace.net/cops/textuers/" + Name + ".png";
    }
    internal void ChangeCarTextureWindow()
    {
        Init();
        var w = Screen.width - 50;
        ShowWindow(delegate
        {
            SetupWindow(w);
            for (int i = 0; i < materials.Length; i++)
            {
                if (_GuiSkins.buttonStyle.name == "")
                    _GuiSkins.buttonStyle = new GUIStyle(skin.button);

                var mainTexture = r.materials[i].mainTexture;
                if (mainTexture == null) continue;
                var w2 = w - 50;
                if (mainTexture.width < w2)
                    w2 = mainTexture.width;

                if (GUILayout.Button(mainTexture, _GuiSkins.buttonStyle, GUILayout.Width(w2), GUILayout.Height(((float)w2 / mainTexture.width) * mainTexture.height)))
                {
                    string url = urls[i] ?? "";
                    ShowWindow(delegate
                    {
                        url = GUILayout.TextArea(url);
                        if (Button("Download"))
                        {
                            DownloadTexture(i, url);
                            Back();
                        }
                    });
                    return;
                }
            }
            if (Button("Reset"))
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].mainTexture = textures[i] = originalTextures[i];
                    urls[i] = null;
                }
            }
        });

    }
    private void DownloadTexture(int i, string url)
    {
        if (!textures[i] || urls[i] == Def(textures[i].name))
            return;
        Download2(url, delegate (WWW w)
        {
            if (string.IsNullOrEmpty(w.error) && materials[i].mainTexture)
            {
                urls[i] = w.url;
                textures[i] = w.textureNonReadable;
                if (started)
                    materials[i].mainTexture = textures[i];
            }
            else if (win.active)
                ShowPopup("failed " + w.error);
        });
    }
    private Material[] materials { get { return r.materials; } }
}