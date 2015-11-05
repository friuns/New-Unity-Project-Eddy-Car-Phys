using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


public partial class Administration
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
            var inArray = memoryStream.ToArray();
#if !UNITY_WEBPLAYER
            var mapDataPath = "Assets/!prefabs/Resources/" + GetMapDataPath(mapName);
            Directory.CreateDirectory(Path.GetDirectoryName(mapDataPath));
            File.WriteAllBytes(mapDataPath, inArray);
#endif
            PlayerPrefs.SetString("map:" + mapName, Convert.ToBase64String(inArray));
            maps.Add(mapName);
            PlayerPrefs.SetString("Maps" + Application.loadedLevelName, string.Join("\n", maps.ToArray()));
        }
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

        foreach (var s in maps)
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
        mapName = TextField("Name", mapName);
        if (Button("Save"))
        {
            if (_Game.GetSpawns().Length == 0)
                ShowPopup("Save failed, please add spawns");
            else
            {
                SaveMap();
                Back();
            }
        }
    }
    public static string GetMapDataPath(string MapName, bool res = false)
    {
        return "maps/" + MapName + (res ? "" : ".txt");
    }


    private GameObject InstantiateSceneObject(string ReadString, Vector3 ReadVector, Quaternion ReadQuater)
    {
        var g = PhotonNetwork.isMasterClient ? PhotonNetwork.InstantiateSceneObject(ReadString, ReadVector, ReadQuater, 0, null) : (GameObject)Instantiate(Resources.Load(ReadString), ReadVector, ReadQuater);
        return g;
    }
}

