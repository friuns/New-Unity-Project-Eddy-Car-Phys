using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using gui = UnityEngine.GUILayout;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;
class LoaderGUI { }
public partial class Menu
{
    public void Start2()
    {
        if (_Loader.mapWWW != null)
        {
            print("Unloading assets");
            if (_Loader.mapWWW.assetBundle != null)
                _Loader.mapWWW.assetBundle.Unload(false);
            _Loader.mapWWW.Dispose();
            _Loader.mapWWW = null;
        }
        //if (previousRoom == null && isDebug)
        //    previousRoom = new RoomInfo("", null);

        if (photonPlayer.curGame.timePlayed > 0)
        {
            var g = photonPlayer.curGame;
            win.ShowWindow(delegate
            {
                win.windowTitle = previousRoom.gameType.ToString();
                win.showBackButton = false;
                g.DrawStats();
                if (GuiClasses.Button("Claim"))
                {
                    photonPlayer.stats.games[(int)previousRoom.gameType].Add(g);
                    GuiClasses.CloseWindow();
                }
            });
            photonPlayer.stats.curGame = new Gts();
        }

        //if (string.IsNullOrEmpty(playerName))
        //    ShowWindow(EnterPlayerNickName);        
    }

    public string search = "";
    private void ServerList() { }
    private const string _nameGametypePlayers = "Name                        GameType       Players V";
    public static string serverTable = CreateTable(_nameGametypePlayers);
    public void SelectServer()
    {
        SetupWindow(600, 600);

        IOrderedEnumerable<RoomInfo> rooms = _Loader.GetRoomList().OrderByDescending(a => a.version).ThenBy(a => a.playerCount == a.maxPlayers);
        rooms = rooms.ThenByDescending(a => a.playerCount);
        search = TextField("Search:", search);
        Label(_nameGametypePlayers);
        foreach (RoomInfo a in rooms)
        {
            if (!string.IsNullOrEmpty(search) && !a.name.ToLower().Contains(search.ToLower())) continue;
            if (string.IsNullOrEmpty(search) && a.privateRoom) continue;

            GUI.enabled = ValidateRoom(a);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(serverTable, a.name, a.gameType, a.playerCount + "/" + a.maxPlayers, a.version != bs.settings.mpVersion ? a.version.ToString() : "");
            //if (a.version != bs.settings.mpVersion)
            //    sb.Append("             (version:" + a.version + ")");
            if (gui.Button(sb.ToString(), skin.label))
            {
                hostRoom = a;
                ShowWindow(() =>
                {
                    _Loader.RoomInfo(room);
                    if (Button("Join"))
                        _Loader.StartCoroutine(_Loader.LoadLevel(false));
                });
            }
            GUI.enabled = true;
        }
    }



    public static bool ValidateRoom(RoomInfo a)
    {
        return a.version <= bs.settings.mpVersion;
    }
    public void HostGame()
    {
        //_Loader.ResetSettings();
        hostRoom = new RoomInfo(bs._Loader.playerName + "'s room", null);
        ShowWindow(HostGameWindow);
    }
    public void HostGameWindow()
    {
        SetupWindow(800, 600);
        gui.BeginHorizontal();
        gui.BeginVertical();
        bool startButton = Button("Start");

        if (Button("Load Map from url"))
            ShowWindow(LoadMapFromUrl);

        Label("Room Name:");
        room.name = gui.TextField(room.name, 20);
        room.privateRoom = Toggle(room.privateRoom, "Private Room");

        GameTypeEnum gameType = (GameTypeEnum)Toolbar((int)room.gameType, new[] { "StreetRace", "Pursuit", "Deathmatch", "TDM" }, hor: 2, title: "Game Type");
        if (gameType != room.gameType)
        {
            room.gameType = gameType;
            room.matchTime = 3 * 60;
        }
        room.maxPlayers2 = (int)HorizontalSlider("Max Players", room.maxPlayers2, 1, 30, startButton);
        gui.EndVertical();
        gui.BeginVertical(skin.box);
        Label(Tr("Select Map"));
        foreach (var a in _Loader.maps)
            if (GlowButton(a.mapName, a == room.sets.mapStats))
                room.sets.mapStats = a;
        gui.EndVertical();
        gui.EndHorizontal();

        if (startButton)
            _Loader.StartCoroutine(_Loader.LoadLevel(true));
    }


    private void LoadMapFromUrl()
    {
        Label("Enter url where unity3d map stored");
        mapUrlTextField = TextField("map url", mapUrlTextField);
        if (Button("Load"))
        {
            room.sets.mapStats = new MapStat { mapUrl = mapUrlTextField, mapName = Path.GetFileNameWithoutExtension(mapUrlTextField), newMap = true };
            _Loader.StartCoroutine(_Loader.LoadLevel(true));
        }

    }
    public void LoadingWindow()
    {
        //win.save = false;
        Label(_Loader.mapName + " Loading " + Mathf.Round(_Loader.mapWWW.progress * 100) + "%");
    }
    private string mapUrlTextField = "";
}


//public void EnterPlayerNickName()
//{
//    gui.Label("Enter name/Введите ваш ник:");
//    //_Loader.defSkin.textField.richText = false;
//    //_Loader.defSkin.textField.wordWrap = false;
//    _Loader.playerName = gui.TextField(_Loader.playerName, 12);
//    if (gui.Button("Ok") && _Loader.playerName.Length > 0)
//    {
//        PhotonNetwork.playerName = _Loader.playerName;
//        ShowWindow(Menu);
//    }
//}
//public void FixNick()
//{
//    playerName = "guest" + Random.Range(0, 999);
//}
//public void OnJoinedRoom()
//{
//    print("OnJoinedRoom");
//    FixNick();
//    PhotonNetwork.player.name = playerName;
//    PhotonNetwork.isMessageQueueRunning = false;
//    Application.LoadLevel(1);
//}
//private string playerName { get { return PlayerPrefs.GetString("name"); } set { PlayerPrefs.SetString("name", value); } }
//public void Menu()
//{
//    if (gui.Button("SinglePlayer"))
//    {
//        PhotonNetwork.offlineMode = true;
//        Application.LoadLevel(1);
//    }
//    if (gui.Button("MultiPlayer"))
//    {            
//        ShowWindow(SelectServer);
//    }
//}