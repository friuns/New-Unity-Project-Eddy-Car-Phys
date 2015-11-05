using System;
using UnityEngine;

namespace doru
{


    public class Button : bs
    {
        public Vector3 defScale;
        public void Start()
        {
            defScale = tr.localScale;
        }
        public new bool enabled { get { return base.enabled; } set { base.enabled = renderer.enabled = value; } }
        public static Button clicked;
        public event Action click;
        public string text { get { return GetComponent<EasyFontTextMesh>().Text; } set { GetComponent<EasyFontTextMesh>().Text = value; } }
        public static int interFrame;
        public static bool down { get { return interFrame == Time.frameCount; } }
        public void Update()
        {
            if (win.enabled) return;

            var intersectRay = Intersects(renderer);
            tr.localScale = Vector3.Lerp(tr.localScale, intersectRay && !Input.GetMouseButton(0) ? defScale * 1.2f : defScale, Time.deltaTime * 5);
            if (intersectRay && Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (settings.showAllStats)
                    PhotonNetwork.player.stats.moneyFound += 1;
                clicked = this;
                if (click != null)
                {
                    print(click.Method.Name);
                    GuiClasses.PlayPushButton();
                    click();
                }
            }
        }
        public static bool Intersects(Renderer Renderer)
        {
            bool intersectRay = interFrame != Time.frameCount && Renderer.bounds.IntersectRay(CameraMain.ScreenPointToRay(Input.mousePosition));

            if (intersectRay)
                interFrame = Time.frameCount;
            return intersectRay;
        }
    }
}