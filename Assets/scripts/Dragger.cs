using UnityEngine;

public class Dragger : bs
{
    public SpriteRenderer sprite;
    public void Update()
    {
        transform.localScale = Vector3.one * Mathf.Max(5, _levelEditor.camera.orthographicSize * settings.editor.draggerSize);
    }

}