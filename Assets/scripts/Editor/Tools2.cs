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
        Debug.Log(JsonMapper.ToJson(new RoomSettings()));
    }

}