using System;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using gui = UnityEngine.GUILayout;
using UnityEngine;

public partial class Loader
{
    public string playerName
    {
        get
        {
            if (!PlayerPrefs.HasKey("name3"))
                playerName = res.nickNames.text.SplitString().Random();
            return PlayerPrefs.GetString("name3");
        }
        set
        {
            PlayerPrefs.SetString("name3", value);
        }
    }
    public int serverRegion { get { return m_serverRegion ?? (m_serverRegion = PlayerPrefs.GetInt("serverRegion", -1)).Value; } set { PlayerPrefs.SetInt("serverRegion", (m_serverRegion = value).Value); } }
    public int? m_serverRegion;

    public float voiceChatVolume { get { return m_voiceChatVolume ?? (m_voiceChatVolume = PlayerPrefs.GetFloat("voiceChatVolume", .3f)).Value; } set { PlayerPrefs.SetFloat("voiceChatVolume", (m_voiceChatVolume = value).Value); } }
    public float? m_voiceChatVolume;


    //public float volume { get { return m_volume ?? (m_volume = PlayerPrefs.GetFloat("volume", 600)).Value; } set { PlayerPrefs.SetFloat("volume", (m_volume = value).Value); } }
    //public float? m_volume;

    //public float musicVolume { get { return m_musicVolume ?? (m_musicVolume = PlayerPrefs.GetFloat("musicVolume", 600)).Value; } set { PlayerPrefs.SetFloat("musicVolume", (m_musicVolume = value).Value); } }
    //public float? m_musicVolume;


    public void RoomInfo(RoomInfo rom)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(room.name);
        sb.AppendLine("Players".PadRight(20) + room.playerCount + "/" + room.maxPlayers);
        foreach (var a in rom.customProperties)
        {
            var value = (ReferenceEquals(a.Key, "gameType") ? ((GameTypeEnum)a.Value) : a.Value);
            sb.AppendLine(a.Key.ToString().PadRight(20) + value);
        }
        gui.Label(sb.ToString(), skin.label);
    }

    Vector2 consoleScroll = new Vector2(0, float.MaxValue);
    public void ConsoleWindow()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(SystemInfo.operatingSystem);
        sb.AppendLine(SystemInfo.deviceType.ToString());
        sb.AppendLine(SystemInfo.deviceName);
        sb.AppendLine("graphicsMemorySize: " + SystemInfo.graphicsMemorySize);
        sb.AppendLine("systemMemorySize: " + SystemInfo.systemMemorySize);

        foreach (var a in reporter.currentLog)
        {
            var stringBuilder = sb.Append(a.logType).Append(":").Append(a.condition);
            if (a.logType == Reporter._LogType.Exception)
                stringBuilder.Append(a.stacktrace).AppendLine();
            stringBuilder.AppendLine();
        }
        string console = sb.ToString();

        ShowWindow(delegate
        {
            win.SetupWindow(1000, 1000);
            Label("Press ctrl+c to copy");
            consoleScroll = gui.BeginScrollView(consoleScroll);
            gui.TextField(console);
            SelectAll(console.Length);
            gui.EndScrollView();
        });

    }


    public void SettingsWindow()
    {
        win.SetupWindow(400, 500);
        AudioListener.volume = HorizontalSlider("Volume", AudioListener.volume, 0, 1);
        _LoaderMusic.audio.volume = HorizontalSlider("Music Volume", _LoaderMusic.audio.volume, 0, 1);
        voiceChatVolume = HorizontalSlider("voice Chat Volume", voiceChatVolume, 0, 1);
        if (!android && Button("Full Screen(f12)"))
            _Loader.FullScreen();
        if (Button("Quality Settings"))
            win.ShowWindow(_AutoQuality.DrawSetQuality);
        if (Button("Error log"))
            ConsoleWindow();
        //if (android)
        //_Loader.controls = (Contr)Toolbar((int)_Loader.controls, new[] { "mouse", "Keys", "Accelerometr" }, true);
        if (Button("Setup Keyboard"))
            win.ShowWindow(KeyboardSetup);

        if (_Menu && Button("Change Nick"))
        {
            _Menu.SetScreen(_Menu.EnterNick);
            win.CloseWindow();
        }
        if (isDebug && Button("Change car texture"))
        {
            var carPrefab = _Player || Application.isEditor ? _Player.m_Car.gameObject : photonPlayer.carSet.carPrefab;
            var componentInChildren = carPrefab.GetComponent<CarTextureChanger>();
            componentInChildren.ChangeCarTextureWindow();
        }

        if (Input.GetKey(KeyCode.LeftShift) || Application.isEditor || settings.m_isDebug)
            bs.settings.m_isDebug = gui.Toggle(bs.settings.m_isDebug, "Debug");
        if (!_Game)
        {
            if (Toggle(serverRegion == -1, "Auto select region"))
                serverRegion = -1;
            var rnames = Enum.GetNames(typeof(CloudServerRegion));
            var toolbar = Toolbar(serverRegion, rnames, title: "Region");
            if (serverRegion != toolbar)
                PhotonNetwork.Disconnect();
            serverRegion = toolbar;
        }

        curDict = Toolbar(curDict, bs.settings.translations.Select(a => a.name).ToArray(), true, false, 99, -1, true, "Language");
    }

    protected void KeyboardSetup()
    {
        win.SetupWindow(700, 600);
        //Label(Trs("keys:").PadRight(30) + "Player1,Player2");

        GUILayoutOption h = gui.Height(32);
        //GUIStyle ww = new GUIStyle(GUI.skin.label) {wordWrap = true};

        GUIStyle serverButton = _GuiSkins.keyboard;

        gui.BeginHorizontal();

        gui.BeginVertical();
        gui.Label("", serverButton, h);
        foreach (KeyValue a in inputManger.keys)
            gui.Label((Tr(a.descr) + ":"), serverButton, h);
        gui.EndVertical();

        for (int i = 0; i < 2; i++)
        {
            gui.BeginVertical();
            gui.Label("", serverButton, h);//"Player " + ((i % 2) + 1)
            foreach (KeyValue a in inputManger.keys)
                if (i < a.keyCodeAlt.Length)
                    if (gui.Button(a.keyCodeAlt[i] + "", serverButton, h))
                        FetchKey(a, i);
            //else
            //    gui.Button(" ", h);
            gui.EndVertical();
        }
        gui.EndHorizontal();

    }
    private void FetchKey(KeyValue a, int i)
    {
        StartCoroutine(AddMethod(() => Input2.anyKeyDown, delegate
        {
            a.keyCodeAlt[i] = FetchKey();
            a.Save();
        }));
    }

    private KeyCode FetchKey()
    {
        if (Input2.GetKeyDown(KeyCode.Escape))
            return KeyCode.None;
        for (int i = 0; i < 429; i++)
        {
            if (Input2.GetKeyDown((KeyCode)i))
                return (KeyCode)i;
        }
        return KeyCode.None;

    }
}