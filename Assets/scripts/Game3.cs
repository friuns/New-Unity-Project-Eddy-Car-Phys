using System;
using System.Collections;
using System.Linq;

using gui = UnityEngine.GUILayout;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public partial class Game
{
    public void Awake3()
    {

    }
    private void InitSpawnsStart3()
    {

        if ((isMaster || room.version < 164))
            _Administration.LoadMap(new MemoryStream(_Loader.mapBytes));

        Bounds bounds = this.bounds;
        Debug.DrawLine(bounds.min, bounds.max, Color.red, 10);
        //var dmSpawn = GetSpawnPrefab(TeamEnum.Dm);
        RefreshSpawns();

        //if (spawns.Count == 0 && isMaster)
        //    SpawnGen(bounds);
        if (GameType.pursuitOrRace)
            checkPoints = FindObjectsOfType<CheckPoint>().OrderBy(a => a.name).ToList();
        if (items == null || items.Count == 0)
            items = FindObjectsOfType<Item>().OrderBy(a => Random.value).ToList();

        if (isMaster && (GameType.weapons || isDebug))
        {
            var c = 0;
            for (int i = 1; i <= 4; i++)
                for (int j = 0; j < 7; j++)
                    if (c < items.Count)
                    {
                        var a = items[c];
                        a.CallRPC(a.SetItem, i);
                        c++;
                    }
            for (; c < items.Count; c++)
                items[c].CallRPC(items[c].SetItem, 0);
        }
        print("Spawns " + spawns.Count);
    }
    public void RefreshSpawns()
    {
        spawns = GetSpawns().ToList();
        raceSpawns = spawns.Where(a => a.team == TeamEnum.Race).ToList();
    }
    //private void SpawnGen(Bounds bounds)
    //{
    //    //Random.seed = 1;
    //    for (int i = 0, j = 0; i < 100 && j < 50; i++)
    //    {
    //        var sp = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
    //            bounds.max.y,
    //            Random.Range(bounds.min.z, bounds.max.z));
    //        RaycastHit h;
    //        Debug.DrawRay(sp, Vector3.down, Color.white, 10);


    //        if (Physics.Raycast(new Ray(sp, Vector3.down), out h, bounds.size.y, Layer.levelMask))
    //            if (Vector3.Dot(h.normal, Vector3.up) > .7f)
    //            {
    //                Transform spawn;
    //                if (j > 20)
    //                {
    //                    spawn = PhotonNetwork.InstantiateSceneObject(res.itemPrefab.fullName, Vector3.zero, Quaternion.identity, 0, null).transform;
    //                    //spawn = ((Item)Instantiate(res.itemPrefab)).tr;
    //                }
    //                else
    //                {
    //                    //spawn = new GameObject("spawn").transform;
    //                    spawn = PhotonNetwork.InstantiateSceneObject(res.spawnDM.fullName, Vector3.zero, Quaternion.identity, 0, null).transform;
    //                    spawns.Add(spawn.GetComponent<Spawn>());
    //                }

    //                spawn.position = h.point + Vector3.up;

    //                spawn.forward = ZeroY(bounds.center - spawn.position);
    //                j++;
    //            }
    //    }

    //    raceSpawns = spawns;
    //}
    //public Spawn GetSpawnPrefab(TeamEnum team)
    //{
    //    return (Spawn)res.tools.FirstOrDefault(a => a is Spawn && (a as Spawn).team == team);

    //}
    public static Bounds GetLevelBounds()
    {
        var g = GameObject.FindGameObjectWithTag("bounds");
        if (g && g.renderer)
            return g.renderer.bounds;
        Bounds b = new Bounds();
        var bs = settings.removeObjects;
        var colliders = FindObjectsOfType<Collider>();

        foreach (var a in colliders)
        {
            b.Encapsulate(a.bounds);
            if (bs.Contains(a.name.ToLower()))
                Destroy(a.gameObject);
        }

        return b;
    }




}