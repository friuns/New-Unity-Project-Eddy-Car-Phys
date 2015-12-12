using System;

[Serializable]
public class ServSettings
{
    public static string redirectUrl = "http://tmrace.net";
    public static string redirectText = "New game version available! Please run game from {0}";
    public string[] blocksites = new string[0];
    public string[] appIds = { "35d8975d-c46f-4f89-a5f2-d7f068d79079" };
    public string warntext;
    public Device android = new Device();
    public Device pc = new Device();
    public string jsUrl = "/web2/runJsUnity.js";
    public string music = "http://tmrace.net/cops/cops.mp3";
    public string[] warnSites;
    public string[] ownSites = { "tmrace.net" };
    internal Device curDev { get { return bs.android ? android : pc; } }
    public class Device
    {
        public int minVer;
        internal string[] linksButtons = { "Download for android", "http://tmrace.net/tm/mobile" };
    }
}