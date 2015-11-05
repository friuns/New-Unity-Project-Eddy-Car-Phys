using UnityEngine;

public class ItemBase : bsNetwork
{
    public GameTypeEnum[] gameType;
    public int index;
    public string id;
    public string fullName { get { return "tools/" + id; } }
    public static GameObject itemsPlaceHolder;
    public virtual void Start()
    {
        if (!itemsPlaceHolder)
            itemsPlaceHolder = new GameObject("items");
        transform.parent = itemsPlaceHolder.transform;
    }
}