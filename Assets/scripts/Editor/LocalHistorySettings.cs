using UnityEngine;

public class LocalHistorySettings :MonoBehaviour
{
    public readonly static string[] FileExtensionsStatic = new string[] { ".unity", ".guiskin", ".prefab", ".fbx", ".controller", ".anim", ".jpg", ".shader",".asset" ,".ttf"};
    public string[] FileExtensions = FileExtensionsStatic;
}