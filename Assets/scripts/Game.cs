using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using gui = UnityEngine.GUILayout;
using Random = UnityEngine.Random;

public partial class Game : GuiClasses
{
    public Team[] teams = new Team[2];
    public Team redTeam { get { return teams[(int)TeamEnum.Red]; } }
    public Team blueTeam { get { return teams[(int)TeamEnum.Blue]; } }
    public List<CheckPoint> checkPoints = new List<CheckPoint>();
    internal Skidmarks skidmarks;
    internal ParticleEmitter m_skidSmoke;
    //public List<Item> bags = new List<Item>();
    //public List<Item> disabledBags = new List<Item>();
    public Bounds bounds;
    public void Awake()
    {
        print("Game Awake");
        InitValues();
        enabled = _Loader.menuLoaded;
        //if (!online || PhotonNetwork.room != null)
        //    OnJoinedRoom();
        bounds = GetLevelBounds();
        skidmarks = GetComponentInChildren<Skidmarks>();
        m_skidSmoke = skidmarks.GetComponentInChildren<ParticleEmitter>();

        if (!_Loader.menuLoaded)
        {
            hostRoom = new Room("", null);
            room.gameType = settings.gameType;
            _Loader.Connect();
        }
        Awake3();
    }

    public void OnValidate()
    {
    }
    private void InitValues()
    {
        _Game = this;
        _MainCamera = MainCamera;
    }

    internal List<Spawn> spawns = new List<Spawn>();
    internal List<Spawn> raceSpawns = new List<Spawn>();
    public Spawn[] GetSpawns()
    {
        return FindObjectsOfType<Spawn>();
    }


    public List<Player> botPlayers = new List<Player>();
    public void Start()
    {
        print("Game Start");
        InitSpawnsStart3();
        if (android)
        {
            room.collFix = false;
            //room.gravity = -9.81f;
        }
        Loader.errorCount = 0;
        SetPlayerName();

        PhotonNetwork.isMessageQueueRunning = true;
        for (int i = 0; i < 2; i++)
            teams[i] = new Team() { team = (TeamEnum)i };

        CreatePlayer();
        ResetCheckPoints();



        _Hud.centerText(room.gameType.ToString());
        //else
        //    _Hud.centerText("Escape cops as long as possible");
        _AutoQuality.OnLevelWasLoaded2();

        PhotonNetwork.player.url = _Loader.url;
        OnPhotonCustomRoomPropertiesChanged();
        fieldOfView = CameraMain.fieldOfView;
        if (isMaster)
        {
            SetGameState((int)GameState.none);
            SetTimeCount(room.matchTime);
        }

        if (GameType.bots)
            SpawnBots();
        if (_AutoQuality.autoFullScreen)
            _Loader.FullScreen();

        //PhotonNetwork.player.SetCustomProperties(PhotonNetwork.player.customProperties);
    }


