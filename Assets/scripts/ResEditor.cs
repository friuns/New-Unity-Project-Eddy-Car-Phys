using System;
using System.Linq;
using System.Reflection;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using gui = UnityEngine.GUILayout;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResEditor : bs
{
    public GameObject bag;
    public Object assetFolder;
    public Object[] scenesToBuild;
    public GameObject[] components;
    private MonoBehaviour o;
    private Vector2 scroll;
    internal bool audioOnly;
    public bool autorun;
#if UNITY_EDITOR
    public override void OnEditorGui()
    {

        audioOnly = gui.Toggle(audioOnly, "Audio Only");
        var c = components[0];
        MonoBehaviour[] monoBehaviours = c.GetComponents<MonoBehaviour>();
        if (o == null && monoBehaviours.Length > 0)
            o = monoBehaviours[0];

        if (o != null)
        {
            Component[] cs = components.Select(a => a.GetComponent(o.GetType())).Where(a => a != null).ToArray();

            var fs = o.GetType().GetFields();
            gui.BeginHorizontal();
            gui.BeginVertical();
            foreach (var m in cs)
                gui.Label(m.name);
            gui.Label("name");
            gui.EndVertical();
            scroll = gui.BeginScrollView(scroll);
            gui.BeginHorizontal();
            foreach (FieldInfo a in fs)
            {
                gui.BeginVertical();
                bool draw = false;
                foreach (Component m in cs)
                {
                    var value = a.GetValue(m);
                    if (value is float && !audioOnly)
                    {
                        float floatField = EditorGUILayout.FloatField((float)value);
                        if (floatField != (float)value)
                        {
                            a.SetValue(m, floatField);
                            EditorUtility.SetDirty(m);
                        }
                        draw = true;
                    }
                    else if (value is int && !audioOnly)
                    {
                        int floatField = EditorGUILayout.IntField((int)value);
                        if (floatField != (int)value)
                        {
                            a.SetValue(m, floatField);
                            EditorUtility.SetDirty(m);
                        }
                        draw = true;
                    }
                    else if (value is AudioClip)
                    {
                        var ofiled = EditorGUILayout.ObjectField((Object)value, typeof(AudioClip), true);
                        if (ofiled != (Object)value)
                        {
                            a.SetValue(m, ofiled);
                            EditorUtility.SetDirty(m);
                        }
                        draw = true;
                    }
                    else if (value is AudioSource && Clip(value))
                    {
                        //SerializedObject so = new SerializedObject(m);
                        //EditorGUILayout.PropertyField(so.FindProperty(a.Name + ".clip"));

                        Object ofiled = EditorGUILayout.ObjectField(((AudioSource)value).clip, typeof(AudioClip), true);
                        if (ofiled != (Object)value)
                        {
                            ((AudioSource)value).clip = (AudioClip)ofiled;
                            if (ofiled != null)
                                EditorUtility.SetDirty(ofiled);
                        }
                        draw = true;
                    }

                    //    gui.Label(a.Name + ":" + value);

                }
                if (draw)
                    gui.Label(a.Name);
                gui.EndVertical();
            }
            gui.EndHorizontal();
            gui.EndScrollView();


            gui.EndHorizontal();
        }

        foreach (var mh in monoBehaviours)
            if (gui.Button(mh.GetType().Name, gui.ExpandWidth(false)))
                o = mh;
        gui.FlexibleSpace();
        base.OnEditorGui();
    }
#endif
    private static bool Clip(object value)
    {
        try
        {
            AudioClip audioClip = ((AudioSource)value).clip;
            if (audioClip == null) throw new Exception();
        } catch (Exception)
        {
            return false;
        }
        return true;
    }
}