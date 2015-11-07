using gui = UnityEngine.GUILayout;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public partial class Player : bsNetwork
{

    public float ping;

    internal float avrVel;
    internal bool bot { get { return bot2 && IsMine; } }
    internal bool bot2;
    public bool cop { get { return GameType.pursuitOrRace && pv.teamEnum == TeamEnum.Blue; } }
    public TeamEnum teamEnum { get { return pv.teamEnum; } set { pv.teamEnum = value; } }
    public PlayerView pv;//{ get { return _Game.playerViews[viewId]; } }
    public Transform fire;
    public Transform smoke;
    public Renderer lodRenderer;
    public new Vector3 pos { get { return transform.position; } set { transform.position = value; } }
    public Vector3 posUp { get { return transform.position + transform.up; } }
    public new Quaternion rot { get { return transform.rotation; } set { transform.rotation = value; } }
    public ParticleEmitter[] nitroParticles;
    private AudioSource nitroAudio;
    public Team team { get { return _Game.teams[(int)pv.teamEnum]; } }
    public string playerName { get { return pv.playerName; } }
    //public override void OnEditorGui()
    //{
    //    if (gui.Button("AddKills"))
    //        pv.AddKills(1);
    //    if (gui.Button("AddDeaths"))
    //        pv.AddDeaths(1);
    //    base.OnEditorGui();
    //}
    public override void OnEditorGui()
    {
        owner.varParse.UpdateValues();
        base.OnEditorGui();
    }
    public void OnValidate()
    {

        if (!Application.isPlaying || !gameObject.activeInHierarchy) return;
        if (!_Player && !bot2)
            _Player = this;
        _Game.players[viewId] = this;
    }
    public void OnDestroy()
    {
        if (!this || !Application.isPlaying) return;
        _Game.playersList.Remove(this);
        if (m_Car != null)
            _Pool.Save(m_Car.gameObject);
        if (playerNameTxt != null)
            Destroy(playerNameTxt.gameObject);
    }
    public Transform weapons;
    public void Awake()
    {
        if (!GameType.weapons)
            weapons.gameObject.SetActive(false);
        enabled = false;
        _Game.playersList.Add(this);
        OnValidate();
    }
    void Start()
    {
        if (_Player == this)
            print("Player Start");
        name = "Player " + ownerName + " " + viewId;

        playerNameTxt.transform.parent = null;
        playerNameTxt.gameObject.hideFlags = HideFlags.HideInHierarchy;
        playerNameTxt.enabled = false;
        //LoadCar(carId);
        nitroAudio = InitSound(res.nitro, false);
        transform2 = gameObject.transform;
        if (bot2 && !online)
            weapons.gameObject.SetActive(false);
        AmplifyMotionEffect.RegisterRecursivelyS(gameObject);

#if !UNITY_WP8 && VOICECHAT
        voiceChatPlayer = GetComponent<VoiceChatPlayer>();
#endif
        //if (bot)
        //    StartCoroutine(StartBot3());
        owner.varParse.UpdateValues();
    }
    public VarParse varParse { get { return m_varParse ?? (m_varParse = new VarParse() { pl = owner, root = m_Car, name = "CarPhys" }); } set { m_varParse = value; } }
    private VarParse m_varParse;

    private AudioSource InitSound(AudioClip audioClip, bool play = true)
    {
        var au = gameObject.AddComponent<AudioSource>();
        au.clip = audioClip;
        au.loop = true;
        if (this == _Player)
            au.priority = 0;
        au.playOnAwake = true;
        au.enabled = !play;
        return au;
    }

    private bool nitroDown;
    private float goBackTime;
    //public Transform fakeTarget;
    public float distToPl;

    public float lifeGrow;
    public void Update()
    {
        if (!active) return;
        if (KeyDebug(KeyCode.V))
        {
            Score kills = _Game.GetPlayers().FirstOrDefault() == pv.pl ? stats.killLeader : stats.kills;
            kills.value++;
            _Hud.centerText(string.Format(Tr("You killed {0}\n+{1}$"), pv.playerName, kills.points));
        }
        UpdateValues();
        UpdateLod();
        if (GameType.weapons && !dead && room.sets.autoLifeRecovery)
        {
            lifeGrow += Time.deltaTime * 2;
            if (lifeGrow > 10 && life < 100)
                life = Mathf.Min(life + lifeGrow * Time.deltaTime, room.lifeDef);
        }
        if (cop)
            nitro = Mathf.MoveTowards(nitro, 3, Time.deltaTime * .05f);

        //if (IsMine && Input2.GetKeyDown(KeyCode.R))
        //{
        //    RaycastHit h;
        //    if (Physics.Raycast(_Player.posUp, Vector3.down, out h, 3, Layer.levelMask))
        //    {
        //        var f = _Player.transform.forward;
        //        _Player.transform.up = h.normal;
        //        _Player.transform.forward = f;
        //    }
        //    AddLife(-10, -1);
        //}

        //if (_Loader.pursuit && !cop && TimeElapsed(10) && !dead)
        //    pv.score += 1;



        if (nitroDown)
            nitro -= Time.deltaTime;
        foreach (var a in nitroParticles)
            a.emit = nitroDown;

        nitroAudio.enabled = nitroDown;
        if (nitroDown)
            rigidbody.AddForce(transform.forward * bs.settings.nitroForce * rigidbody.mass);
        //if(KeyDebug(KeyCode.A))
        //    DisableCar();
        fire.gameObject.SetActive(life < 0);
        smoke.gameObject.SetActive(life < 40 && (highQuality || !android));
        fire.up = Vector3.up;
        smoke.up = Vector3.up;

        if (bot)
            UpdateBot();
        else if (IsMine)
        {
            foreach (KeyCode a in useKeys)
            {

                bool k = (Input2.GetKey(a) || pressedKey.Contains(a)) && !unpressKey.Contains(a);
                if (keys[(int)a] != k)
                    CallRPC(SetKey, (int)a, k);
            }

            pressedKey.Clear();
            unpressKey.Clear();
        }

        if (!dead && _Game.started && !win.active)
            m_bBrakeRelease = UpdateInput(m_Car, false, m_bBrakeRelease);
        else
            DisableCar();
        if (IsMine)
            FallCheck();
        UpdateText();
        UpdateDm();

        if (_Player == this && !dead)
        {
            RaycastHit h = default(RaycastHit);
            if (Vector3.Dot(transform.up, Vector3.up) > .1f || !Physics.Raycast(_Player.posUp, Vector3.down, out h, 3, Layer.levelMask))
                reverseTime = Time.time;
            if (Time.time - reverseTime > 3)
            {
                var f = _Player.transform.forward;
                _Player.transform.up = h.normal;
                _Player.transform.forward = f;
                _Player.rigidbody.angularVelocity = Vector3.zero;
                reverseTime = Time.time;
            }

        }
        avrVel = Mathf.Lerp(avrVel, velm, Time.deltaTime);
    }
    public void UpdateValues()
    {
        velm = rigidbody.velocity.magnitude;
        if (lodRenderer)
        {
            if (!(!IsMine && lowQuality && (pos - CameraMainTransform.position).magnitude > (android ? 30 : 60) && !bot))
                lodTime = Time.time;
            lod = Time.time - lodTime > 1;
        }
        distToPl = (_Player.pos - pos).magnitude;
    }
    private void UpdateLod()
    {
        cc.gameObject.SetActive(!lod);
        if (lodRenderer && lodRenderer.enabled != lod)
        {
            if (!lod)
            {
                transform.position = transform2.position;
                transform.rotation = transform2.rotation;
            }
            lodRenderer.enabled = lod;
        }
        if (lod)
        {
            transform2.position += vel * Time.deltaTime;
            transform2.Rotate(syncAng * Time.deltaTime);
        }
        else
        {
            transform2.position = transform.position;
            transform2.rotation = transform.rotation;
        }
    }

    public static List<KeyCode> unpressKey = new List<KeyCode>();
    public static List<KeyCode> pressedKey = new List<KeyCode>();

    private float averVel;
    private float reverseTime;
    public bool lod;
    public float lodTime;
    private float maxLerp;
    public void FixedUpdate()
    {
        if (lod) return;
        if (!active) return;


        //if (!new[] { m_Car.WheelFL, m_Car.WheelFR, m_Car.WheelRR, m_Car.WheelRL }.All(a => a.m_grounded))
        //rigidbody.AddForce(Vector3.down * setting.extraGravity, ForceMode.Acceleration);

        if (!IsMine && Time.time - collisionTime > .3f)
        {
            maxLerp = Mathf.Clamp(distToPl / 5, 1, 3);
            var mt = Vector3.MoveTowards(Vector3.zero, offsetPos, Time.deltaTime * bs.settings.lerpMove * maxLerp);
            offsetPos -= mt;
            rigidbody.MovePosition(pos + mt);

            var rt = Quaternion.Slerp(Quaternion.identity, offsetRot, Time.deltaTime * bs.settings.lerpRot * maxLerp);
            //offsetRot = Quaternion.Lerp(offsetRot, Quaternion.identity, 1 - Time.deltaTime * 10);
            offsetRot *= Quaternion.Inverse(rt);
            rigidbody.MoveRotation(rt * rot);
        }

        if (bot2)
        {
            RaycastHit h;
            if (Physics.Raycast(pos, Vector3.down, out h, 100, Layer.levelMask))
            {
                Vector3 eulerAngles = Quaternion.FromToRotation(transform.up, h.normal).eulerAngles;
                eulerAngles = ext.ClampAngle(eulerAngles);
                rigidbody.AddTorque(eulerAngles * .1f * rigidbody.mass);
            }
        }

        distWent += velm;
        //UpdateCollision();
    }

    //private void UpdateCollision()
    //{
    //    var o = vellChange;
    //    vellChange *= .9f;
    //    vellChange += Mathf.Abs(oldVel.magnitude - vel.magnitude);
    //    if (vellChange > 10 && vellChange < o)
    //    {
    //        //print(vellChange);
    //        //Hit(cc.m_CarVisuals.contactPoint,cc.m_CarVisuals.contactVel,);
    //        vellChange = 0;
    //    }
    //    oldVel = vel;
    //}
    //Vector3 oldVel;
    //float vellChange;

    [RPC]
    private void SetKey(int a, bool k)
    {
        keys[a] = k;
    }

    private float handbrakeValue;
    private float reverseValue;
    private float forwardValue;
    private float steerValue;

    //private float handbrakeValue2;
    //private float reverseValue2;
    //private float forwardValue2;
    //private float steerValue2;
    private KeyCode[] useKeys = new KeyCode[] { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S, KeyCode.Space, KeyCode.LeftShift, KeyCode.X, KeyCode.Mouse0, KeyCode.R, };


    public bool InputGetKey(KeyCode keyCode)
    {

        if (isDebug)
        {
            bool any = false;
            foreach (KeyCode a in useKeys)
            {
                if (a == keyCode)
                {
                    any = true;
                    break;
                }
            }
            if (!any)
            {
            }
        }
        return keys[(int)keyCode];
    }
    public bool[] keys = new bool[400];
    static private float GEARSPEEDMIN = 0.2f;

    public override void OnPlConnected()
    {
        InitNetwork();
        base.OnPlConnected();
    }
    public void InitNetwork()
    {
        CallRPC(SetTeam, (int)team.team);
        CallRPC(SetBot, bot2);
        if (isAndroid)
            CallRPC(SetAndroid, isAndroid);

        ////
        CallRPC(Reset);
        CallRPC(SetActive, active);
        CallRPC(SetEnabled);
    }
    [RPC]
    public void SetAndroid(bool b)
    {
        isAndroid = b;
    }
    [RPC]
    public void SetBot(bool bot)
    {
        this.bot2 = bot;
    }

    [RPC]
    public void SetEnabled()
    {
        enabled = true;
    }
    public float life = 100;
    //[RPC]
    //public void SetLife(float value, int killerId)
    //{        
    //    if (life < 0) return;
    //    life = value;
    //    audio.PlayOneShot(res.hit.Random());
    //    if (life < _Loader.lifeDef / 3)
    //        smoke.gameObject.SetActive(true);
    //    if (life < 0)
    //        Die();
    //}
    //private float spawnTime;
    //private float botSpawnTime;
    internal float lastHitTime;
    //internal float showTextTime;
    private Player killedBy;
    internal float nitro = 3;
    public Vector3 vel { get { return rigidbody.velocity; } set { rigidbody.velocity = value; } }
    public float velm;

    internal void AddLife(float nlife, int killerId = -1)
    {
        if (killerId == -1)
            killerId = killedBy ? killedBy.viewId : -1;
        CallRPC(SetLife, life + nlife, killerId);
    }

    [RPC]
    internal void SetLife(float nlife, int killerId = -1)
    {
        Profiler.BeginSample("SetLife");
        if (dead)
            return;
        lifeGrow = 0;
        killedBy = _Game.players.TryGet(killerId);
        //if (velm > 10f && IsMine)
        //    CallRPC(SetVel, vel * (velm > 30f ? .5f : .9f));
        var damage = life - nlife;
        life = nlife;
        if (killedBy != null)
        {
            if (IsMine)
                killedBy.lastHitTime = Time.time;
            else if (killedBy.IsMine)
                lastHitTime = Time.time;


            if (killedBy != this)
            {
                if (killedBy == _Player)
                {
                    //showTextTime = Time.time;
                    if (GameType.pursuitOrRace)
                        _Player.nitro += .1f * damage;
                    if (damage > 1)
                    {
                        _Hud.PlayScore((int)damage, Color.blue);
                        //if(pursuit)
                        //_Player.pv.score += damage / 50f;
                    }
                    //PlayOneShotGui(res.hitFeedback);
                }

                if (this == _Player && killedBy != _Player)
                {
                    //DamageText(damage);

                    //killedBy.showTextTime = Time.time;
                    _Hud.damageAnim.Rewind();
                    _Hud.damageAnim.Play();
                    var angle = Quaternion.LookRotation(killedBy.pos - pos).eulerAngles.y;
                    angle = Mathf.DeltaAngle(angle, CameraMainTransform.eulerAngles.y);
                    _Hud.damage.eulerAngles = new Vector3(0, 0, angle);
                }
            }


        }
        if (life < 0)
            Die();
        //PlayCrashHitSound(true);
        Profiler.EndSample();
    }
    private float dieTime;
    public void Die()
    {
        dieTime = Time.time;
        DisableCar();
        fire.gameObject.SetActive(true);

        var g = (GameObject)Instantiate(res.explosion);
        Destroy(g, 5);
        g.transform.position = pos;


        //if (survival && !bot2)
        //    SurvivalWin();
        //else
        StartCoroutine(AddMethod(6, delegate
        {
            if (dead && IsMine)
            {
                if (GameType.race)
                    CallRPC(SetTeam, (int)TeamEnum.Blue);
                CallRPC(Reset);
            }
        }));


        _Hud.killText.chat(killedBy == this || killedBy == null ? pv.getText() + " died" : killedBy.pv.getText() + " killed " + pv.getText());
        //if (!survival)
        //{
        if (killedBy != null)
        {
            if (this == _Player)
            {
                if (killedBy.revenge == _Player)
                    PlayOneShotGui(res.revenge, 2);
                //else
                //PlayOneShotGui(res.ownage, 2);
                pv.AddDeaths(1);
                if (killedBy != null)
                {
                    if (killedBy == this)
                        _Hud.centerText(Tr("Suicided"));
                    else
                        _Hud.centerText(string.Format(Tr("{0} killed You"), killedBy.pv.playerName));
                }
            }
            else if (killedBy == _Player)
            {
                if (Time.time - lastTimeKill > 10)
                    megaKill = 0;
                lastTimeKill = Time.time;
                megaKill++;

                //if (megaKill > 1)


                if (this == _Player.revenge && megaKill < 2)
                    PlayOneShotGui(res.revenge, 2f);
                else
                    PlayOneShotGui(megaKill == 1 ? res.kill1.Random() : megaKill == 2 ? res.kill2 : megaKill == 3 ? res.kill3 : res.kill4);
                //if (dm )

                var leader = _Game.GetPlayers().FirstOrDefault() == pv.pl;
                killedBy.pv.AddKills(1);
                if (!room.noKillScore)
                    killedBy.pv.AddScore(GameType.race || leader ? 3 : 1);

                //if (pursuit)
                //    if (!cop)
                if (cop)
                {
                    stats.arest.value++;
                    _Hud.centerText(string.Format(Tr("You Arested {0}\n+{1}$"), pv.playerName, stats.arest.points));
                }
                else
                {
                    Score kills = leader ? stats.killLeader : stats.kills;
                    kills.value++;
                    _Hud.centerText(string.Format(Tr("You killed {0}\n+{1}$"), pv.playerName, kills.points));
                    if (leader)
                        _Hud.centerText2("Leader kill +3");
                }
                //else
                //    _Hud.centerText(string.Format(Tr("You killed a cop!")), 3);
            }
            revenge = killedBy;
        }
        if (!firstBlood && online)
            PlayOneShotGui(res.firstBlood, 3);
        firstBlood = true;
        //}
    }
    public Gts stats { get { return owner.curGame; } }
    //private static void DamageText(float score)
    //{
    //    score = (int) score;
    //    _Hud.zombieScore.text = "<color=blue>"+score+"</color>";
    //    _Hud.zombieScoreAnim.Rewind();
    //    _Hud.zombieScoreAnim.Play();
    //}
    //[RPC]
    //private void SetVel(Vector3 v)
    //{
    //    vel = v;
    //}

    //private void PlayCrashHitSound(bool b)
    //{
    //    var audioClips = b ? res.hitSoundBig : res.hitSound;
    //    audio.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    //}

    public GUIText playerNameTxt;
    public void RefreshText()
    {
        playerNameTxt.text = pv.getText();
    }
    public void UpdateText()
    {
        if (!IsMine && !bot2)
        {
            var mag = (_Player.pos - pos).magnitude;
            var dist = 200;
            if (playerNameTxt.enabled || FramesElapsed(10))
            {
                var position = CameraMain.WorldToViewportPoint(pos + Vector3.up) + Vector3.up * .05f;
                if (playerNameTxt.transform.position.z < 0)
                {
                    position.y = .1f;
                    position.x = Mathf.Clamp(1f - position.x, .2f, .7f);
                    playerNameTxt.transform.position = position;
                }
                playerNameTxt.transform.position = position;
                playerNameTxt.fontSize = (int)Mathf.Lerp(16, 8, mag / dist);
            }
            //if (_Loader.pursuitRace)
            playerNameTxt.enabled = _Player.voiceChatting || !_Player.IsEnemy(this) || !GameType.weapons || Time.time - lastHitTime < 3 || Time.time - _Game.stateChangeTime < 10;//|| dm ? Time.time - lastHitTime < 3 : _Player.team.players.Any(a => a.Seeing(this));
            playerNameTxt.color = (_Player.IsEnemy(this) ? Color.red : Color.green); // -new Color(0, 0, 0, Mathf.Min(.7f, mag / 400f));            
        }
        else
            playerNameTxt.enabled = false;
    }
    //private float seeing = MinValue;
    //public bool Seeing(Player pl)
    //{
    //    if (Time.time - pl.seeing < 3) return true;
    //    Vector3 a = pos + Vector3.up;
    //    Vector3 b = pl.pos + Vector3.up;
    //    bool v = Vector3.Dot(transform.forward, (b - a).normalized) > .3 && !Linecast(b, a);
    //    if (v)
    //        pl.seeing = Time.time;
    //    return v;
    //}
    //private static bool Linecast(Vector3 b, Vector3 a)
    //{        
    //    bool linecast = Physics.Linecast(b, a, Layer.levelMask);
    //    Debug.DrawLine(b,a, Color.red);
    //    return linecast;
    //}


    public bool IsEnemy(Player other)
    {
        if (other == null)
            return true;
        if (other == this) return false;
        if (other.pv.ally) return false;
        if (GameType.deathmatch) return true;
        if (teamEnum == other.teamEnum) //other.team.team != team.team || 
            return false;

        return true;
    }
    internal float collisionTime;
    private float damageAc;

    //private void Die()
    //{
    //    DisableCar();
    //    fire.gameObject.SetActive(true);
    //    StartCoroutine(AddMethod(3, delegate
    //    {
    //        if (dead && IsMine)
    //        {
    //            pv.CallRPC(pv.SetTeam, (int)pv.teamEnum);
    //            CallRPC(Reset);
    //        }
    //    }));
    //}


    public static float lastTimeKill;
    public static int megaKill;
    public Player revenge;
    public static bool firstBlood = true;
    [RPC]
    public void Reset()
    {
        Profiler.BeginSample("Reset");
        //spawnTime = Time.time;
        life = bs.settings.megaLife ? 1000 : room.lifeDef * (cop && online ? 3 : 1);
        LoadCar();
        RefreshText();
        nitro = cop ? 4 : 3;
        Profiler.EndSample();
    }
    internal bool dead { get { return life < 0; } }
    private Vector3 syncPos;
    private Quaternion syncRot;
    private Vector3 syncVel;
    private Vector3 syncAng;
    protected void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //stream.isWriting
        if (!rigidbody) return;
        if (stream.isWriting)
        {
            syncPos = rigidbody.position;
            syncRot = rigidbody.rotation;
            syncVel = rigidbody.velocity;
            syncAng = rigidbody.angularVelocity;
        }
        stream.Serialize(ref syncPos);
        stream.Serialize(ref syncRot);
        stream.Serialize(ref syncVel);
        stream.Serialize(ref syncAng);
        if (GameType.weapons)
        {
            stream.Serialize(ref distanceToCursor2);
            stream.Serialize(ref TargetPlayerId);
        }
        if (stream.isReading)
        {

            if (Vector3.Distance(rigidbody.position, syncPos) > clampMax && Time.time - collisionTime > 1)
            {
                transform.position = syncPos;
                transform.rotation = syncRot;
            }
            syncPos += syncVel * Mathf.Min(.2f, ping);
            Debug.DrawLine(pos, syncPos, Color.yellow, 2);
            rigidbody.velocity = syncVel;
            rigidbody.angularVelocity = syncAng;
            offsetPos = syncPos - pos;
            if (Mathf.Abs(offsetPos.y) < .1f) offsetPos.y = 0;
            offsetRot = syncRot * Quaternion.Inverse(rot);
        }

        ping = Mathf.Lerp(ping, Mathf.Max(0, (float)(PhotonNetwork.time - info.timestamp)), .1f);
    }
    private Vector3 offsetPos;
    private Quaternion offsetRot;
    [RPC]
    public void SetTeam(int t)
    {
        pv.teamEnum = (TeamEnum)t;

    }

    public int clampMax = 10;
    internal new bool active = true;
    [RPC]
    public void SetActive(bool b)
    {
        active = b;
        //gameObject.SetActive(b);
        //pv.gameObject.SetActive(b);
        m_Car.gameObject.SetActive(b);
    }
    //public int checkPoints;
    public float distWent;
    internal MeshTest meshTest { get { return m_Car.m_CarDamage.meshTest; } }
    public float hitTime;


    public void OnPlayerCollision(Vector3 force, Vector3 point, Player other)
    {

        if (_Player == this)
            _MainCamera.m_scrSmoothFollow.shake = Mathf.Min(2, force.magnitude) * .6f;
        //print("Other collides");
        collisionTime = Time.time;
        if (IsMine)
            if (IsEnemy(other) && !other.dead)
            {
                //damageAc += Mathf.Sqrt(values * 10);


                damageAc += Mathf.Min(other.pcVsAndroid ? 20 : 60, force.magnitude * (other.pcVsAndroid ? 5 : 20));
                if (damageAc > 3)
                {
                    CallRPC(SetLife, life - damageAc, other ? other.viewId : -1);
                    damageAc = 0;
                }
            }
    }

    public void OnCollision(Vector3 point, float cnt = 10, int max = 20, bool skip = false)
    {
        if (_AutoQuality.destrQuality == 0) return;
        if (IsMine && life > 30)
            AddLife(-cnt, viewId);

        if (Time.time - dieTime < 1 && highQuality)
            cnt *= 3;
        Profiler.BeginSample("MeshTest");
        if ((highQuality || Time.time - hitTime > .5f) && checkVisible(point))
        {
            hitTime = Time.time;
            meshTest.Hit(point, m_Car.rigidbody.velocity * .5f, cnt, max);
            if (_AutoQuality.destrQuality == 2 && !skip)
                meshTest.Damage(point);
        }
        Profiler.EndSample();
    }
    //[RPC]
    //public void SetCheckPoints(int c)
    //{
    //    distWent = 0;
    //    checkPoints = c;
    //}
    [RPC]
    private void SetPv(int Obj)
    {
        if (online)
        {
            pv = PhotonView.Find(Obj).GetComponent<PlayerView>();
        }
    }
    public void SetPvID()
    {
        if (online)
            photonView.RPC("SetPv", PhotonTargets.AllBufferedViaServer, pv.photonView.viewID);
    }


}