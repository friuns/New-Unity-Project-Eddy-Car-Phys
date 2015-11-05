using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using gui = UnityEngine.GUILayout;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public partial class Game
{
    private void SpawnBots()
    {
        //if(isMaster)
        var len = (isDebug ? 1 : 6);
        for (int i = 0; i < len; i++)
        {
            var pv = PhotonNetwork.Instantiate("PlayerView", Vector3.zero, Quaternion.identity, 0).GetComponent<PlayerView>();
            var pl = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0).GetComponent<Player>();
            pl.pv = pv;
            pv.pl = pl;
            pl.SetPvID();
            pv.SetPlID();

            pl.bot2 = true;
            //pl.teamEnum = TeamEnum.Red;
            pl.teamEnum = i < len / 2 ? TeamEnum.Blue : TeamEnum.Red;
            botPlayers.Add(pl);
            pv.playerName = "Bot " + i;
            pl.InitNetwork();
            pv.InitNetwork();
        }
    }
    private void UpdateBots()
    {
        //if (survival)
        //{
        //    for (int i = 0; i < botPlayers.Count; i++)
        //    {
        //        var b = i <= (Mathf.Min(spTime, 15f / Mathf.Max(1, PhotonNetwork.playerList.Length))) && !_Player.cop;
        //        if (botPlayers[i].active != b)
        //            botPlayers[i].CallRPC(botPlayers[i].SetActive, b);
        //    }
        //}
    }
    public override void OnSceneGui(SceneView sc)
    {
#if UNITY_EDITOR
        bool spa = Event.current.keyCode == KeyCode.F1;
        bool bag = Event.current.keyCode == KeyCode.F2;
        if (Event.current.type == EventType.keyDown && (spa || bag))
        {
            RaycastHit h;
            var spw = GameObject.Find("dmSpawns");
            if (spw == null)
                spw = new GameObject("dmSpawns");

            Vector2 mousePosition = Event.current.mousePosition;
            mousePosition.y = Screen.height - mousePosition.y - 40;

            if (Physics.Raycast(sc.camera.ScreenPointToRay(mousePosition), out h))
            {
                GameObject a;
                if (bag)
                    a = (GameObject)PrefabUtility.InstantiatePrefab(resEditor.bag);
                else
                    a = new GameObject("Spawn");
                Undo.RegisterCreatedObjectUndo(a, "a");
                a.transform.position = h.point + Vector3.up * 1;
                a.transform.parent = spw.transform;
                if (spa)
                    a.AddComponent<Spawn>();
            }
        }
#endif
    }


}

