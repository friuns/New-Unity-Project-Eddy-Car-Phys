using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    public Canvas myCanvas;
    public void Start()
    {
        myCanvas = transform.GetComponentInParent<Canvas>();
    }
    public void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
        transform.position = myCanvas.transform.TransformPoint(pos);
    }
}