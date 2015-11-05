using System.Collections;
using System.IO;
using UnityEngine;

public class LevelTest : MonoBehaviour
{
    public IEnumerator Start()
    {
        DontDestroyOnLoad(this);
        LightmapSettings.bakedColorSpace = ColorSpace.Linear;
        print("LevelTest Start");
        var path = "http://cdn2.mwogame.com/mwosite/Levels/BattleMap_19_web.unity3d";
        var w = WWW.LoadFromCacheOrDownload(path, 0);
        yield return w;
        print("Loaded " + w.assetBundle);
        yield return Application.LoadLevelAsync(Path.GetFileNameWithoutExtension(path));
        print("Loaded");
        var level = new GameObject("level").transform;
        foreach (var a in FindObjectsOfType<Transform>())
        {
            if (a.parent == null)
                a.parent = level;
            if (a.gameObject.layer > 7 || a.gameObject.layer == 0)
                a.gameObject.layer = Layer.level;
        }
        foreach (var a in FindObjectsOfType<Camera>())
            Destroy(a.gameObject);
        LightmapSettings.bakedColorSpace = ColorSpace.Linear;
        Application.LoadLevelAdditive("2");
    }
}