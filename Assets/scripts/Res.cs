using System.Collections.Generic;
using UnityEngine;
public class Res : bs
{
    //public AudioClip noAmmo;
    public AudioClip endGameSound;
    public AudioClip takenLead;
    public AudioClip lostLead;
    public AudioClip victoryRedTeam;
    public AudioClip victoryBlueTeam;
    public AudioClip victoryWin;
    public AudioClip revenge;
    public AudioClip firstBlood;
    public AudioClip[] kill1;
    public AudioClip kill2;
    public AudioClip kill3;
    public AudioClip kill4;
    public AudioClip three;
    public AudioClip two;
    public AudioClip one;
    public AudioClip ownage;
    public AudioClip reload;
    //public AudioClip reloadEnd;
    //public AudioClip takeLead;
    //public AudioClip lostLead;
    //public AudioClip tieLead;
    public AudioClip[] prepare;

    public AudioClip[] item;
    public AudioClip[] pickup;
    public AudioClip checkPoint;
    public AudioClip beep;
    public AudioClip missileDetected;
    public AudioClip start;
    //public AudioClip[] checkpoint2;
    public AudioClip nitro;
    public AudioClip chat;
    public AudioClip[] hitSoundBig;
    public AudioClip[] hitSound;
    public AudioClip[] damageSound;
    public AudioClip hitFeedback;
    public AudioClip[] oskolok;
    public AudioClip[] ric;
    public AudioClip[] gearChange;
    public AudioClip[] pushButton;

    //public int maxBullets = 300;
    //public int bulletDecline = 50;
    ////public int bulletGrow = 10;
    //public int lifeGrow = 2;


    public GUISkin necroSkin;
    public float volumeFactor = .3f;
    public GameObject explosion;
    public Item itemPrefab;
    public Spawn spawnDM;
    public Dragger dragger;
    public List<ItemBase> tools;
    public TextAsset nickNames;
    public Texture2D host;
    public Texture2D dev;
    public void Awake()
    {
        OnValidate();
    }
    public void OnValidate()
    {
        res = this;
    }

    public override void OnEditorGui()
    {
        if (GUILayout.Button("init"))
            for (int i = 0; i < tools.Count; i++)
            {
                tools[i].index = i;
                tools[i].id = tools[i].name;
                tools[i].SetDirty();
            }
        base.OnEditorGui();
    }
}
