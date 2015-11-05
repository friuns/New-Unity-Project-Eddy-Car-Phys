using UnityEngine;


public class Spawn : ItemBase
{

    //public void Start()
    //{
    //    if (type == SpawnTool.StartPos && _Game)
    //    {
    //        rot = Quaternion.LookRotation(ZeroY(_Game.bounds.center - pos));
    //    }
    //}
    public TeamEnum team = TeamEnum.Dm;
    public void OnDrawGizmos()
    {
        Gizmos.DrawIcon(tr.position, "spawn.psd", true);
        Gizmos.DrawSphere(tr.position, 1);
    }
}