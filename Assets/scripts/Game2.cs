using System;
using System.Linq;

using gui = UnityEngine.GUILayout;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public partial class Game
{
    private List<Item> items = new List<Item>();
    public List<Item> disabledItems = new List<Item>();

    private static void SetPlayerName()
    {
        PhotonNetwork.playerName = (PhotonNetwork.isMasterClient ? "Host" : "Client") + PhotonNetwork.player.ID;

        if (!isDebug || bs._Loader.menuLoaded)
            if (string.IsNullOrEmpty(bs._Loader.playerName))
                win.ShowWindow(_GameGui.EnterPlayerNickName);
            else
                PhotonNetwork.playerName = bs._Loader.playerName;
    }


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnEditorGui()
    {
#if UNITY_EDITOR
        gui.Label("Game");
        if (gui.Button("Delete All"))
            PlayerPrefs.DeleteAll();
        if (gui.Button("init bounds"))
            bounds = GetLevelBounds();
        //if (gui.Button("put spawns"))
        //{
        //    //EditorGUIUtility.mouse
        //    var b = GetLevelBounds();
        //    b.Expand(-b.size / 2);
        //    var t = new GameObject("dmSpawns").transform;

        //    for (int i = 0, j = 0; i < 100 && j < 20; i++)
        //    {
        //        var p = b.min + new Vector3(Random.Range(0, b.size.x), b.size.y, Random.Range(0, b.size.z));
        //        RaycastHit h;
        //        if (Physics.Raycast(p, Vector3.down, out h, 1000, Layer.levelMask))
        //        {
        //            if (h.collider.gameObject.layer != Layer.dontSpawn)
        //            {
        //                Debug.DrawRay(p, Vector3.down * 1000, Color.red, 10);
        //                var g = new GameObject("dmSpawn");
        //                g.transform.position = h.point + Vector3.up * 4;
        //                g.transform.rotation = Quaternion.LookRotation(ZeroY(Random.insideUnitSphere));
        //                g.transform.parent = t;
        //                j++;
        //                g.AddComponent<Spawn>();
        //            }
        //        }
        //    }
        //}

#endif
    }

    private void UpdateGameState()
    {
        if (bs.settings.offline && isDebug)
            return;
        if (isMaster)
        {
            
            if (GameType.pursuitOrRace && _Game.started && Time.time - stateChangeTime > 5 && _Game.playersList.Count > 1 && redTeam.players.All(a => a.dead))
                CallRPC(SetTimeCount, 0f);

            GameState gameState = timeCountMatch <= 0 ? GameState.finnish : timeCountMatch > bs.room.matchTime ? GameState.none : GameState.started;
            if (gameState != _Game.gameState)
                CallRPC(SetGameState, (int)gameState);

            if (timeCountMatch < (isDebug ? -1 : -5))
                CallRPC(SetTimeCount, bs.room.matchTime + (isDebug ? 1 : 3));
        }
        timeCountMatch -= Time.deltaTime / Time.timeScale;

        //if (survival)
        spTime += Time.deltaTime / 60;
    }
    public bool editControls;

    public HashSet<int> allies = new HashSet<int>();
    public HashSet<int> allyVisible = new HashSet<int>();
    //public void AllyWindow()
    //{
    //    win.SetupWindow(400, 700);
    //    win.showBackButton = false;
    //    Label("If you ally with player, player will see where you are");
    //    foreach (Player a in listOfPlayers.Where(a => a != _Player))
    //    {
    //        var contains = allies.Contains(a.playerId);
    //        var ally = gui.Toggle(contains, Tr("ally with ") + a.playerName);
    //        if (ally != contains && (!ally || allies.Count < 4))
    //        {

    //            CallRPC(SetAlly, myId, a.playerId, ally);
    //            if (!ally && allyVisible.Contains(a.playerId))
    //                CallRPC(SetAlly, a.playerId, myId, false);
    //        }
    //    }
    //    if (BackButton())
    //        win.Back();
    //}
    [RPC]
    public void SetAlly(int from, int to, bool b)
    {
        if (from == myViewId)
            if (b)
                allies.Add(to);
            else
                allies.Remove(to);

        Player pl = _Game.players[@from];
        if (to == myViewId)
        {
            if (b)
            {
                allyVisible.Add(from);
                _Hud.centerText(string.Format(Tr("To ally with {0} press b"), pl.playerName));
                pl.pv.ally = true;
                pl.RefreshText();
            }
            else
            {
                allyVisible.Remove(from);
                pl.pv.ally = false;
                pl.RefreshText();
            }
        }
        _ChatGui.Chat(pl.pv.playerName + Tr(b ? " Allied with " : " UnAllied with ") + _Game.players[to].pv.playerName);
    }




}