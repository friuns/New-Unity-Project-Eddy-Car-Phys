using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Security.Permissions;
using System.Text;
using ExitGames.Client.Photon;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;
using gui = UnityEngine.GUILayout;

public partial class Loader : GuiClasses
{
    public ConsoleWindow consoleWindow;
    public InputManager m_Inputmanager;
    public GuiSkins m_guiSkins;
    public Window m_win;
    public Res m_res;
    //internal GUISkin defSkin;
    public static bool onValidateTest;
    //public Contr? m_controls;
    internal const Contr controls = Contr.keys;// { get { return m_controls ?? (m_controls = (Contr)PlayerPrefs.GetInt(platformPrefix + "controls2", (int)Contr.keys)).Value; } set { PlayerPrefs.SetInt(platformPrefix + "controls2", (int)(m_controls = value).Value); } }    
    public void Awake()
    {
        if (FindObjectsOfType(typeof(Loader)).Length > 1)
        {
            DestroyImmediate(gameObject);
            return;
        }
        InitValues();
        onValidateTest = true;
        if (FindObjectOfType<Game>() == null)
            DontDestroyOnLoad(this);
        print("Loader Awake");

        //hostRoom = new RoomInfo(bs._Loader.playerName + "'s room", null);
        Application.RegisterLogCallback(OnLogCallBack);
        //Physics.solverIterationCount = !android ? 6 : 2;
    }
    public Reporter reporter;
    public override void OnEditorGui()
    {
        foreach (var a in PhotonNetwork.playerList)
        {
            //a.varParse.UpdateValues();
            gui.BeginVertical(skin.box);
            GUILayout.Label(a.name);
            a.stats.games[0].kills.value = (int)GUILayout.HorizontalSlider(a.stats.games[0].kills.value, 0, 99);

            a.customProperties["/stats/[0]/kills/value"] = (int)GUILayout.HorizontalSlider((int)a.customProperties.TryGet("/stats/[0]/kills/value", 0), 0, 99);
            gui.Label(a.ToString2());
            gui.EndVertical();
        }
        repaint = true;

        //#if UNITY_EDITOR
        //        playCount = EditorGUILayout.IntField("playcount", playCount);
        //        startDay = EditorGUILayout.IntField("startDay ", startDay);
        //#endif
    }
    public void OnValidate()
    {
        if (FindObjectsOfType(typeof(Loader)).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        if (Application.isPlaying && !onValidateTest)
        {
            print("RESTART!!!!!!! " + Application.loadedLevelName);
            RestartLevel();
        }
        onValidateTest = true;
        InitValues();
    }

    public void RestartLevel()
    {
        Debug.LogWarning("RestartLevel");
        //        Application.LoadLevel(Application.loadedLevelName);
        //#if UNITY_EDITOR
        //        var logEntries = Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
        //        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        //        clearMethod.Invoke(null, null);
        //#endif
    }

    private void InitValues()
    {
        AudioListener.volume = .1f;
        web.corObj = bs._Loader = this;
        Input2 = m_Inputmanager;
        _GuiSkins = m_guiSkins;
        bs.settings.InitSettings();
        res = m_res;
        _AutoQuality = GetComponent<AutoQuality>();
    }


    public void Start()
    {

        //Download(mainSite + "scripts/count.php", null, false, "submit", platformPrefix + setting.version + "/" + group + "/" + name);
        if (!settings.fastLoad)
        {
            InitUrl();
            StartCoroutine(LoadSettings());
            web.Download("scripts/count.php?platform=" + platformPrefix + "&version=" + bs.settings.version, delegate { }, false);
            if (!string.IsNullOrEmpty(deviceUniqueIdentifier))
                web.Download("ids.txt", delegate (string s, bool b) { if (b && s.Contains(deviceUniqueIdentifier)) { ext.Block(" hack catched"); } }, false);

            web.Download("scripts/getItems.php?id=" + deviceUniqueIdentifier, delegate (string s, bool b) { if (b) Paypal.SetPaypalItems(s.SplitString().ToList()); }, false);
        }
        LoadFriends();
        if (Application.isEditor)
            PhotonNetwork.player.accType = AccType.Dev;
        PhotonNetwork.player.deviceId = deviceUniqueIdentifier;
        if (!isDebug)
            _LoaderMusic.LoadMusic(_LoaderMusic.music);
        //player = photonPlayer;
    }
    //public PhotonPlayer player;

    public static string Filter(string textField)
    {
        string arr = "qwertyuiopasdfghjklzxcvbnm1234567890яшертыуиопжасдфгхйклцзьчвбнмэщ"; //
                                                                                           //if (setting.vk)

        //textField = Cyrilic(textField);
        var ss = new List<char>(arr.ToCharArray());
        ss.AddRange(arr.ToUpper().ToCharArray());
        var ar = textField.ToCharArray();
        for (int i = 0; i < textField.Length; i++)
        {
            if (!ss.Contains(textField[i]))
                ar[i] = '-';
        }
        return new string(ar);
    }


    //public float playerScore { get { return PlayerPrefs.GetFloat("score"); } set { PlayerPrefs.SetFloat("score", value); } }

    public void Connect()
    {

        print("Connect");
        if (bs.settings.offline && _Game)
        {
            PhotonNetwork.offlineMode = true;
            bs._Loader.HostRoom();
        }
        else if (bs.settings.useLan)
            PhotonNetwork.ConnectToMaster("127.0.0.1", 5055, "", "asd");
        else
            Connect2();
    }

    internal int appId;
    public void Connect2()
    {
        var gameVersion = menuLoaded ? "eddy" + 5 : "1";
        PhotonNetwork.PhotonServerSettings.AppID = bs.settings.appIds[appId % bs.settings.appIds.Length];
        if (serverRegion == -1)
            PhotonNetwork.ConnectToBestCloudServer(gameVersion);
        else
            PhotonNetwork.ConnectToMaster(ServerSettings.FindServerAddressForRegion(serverRegion), 5055, PhotonNetwork.PhotonServerSettings.AppID, gameVersion);
    }



    public GUIText log;
    internal bool menuLoaded;
    public float deltaTime { get { return Time.unscaledDeltaTime; } }
    public void FullScreen(bool fullscreen = true)
    {
        if (!highQuality)
            Screen.fullScreen = fullscreen;
        else
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen);
    }
    public void UpdateValues()
    {
    }

