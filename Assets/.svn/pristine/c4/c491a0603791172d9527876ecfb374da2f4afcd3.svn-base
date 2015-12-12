using System;
using System.Collections;
using gui = UnityEngine.GUILayout;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using doru;
using ExitGames.Client.Photon;
using UnityEngine;
using Random = UnityEngine.Random;
//[Serializable]
//public class ShopItem
//{
//    public string text;
//    public string itemId;
//    public Texture2D texture;
//    public float price;
//    public bool have;
//}
public class CarShop : GuiClasses
{
    //public Texture2D paypal;
    public void Awake()
    {
        carShop = this;
    }
    public Button buy;
    public Button back;    
    //public List<ShopItem> items=  new List<ShopItem>();
    public void Start()
    {
        //if (!guiSkins.shopButton2.normal.background)
            //guiSkins.shopButton2 = new GUIStyle(guiSkins.roboGui.button);

        oldRot = mainTransform.rotation;
        back.click += back_click;

        buy.click += buy_click;
        web.LogEvent(EventGroup.Shop,"Start");
    }
 

    void buy_click()
    {
        StartCoroutine(PaypalBuy("Lamborghini Reventon", "car", deviceUniqueIdentifier, (isDebug ? "0" : "10")));
        web.LogEvent(EventGroup.Shop, "Buy");
    }

    void back_click()
    {
        Application.LoadLevel("1");
    }
    internal Quaternion oldRot;
    public Transform mainTransform;

    public void Update()
    {
        buy.enabled = !Paypal.haveCar;
        Vector3 mp = Vector3.one / 2;
        if (!android)
        {
            mp = Input.mousePosition;
            mp.x /= Screen.width;
            mp.y /= Screen.height;
        }

        mainTransform.rotation = Quaternion.Slerp(mainTransform.rotation, mainTransform.rotation * Quaternion.Euler((mp.y - .5f) * 0, (mp.x - .5f) * 7, 0), Time.deltaTime * 3);
        mainTransform.rotation = Quaternion.Slerp(mainTransform.rotation, oldRot, Time.deltaTime*.5f);
        //mainTransform.Rotate(0, (mp.x - .5f) * 7, 0);
    }

    public IEnumerator PaypalBuy(string text, string itemId, string trackId, string price)
    {        
        bool yes = false;
        yield return StartCoroutine(win.ShowWindow2(delegate
        {
            SetupWindow(300, 200);
            win.showBackButton = false;
            gui.Label("Buy " + text + " for " + price + "$?");            
            gui.BeginHorizontal();
            yes = gui.Button("Yes",gui.ExpandWidth(true));
            var no = gui.Button("No", gui.ExpandWidth(true));
            gui.EndHorizontal();
            if (yes || no) CloseWindow();
        }));
        if (!yes) yield break;

        string returnUrl = "http%3a%2f%2ftmrace%2enet%2f";

        var paypalUrl = ("https://www.paypal.com/cgi-bin/webscr?cmd=_xclick&business=LGZU9LEFK7L2A&lc=FI") +
                        "&item_name=" + Uri.EscapeUriString("TrackRacing Pursuit "+text) +
                        "&item_number=car&amount=" + price +
                        "%2e00&currency_code=USD&button_subtype=services&no_note=1&no_shipping=1&rm=1&return=" + returnUrl +
                        "&cancel_return=" + returnUrl +
                        "http%3a%2f%2ftmrace%2enet%2f&bn=PP%2dBuyNowBF%3abtn_buynowCC_LG%2egif%3aNonHosted&item_number=" + itemId +
                        "&custom=" + trackId;

        win.ShowWindow(delegate
        {
            SetupWindow(300, 300);
            win.showBackButton = false;
            //gui.Label(paypal);
            gui.Label("You will be redirected to paypal for payment, 1 day refund or bug reports contact soulkey4@gmail.com");
            if (gui.Button("Ok"))
            {
                web.LogEvent(EventGroup.Shop, "Redirect");
                Loader.OpenUrl(paypalUrl);
                ShowWindow(delegate
                {
                    Label("Please copy link bellow ctrl+c and paste ctrl+p, make order and restart game after you complete order");
                    GUI.SetNextControlName("l");
                    gui.TextArea(paypalUrl);
                    GUI.FocusControl("l");
                    SelectAll(paypalUrl.Length);
                    if (Button("Ok"))CloseWindow();
                });
            }
        });

        //Application.ExternalEval("window.open('" + paypalUrl + "', '_blank', 'width=1024,height=768,location=1,resizable=1,scrollbars=1,status=1', true);");
        
    }
    

}