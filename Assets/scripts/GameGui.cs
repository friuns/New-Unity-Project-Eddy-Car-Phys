using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
using gui = UnityEngine.GUILayout;
using UnityEngine;

public partial class GameGui : GuiClasses
{
    public void Awake()
    {
        OnValidate();
    }
    public void OnValidate()
    {
        _GameGui = this;
    }
    public void LabelCenter(string s, int size = 18)
    {
        gui.Label(Tr(s), GUI.skin.FindStyle("label"));
    }


    public void EnterPlayerNickName()
    {
        gui.Label("Enter name/Введите ник:");
        //_Loader.defSkin.textField.richText = false;
        //_Loader.defSkin.textField.wordWrap = false;
        bs._Loader.playerName = gui.TextField(bs._Loader.playerName, 12);
        if (Button("Ok") && bs._Loader.playerName.Length > 0)
        {
            PhotonNetwork.playerName = bs._Loader.playerName;
            Back();

        }
    }

    private GUILayoutOption h;
    private Player[] players;
    public void ScoreBoard()
    {
        SetupWindow(500, 700);

        gui.BeginHorizontal();
        gui.Label(bs.room.name);
        gui.FlexibleSpace();
        gui.Label(mapStats.mapName);
        gui.FlexibleSpace();
        if ((!mapStats.liked || isDebug) && Button("like"))
        {
            mapStats.liked = true;
            Download("scripts/scoreMap.php", null, null, false, "mapStats", JsonMapper.ToJson(mapStats));
        }
        gui.EndHorizontal();
        if (online && _Game.finnish && _Game.playerWin)
            gui.Box("***<size=30>" + _Game.playerWin.pv.playerName + " Best Score</size>***");
        if (GameType.teamGame && _Game.teamWin != null)
            gui.Box("***<size=30>" + _Game.teamWin + " Best</size>***");


        if (GameType.tdm)
        {
            gui.BeginHorizontal();
            Label(Trs("Red Team: ") + _Game.redTeam.score2);
            Label(Trs("Blue Team: ") + _Game.blueTeam.score2);
            gui.EndHorizontal();
        }

        players = _Game.GetPlayers().ToArray();
        gui.BeginHorizontal();
        {
            h = gui.Height(20);
            skin.label.alignment = TextAnchor.MiddleLeft;

            gui.BeginVertical();
            Label("");
            foreach (var a in players)
            {
                gui.BeginHorizontal(gui.ExpandWidth(false));
                if (gui.Button(a.pv.getText(), skin.label, h, gui.ExpandWidth(false)))
                    PlayerInfo(a);
                gui.Label(a.owner.dev ? res.dev : a.owner.isMasterClient ? res.host : null, skin.label, h, gui.ExpandWidth(false));
                gui.EndHorizontal();
            }
            gui.EndVertical();
            //GetValue(a => a.replay.getText(), "Name");                

            GetValue(a => a.pv.scoreInt.ToString(), "Score");

            if (!room.noKillScore && GameType.pursuitOrRace)
                GetValue(a => a.pv.kills.ToString(), "Kills");
            if (GameType.weapons)
                GetValue(a => a.pv.deaths.ToString(), "Deaths");

            //GetValue(a => a.pv.totalScore.ToString(), "Total Score");
            if (online)
                GetValue(a => ((int)(a.IsMine ? PhotonNetwork.GetPing() : a.ping * 1000)).ToString(), "ping");

            if (isDebug)
            {
                GetValue(a => a.pv.version.ToString(), "Ver");
                //GetValue(a => a.photonView.owner.url, "url");
            }

        }

        gui.EndHorizontal();
        if (ButtonLeft("Game Info"))
            ShowWindow(() => _Loader.RoomInfo(room));
    }


    private void GetValue(Func<Player, string> ac, string title)
    {
        {
            gui.BeginVertical();
            gui.Label(Tr(title), new GUIStyle(GUI.skin.label) { wordWrap = false });
            foreach (Player a in players)
                gui.Label(ac(a), h);
            gui.EndVertical();
        }
    }
    public void Menu()
    {
        win.SetupWindow(400, 600);
        if (Button("Administration"))
            _levelEditor.ShowAdmin();
        if (_Game.editControls)
        {
            win.style = GUIStyle.none;
            Label("Now Drag and Scale Icons, press back button when done");
            gui.BeginHorizontal();
            bool sm = false;
            if (Button("Bigger") || (sm = Button("Smaller")))
                foreach (var a in Input2.dict.Values)
                {
                    a.scale += sm ? -.2f : .2f;
                    a.UpdateScale();
                }
            gui.EndHorizontal();
            return;
        }

        if (android && Button("Edit Controls"))
            _Game.editControls = true;

        if (Button("Players"))
            _Game.ShowScoreBoard();
        //if (GameType.weapons && Button("Ally"))
        //win.ShowWindow(_Game.AllyWindow);

        if (Button("Settings"))
            ShowWindow(_Loader.SettingsWindow);

        if (Button(GameType.tdm ? "Change Team" : "Self Destruct"))
        {
            if (!_Player.dead)
            {
                _Player.AddLife(-1000);
                CloseWindow();
            }
            if (GameType.tdm)
                _Player.SetTeam((int)(_Player.teamEnum == TeamEnum.Blue ? TeamEnum.Red : TeamEnum.Blue));

        }
        if (Button("Quit"))
            _Game.LeaveRoom();
    }


    public void PlayerInfo(Player pl)
    {
        win.ShowWindow(delegate
        {
            win.windowTitle = pl.name;
            win.showVertical = true;
            gui.Label(pl.owner.ToString2());

            //TextEditor te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            //te.pos = 0; //set cursor position
            //te.selectPos = len; //se

            gui.BeginHorizontal();
            var haveFriend = _Loader.friends.Contains(pl.name);
            if (!haveFriend && Button("Add Friend"))
                _Loader.AddFriend(pl.name);
            if (haveFriend && Button("Remove Friend"))
                _Loader.RemoveFriend(pl.name);

            if (!Player.votedKick && Button("Vote Kick"))
                Player.VoteKick(pl);
            if (!reported && Button("Report"))
                SendReportWindow(pl);
            gui.EndHorizontal();
            gui.BeginHorizontal();
            pl.owner.mute = Toggle(pl.owner.mute, "Mute");
            if (GameType.weapons || isDebug)
            {
                var contains = _Game.allies.Contains(pl.viewId);
                var ally = gui.Toggle(contains, Tr("ally"));
                if (ally != contains && (!ally || _Game.allies.Count < 4))
                {

                    CallRPC(_Game.SetAlly, myViewId, pl.viewId, ally);
                    if (!ally && _Game.allyVisible.Contains(pl.viewId))
                        CallRPC(_Game.SetAlly, pl.viewId, myViewId, false);
                }
            }
            gui.EndHorizontal();

        });

    }
    private void SendReportWindow(Player a)
    {
        string reportText = "";

        ShowWindow(delegate
        {
            win.addflexibleSpace = false;
            Label("reason");
            reportText = gui.TextArea(reportText, gui.ExpandHeight(true));
            if (Button("report"))
            {
                SendReport(a, reportText);
                win.CloseWindow();
            }
        });
        reported = true;
    }
    private bool reported;
    public void SendReport(Player pl, string msg)
    {
        if (!string.IsNullOrEmpty(pl.owner.deviceId))
            Download("scripts/report.php", Debug.Log, Debug.LogWarning, false, new object[] { "ip", pl.owner.ip, "devid", pl.owner.deviceId, "plname", pl.owner.name, "version", settings.version, "msg", msg });
    }
}