    public void RefreshAds()
    {
        if (_Menu && _Menu.ads)
            _Menu.ads.SetActive(!bs._Loader.vkSite && !Application.isEditor && !android);
    }

    public void Update()
    {
        UpdateOther();
        UpdateKeys();
        UpdateLog();
        UpdateTouchInput();

        foreach (var a in PhotonNetwork.playerList)
            a.Update();
    }
    internal string GetMoneyText(float money)
    {
        return string.Format("Money:{0}$", Mathf.RoundToInt(money));
    }
    public float money;

    private void UpdateOther()
    {
        money = Mathf.Lerp(money, photonPlayer.GetMoney(), Time.deltaTime * 3);
    }

    private void UpdateLog()
    {
        StringBuilder sb = new StringBuilder("version:" + bs.settings.version);
        if (!bs._Loader.ownSite)
            sb.Append("a");

        if (errorCount > 0)
            sb.Append(" errors:" + errorCount);
        if (isDebug)
            sb.Append(" Debug");
        LogScreen(sb.ToString());
        if (Input2.GetKey(android ? KeyCode.Alpha4 : KeyCode.F4) && Input2.GetKeyDown(android ? KeyCode.Alpha6 : KeyCode.F6))
            consoleWindow.enabled = !consoleWindow.enabled;
        //print(PhotonNetwork.connectionStateDetailed);
        sbuilder.Append(sbuilder2);
        if (log)
            log.text = sbuilder.ToString();
        sbuilder = new StringBuilder();
    }
    private void UpdateKeys()
    {

        if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse3))
            Application.CaptureScreenshot("screenshots/" + DateTime.Now.Ticks + ".png");
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.P))
                Debug.Break();
        }
        if (Input.GetKeyDown(KeyCode.Backslash))
            reporter.gameObject.SetActive(!reporter.gameObject.activeSelf);
        if (Input2.GetKeyDown(KeyCode.B, true))
            _Administration.Toggle();

        if (Input.GetKeyDown(KeyCode.Escape) || Input2.GetKeyDown(KeyCode.M))
        {
            if (!win.enabled && _GameGui)
                win.ShowWindow(_GameGui.Menu);
            else
                win.Back();
        }

        if (Input2.GetKeyDown(KeyCode.F12, true) || Input2.GetKeyDown(KeyCode.F11, true))
            FullScreen();
    }

    private Vector2 oldTouch;
    private void UpdateTouchInput()
    {
        if (UnityEngine.Input.touchCount > 0)
        {
            Vector2 mouseDelta = Vector2.zero;
            if (oldTouch != Vector2.zero)
                mouseDelta = UnityEngine.Input.touches[0].position - oldTouch;
            oldTouch = UnityEngine.Input.touches[0].position;
            touchDelta = mouseDelta;
        }
        else
            oldTouch = Vector2.zero;

    }
    public static Vector2 touchDelta;
    public Vector2 getMouseDelta(bool returnMouse = true)
    {
        if (UnityEngine.Input.touchCount > 0)
            return touchDelta / 10;
        return returnMouse ? new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y")) : Vector2.zero;

        //return android && Input.touchCount > 0 ? Input.touches[0].deltaPosition / 5f : returnMouse ? new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) : Vector2.zero;
    }

    public void FixedUpdate()
    {
        sbuilder2 = new StringBuilder();
    }

    public static int errorCount;
    public void OnLogCallBack(string condition, string stacktrace, LogType type)
    {
        if (type == LogType.Exception)
            errorCount++;
        //#if GA
        //        GA.API.Debugging.HandleLog(condition, stacktrace, type);
        //#endif
#if UNITY_WEBPLAYER
        if (Application.isWebPlayer)
        {
            var args = "Unity:" + condition + (type == LogType.Exception ? "\r\n" + stacktrace : "");
            Application.ExternalCall("console." + (type == LogType.Exception ? "error" : type == LogType.Log ? "log" : "warn"), args);
        }
#endif
        reporter.AddLog(condition, stacktrace, type);
    }



    public void HostRoom()
    {
        print(">>>>>>>>>HostRoom");
        //= hostRoom.gameType == GameType.survival ? 4 : 10;
        //hostRoom.gravity = -12;
        hostRoom.version = bs.settings.mpVersion;
        hostRoom.collFix = hostRoom.gameType != GameTypeEnum.DeathMatch;
        hostRoom.android = android;
        RoomOptions ro = new RoomOptions();
        ro.maxPlayers = hostRoom.maxPlayers2;
        ro.customRoomProperties = hostRoom.customProperties;
        ro.customRoomPropertiesForLobby = hostRoom.customProperties.Keys.Select(a => a.ToString()).ToArray();
        PhotonNetwork.CreateRoom(room.name + (PhotonNetwork.GetRoomList().Any(a => a.name == room.name) ? Random.Range(0, 9) + "" : ""), ro, TypedLobby.Default);
    }
    public void OnPhotonCreateRoomFailed()
    {
        ShowPopup("Room with same name already exist");
    }
    public static void OpenUrl(string url)
    {
        Application.ExternalEval("window.top.location = '" + url + "';");
        Application.OpenURL(url);
    }

    internal WWW mapWWW;
    internal string mapName { get { return Path.GetFileName(mapUrl); } }
    internal string mapUrl { get { return room.mapUrl; } }
    //public IEnumerator LoadAndHost()
    //{
    //    yield return StartCoroutine(LoadLevel());

    //    //while (PhotonNetwork.room == null)
    //    //    yield return null;

    //    //yield return Application.LoadLevelAdditiveAsync("2");
    //}

    //public IEnumerator JoinRoom(RoomInfo name)
    //{
    //    mapUrl = name.mapUrl;
    //    if (!string.IsNullOrEmpty(mapUrl))
    //        yield return StartCoroutine(LoadLevel());

    //}
    internal List<string> ignoredRooms = new List<string>();

    private void LoadMap() { }
    internal byte[] mapBytes
    {
        get
        {
            if (m_mapBytes == null)
            {
                var asset = Resources.Load<TextAsset>(Administration.GetMapDataPath(mapName, true));
                if (asset)
                    return asset.bytes;
                return new byte[0];
            }
            return m_mapBytes;
        }
        set { m_mapBytes = value; }
    }
    private byte[] m_mapBytes;
    public IEnumerator LoadLevel(bool host)
    {

        if (!Application.CanStreamedLevelBeLoaded(mapName))
        {
            mapWWW = WWW.LoadFromCacheOrDownload(mapUrl, 0);
            win.ShowWindow(_LoaderGui.LoadingWindow, false);
            yield return mapWWW;
        }

        var w = Download(mainSite + Administration.GetMapDataPath(mapName));
        yield return w;
        if (string.IsNullOrEmpty(w.error))
            mapBytes = w.bytes;
        if (mapWWW != null && !mapWWW.assetBundle)
        {
            var s = "Could not load map " + mapWWW.error;
            ShowPopup(s);
            throw new Exception(s);
        }
        else
        {
            CloseWindow();

            var local = Application.CanStreamedLevelBeLoaded(mapUrl);//!mapUrl.StartsWith("http");
            if (!local)
            {
                yield return Application.LoadLevelAsync(Path.GetFileNameWithoutExtension(mapUrl));
                print("Loaded");
                SetupLevel();
            }
            if (host)
                _Loader.HostRoom();
            else
                PhotonNetwork.JoinRoom(room.name);

            while (PhotonNetwork.connectionStateDetailed != PeerState.Joined)
                yield return null;
            print("Loadlevel joined");
            if (local)
                yield return Application.LoadLevelAsync(mapName);
            else
                yield return Application.LoadLevelAdditiveAsync("2");

        }
    }

    private void SetupLevel()
    {
        try
        {
            var level = new GameObject("level").transform;
            foreach (var a in FindObjectsOfType<Transform>())
                if (a != tr)
                {
                    if (a.parent == null)
                        a.parent = level;
                    if (a.gameObject.layer > 7 || a.gameObject.layer == 0)
                        a.gameObject.layer = Layer.level;
                }
            foreach (var a in FindObjectsOfType<Camera>())
                Destroy(a.gameObject);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }


    public void OnPhotonJoinRoomFailed()
    {
        print("OnPhotonJoinRoomFailed");
    }
    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        previousRoom = room;
        PhotonNetwork.isMessageQueueRunning = false;


    }
    public IEnumerable<RoomInfo> GetRoomList()
    {
        return PhotonNetwork.GetRoomList().Where(a => !ignoredRooms.Contains(a.name));
    }

}