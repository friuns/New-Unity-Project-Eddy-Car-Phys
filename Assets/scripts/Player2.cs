using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public partial class Player
{


    internal CarSound m_CarSound;
    private CarSettings m_CarSettings;
    internal CarControl m_Car;
    internal CarControl model { get { return m_Car; } }
    public CarControl cc { get { return m_Car; } }
    internal new Transform transform { get { return lod ? transform2 : transformCar; } }
    internal Transform transformCar;
    internal new Rigidbody rigidbody;
    public Transform transform2;
    private bool m_bBrakeRelease;


    void DisableCar(bool brake = true)
    {
        CarCameras CarCams = m_Car.GetComponent<CarCameras>() as CarCameras;
        CarCams.showFixedCams = false;
        m_Car.motorInput = 0.0f;
        m_Car.brakeInput = brake ? 1.0f : 0;
        CarSettings Settings = m_Car.GetComponent<CarSettings>() as CarSettings;
        if (Settings)
            Settings.bypassABS = true;
        m_Car.readUserInput = false;
        nitroDown = false;
    }
    public bool inited { get { return m_Car; } }
    void SelectCar() { }
    void LoadSkin() { }

    void LoadCar()
    {
        if (m_Car)
            _Pool.Save(m_Car.gameObject);
        string path = cop ? "police" : owner.carSet.carName;
        Spawn spawnPos = GetSpawnStartPos();
        m_Car = _Pool.Load2(Resources.Load(path), spawnPos.pos, spawnPos.rot).GetComponent<CarControl>();
        if (m_Car.m_CarDamage.meshTest)
        {
            m_Car.m_CarDamage.meshTest.Reset(this == _Player);
            m_Car.rigidbody.velocity = m_Car.rigidbody.angularVelocity = Vector3.zero;
        }

        //m_Car = ((GameObject)Instantiate(Resources.Load(path), spawnPos.pos, spawnPos.rot)).GetComponent<CarControl>();
        m_Car.name = "Player Car " + pv.playerName;
        transformCar = m_Car.tr;
        rigidbody = m_Car.rigidbody;
        rigidbody.solverIterationCount = 6;
        m_CarSettings = m_Car.GetComponent<CarSettings>() as CarSettings;
        m_CarSound = m_Car.GetComponent<CarSound>() as CarSound;
        m_CarSettings.bypassABS = false;
        m_Car.pl = this;
        OnQualityChanged();
        var priority = this == _Player ? 100 : 130;
        foreach (var a in GetComponents<AudioSource>())
            a.priority = priority;
        foreach (var a in m_Car.audioSources)
            a.priority = priority;

        if (IsMine && !bot2)
            _Game.MainCamera.Target = m_Car.tr;
        AmplifyMotionEffect.RegisterRecursivelyS(m_Car.gameObject);
    }
    public override void OnQualityChanged()
    {
        foreach (var l in m_Car.lights)
            l.gameObject.SetActive(_AutoQuality.graphQuality > 0 || _Player == this);
    }
    protected void Collide() { }
    private void FallCheck()
    {
        if (pos.y < -100)
        {
            pos = GetSpawnStartPos().pos;
            rot = Quaternion.identity;
            vel = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            AddLife(-30);
        }

    }
    public bool pcVsAndroid { get { return android && !isAndroid || Paypal.haveCar; } }
    public bool isAndroid;

    public Spawn GetSpawnStartPos()
    {

        if ((GameType.race || isDebug) && _Game.raceSpawns.Any())
        {
            IEnumerable<PhotonPlayer> photonPlayers = PhotonNetwork.playerList.Where(a => a != null);
            List<PhotonPlayer> players = photonPlayers.OrderBy(a => a.ID).ToList();
            var raceSpawns = _Game.raceSpawns.ToArray();
            return raceSpawns[players.IndexOf(photonView.owner) % raceSpawns.Length];
        }
        if (_Game.spawns.Count > 0)
        {
            var oe = _Game.spawns.OrderBy(a => a.team == (GameType.deathmatch ? TeamEnum.Dm : teamEnum));
            Player alli = _Game.playersList.FirstOrDefault(a => a.pv.ally);
            if (alli)
                oe = oe.ThenBy(a => Vector3.Distance(a.pos, alli.pos));

            return oe.FirstOrDefault();
        }

        Debug.LogWarning("Spawns not found generating");
        for (int i = 0, j = 0; i < 100 && j < 50; i++)
        {
            var bounds = _Game.bounds;
            var sp = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                bounds.max.y,
                Random.Range(bounds.min.z, bounds.max.z));
            RaycastHit h;
            Debug.DrawRay(sp, Vector3.down, Color.white, 10);
            if (Physics.Raycast(new Ray(sp, Vector3.down), out h, bounds.size.y, Layer.levelMask))
                if (Vector3.Dot(h.normal, Vector3.up) > .7f)
                {
                    var spawn = ((GameObject)Instantiate(Resources.Load(res.spawnDM.fullName), Vector3.zero, Quaternion.identity)).transform;
                    spawn.position = h.point + Vector3.up;
                    spawn.forward = ZeroY(bounds.center - spawn.position);
                    j++;
                    Destroy(spawn.gameObject, 0);
                    return spawn.GetComponent<Spawn>();
                }
        }

        return null;
    }

    //public new void print(object s)
    //{
    //    _Loader.sb.AppendLine(s + "");
    //}

    public bool UpdateInput(CarControl Car, bool reverseRequiresStop, bool brakeRelease)
    {

        if (!bot && IsMine && nitro <= 0)
            unpressKey.Add(KeyCode.LeftShift);

        if (Input2.GetKey(KeyCode.LeftShift))
            pressedKey.Add(KeyCode.W);
        nitroDown = InputGetKey(KeyCode.LeftShift);
        var factor = Time.deltaTime;
        bool left = InputGetKey(KeyCode.A);
        bool right = InputGetKey(KeyCode.D);
        var brake = InputGetKey(KeyCode.Space);
        var forward = InputGetKey(KeyCode.W) || nitroDown;
        var back = InputGetKey(KeyCode.S);



        steerValue = Lerp(steerValue, left ? -1 : right ? 1 : 0, factor);
        forwardValue = Lerp(forwardValue, forward ? 1 : 0, factor * 3);
        reverseValue = Lerp(reverseValue, back ? 1 : 0, factor * 3);
        if (forward)
            reverseValue = 0;
        if (back)
            forwardValue = 0;
        handbrakeValue = brake ? 1 : 0;

        float speedForward = Vector3.Dot(Car.rigidbody.velocity, Car.tr.forward);
        float speedSideways = Vector3.Dot(Car.rigidbody.velocity, Car.tr.right);
        float speedRotation = Car.rigidbody.angularVelocity.magnitude;
        float speedAbs = speedForward * speedForward;
        speedSideways *= speedSideways;
        float motorInput = 0;
        float brakeInput = 0;
        if (reverseRequiresStop)
        {
            if (speedAbs < GEARSPEEDMIN && forwardValue == 0 && reverseValue == 0)
            {
                brakeRelease = true;
                Car.gearInput = 0;
            }
            if (brakeRelease)
                if (speedAbs < GEARSPEEDMIN)
                {
                    if (reverseValue > 0)
                    {
                        Car.gearInput = -1;
                    }
                    if (forwardValue > 0)
                    {
                        Car.gearInput = 1;
                    }
                }
                else if (speedSideways < GEARSPEEDMIN && speedRotation < GEARSPEEDMIN)
                {
                    if (speedForward > 0 && Car.gearInput <= 0 && (forwardValue > 0 || reverseValue > 0))
                        Car.gearInput = 1;
                    if (speedForward < 0 && Car.gearInput >= 0 && (forwardValue > 0 || reverseValue > 0))
                        Car.gearInput = -1;
                }
            if (Car.gearInput < 0)
            {
                motorInput = reverseValue;
                brakeInput = forwardValue;
            }
            else
            {
                motorInput = forwardValue;
                brakeInput = reverseValue;
            }
            if (brakeInput > 0)
                brakeRelease = false;
        }
        else
        {
            if (speedForward > GEARSPEEDMIN)
            {
                Car.gearInput = 1;
                motorInput = forwardValue;
                brakeInput = reverseValue;
            }
            else if (speedForward <= GEARSPEEDMIN && reverseValue > GEARSPEEDMIN)
            {
                Car.gearInput = -1;
                motorInput = reverseValue;
                brakeInput = 0;
            }
            else if (forwardValue > GEARSPEEDMIN && reverseValue <= 0)
            {
                Car.gearInput = 1;
                motorInput = forwardValue;
                brakeInput = 0;
            }
            else if (forwardValue > GEARSPEEDMIN)
                Car.gearInput = 1;
            else if (reverseValue > GEARSPEEDMIN)
                Car.gearInput = -1;
            else
                Car.gearInput = 0;
            brakeRelease = false;
        }
        Car.steerInput = steerValue;
        Car.motorInput = motorInput;
        Car.brakeInput = brakeInput;
        Car.handbrakeInput = handbrakeValue;
        return brakeRelease;
    }
    private float Lerp(float SteerValue, int i, float factor)
    {
        var moveTowards = Mathf.MoveTowards(SteerValue, i, factor);
        var clamp = Mathf.Clamp(moveTowards, i < 0 ? -1 : 0, i > 0 ? 1 : 0);
        return Mathf.Lerp(moveTowards, clamp, Time.deltaTime * 20);
    }


    public static bool inFrontInvisible(Vector3 pos)
    {
        Camera cam = CameraMain;
        var wp = cam.WorldToViewportPoint(pos);
        RaycastHit h;
        Transform camTr = CameraMainTransform;
        //if (Vector3.Dot(_Player.transform.forward, pos - _Player.pos) < 0)
        //    return false;
        if (!(new Rect(0, 0, 1, 1).Contains(wp) && wp.z > 0)) return false;

        bool linecast = Physics.Linecast(camTr.position, pos, out h, Layer.allmask) &&
            Physics.Linecast(camTr.position, pos + camTr.right, out h, Layer.allmask) &&
            Physics.Linecast(camTr.position, pos - camTr.right, out h, Layer.allmask);

        if (linecast)
            Debug.DrawRay(h.point, Vector3.one, Color.red, 10);
        return linecast;

    }

}