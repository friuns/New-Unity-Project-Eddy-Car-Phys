using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


public partial class LevelEditor
{

    public void SaveMap()
    {
        print("Save map " + mapName);
        var memoryStream = new MemoryStream();
        using (var bw = new BinaryWriter(memoryStream))
        {
            bw.Write((int)PacketType.Version);
            foreach (var a in FindObjectsOfType<ItemBase>())
            {
                bw.Write((int)PacketType.Spawn);
                bw.Write(a.fullName);
                bw.Write(a.pos);
                bw.Write(a.rot);
            }
            byte[] inArray = memoryStream.ToArray();
            PlayerPrefs.SetString("map:" + mapName, Convert.ToBase64String(inArray));
            localMaps.Add(mapName);
            PlayerPrefs.SetString("Maps" + Application.loadedLevelName, string.Join("\n", localMaps.ToArray()));
            Download("scripts/addMap.php", null, null, true, new object[] { "mapStats", JsonMapper.ToJson(room.sets.mapStats), "file", inArray, "icon", makeMapIcon.EncodeToJPG(), "override", isDebug ? 1 : 0 });
        }
    }

    private Texture2D makeMapIcon;
    private Texture2D MakeMapIcon()
    {
        var resWidth = 128;
        var resHeight = 128;
        var camera = Camera.main;

        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false, true);
        camera.GetComponent<GUILayer>().enabled = false;
        camera.Render();
        camera.GetComponent<GUILayer>().enabled = true;
        RenderTexture.active = rt;
        if (!isHttps)
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        //DestroyImmediate(rt);        
        screenShot.Apply(false);

        return screenShot;

    }

    public void LoadMap(Stream ms)
    {
        ClearMap();
        using (ms)
        {
            var br = new BinaryReader(ms);
            while (ms.Position < ms.Length)
            {
                var pk = (PacketType)br.ReadInt32();
                if (pk == PacketType.Spawn)
                {
                    var readString = br.ReadString();
                    InstantiateSceneObject(readString, br.ReadVector(), br.ReadQuater());
                }
            }
        }
    }
    public void LoadMapWindow()
    {

        foreach (var s in localMaps)
            if (Button(s))
            {
                print("Load map " + s);
                mapName = s;
                var a = PlayerPrefs.GetString("map:" + mapName, "");
                if (string.IsNullOrEmpty(a)) return;
                LoadMap(new MemoryStream(Convert.FromBase64String(a)));
                ShowDraggers(true);
                Back();
            }
        GUILayout.Label("");
    }
    string mapName = "";
    public void SaveMapWindow()
    {
        GUILayout.Label(makeMapIcon);
        mapName = TextField("Name", mapName);
        if (Button("Save"))
        {
            if (_Game.spawns.Count == 0)
                ShowPopup("Save failed, please add spawns");
            else
            {
                SaveMap();
                Back();
            }
        }
    }



    private GameObject InstantiateSceneObject(string ReadString, Vector3 ReadVector, Quaternion ReadQuater)
    {
        var g = PhotonNetwork.isMasterClient ? PhotonNetwork.InstantiateSceneObject(ReadString, ReadVector, ReadQuater, 0, null) : (GameObject)Instantiate(Resources.Load(ReadString), ReadVector, ReadQuater);
        return g;
    }
}

