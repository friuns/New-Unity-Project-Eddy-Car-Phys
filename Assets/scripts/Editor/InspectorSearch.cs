using System.Text;
using UnityEditor;
using System;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using gui = UnityEngine.GUILayout;
using egui = UnityEditor.EditorGUILayout;
using Object = UnityEngine.Object;
using Bs = bs;
using print = UnityEngine.Debug;

public class InspectorSearch : EditorWindow
{
    string search = "";
    public bool SetPivot;
    public bool SetCam = false;
    //public bool drawSearch2;
    Vector3 oldPivot;
    public bool advanced;
    private bool focus = false;
    public void OnFocus()
    {

        focus = false;
    }
    [MenuItem("Window/RInspector Search")]
    static void OpenWindow()
    {
        var v = GetWindow<InspectorSearch>();
        v.title = "Inspector S";
    }
    protected virtual void OnGUI()
    {
        GUI.SetNextControlName("search");
        search = gui.TextField(search);
        if (!focus)
        {
            GUI.FocusControl("search");
            focus = true;
        }

        gui.BeginHorizontal();
        lck = gui.Toggle(lck, "lock");
        advanced = gui.Toggle(advanced, "advanced");
        gui.EndHorizontal();
        if (advanced)
        {
            if (!SetPivot && Selection.activeGameObject) oldPivot = Selection.activeGameObject.transform.position;
            SetPivot = (gui.Toggle(SetPivot, "Pivot", gui.ExpandWidth(false)) && Selection.activeGameObject != null);
            SetCam = (gui.Toggle(SetCam && Camera.main != null, "Cam", gui.ExpandHeight(false))); //camset        
            QualitySettings.shadowDistance = egui.FloatField("LightmapDist", QualitySettings.shadowDistance);
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);
        egui.BeginHorizontal();
        showPrivate = gui.Toggle(showPrivate, "Show private");
        showGetters = gui.Toggle(showGetters, "Search properties");
        showStatic = gui.Toggle(showStatic, "Search static");
        egui.EndHorizontal();

        if (lck && activeGameObject == null || !lck)
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Base>() != null)
                activeGameObject = Selection.activeGameObject.GetComponent<Base>();

