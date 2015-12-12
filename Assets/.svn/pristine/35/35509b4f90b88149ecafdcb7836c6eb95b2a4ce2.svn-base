using UnityEngine;
using System.Collections;

public class CarMain : Base
{
    public CarControl[] Cars;
    public int startupCar = 0;
    public CameraControl MainCamera;
    public Light MainLight;
    public bool reverseRequiresstop;
    public Rigidbody Payload;
    public float slowTime = 0.25f;
    public bool showHelp = false;
    public bool guiEnabled = true;
    public GUIStyle BigTextStyle;
    public GUIStyle TextStyle;
    public GUIStyle BoxStyle;
    public GUISkin guiSkin;
    private int m_currentCar = 0;
    private CarControl m_Car;
    private CarTelemetry m_CarTelemetry;
    private CarSettings m_CarSettings;
    private CarCameras m_CarCameras;
    internal CarSound m_CarSound;
    private bool m_bBrakeRelease = false;
    static private float GEARSPEEDMIN = 0.2f;
    
    public CarControl getSelectedCar()
    {
        return Cars[m_currentCar];
    }
    void Start()
    {

        m_CarTelemetry = GetComponent<CarTelemetry>() as CarTelemetry;
        m_currentCar = startupCar;
        if (m_currentCar >= Cars.Length)
            m_currentCar = Cars.Length - 1;
        for (int i = 0; i < Cars.Length; i++)
            DisableCar(Cars[i]);
        SelectCar(Cars[m_currentCar]);
        //SetTimeScale(2);
        
    }
    

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Tab))
            //SetTimeScale(Time.timeScale > 1.9f ? 1 : 2);
        if (!m_CarSettings.externalInput)
            m_bBrakeRelease = SendInput(m_Car, reverseRequiresstop, m_bBrakeRelease);
        bool bShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if (Input.GetKeyDown(KeyCode.P))
            Debug.Break();
        if (Input.GetKeyDown(KeyCode.T))
            if (Time.timeScale < 1.0f)
                Time.timeScale = 1.0f;
            else
                Time.timeScale = slowTime;
        if (Input.GetKeyDown(KeyCode.E) && Payload)
            Payload.rigidbody.AddForce(new Vector3(0, 6, 0), ForceMode.VelocityChange);
        if (Input.GetKeyDown(KeyCode.R) && !bShift)
            Application.LoadLevel(0);
        if (Input.GetKeyDown(KeyCode.PageUp))
            SwitchCar(-1);
        if (Input.GetKeyDown(KeyCode.PageDown))
            SwitchCar(1);
        if (Input.GetKeyDown(KeyCode.Return))
        {
            m_Car.rigidbody.velocity = Vector3.zero;
            m_Car.rigidbody.angularVelocity = Vector3.zero;
            m_Car.tr.localEulerAngles = new Vector3(0, m_Car.tr.localEulerAngles.y, 0);
            m_Car.tr.position += new Vector3(0, 1.6f, 0);
        }
        if (Input.GetKeyDown(KeyCode.R) && bShift)
        {
            CarDamage DeformScript = m_Car.GetComponent<CarDamage>() as CarDamage;
            if (DeformScript)
                DeformScript.meshTest.Reset(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
            m_CarSettings.abs = !m_CarSettings.abs;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            m_CarSettings.tc = !m_CarSettings.tc;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            m_CarSettings.esp = !m_CarSettings.esp;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            m_CarSettings.tractionAxle++;
        if (Input.GetKeyDown(KeyCode.Alpha5))
            m_CarSettings.stabilizerMode++;
        if (Input.GetKeyDown(KeyCode.I))
            m_CarSettings.externalInput = !m_CarSettings.externalInput;
        if (Input.GetKeyDown(KeyCode.C))
            MainCamera.Next();
        if (Input.GetKeyDown(KeyCode.F1))
            MainCamera.SwitchTo(0);
        if (Input.GetKeyDown(KeyCode.F2))
            MainCamera.SwitchTo(1);
        if (Input.GetKeyDown(KeyCode.F3))
            MainCamera.SwitchTo(2);
        if (Input.GetKeyDown(KeyCode.F4))
            MainCamera.SwitchTo(3);
        if (Input.GetKeyDown(KeyCode.F5))
            MainCamera.SwitchTo(4);
        if (Input.GetKeyDown(KeyCode.F6))
            MainCamera.SwitchTo(5);
        if (Input.GetKeyDown(KeyCode.M))
            MainCamera.ToggleMap();
        if (Input.GetKeyDown(KeyCode.V))
            m_CarCameras.Next();
        if (Input.GetKeyDown(KeyCode.N))
            MainCamera.showMirrors = !MainCamera.showMirrors;
        if (m_CarTelemetry && Input.GetKeyDown(KeyCode.B))
        {
            if (!bShift)
                m_CarTelemetry.Enabled = !m_CarTelemetry.Enabled;
            else
                m_CarTelemetry.Curves = !m_CarTelemetry.Curves;
        }
        if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Escape))
            showHelp = !showHelp;
        if (Input.GetKeyDown(KeyCode.F12))
        {
            bool bQuality = MainLight.light.shadows != LightShadows.None;
            if (bQuality)
            {
                MainLight.light.shadows = LightShadows.None;
                MainCamera.enableImageEffects = false;
            }
            else
            {
                MainLight.light.shadows = LightShadows.Soft;
                MainCamera.enableImageEffects = true;
            }
        }
    }
    void SwitchCar(int iDir)
    {
        if (Cars.Length < 2)
            return;
        DisableCar(Cars[m_currentCar]);
        m_currentCar += (int)Mathf.Sign(iDir);
        if (m_currentCar < 0)
            m_currentCar = Cars.Length - 1;
        else if (m_currentCar >= Cars.Length)
            m_currentCar = 0;
        SelectCar(Cars[m_currentCar]);
    }
    void DisableCar(CarControl Car)
    {
        CarCameras CarCams = Car.GetComponent<CarCameras>() as CarCameras;
        CarCams.showFixedCams = false;
        Car.motorInput = 0.0f;
        Car.brakeInput = 1.0f;
        CarSettings Settings = Car.GetComponent<CarSettings>() as CarSettings;
        if (Settings)
            Settings.bypassABS = true;
        Car.readUserInput = false;
    }
    void SelectCar(CarControl Car)
    {        
        if (m_Car)
            MainCamera.Target2 = m_CarCameras.CameraLookAtPoint;
        m_Car = Car;
        m_CarSettings = m_Car.GetComponent<CarSettings>() as CarSettings;
        m_CarCameras = m_Car.GetComponent<CarCameras>() as CarCameras;
        m_CarSound = m_Car.GetComponent<CarSound>() as CarSound;
        MainCamera.Target = m_Car.tr;
        m_CarCameras.showFixedCams = true;
        m_CarSettings.bypassABS = false;
        if (m_CarTelemetry)
            m_CarTelemetry.Target = Car;
    }
    public static bool SendInput(CarControl Car, bool reverseRequiresStop, bool brakeRelease)
    {
        float steerValue = Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1);
        float forwardValue = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);
        float reverseValue = -1 * Mathf.Clamp(Input.GetAxis("Vertical"), -1, 0);
        float handbrakeValue = Mathf.Clamp(Input.GetAxis("Jump"), 0, 1);
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
    private Rect ScrRect(float x, float y, float wd, float ht)
    {
        return new Rect(x * Screen.width, y * Screen.height, wd * Screen.width, ht * Screen.height);
    }
    private void GUIBoxedText(Rect pos, string text, GUIStyle textStyle, Color textColor)
    {
        GUI.Box(pos, "");
        Color savedCol = textStyle.normal.textColor;
        textStyle.normal.textColor = textColor;
        GUI.Label(pos, text, textStyle);
        textStyle.normal.textColor = savedCol;
    }
    //void OnGUI()
    //{
    //    if (!guiEnabled)
    //        return;
    //    GUI.skin = guiSkin;
    //    CarControl Car = getSelectedCar();
    //    Rect rc = ScrRect(0.25f, 0.89f, 0.35f, 0.2f);
    //    if (m_CarSettings.HasExternalInput())
    //        m_CarSettings.externalInput = GUI.Toggle(new Rect(rc.x + 160, rc.y + 2, 50, 18), m_CarSettings.externalInput, "AI", guiSkin.button);
    //    if (GUI.Toggle(new Rect(rc.x + 220, rc.y + 2, 50, 18), true, "car >", guiSkin.button) == false ||
    //    GUI.Button(new Rect(rc.x + 280, rc.y, 400, 20), m_CarSettings.description, "helptext"))
    //        SwitchCar(1);
    //    float SpeedMS = new Vector2(Vector3.Dot(Car.rigidbody.velocity, Car.transform.forward), Vector3.Dot(Car.rigidbody.velocity, Car.transform.right)).magnitude;
    //    GUI.Label(new Rect(rc.x, rc.y, 70, 40), string.Format("{0:0.}", SpeedMS * 3.6f), BigTextStyle);
    //    string carGear;
    //    int gear;
    //    if (m_CarSound)
    //    {
    //        gear = m_CarSound.currentGear;
    //        carGear = gear > 0 ? gear.ToString() : gear < 0 ? "R" : "N";
    //    }
    //    else
    //    {
    //        gear = Car.gearInput;
    //        carGear = Car.getGear();
    //    }
    //    GUI.Label(new Rect(rc.x, rc.y + 42, 70, 40), string.Format("{0:0.} mph", SpeedMS * 2.237), TextStyle);
    //    GUIBoxedText(new Rect(rc.x + 80, rc.y + 6, 55, 50), string.Format("{0}", carGear), BoxStyle, gear < 0 ? Color.white : gear > 0 ? new Color(0.6f, 0.6f, 0.6f) : new Color(0, 0.5f, 0.0f));
    //    if (m_CarSettings.hasABS)
    //        m_CarSettings.abs = GUI.Toggle(new Rect(rc.x + 160, rc.y + 26, 50, 30), m_CarSettings.abs, "ABS", guiSkin.button);
    //    if (m_CarSettings.hasTC)
    //        m_CarSettings.tc = GUI.Toggle(new Rect(rc.x + 220, rc.y + 26, 50, 30), m_CarSettings.tc, "TC", guiSkin.button);
    //    if (m_CarSettings.hasESP)
    //        m_CarSettings.esp = GUI.Toggle(new Rect(rc.x + 280, rc.y + 26, 50, 30), m_CarSettings.esp, "ESP", guiSkin.button);
    //    bool bShowEnabled = m_CarSettings.hasTractionModes > 0;
    //    if (bShowEnabled)
    //        if (GUI.Toggle(new Rect(rc.x + 340, rc.y + 26, 50, 30), bShowEnabled, m_CarSettings.getTractionAxleStr(), guiSkin.button) != bShowEnabled)
    //            m_CarSettings.tractionAxle++;
    //    bShowEnabled = m_CarSettings.hasVariableStabilizer;
    //    if (bShowEnabled)
    //        if (GUI.Toggle(new Rect(rc.x + 400, rc.y + 26, 60, 30), bShowEnabled, m_CarSettings.getStabilizerModeShortStr(), guiSkin.button) != bShowEnabled)
    //            m_CarSettings.stabilizerMode++;
    //    showHelp = GUI.Toggle(new Rect(rc.x + 470, rc.y + 26, 60, 30), showHelp, "H Help", "helptext");
    //    rc.y = rc.y - 350;
    //    rc.x = rc.x + 20;
    //    rc.width = 520;
    //    rc.height = 270;
    //    if (showHelp)
    //    {
    //        if (GUI.Button(rc, "Offroader v4 / 2011.11.02 - by Edy", guiSkin.box))
    //            showHelp = false;
    //        rc.x += 10;
    //        rc.y += 35;
    //        string sHelp1 = "Arrows / WSAD - Accelerate, brake, turn\n" +
    //                "Space - handbrake\n" +
    //                "Return - unflip car\n" +
    //                "shift+R - repair damage\n\n" +
    //                "12345 - ABS, TC, ESP, 4x4, Stabilization\n" +
    //                "I - Enable AI (pseudo-random)\n\n" +
    //                "PgUp / PgDown - select vehicle\n" +
    //                "E - Eject payload\n" +
    //                "R - Restart demo\n" +
    //                "T - Slow motion\n\n";
    //        string sHelp2 = "C - camera (also F1-F6)\n" +
    //                "V - secondary camera\n" +
    //                "M - minimap\n" +
    //                "N - mirrors (bus driver only)\n" +
    //                "B - telemetry\n" +
    //                "shift+B - tire telemetry\n\n" +
    //                "mouse - move view in cameras F1,F3,F5\n" +
    //                "mouseWheel - zoom in F4,F5, dist in F3\n" +
    //                "NumPad - move camera position in F4,F5\n\n" +
    //                "F12 - toggle low quality mode\n";
    //        string sHelp3 = "Switching gears between (D) and reverse (R) requires the car to completely stop, release accelerate and brake keys, then press accelerate (forward) or brake (reverse).";
    //        GUI.Label(rc, sHelp1);
    //        Rect rc2 = rc;
    //        rc2.x += 265;
    //        GUI.Label(rc2, sHelp2);
    //        rc2 = rc;
    //        rc2.y += 190;
    //        rc2.width -= 10;
    //        GUI.Label(rc2, sHelp3);
    //    }
    //}
}
