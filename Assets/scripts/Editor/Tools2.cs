using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using LitJson;
using UnityEditor;
using UnityEngine;

public class Tools2 : Editor
{

    [MenuItem("RTools/test")]
    public static void BuildAssets()
    {
        //VarParse varParse = new VarParse();
        //varParse.UpdateValues(new RoomSettings(), "RoomSettings");
        //Debug.Log(varParse.jsonData.ToJson());

        ExitGames.Client.Photon.Hashtable d = new ExitGames.Client.Photon.Hashtable();
        var s1 = new StringBuilder();
        var s2 = new StringBuilder();
        s1.Append("/");
        s2.Append("/");
        d.Add(s1, null);
        Debug.Log(s1.GetHashCode() == s2.GetHashCode());

    }
}