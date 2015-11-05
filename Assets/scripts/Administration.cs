using gui = UnityEngine.GUILayout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public partial class Administration : GuiClasses
{

    internal Transform cameraTr;
    public new Camera camera;

    HashSet<string> maps = new HashSet<string>();
    public void Awake()
    {
        maps = new HashSet<string>(PlayerPrefs.GetString("Maps" + Application.loadedLevelName).SplitString());
        cameraTr = camera.transform;
        mapName = Application.loadedLevelName;
    }
    public void OnValidate()
    {
        if (!_Game && Application.isPlaying)
            hostRoom = new RoomInfo("", null);
    }
    public void Start()
    {

        if (!_Game)
        {
            hostRoom = new RoomInfo("", null);
            ShowAdmin();
        }
        bounds = Game.GetLevelBounds();
        Frame();
    }
    public void Toggle()
    {
        if (isActiveAndEnabled)
            CloseAdmin();
        else
            ShowAdmin();
    }
    public void ShowAdmin()
    {
        gameObject.SetActive(true);
        ShowDraggers(true);
        ShowWindow(GuiWindow);
        _MainCamera.gameObject.SetActive(false);
    }
    private void CloseAdmin()
    {
        if (_Game.GetSpawns().Length == 0)
            _Administration.LoadMap(new MemoryStream(_Loader.mapBytes));
        _Game.RefreshSpawns();
        CloseWindow();
        ShowDraggers(false);
        _MainCamera.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    public void ShowDraggers(bool b)
    {
        foreach (Dragger a in FindObjectsOfType(typeof(Dragger)))
            Destroy(a.gameObject);
        if (b)
            foreach (ItemBase a in FindObjectsOfType(typeof(ItemBase)))
                CreateDragger(a);
    }
    private Dragger CreateDragger(ItemBase item)
    {
        Dragger d = (Dragger)Instantiate(res.dragger);
        d.sprite.color = colors[item.index];
        d.tr.parent = item.transform;
        d.tr.localPosition = Vector3.up * 4;
        d.tr.localRotation = Quaternion.identity;
        d.m_transform = item.transform;
        return d;
    }
    private Bounds bounds;
    private Vector3 mouseMove;
    public RaycastHit hitpos;

    private Vector3 mouseDrag;
    public Dragger dragging;
    public Dragger hover;
    public Vector3 lastForward = Vector3.forward;
    public void Update()
    {
        if (win.WindowHit) return;
        var sets = settings.editor;
        var mpos = Input.mousePosition;
        mouseMove = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouseDrag = new Vector3(mouseMove.x, 0, mouseMove.y) * sets.factorMove * camera.orthographicSize;
        var move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * sets.factorMove * camera.orthographicSize;

        var ray = camera.ScreenPointToRay(mpos);
        Physics.Raycast(ray, out hitpos, 99999, Layer.levelMask);

        hover = null;
        {
            RaycastHit h;
            if (Physics.Raycast(ray, out h, 99999, 1 << Layer.dragger) && (isMaster || isDebug))
                hover = h.transform.GetComponent<Dragger>();
        }
        if (!(isMaster || isDebug))
            curTool = ToolType.Select;
        var mouse0 = Input.GetMouseButton(0);
        var mouse1 = Input.GetMouseButton(1);
        var mouse2 = Input.GetMouseButton(2);
        var mouseDown0 = Input.GetMouseButtonDown(0);
        var mouseDown1 = Input.GetMouseButtonDown(1);

        if (mouseDown0 && hover)
            dragging = hover;
        if (!mouse0)
            dragging = null;

        camera.orthographicSize -= (mouse1 ? (mouseMove.x - mouseMove.y) * .1f : Input.GetAxis("Mouse ScrollWheel")) * camera.orthographicSize * sets.factorScale;

        if (dragging)
        {
            var zeroY = ZeroY(hitpos.point - dragging.pos);
            if (zeroY.magnitude > 0)
                lastForward = dragging.tr.forward = zeroY;
            dragging.pos = Vector3.Lerp(dragging.pos, _Administration.hitpos.point + Vector3.up, Time.deltaTime * 10);

        }
        else
        {
            if (mouseDown0 && prefab)
            {
                var a = InstantiateSceneObject(prefab.fullName, hitpos.point, Quaternion.LookRotation(lastForward)).GetComponent<ItemBase>();
                //var a = (MonoBehaviour)Instantiate(prefab, hitpos.point, Quaternion.LookRotation(ZeroY(bounds.center - hitpos.point)));
                dragging = CreateDragger(a);
            }
            else if (mouse0 || mouse2)
                cameraTr.position -= mouseDrag;
        }
        cameraTr.position += move;


        if (hover && (mouseDown0 && curTool == ToolType.Delete || mouseDown1))
            PhotonNetwork.Destroy(hover.tr.gameObject);
    }
    private ItemBase prefab;
    //bool curToolItem { get { return GetTool(curTool) != null; } }
    //bool curToolSpawn { get { return curTool == SpawnTool.Team_1 || curTool == SpawnTool.Team_2 || curTool == SpawnTool.dmSpawn || curTool == SpawnTool.StartPos; } }

    Color[] colors = new Color[] { Color.white, Color.green, Color.blue, Color.red, Color.cyan, Color.yellow, };

    private static void ClearMap()
    {
        foreach (var a in FindObjectsOfType<ItemBase>())
            PhotonNetwork.Destroy(a.gameObject);
    }

    public void Frame()
    {
        bounds.size = new Vector3(bounds.size.x, 0, bounds.size.z);
        Camera c = camera;
        Vector3 max = bounds.size;
        float radius = max.magnitude / 2f;
        float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(c.fieldOfView * Mathf.Deg2Rad / 2f) * c.aspect) * Mathf.Rad2Deg;
        float fov = Mathf.Min(c.fieldOfView, horizontalFOV);
        float dist = radius / (Mathf.Sin(fov * Mathf.Deg2Rad / 2f));
        c.transform.position = bounds.center + dist * Vector3.up;
        if (c.orthographic)
            c.orthographicSize = radius * .7f / ((float)Screen.width / Screen.height);
        c.transform.LookAt(bounds.center);
    }
    internal ToolType curTool = ToolType.Select;

    public Administration()
    {
        _Administration = this;

    }
    Dictionary<string, ItemBase> tools;
    public ItemBase GetTool(string tool)
    {
        if (tools == null)
        {
            tools = new Dictionary<string, ItemBase>();
            foreach (var a in res.tools)
                tools.Add(a.name, a);
        }
        ItemBase b;
        tools.TryGetValue(tool, out b);
        return b;
    }



}