    private void CreatePlayer()
    {
        _PlayerView = PhotonNetwork.Instantiate("PlayerView", Vector3.zero, Quaternion.identity, 0).GetComponent<PlayerView>();
        _Player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0).GetComponent<Player>();
        _Player.pv = _PlayerView;
        _PlayerView.playerName = _Loader.playerName;
        _PlayerView.pl = _Player;
        _PlayerView.version = settings.version;
        _Player.SetPvID();
        _Player.isAndroid = android;
        _PlayerView.SetPlID();
        if (GameType.teamGame)
            _Player.teamEnum = teams[0].count > teams[1].count ? teams[1].team : teams[0].team;
        _Player.InitNetwork();
        _PlayerView.InitNetwork();
    }

    public void ResetCheckPoints()
    {



    }

    internal GameState gameState = GameState.started;

    internal float spTime;


    internal bool easy { get { return dif == Dif.Easy; } }
    internal bool norm { get { return dif == Dif.Normal; } }
    internal bool hard { get { return dif == Dif.Hard; } }
    //public int bagsCollected;
    public Dif dif { get { return !online && spTime < 1 ? Dif.Easy : spTime < 6 ? Dif.Normal : Dif.Hard; } }

    public bool startedOrFinnished { get { return gameState != GameState.none; } }
    public bool started { get { return gameState == GameState.started; } }
    internal bool finnish { get { return gameState == GameState.finnish; } }
    internal bool none { get { return gameState == GameState.none; } }

    internal float stateChangeTime = MinValue;
    public void ResetGame() { }
    [RPC]
    public void SetGameState(int state)
    {
        stateChangeTime = Time.time;
        _Game.gameState = (GameState)state;
        CheckPoint.lastCheckPoint = null;
        print("SetGameState " + _Game.gameState);
        if (_Game.gameState == GameState.started)
            PlayOneShotGui(res.start);
        if (_Game.gameState == GameState.none)
            StartGame();
        if (_Game.gameState == GameState.finnish)
            WinGame();
        else
        {
            if (win.act == _GameGui.ScoreBoard)
                win.Back();
        }
    }
    private void StartGame()
    {
        ResetCheckPoints();
        Player.firstBlood = false;
        if (isMaster)
        {
            int i = 0;

            if (GameType.pursuit)
                foreach (var a in playersList.OrderBy(a => Random.value))
                    if (!a.bot2)
                    {
                        bool cop = GameType.teamGame ? i < playersList.Count / 2 : false; //_Loader.Race || _Loader.dm ? false : i < playersList.Count / 3 || i == 0;
                        a.CallRPC(a.SetTeam, (int)(cop ? TeamEnum.Blue : TeamEnum.Red));
                        //a.CallRPC(a.SetCar, Random.Range(0, res.Cars.Length));
                        a.CallRPC(a.Reset);
                        i++;
                    }
        }
        spTime = 0;
        if (!GameType.pursuit)
            ResetScore();
        if (GameType.weapons)
            PlayOneShotGui(res.prepare.Random());
    }
    private void WinGame()
    {
        SetTimeCount(0);

        StartCoroutine(AddMethod(2, delegate { if (finnish) ShowScoreBoard(); }));
        if (PhotonNetwork.playerList.Length > 1)
        {
            if (GameType.pursuit)
            {
                teamWin = redTeam.count > 0 ? redTeam : blueTeam;
                if (!_Player.dead && _Player.teamEnum == TeamEnum.Red)
                {
                    _Player.pv.AddScore(2);
                    _Player.stats.escape.value += _Game.blueTeam.count;
                    _Hud.centerText(string.Format("You escaped! +{0}$", _Player.stats.escape.points * _Game.blueTeam.count));
                }
            }
            else if (GameType.tdm)
            {
                teamWin = redTeam.score > blueTeam.score ? redTeam : blueTeam;
                PlayOneShot(redTeam.score > blueTeam.score ? res.victoryRedTeam : res.victoryBlueTeam);
            }
            else if (GameType.race || GameType.weapons)
                playerWin = GetPlayers().FirstOrDefault();

            if (_Game.listOfPlayers.Count > 1)
            {
                if (teamWin != null)
                {
                    photonPlayer.curGame.teamWon.value++;
                    _Hud.centerText(teamWin + " Win!\nPrice:" + _Hud.MoneyDif + "$");
                    if (isMaster)
                        CallRPC(SetTeamScore, (int)teamWin.team, teamWin.score + 1);
                }
                else if (playerWin != null)
                {
                    photonPlayer.curGame.wonRace.points += listOfPlayers.Count;
                    _Hud.centerText(playerWin.pv.playerName + " Win Race!\nPrice:" + _Hud.MoneyDif + "$");
                }

                if (playerWin == _Player)
                    PlayOneShotGui(res.victoryWin, 3);
            }
            PlayOneShot(res.endGameSound);
        }
    }

    internal Player playerWin;
    internal Team teamWin;

    public void ResetScore()
    {
        foreach (var a in playersList)
            a.pv.ResetScore();
    }
    public IOrderedEnumerable<Player> GetPlayers(bool cops = true, bool showBots = true)
    {
        var enumerable = _Game.playersList.Where(a => a.active);
        if (!cops)
            enumerable = enumerable.Where(a => !a.cop);
        if (!showBots)
            enumerable = enumerable.Where(a => !a.bot2);
        if (GameType.bots)
            return enumerable.OrderBy(a => a.cop).ThenByDescending(a => a.pv.score).ThenByDescending(a => a.distWent);
        return enumerable.OrderByDescending(a => a.pv.score).ThenByDescending(a => a.distWent);
    }
    float fieldOfView;
    public void Update()
    {

        CameraMain.fieldOfView = Mathf.Min(CameraMain.fieldOfView * 1.5f, Mathf.Lerp(CameraMain.fieldOfView, fieldOfView + Mathf.Max(0, _Player.avrVel) * .2f + Mathf.Max(0, _Player.velm - _Player.avrVel) * .5f, Time.deltaTime * 3));

        if (online)
        {
            //AudioListener.pause = Time.timeSinceLevelLoad < 2;
            if (listOfPlayers.Where(a => a != null).Count(a => !a.enabled) > 2 && Time.timeSinceLevelLoad > 5)
                Debug.LogException(new Exception("server sync error"));
        }

        if (Input2.GetKeyDown(KeyCode.F3))
            Player.VoteKick(Player.kickPlayer);




        if (KeyDebug(KeyCode.R, "Restart level"))
            CallRPC(SetTimeCount, 0f);

#if !UNITY_WP8 && VOICECHAT
        if (Input.GetKeyDown(KeyCode.Y) && !room.disableVoiceChat)
        {
            //_GameGui.CallRPC(_GameGui.Chat, _Loader.playerName + Tr(" voice chat"));
            print("Voice Chat: " + Application.HasUserAuthorization(UserAuthorization.Microphone));
            if (!VoiceChatRecorder.Instance.enabled)
                StartCoroutine(InitMicrophone());
        }
#endif

        //if (Input2.GetKeyDown(KeyCode.N))
        //ToggleWindow(AllyWindow);
        if (Loader.errorCount > 100 && !isDebug)
        {
            _Loader.ignoredRooms.Add(room.name);
            LeaveRoom();
        }
        UpdateBots();

        if (Input2.GetKeyUp(KeyCode.Return))
            _ChatGui.EnableChat();

        if (Input2.GetKeyDown(KeyCode.C))
            MainCamera.Next();
        if (Input2.GetKeyDown(KeyCode.Tab))
            ShowScoreBoard();
        if (Input2.GetKeyUp(KeyCode.Tab))
            CloseWindow();

        UpdateGameState();
        if (SecureInt.detected)
            ext.Block(" hack");

        SetSecure("life", (int)_Player.life);
        SetSecure("bullets", (int)_Player.curWeapon.bullets);
        SetSecure("bullets2", (int)_Player.machineGun.bullets);
        room.Update();
    }
    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        var hash = playerAndUpdatedProps[1] as Hashtable;
        foreach (var a in hash)
        {
            VarParse.OnValueRead((string)a.Key, a.Value);
        }
    }
    public void OnPhotonCustomRoomPropertiesChanged()
    {
        room.varParse.UpdateValues();
        Time.timeScale = room.gameSpeed;
        Time.fixedDeltaTime = 0.02f / Time.timeScale;
        Physics.gravity = new Vector3(0, room.gravity, 0);
    }
    [RPC]
    public void SetTimeCount(float time)
    {
        timeCountMatch = time;
    }
    [RPC]
    public void SetSpTime(float time)
    {
        spTime = time;
    }
    internal float timeCountMatch;
    public void ShowScoreBoard()
    {
        ToggleWindow(_GameGui.ScoreBoard);
    }

    public override void OnPlConnected()
    {
        if (GameType.pursuit)
        {
            CallRPC(SetTeamScore, 0, teams[0].score);
            CallRPC(SetTeamScore, 1, teams[1].score);
        }
        CallRPC(SetGameState, (int)gameState);
        CallRPC(SetTimeCount, timeCountMatch);
        CallRPC(SetSpTime, spTime);

        if (!string.IsNullOrEmpty(music))
            CallRPC(LoadMusic, music);
        base.OnPlConnected();
    }

    private static string music;
    [RPC]
    public void LoadMusic(string Obj)
    {

        music = Obj;
        _LoaderMusic.LoadMusic(Obj);
    }

    [RPC]
    internal void SetTeamScore(int id, int score)
    {
        Team team = teams[id];
        team.score = score;
    }

    public Dictionary<int, PlayerView> playerViews = new Dictionary<int, PlayerView>();
    public Dictionary<int, Player> players = new Dictionary<int, Player>();

    public List<Player> playersList = new List<Player>();
    public List<Player> listOfPlayers { get { return playersList; } }
    public IEnumerable<Player> listOfPlayersActive { get { return playersList.Where(a => a.active); } }
    public CameraControl MainCamera;

    public ParticleEmitter[] sparks;
    public ParticleEmitter[] sparks2;
    public ParticleEmitter[] concrete;

    //public ParticleSystem holes;
    public GameObject hole;
    public void Hole(Vector3 position, Vector3 vector3)
    {
        //holes.Emit(new ParticleSystem.Particle() { position = position, velocity = vector3,  axisOfRotation = vector3, rotation = Random.value, startLifetime = 10, lifetime = 10, size = 2, color = Color.white });
        //var g = (GameObject)Instantiate(hole, position, Quaternion.LookRotation(vector3) * Quaternion.Euler(0, 0, Random.Range(0, 360)));
        //g.hideFlags = HideFlags.HideInHierarchy;
        //Destroy(g, highQuality ? 100 : 20);
    }

    public void Emit(Vector3 point, Vector3 normal, ParticleEmitter[] ParticleEmitters)
    {
        for (int i = 0; i < ParticleEmitters.Length; i++)
        {
            ParticleEmitter b = ParticleEmitters[i];
            if (i == 0)
            {
                b.transform.position = point;
                b.transform.up = normal + Random.insideUnitSphere * .1f;
            }
            b.Emit();
        }
    }
    public void OnLeftRoom()
    {
        PhotonNetwork.player.curGame.timePlayed += (int)Time.timeSinceLevelLoad;
        if (_Loader.menuLoaded)
            Application.LoadLevel(0);
    }
    public void OnJoinedRoom()
    {
        enabled = true;
    }
    void OnReceivedRoomListUpdate()
    {

    }
    void OnJoinedLobby()
    {
        print("OnJoinedLobby");
        OnPhotonJoinRoomFailed();
    }
    private int tmpi;
    void OnPhotonJoinRoomFailed()
    {
        PhotonNetwork.JoinOrCreateRoom("room2" + tmpi, new RoomOptions() { maxPlayers = 10, customRoomProperties = room.customProperties }, TypedLobby.Default);
        tmpi++;
    }
    public void OnDestroy()
    {
        if (!_Loader.menuLoaded)
        {
            Debug.Log("Debug Disconnnect");
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();

        }
    }
}

