using System.Collections;
using UnityEngine;

public class CarControl : bs
{
    internal Player pl;
    public Light[] lights;
    public AudioSource[] audioSources;
    public CarVisuals m_CarVisuals;
    public CarDamage m_CarDamage;
    public CarAntiRollBar AntiRollFront;
    public CarAntiRollBar AntiRollRear;
    public Transform CenterOfMass;
    public CarFrictionCurve ForwardWheelFriction;
    public CarFrictionCurve SidewaysWheelFriction;
    public CarWheel WheelFL;
    public CarWheel WheelFR;
    public CarWheel WheelRL;
    public CarWheel WheelRR;
    public float airDrag = 3.0f * 2.2f;
    public float antiRollLevel = 1.0f;
    public bool autoBrakeMax = true;
    public bool autoMotorMax = true;
    public float autoSteerLevel = 1.0f;
    public float brakeBalance = 0.5f;
    public float brakeForceFactor = 1.5f;
    public float brakeInput = 0.0f;
    public float brakeMax = 1.0f;
    public float frictionDrag = 30.0f;
    public float frontSidewaysGrip = 1.0f;
    public int gearInput = 1;
    public float handbrakeInput = 0.0f;
    private bool m_brakeRelease;
    private float m_brakeSlip;
    private float m_frontBrakeSlip;
    private float m_frontMotorSlip;
    private float m_maxRollAngle = 45.0f;
    private float m_motorSlip;
    private float m_rearBrakeSlip;
    private float m_rearMotorSlip;
    private float m_steerAngleMax;
    private float m_steerL;
    private float m_steerR;
    public float maxSpeed = 40.0f;
    public float motorBalance = 0.5f;
    public float motorForceFactor = 1.0f;
    public float motorInput = 0.0f;
    public float motorMax = 1.0f;
    public float motorPerformancePeak = 5.0f;
    public bool optimized = false;
    public bool readUserInput = false;
    public float rearSidewaysGrip = 1.0f;
    public bool reverseRequiresStop = false;
    public float rollingFrictionSlip = 0.075f;
    public bool serviceMode = false;
    public float sidewaysDriftFriction = 0.35f;
    public float staticFrictionMax = 1500.0f;
    public float steerInput = 0.0f;
    public float steerMax = 45.0f;
    public bool tractionV3 = false;
    public string getGear() { return gearInput > 0 ? "D" : gearInput < 0 ? "R" : "N"; }
    public float getMotor() { return motorInput; }
    public float getBrake() { return brakeInput; }
    public float getHandBrake() { return handbrakeInput; }
    public float getSteerL() { return m_steerL; }
    public float getSteerR() { return m_steerR; }
    public float getMaxRollAngle() { return m_maxRollAngle; }
    private void OnEnable()
    {
        m_CarDamage = GetComponent<CarDamage>();
        m_CarVisuals = GetComponent<CarVisuals>();
        ApplyEnabled(WheelFL, true);
        ApplyEnabled(WheelFR, true);
        ApplyEnabled(WheelRL, true);
        ApplyEnabled(WheelRR, true);

        if (CenterOfMass)
            rigidbody.centerOfMass = new Vector3(CenterOfMass.localPosition.x * tr.localScale.x, CenterOfMass.localPosition.y * tr.localScale.y, CenterOfMass.localPosition.z * tr.localScale.z);
        var WheelC = WheelFL.GetComponent<WheelCollider>();
        Vector3 V = rigidbody.centerOfMass - tr.InverseTransformPoint(WheelC.transform.position);
        float h = Mathf.Abs((V.y + WheelC.radius + WheelC.suspensionDistance / 2.0f) * tr.localScale.y);
        float l = Mathf.Abs(V.x * tr.localScale.x);
        m_maxRollAngle = Mathf.Atan2(l, h) * Mathf.Rad2Deg;
        rigidbody.maxAngularVelocity = 10;
        rigidbody.useConeFriction = false;
        if (optimized)
        {
            ApplyCommonParameters(WheelFL);
            ApplyCommonParameters(WheelFR);
            ApplyCommonParameters(WheelRL);
            ApplyCommonParameters(WheelRR);
            WheelFL.RecalculateStuff();
            WheelFR.RecalculateStuff();
            WheelRL.RecalculateStuff();
            WheelRR.RecalculateStuff();
        }
    }
    private void OnDisable()
    {
        ApplyEnabled(WheelFL, false);
        ApplyEnabled(WheelFR, false);
        ApplyEnabled(WheelRL, false);
        ApplyEnabled(WheelRR, false);
    }
    private void Awake()
    {
        audioSources = GetComponentsInChildren<AudioSource>();
    }
    private void Start()
    {
        
    }
    private void FixedUpdate()
    {
        ApplyCommonParameters(WheelFL);
        ApplyCommonParameters(WheelFR);
        ApplyCommonParameters(WheelRL);
        ApplyCommonParameters(WheelRR);
        ApplyFrontParameters(WheelFL);
        ApplyFrontParameters(WheelFR);
        ApplyRearParameters(WheelRL);
        ApplyRearParameters(WheelRR);
        m_motorSlip = motorInput * motorMax;
        m_brakeSlip = brakeInput * brakeMax;
        if (gearInput == 0)
            m_motorSlip = 0;
        else if (gearInput < 0)
            m_motorSlip = -m_motorSlip;
        if (serviceMode)
        {
            m_frontMotorSlip = m_motorSlip;
            m_rearMotorSlip = m_motorSlip;
        }
        else if (motorBalance >= 0.5f)
        {
            m_frontMotorSlip = m_motorSlip * (1.0f - motorBalance) * 2.0f;
            m_rearMotorSlip = m_motorSlip;
        }
        else
        {
            m_frontMotorSlip = m_motorSlip;
            m_rearMotorSlip = m_motorSlip * motorBalance * 2.0f;
        }
        if (serviceMode)
        {
            m_frontBrakeSlip = m_brakeSlip;
            m_rearBrakeSlip = m_brakeSlip;
        }
        else if (brakeBalance >= 0.5f)
        {
            m_frontBrakeSlip = m_brakeSlip * (1.0f - brakeBalance) * 2.0f;
            m_rearBrakeSlip = m_brakeSlip;
        }
        else
        {
            m_frontBrakeSlip = m_brakeSlip;
            m_rearBrakeSlip = m_brakeSlip * brakeBalance * 2.0f;
        }
        ApplyTraction(WheelFL, m_frontMotorSlip, m_frontBrakeSlip, 0.0f);
        ApplyTraction(WheelFR, m_frontMotorSlip, m_frontBrakeSlip, 0.0f);
        ApplyTraction(WheelRL, m_rearMotorSlip, m_rearBrakeSlip, handbrakeInput);
        ApplyTraction(WheelRR, m_rearMotorSlip, m_rearBrakeSlip, handbrakeInput);
        if (autoSteerLevel > 0.0f)
        {
            float peakSlip = WheelFL.getSidewaysPeakSlip();
            float forwardSpeed = Mathf.Abs(tr.InverseTransformDirection(rigidbody.velocity).z * autoSteerLevel);
            if (forwardSpeed > peakSlip)
                m_steerAngleMax = 90.0f - Mathf.Acos(peakSlip / forwardSpeed) * Mathf.Rad2Deg;
            else
                m_steerAngleMax = steerMax;
        }
        else
            m_steerAngleMax = steerMax;
        WheelFL.getWheelCollider().steerAngle = m_steerL;
        WheelFR.getWheelCollider().steerAngle = m_steerR;
        if (!serviceMode)
        {
            float Cdrag = 0.5f * airDrag * 1.29f;
            float Crr = frictionDrag * Cdrag;
            Vector3 Fdrag = -Cdrag * rigidbody.velocity * rigidbody.velocity.magnitude;
            Vector3 Frr = -Crr * rigidbody.velocity;
            rigidbody.AddForce(Fdrag + Frr);
        }
        if (AntiRollFront)
            AntiRollFront.AntiRollFactor = antiRollLevel;
        if (AntiRollRear)
            AntiRollRear.AntiRollFactor = antiRollLevel;
    }
    private void Update()
    {
        //if (readUserInput)
            //m_brakeRelease = CarMain.SendInput(this, reverseRequiresStop, m_brakeRelease);
        CalculateSteerAngles();
    }
    private void ApplyEnabled(CarWheel Wheel, bool enable)
    {
        if (Wheel != null)
            Wheel.enabled = enable;
    }
    private void ApplyCommonParameters(CarWheel Wheel)
    {
        Wheel.ForwardWheelFriction = ForwardWheelFriction;
        Wheel.SidewaysWheelFriction = SidewaysWheelFriction;
        Wheel.brakeForceFactor = brakeForceFactor;
        Wheel.motorForceFactor = motorForceFactor;
        Wheel.performancePeak = motorPerformancePeak;
        Wheel.sidewaysDriftFriction = sidewaysDriftFriction;
        Wheel.staticFrictionMax = staticFrictionMax;
        Wheel.serviceMode = serviceMode;
        Wheel.optimized = optimized;
    }
    private void ApplyFrontParameters(CarWheel Wheel) { Wheel.sidewaysForceFactor = frontSidewaysGrip; }
    private void ApplyRearParameters(CarWheel Wheel) { Wheel.sidewaysForceFactor = rearSidewaysGrip; }
    private void ApplyTraction(CarWheel Wheel, float motorSlip, float brakeSlip, float handBrakeInput)
    {
        float slipPeak = Wheel.getForwardPeakSlip();
        float slipMax = Wheel.getForwardMaxSlip();
        var Hit = new WheelHit();
        float slip;
        float motor = Mathf.Abs(motorSlip);
        bool grounded = Wheel.getWheelCollider().GetGroundHit(out Hit);
        if (grounded)
        {
            Quaternion steerRot = Quaternion.AngleAxis(Wheel.getWheelCollider().steerAngle, Wheel.transform.up);
            Vector3 wheelDir = steerRot * Wheel.transform.forward;
            Vector3 pointV = rigidbody.GetPointVelocity(Hit.point);
            if (Hit.collider.attachedRigidbody)
                pointV -= Hit.collider.attachedRigidbody.GetPointVelocity(Hit.point);
            float v = Mathf.Abs(Vector3.Dot(pointV, wheelDir));
            if (v + slipPeak <= motorMax)
            {
                slip = motor - v;
                if (slip < 0)
                    slip = 0;
                else if (autoMotorMax && slip > slipPeak)
                    slip = slipPeak;
            }
            else
            {
                float maxSlip;
                if (tractionV3)
                    maxSlip = Mathf.Lerp(slipPeak, 0, Mathf.InverseLerp(motorMax - slipPeak, maxSpeed, v));
                else
                    maxSlip = slipPeak;
                slip = maxSlip * motor / motorMax;
            }
            if (motorSlip < 0)
                slip = -slip;
        }
        else
            slip = motorSlip;
        if (autoBrakeMax && brakeSlip > slipPeak)
            brakeSlip = slipPeak;
        brakeSlip = Mathf.Max(brakeSlip, handBrakeInput * slipMax);
        if (motorInput == 0.0f)
            brakeSlip += rollingFrictionSlip / brakeForceFactor;
        if (!grounded)
        {
            float omega = Wheel.getWheelCollider().rpm * Mathf.Deg2Rad;
            brakeSlip += omega * omega * 0.0008f / brakeForceFactor;
        }
        Wheel.motorInput = slip;
        Wheel.brakeInput = brakeSlip;
    }
    private void CalculateSteerAngles()
    {
        float B = (WheelFL.transform.position - WheelFR.transform.position).magnitude;
        float H = (WheelFR.transform.position - WheelRR.transform.position).magnitude;
        if (steerInput > 0.0f)
        {
            m_steerR = steerMax * steerInput;
            if (m_steerR > m_steerAngleMax)
                m_steerR = m_steerAngleMax;
            m_steerL = Mathf.Rad2Deg * Mathf.Atan(1.0f / (Mathf.Tan((90 - m_steerR) * Mathf.Deg2Rad) + B / H));
        }
        else if (steerInput < 0.0f)
        {
            m_steerL = steerMax * steerInput;
            if (m_steerL < -m_steerAngleMax)
                m_steerL = -m_steerAngleMax;
            m_steerR = -Mathf.Rad2Deg * Mathf.Atan(1.0f / (Mathf.Tan((90 + m_steerL) * Mathf.Deg2Rad) + B / H));
        }
        else
        {
            m_steerL = 0;
            m_steerR = 0;
        }
    }
}