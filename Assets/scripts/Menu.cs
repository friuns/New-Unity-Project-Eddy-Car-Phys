using System;
using System.Linq;
using System.Security.Policy;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using Button = doru.Button;
using InputField = doru.InputField;
using Random = UnityEngine.Random;


public partial class Menu : GuiClasses
{

    public Button singlePlayer;
    public Button multiplayer;
    public Button browseServers;
    public Button EnterNickOk;
    public Button joinRace;
    public Button joinDeatmatch;
    public Button hostGame;
    public Button joinPursuit;
    public Button joinTDM;
    public Button getOnAndroid;
    public Button friends;
    public Button shop;
    public new Button settings;
    public InputField enterNickField;
    public Transform mainTransform;
    public Transform mainMenu;
    public Transform EnterNick;
    public Transform multiplayerScene;
    public Transform current;
    public EasyFontTextMesh version;
    public EasyFontTextMesh playerCount;
    public GameObject androdOnly;
    public Camera mainCam;
    public GameObject ads;
    public void Awake()
    {
        _LoaderGui = _Menu = this;
        foreach (var a in Camera.allCameras)
            if (mainCam != a && a.gameObject.tag == "MainCamera")
                a.gameObject.SetActive(false);
    }




    public void Start()
    {
        _Loader.RefreshAds();
        _Loader.menuLoaded = true;
        //PhotonNetwork.offlineMode = false;
        _Loader.Connect();
        enterNickField.Text = bs._Loader.playerName;


        if (string.IsNullOrEmpty(bs._Loader.playerName))
            SetScreen(EnterNick);
        else
            SetScreen(mainMenu);
        //PhotonNetwork.offlineMode = false;
        //PhotonNetwork.Disconnect();

        version.Text = "version:" + bs.settings.version + " " + bs.settings.versionDate;
        if (androdOnly)
            androdOnly.SetActive(android && !bs.settings.release);
        if (android)
            foreach (var a in GameObject.FindGameObjectsWithTag(Tag.PCOnly))
                a.SetActive(false);
        singlePlayer.click += singlePlayer_click;
        multiplayer.click += multiplayer_click;
        browseServers.click += browseServers_click;
        EnterNickOk.click += EnterNickOk_click;
        enterNickField.a += EnterNickOk_click;
        joinDeatmatch.click += join_click;
        joinPursuit.click += join_click;
        joinRace.click += join_click;
        settings.click += settings_click;
        getOnAndroid.click += getOnAndroid_click;
        friends.click += friends_click;
        hostGame.click += hostGame_click;
        shop.click += shop_click;
        StartCoroutine(AddMethod(() => Loader.settingsLoaded, delegate
        {
            var linksButtons = bs.settings.serv.curDev.linksButtons;
            if (linksButtons.Length == 0) return;
            var d = Random.Range(0, linksButtons.Length / 2) * 2;
            getOnAndroid.text = linksButtons[d];
            buttonLink = linksButtons[d + 1];
        }));
        money.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => win.ShowWindow(() => PhotonNetwork.player.DrawStats()));
        Start2();
    }


    void shop_click()
    {
        Application.LoadLevel("!carshop");
        win.ShowPopup("Comming soon");
    }

    //private string buttonText;
    private string buttonLink;

    void getOnAndroid_click()
    {
        var url = buttonLink;
        win.ShowWindow(() => GuiClasses.TextField("Copy Link:", url));
        Loader.OpenUrl(url);
    }

    void friends_click()
    {
        win.ShowWindow(bs._Loader.FriendsWindow);
    }




    void settings_click()
    {
        win.ShowWindow(bs._Loader.SettingsWindow);
    }

    void hostGame_click()
    {
        _LoaderGui.ShowHostRoomWindow();
    }

    private void FindRoom() { }
    private void JoinRoom() { }
    private void JoinButton() { }
    private RoomInfo GetRoom(GameTypeEnum GameType)
    {
        var rooms = _Loader.GetRoomList();
        var orderByDescending = rooms.Where(a => !a.privateRoom && ValidateRoom(a) && a.playerCount < 8 && (a.version == bs.settings.mpVersion)).OrderByDescending(a => a.playerCount);
        if (android && !Application.isEditor)
            orderByDescending = orderByDescending.OrderBy(a => a.maxPlayers);
        RoomInfo firstOrDefault = orderByDescending.FirstOrDefault(a => a.gameType == (GameType));

        return firstOrDefault;
    }

    void EnterNickOk_click()
    {
        if (enterNickField.Check())
        {
            bs._Loader.playerName = enterNickField.Text;
            SetScreen(mainMenu);
        }
        else
        {
            _LoaderGui.ShowPopup("Write your nick name");
        }
    }

    void browseServers_click()
    {
        win.ShowWindow(_LoaderGui.ServerList);
    }

    void multiplayer_click()
    {
        _Loader.Connect();
        SetScreen(multiplayerScene);
    }
    public void OnJoinedLobby()
    {
        bs._Loader.RefreshFriends();
    }
    public void OnConnectedToPhoton()
    {

    }


    public void OnFailedToConnectToPhoton()
    {
        OnConnectionFailed();
    }

    internal void OnConnectionFailed()
    {
        Debug.LogWarning("OnFailedToConnectToPhoton");
        //PhotonNetwork.Disconnect();
        StartCoroutine(AddMethod(() => PhotonNetwork.connectionState == ConnectionState.Disconnected, delegate
        {
            bs._Loader.appId++;
            bs._Loader.Connect2();
        }));
    }

    void singlePlayer_click()
    {
        if (!PhotonNetwork.insideLobby)
            _Loader.Connect(true);
        //hostRoom.gameType = GameTypeEnum.DeathMatch;
        ShowHostRoomWindow();
    }


    void join_click()
    {
        var r = GetRoom(doru.Button.clicked == joinRace ? GameTypeEnum.pursuitRace : doru.Button.clicked == joinPursuit ? GameTypeEnum.pursuit : doru.Button.clicked == joinTDM ? GameTypeEnum.TDM : GameTypeEnum.DeathMatch);
        //r.varParse.UpdateValues();
        hostRoom = r;
        _Loader.StartCoroutine(_Loader.LoadLevel(false));
    }


    public Text money;

    public void Update()
    {
        money.text = _Loader.GetMoneyText(_Loader.money);
        shop.enabled = !bs._Loader.vkSite && !bs._Loader.kongregate && bs._Loader.daysElapsed > 0;

        getOnAndroid.enabled = buttonLink != null;
        //if (Input.GetKeyDown(KeyCode.Tab))
        //current = current == mainMenu ? EnterNick : mainMenu;
        singlePlayer.enabled = !android;

        if (Input.GetKeyDown(KeyCode.Escape) && current == mainMenu && !win.enabled)
            Application.Quit();
        if (multiplayerScene == current)
        {
            joinPursuit.enabled = (GetRoom(GameTypeEnum.pursuit) != null);
            joinTDM.enabled = (GetRoom(GameTypeEnum.TDM) != null);
            joinRace.enabled = (GetRoom(GameTypeEnum.pursuitRace) != null);
            joinDeatmatch.enabled = (GetRoom(GameTypeEnum.DeathMatch) != null);
            friends.GetComponent<EasyFontTextMesh>().Text = Tr("Friends Online: ") + bs._Loader.Friends.Count(a => a.IsOnline) + "/" + bs._Loader.Friends.Count;
        }
        if (PhotonNetwork.connected)
            playerCount.Text = "Players:" + (int)(PhotonNetwork.countOfPlayers * (Application.isEditor ? 1 : 2.567f)) + " Rooms:" + (int)((PhotonNetwork.countOfRooms * (Application.isEditor ? 1 : 2.567f)));

        int v = 3;
        mainTransform.position = Vector3.Lerp(mainTransform.position, current.position, Time.deltaTime * v);


        Vector3 mp = Vector3.one / 2;
        if (!android)
        {
            mp = Input.mousePosition;
            mp.x /= Screen.width;
            mp.y /= Screen.height;
        }
        mainTransform.rotation = Quaternion.Slerp(mainTransform.rotation, current.rotation * Quaternion.Euler((mp.y - .5f) * 7, (mp.x - .5f) * 7, 0), Time.deltaTime * v);
        if (current != EnterNick && Input.GetKeyDown(KeyCode.Escape))
        {
            win.CloseWindow();
            SetScreen(mainMenu);
        }


    }
    public void SetScreen(Transform screen)
    {
        print(screen.name);
        if (screen == mainMenu)
        {
            PhotonNetwork.playerName = bs._Loader.playerName;
            //PhotonNetwork.offlineMode = false;
            //if (PhotonNetwork.connected)
            //    PhotonNetwork.Disconnect();
        }
        current = screen;
    }
}

