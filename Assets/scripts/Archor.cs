﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Archor : bs
{
    public new Camera camera;
    private Vector3 oldPos;
    public float x = 0;
    public float y = 0;
    private float height;
    private float width;
    private Vector3 Scale;
    public float scale { set { tr.localScale = Scale * value; m_size = null; } }
    private ScreenOrientation oldor;
    private Vector3? m_size;
    public new bool enabled { get { return renderer.enabled; } set { renderer.enabled = value; } }
    private SpriteRenderer spriteRenderer;
    public void Awake()
    {
        if (camera == null) camera = tr.GetComponentInParent<Camera>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Init2();
    }
    private void Init2()
    {
        width = Screen.width;
        height = Screen.height;
        oldPos = pos;
        Scale = tr.localScale;
        oldor = Screen.orientation;
    }
    public void Start()
    {
        Init2();
        if (skip) return;
        if (Application.isPlaying)
            Invoke("ResolutionChanged", .5f);
    }
    public void Update()
    {
        if (skip) return;
        if (width != 0 && height != 0)
            if (Screen.width != width || Screen.height != height || pos != oldPos || oldor != Screen.orientation)
                ResolutionChanged();
    }
    private void ResolutionChanged()
    {
        m_size = null;
        if (!skip)
        {
            oldPos = pos;
            width = Screen.width;
            height = Screen.height;
            
            oldor = Screen.orientation;
            if (x == 0 && y == 0)
            {
                var v = camera.WorldToViewportPoint(tr.position);
                x = v.x;
                y = v.y;
            }
            else
                tr.position = camera.ViewportToWorldPoint(inversePos);
        }
    }
    public bool HitTest(Vector2 vector2)
    {
        return enabled && renderer.bounds.IntersectRay(camera.ScreenPointToRay(new Vector3(vector2.x, vector2.y, 1)));
    }
    public Vector3 Abs(Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }
    public Vector3 size { get { return m_size ?? (m_size = Abs(camera.WorldToScreenPoint(renderer.bounds.min) - camera.WorldToScreenPoint(renderer.bounds.max))).Value; } }
    internal Color m_color;
    public Color color { get { return m_color; } set { if (m_color != value) spriteRenderer.color = m_color = value; } }//renderer.sharedMaterial.GetColor("_Emission"); 
    private static bool skip
    {
        get { return bs.settings == null || !bs.settings.enableGuiEdit && !Application.isPlaying; }
    }
    public Vector3 screenPos { set { inversePos = new Vector3(value.x / Screen.width, value.y / Screen.height, value.z); } }
    private bool reverse { get { return false; } }
    public Vector3 inversePos
    {
        get { return new Vector3(reverse ? 1 - x : x, reverse ? 1 - y : y, 1); }
        set { x = reverse ? 1 - value.x : value.x; y = reverse ? 1 - value.y : value.y; }
    }
    public new Vector3 pos { get { return new Vector3(x, y, 1); } set { x = value.x; y = value.y; } }
}