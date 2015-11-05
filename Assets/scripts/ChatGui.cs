using System.IO;
using gui = UnityEngine.GUILayout;
using UnityEngine;







public class ChatGui : GuiClasses
{
    public void Awake()
    {
        OnValidate();
    }
    public void OnValidate()
    {
        _ChatGui = this;
    }
    private string chatInput = "";
    public void OnGUI()
    {
        var minMaxRect = Rect.MinMaxRect(.003f * Screen.width, .13f * Screen.height, Screen.width, Screen.height);

        GUILayout.BeginArea(minMaxRect);
        //GUI.skin.label.wordWrap = false;
        //GUI.skin.label.fontSize = 20;

        GUI.SetNextControlName("Chat");
        chatInput = gui.TextField(chatInput);
        GUI.FocusControl("Chat");
        //print(Event.current.keyCode);

        if (Event.current.keyCode == KeyCode.Return && chatInput.Length > 0)
        {
            enabled = false;
            if (chatInput.Length > 0)
            {
                CallRPC(Chat, _Player.pv.playerName + ": " + chatInput);
            }
            chatInput = "";
        }
        else if (Event.current.keyCode == KeyCode.Return && Event.current.isKey)
            enabled = false;
        gui.EndArea();
    }
    public new bool enabled { get { return base.enabled; } set { base.enabled = value; } }
    bool firstTimeChat;
    public Chat chatOutput = new Chat();
    [RPC]
    public void Chat(string s)
    {
        var msg = NetworkingPeer.photonMessageInfo;
        if (msg !=null && msg.sender.mute) return;
        if (!firstTimeChat)
        {
            firstTimeChat = true;
            _Hud.centerText(Tr("press enter to reply"));
        }
        if (!chatOutput.text.EndsWith(s + "\r\n"))
            chatOutput.chat(s);

        _Game.audio.PlayOneShot(res.chat);
    }
    public void EnableChat()
    {
        if (android)
        {
            print("Adnroid Chat");
            var t = TouchScreenKeyboard.Open("");
            StartCoroutine(AddMethod(() => t.done || t.wasCanceled, delegate
            {
                if (t.done && t.text.Length > 0)
                    CallRPC(Chat, _Player.pv.playerName + ": " + t.text);
            }));
        }
        else
            enabled = true;
    }
}