using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class BuildTools : Editor
{
    private static BuildTarget activeBuildTarget { get { return EditorUserBuildSettings.activeBuildTarget; } }
    [MenuItem("RTools/build assets")]
    public static void BuildAssets()
    {
        BuildAssets("resources", resEditor.assetFolder);
    }

    public static void BuildAssets(string resources, Object assetObject)
    {
        BuildTarget bt = activeBuildTarget;
        var file = resources + ".unity3d" + bs.platformPrefix2;
        var path = AssetDatabase.GetAssetPath(assetObject);

        var asss = new List<Object>();
        var asss2 = new List<string>();
        foreach (var f in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(f);
            if (asset != null)
            {
                asss.Add(asset);
                int startIndex = path.Length + 1;
                var str = f.Substring(startIndex, f.LastIndexOf('.') - startIndex).Replace('\\', '/');
                asss2.Add("@" + str);
            }
        }

        BuildPipeline.BuildAssetBundleExplicitAssetNames(asss.ToArray(), asss2.ToArray(), outputFolder + file, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, bt);
        //DisableRes(false);
    }
    public static string outputFolder = "cops2/";
    private static Settings settings { get { return bs.settings; } }

    public static ResEditor resEditor
    {
        get { return (ResEditor)Resources.LoadAssetAtPath("Assets/!Prefabs/resEditor.prefab", typeof(ResEditor)); }
    }

    public static void Build2(string s, BuildOptions buildOptions = BuildOptions.None)
    {
        if (resEditor.autorun)
            buildOptions |= BuildOptions.AutoRunPlayer;
        Build(buildOptions, "cops" + (settings.version + 1) + s);
    }
    [MenuItem("RTools/build Debug %g")]
    public static void Build()
    {
        Build2("debug", BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler);
    }
    [MenuItem("RTools/build Release")]
    public static void Release()
    {
        Build2("");
    }
    public static void Build(BuildOptions buildOptions, string name, bool includePackages = true)
    {
        Time.timeScale = 1;
        if (settings.release)
        {
            PlayerSettings.Android.keyaliasName = "friuns";
            PlayerSettings.Android.keystorePass = PlayerSettings.Android.keyaliasPass = "er54s4";
        }
        else
            PlayerSettings.Android.keyaliasName = PlayerSettings.Android.keyaliasPass = "";
        var unsigned = string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass);
        settings.version++;
        settings.SetDirty();
        PlayerSettings.Android.bundleVersionCode = settings.version;
        PlayerSettings.bundleVersion = settings.version.ToString();
        settings.versionDate = DateTime.Now.ToShortDateString();
        if (unsigned)
        {
            PlayerSettings.Android.keyaliasName = "";
            settings.free = false;
        }
        else
            name += "Release";
        if (settings.free)
            name += "Free";

        var flash = activeBuildTarget == BuildTarget.FlashPlayer;
        var android = activeBuildTarget == BuildTarget.Android;
        var web = activeBuildTarget == BuildTarget.WebPlayer || activeBuildTarget == BuildTarget.WebPlayerStreamed;
        var linux = activeBuildTarget == BuildTarget.StandaloneLinux;
        PlayerSettings.bundleIdentifier = "com.dm.race" + (settings.free ? "free" : "");
        string outputFolder = android ? "cops/" + name + ".apk" : flash ? "cops/" + name + ".swf" : web ? "cops" : linux ? "cops/tmlinux/TrackRacing" : "cops/" + name + "PC" + "/" + name + ".exe";
        Debug.Log(outputFolder);
        Debug.Log(buildOptions);
        BuildPipeline.BuildPlayer(packages, outputFolder, activeBuildTarget, buildOptions);
#if UNITY_EDITOR
        if (web && name != "debug")
            File.Copy("cops/cops.unity3d", "cops/" + name + ".unity3d");
#endif
    }

    private static string[] packages
    {
        get { return resEditor.scenesToBuild.Select(a => AssetDatabase.GetAssetPath(a)).ToArray(); }
    }
}