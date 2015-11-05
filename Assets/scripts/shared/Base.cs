#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Base : Photon.MonoBehaviour
{
    public virtual void OnEditorGui() { }
    public virtual void OnSceneGui(SceneView sc){}
    
    public virtual void OnQualityChanged() { }
}

#if !UNITY_EDITOR
public class SceneView
{
    public static OnSceneFunc onSceneGUIDelegate;
    public Camera camera;

    public delegate void OnSceneFunc(SceneView sceneView);
}
#endif