        if (search.Length > 0)
            DrawSearch();
        else
            OnGUIMono();
        EditorGUILayout.EndScrollView();


    }


    InspectorSearch()
    {
        SceneView.onSceneGUIDelegate = OnSceneUpdate;
    }
    private void OnSceneUpdate(SceneView scene)
    {
        UpdateOther(scene);
        UpdateSetCam(scene);
    }
    private void UpdateOther(SceneView scene)
    {
        bool repaint = false;
        //foreach (var a in GameObject.FindGameObjectsWithTag("EditorGUI"))
        //    if (a.GetComponent<Base>() != null)
        //        a.GetComponent<Base>().OnSceneGUI(scene, ref repaint);
        //    else
        //        Debug.Log(a.name);
        if (repaint) { Repaint(); }
        var ago = Selection.activeGameObject;
        if (SetPivot)
        {
            var move = oldPivot - ago.transform.position;
            foreach (Transform t in ago.transform)
                t.position += move;
        }
        if (ago != null)
            oldPivot = ago.transform.position;
    }
    private void UpdateSetCam(SceneView scene)
    {
        if (SetCam)
        {
            if (EditorApplication.isPlaying && !EditorApplication.isPaused && Camera.main != null)
            {
                var t = Camera.main.transform;
                scene.LookAt(t.position, t.rotation, 3);
            }
        }
    }
    private static Vector2 scroll;
    public static string searchLog = "";
    public static bool lck;
    public static bool showPrivate;
    public static bool showStatic;
    public static bool showGetters;
    private Base activeGameObject;// { get { return Bs.resEditor.activeGameObj; } set { Bs.resEditor.activeGameObj = value; } }
    private void OnGUIMono()
    {
        if (activeGameObject != null)
            activeGameObject.OnEditorGui();
        if (bs.repaint)
        {
            Repaint();
            bs.repaint = false;
        }
    }
    
    public void OnSelectionChange()
    {
        //foreach (var a in GameObject.FindGameObjectsWithTag("EditorGUI"))
        //    a.GetComponent<Base>().OnSelectionChanged();
        SetPivot = false;
        //search = "";
        this.Repaint();
    }
    public static bool isPlaying { get { return EditorApplication.isPlaying || EditorApplication.isPaused || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode; } }


    private void DrawSearch()
    {

        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        if (showPrivate)
            flags |= BindingFlags.NonPublic;
        if (showGetters && search.Length > 2)
            flags |= BindingFlags.Static;
        //if (search.Length > 2)
        var ago = activeGameObject;
        if (ago)
        {
            var ms = ago.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour m in ms)
            {
                if (m == null) return;
                Type type = m.GetType();


                IEnumerable<MemberInfo> memberInfos = type.GetFields(flags | BindingFlags.DeclaredOnly).Cast<MemberInfo>().Concat(type.BaseType.GetFields(flags));
                if (showGetters && search.Length > 2)
                    memberInfos = memberInfos.Concat(type.GetProperties(flags | BindingFlags.DeclaredOnly)).Concat(type.BaseType.GetProperties(flags));
                SerializedObject so = new SerializedObject(m);
                var soChanged = false;
                foreach (MemberInfo a in memberInfos)
                {
                    Type rt = a.ReflectedType;

                    if (rt != typeof(Object) && rt != typeof(Component) && rt != typeof(MonoBehaviour) && rt != typeof(Behaviour))
                        if (string.IsNullOrEmpty(search) || a.Name.ToLower().Contains(search))
                        {
                            Type t;
                            object value = a.GetValue(m, out t);
                            if (value is float)
                            {
                                float floatField = EditorGUILayout.FloatField(a.Name, (float)value);
                                if (floatField != (float)value)
                                    a.SetValue(m, floatField);
                            }
                            else if (value is int)
                            {
                                int floatField = EditorGUILayout.IntField(a.Name, (int)value);
                                if (floatField != (int)value)
                                    a.SetValue(m, floatField);
                            }
                            else if (value is bool)
                            {
                                bool floatField = EditorGUILayout.Toggle(a.Name, (bool)value);
                                if (floatField != (bool)value)
                                    a.SetValue(m, floatField);
                            }
                            else if (t != null && t.BaseType == typeof(Object))
                            {
                                Object floatField = EditorGUILayout.ObjectField(a.Name, (Object)value, t, false);
                                if (floatField != (Object)value)
                                    a.SetValue(m, floatField);
                            }
                            else if (value is Vector3)
                            {
                                Vector3 floatField = EditorGUILayout.Vector3Field(a.Name, (Vector3)value);
                                if (floatField != (Vector3)value)
                                    a.SetValue(m, floatField);
                            }
                            else if (value is Vector4)
                            {
                                Vector4 floatField = EditorGUILayout.Vector4Field(a.Name, (Vector4)value);
                                if (floatField != (Vector4)value)
                                    a.SetValue(m, floatField);
                            }
                            else if (value is string)
                            {
                                string floatField = EditorGUILayout.TextField(a.Name, (string)value);
                                if (floatField != (string)value)
                                    a.SetValue(m, floatField);
                            }
                            else if (value is AnimationCurve)
                            {
                                AnimationCurve ac = EditorGUILayout.CurveField(a.Name, (AnimationCurve)value);
                                if (ac != value)
                                    a.SetValue(m, ac);
                            }
                            else if (value is Enum)
                            {
                                Enum en = EditorGUILayout.EnumPopup(a.Name, (Enum)value);
                                if (!en.Equals(value))
                                    a.SetValue(m, en);
                            }
                            else
                            {
                                var fp = so.FindProperty(a.Name);
                                if (fp != null)
                                    EditorGUILayout.PropertyField(fp, true);
                                soChanged = true;
                                //gui.Label(a.Name + ":" + value);
                            }

                        }

                }
                if (soChanged)
                    so.ApplyModifiedProperties();
                if (valueChanged)
                {
                    EditorUtility.SetDirty(m);
                    Undo.RegisterCompleteObjectUndo(m, m.name);
                }
                valueChanged = false;
            }


            if (search.Length > 2 && Application.isPlaying)
                Repaint();
        }

    }
    public static bool valueChanged;

}

public static class InspectorSearchExt
{
    public static object GetValue(this MemberInfo m, object o, out Type type)
    {
        try
        {

            if (m is FieldInfo)
            {
                type = ((FieldInfo)m).FieldType;
                return ((FieldInfo)m).GetValue(o);
            }
            else
            {
                type = ((PropertyInfo)m).PropertyType;
                return ((PropertyInfo)m).GetValue(o, null);
            }
        }
        catch (System.Exception)
        {
            type = null;
            return null;
        }
    }
    //    public static void SetValue(this MemberInfo m, object o, object v)
    //    {
    //        InspectorSearch.valueChanged = true;
    //        try
    //        {
    //            if (m is FieldInfo)
    //                ((FieldInfo)m).SetValue(o, v);
    //            else
    //                ((PropertyInfo)m).SetValue(o, v, null);
    //        } catch (System.Exception) { }
    //    }
}