using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[System.Serializable]
public class WheelVisualData
{
    public float colliderOffset = 0.0f;
    public float skidmarkOffset = 0.0f;
    public Vector3 wheelVelocity = Vector3.zero;
    public Vector3 groundSpeed = Vector3.zero;
    public float angularVelocity = 0.0f;
    public float lastSuspensionForce = 0.0f;
    public float suspensionStress = 0.0f;
    public int lastSkidmark = -1;
    public float skidmarkTime = 0.0f;
    public float skidSmokeTime = Time.time;
    public Vector3 skidSmokePos = Vector3.zero;
    public float skidSmokeIntensity = 0.0f;
    public float skidValue = 0.0f;
}
public partial class CarVisuals : bs
{
    public Transform PivotFL;
    public Transform PivotFR;
    public Transform PivotRL;
    public Transform PivotRR;
    public Transform MeshFL;
    public Transform MeshFR;
    public Transform MeshRL;
    public Transform MeshRR;
    public Transform SteeringWheel;
    public Collider[] ignoredColliders;
    public float forwardSkidmarksBegin = 1.5f;
    public float forwardSkidmarksRange = 1.0f;
    public float sidewaysSkidmarksBegin = 1.5f;
    public float sidewaysSkidmarksRange = 1.0f;
    public float skidmarksWidth = 0.275f;
    public float skidmarksOffset = 0.0f;
    public bool alwaysDrawSkidmarks = false;
    public float forwardSmokeBegin = 5.0f;
    public float forwardSmokeRange = 3.0f;
    public float sidewaysSmokeBegin = 4.0f;
    public float sidewaysSmokeRange = 3.0f;
    public float smokeStartTime = 2.0f;
    public float smokePeakTime = 8.0f;
    public float smokeMaxTime = 10.0f;
    public float wheelGroundedBias = 0.02f;
    public int steeringWheelMax = 520;
    public float impactThreeshold = 0.6f;
    public float impactInterval = 0.2f;
    public float impactIntervalRandom = 0.4f;
    public float impactMinSpeed = 2.0f;
    public bool disableRaycast = false;
    public bool disableWheelVisuals = false;
    [HideInInspector]
    public float spinRateFL = 0.0f;
    [HideInInspector]
    public float spinRateFR = 0.0f;
    [HideInInspector]
    public float spinRateRL = 0.0f;
    [HideInInspector]
    public float spinRateRR = 0.0f;
    [HideInInspector]
    public float skidValueFL = 0.0f;
    [HideInInspector]
    public float skidValueFR = 0.0f;
    [HideInInspector]
    public float skidValueRL = 0.0f;
    [HideInInspector]
    public float skidValueRR = 0.0f;
    [HideInInspector]
    public float suspensionStressFL = 0.0f;
    [HideInInspector]
    public float suspensionStressFR = 0.0f;
    [HideInInspector]
    public float suspensionStressRL = 0.0f;
    [HideInInspector]
    public float suspensionStressRR = 0.0f;
    [HideInInspector]
    public Vector3 localImpactPosition = Vector3.zero;
    [HideInInspector]
    public Vector3 localImpactVelocity = Vector3.zero;
    [HideInInspector]
    public bool localImpactSoftSurface = false;
    [HideInInspector]
    public Vector3 localDragPosition = Vector3.zero;
    [HideInInspector]
    public Vector3 localDragVelocity = Vector3.zero;
    [HideInInspector]
    public bool localDragSoftSurface = false;
    [HideInInspector]
    public Vector3 localDragPositionDiscrete = Vector3.zero;
    [HideInInspector]
    public Vector3 localDragVelocityDiscrete = Vector3.zero;
    private CarControl m_Car;
    private Skidmarks m_skidmarks;
    private ParticleEmitter m_skidSmoke;
    private WheelVisualData[] m_wheelData;
    void Awake()
    {
        var ts = tr.GetComponentsInChildren<Transform>();
        m_Car = GetComponent<CarControl>() as CarControl;
        if (!MeshFL)
        {
            m_Car.CenterOfMass = ts.FirstOrDefault(a => a.name == "CoM");
            m_Car.WheelFL = ts.FirstOrDefault(a => a.name == "WheelFL").GetComponent<CarWheel>();
            m_Car.WheelFR = ts.FirstOrDefault(a => a.name == "WheelFR").GetComponent<CarWheel>();
            m_Car.WheelRL = ts.FirstOrDefault(a => a.name == "WheelRL").GetComponent<CarWheel>();
            m_Car.WheelRR = ts.FirstOrDefault(a => a.name == "WheelRR").GetComponent<CarWheel>();

            MeshFL = ts.FirstOrDefault(a => a.name == "fl");
            MeshFR = ts.FirstOrDefault(a => a.name == "fr");
            MeshRL = ts.FirstOrDefault(a => a.name == "rl");
            MeshRR = ts.FirstOrDefault(a => a.name == "rr");
        }

        if (!PivotFL)
        {
            PivotFL = InitWheel(MeshFL, m_Car.WheelFL.transform.position);
            PivotFR = InitWheel(MeshFR, m_Car.WheelFR.transform.position);
            PivotRL = InitWheel(MeshRL, m_Car.WheelRL.transform.position);
            PivotRR = InitWheel(MeshRR, m_Car.WheelRR.transform.position);
        }
    }
    private Transform InitWheel(Transform b, Vector3 pos)
    {
        var a = new GameObject(b.name + " pivot").transform;

        a.position = b.position;
        a.rotation = tr.rotation;
        a.parent = b.transform.parent;
        b.transform.parent = a;
        //StartCoroutine(AddMethod(delegate { b.position = new Vector3(old.x, b.position.y, old.z); }));
        return a;
    }

    void Start()
    {


        m_skidmarks = bs._Game.skidmarks; //FindObjectOfType(typeof(Skidmarks)) as Skidmarks;

        m_skidSmoke = bs._Game.m_skidSmoke;

        m_wheelData = new WheelVisualData[4];
        for (int i = 0; i < 4; i++)
            m_wheelData[i] = new WheelVisualData();

        m_wheelData[0].colliderOffset = tr.InverseTransformDirection(PivotFL.position - m_Car.WheelFL.transform.position).x;
        m_wheelData[1].colliderOffset = tr.InverseTransformDirection(PivotFR.position - m_Car.WheelFR.transform.position).x;
        m_wheelData[2].colliderOffset = tr.InverseTransformDirection(PivotRL.position - m_Car.WheelRL.transform.position).x;
        m_wheelData[3].colliderOffset = tr.InverseTransformDirection(PivotRR.position - m_Car.WheelRR.transform.position).x;
        m_wheelData[0].skidmarkOffset = m_wheelData[0].colliderOffset - skidmarksOffset;
        m_wheelData[1].skidmarkOffset = m_wheelData[1].colliderOffset + skidmarksOffset;
        m_wheelData[2].skidmarkOffset = m_wheelData[2].colliderOffset - skidmarksOffset;
        m_wheelData[3].skidmarkOffset = m_wheelData[3].colliderOffset + skidmarksOffset;

        MeshFL.localPosition += tr.InverseTransformDirection(PivotFL.position - m_Car.WheelFL.transform.position).z * Vector3.forward;
        MeshFR.localPosition += tr.InverseTransformDirection(PivotFR.position - m_Car.WheelFR.transform.position).z * Vector3.forward;
        MeshRL.localPosition += tr.InverseTransformDirection(PivotRL.position - m_Car.WheelRL.transform.position).z * Vector3.forward;
        MeshRR.localPosition += tr.InverseTransformDirection(PivotRR.position - m_Car.WheelRR.transform.position).z * Vector3.forward;
    }
    private bool isEnabled { get { return (m_Car.pl == bs._Player || bs.highQuality) && bs._AutoQuality.skidmarks; } }
    void Update()
    {
        DoWheelVisuals(m_Car.WheelFL, MeshFL, PivotFL, m_wheelData[0]);
        DoWheelVisuals(m_Car.WheelFR, MeshFR, PivotFR, m_wheelData[1]);
        DoWheelVisuals(m_Car.WheelRL, MeshRL, PivotRL, m_wheelData[2]);
        DoWheelVisuals(m_Car.WheelRR, MeshRR, PivotRR, m_wheelData[3]);
        spinRateFL = m_wheelData[0].angularVelocity;
        spinRateFR = m_wheelData[1].angularVelocity;
        spinRateRL = m_wheelData[2].angularVelocity;
        spinRateRR = m_wheelData[3].angularVelocity;
        skidValueFL = m_wheelData[0].skidValue;
        skidValueFR = m_wheelData[1].skidValue;
        skidValueRL = m_wheelData[2].skidValue;
        skidValueRR = m_wheelData[3].skidValue;
        suspensionStressFL = m_wheelData[0].suspensionStress;
        suspensionStressFR = m_wheelData[1].suspensionStress;
        suspensionStressRL = m_wheelData[2].suspensionStress;
        suspensionStressRR = m_wheelData[3].suspensionStress;
        ProcessImpacts();
        ProcessDrags(Vector3.zero, Vector3.zero, false);
        float steerL = m_Car.getSteerL();
        float steerR = m_Car.getSteerR();
        foreach (Collider coll in ignoredColliders)
            coll.gameObject.layer = 2;
        DoWheelPosition(m_Car.WheelFL, PivotFL, steerL, m_wheelData[0]);
        DoWheelPosition(m_Car.WheelFR, PivotFR, steerR, m_wheelData[1]);
        DoWheelPosition(m_Car.WheelRL, PivotRL, 0, m_wheelData[2]);
        DoWheelPosition(m_Car.WheelRR, PivotRR, 0, m_wheelData[3]);
        foreach (Collider coll in ignoredColliders)
            coll.gameObject.layer = Layer.car;
        if (SteeringWheel)
        {
            float currentAngle = m_Car.steerInput >= 0.0 ? steerR : steerL;
            Vector3 t = SteeringWheel.localEulerAngles;
            t.z = -steeringWheelMax * currentAngle / m_Car.steerMax;
            SteeringWheel.localEulerAngles = t;
        }
    }
    private static bool IsHardSurface(Collider col)
    {
        return !col.sharedMaterial || col.attachedRigidbody != null;
    }
    private static bool IsStaticSurface(Collider col)
    {
        return !col.attachedRigidbody;
    }
    void DoWheelVisuals(CarWheel Wheel, Transform Graphic, Transform Pivot, WheelVisualData wheelData)
    {
        WheelCollider WheelCol;
        WheelHit Hit = new WheelHit();
        float deltaT;
        float Skid;
        float wheelSpeed;
        float forwardSkidValue;
        float sidewaysSkidValue;
        WheelCol = Wheel.getWheelCollider();
        if (!disableWheelVisuals && WheelCol.GetGroundHit(out Hit))
        {
            wheelData.suspensionStress = Hit.force - wheelData.lastSuspensionForce;
            wheelData.lastSuspensionForce = Hit.force;
            wheelData.wheelVelocity = rigidbody.GetPointVelocity(Hit.point);
            if (Hit.collider.attachedRigidbody)
                wheelData.wheelVelocity -= Hit.collider.attachedRigidbody.GetPointVelocity(Hit.point);
            wheelData.groundSpeed = Pivot.transform.InverseTransformDirection(wheelData.wheelVelocity);
            wheelData.groundSpeed.y = 0.0f;
            float frictionPeak = Wheel.getForwardPeakSlip();
            float frictionMax = Wheel.getForwardMaxSlip();
            float MotorSlip = Wheel.motorInput;
            float BrakeSlip = Wheel.brakeInput;
            float TorqueSlip = Mathf.Abs(MotorSlip) - Mathf.Max(BrakeSlip);
            if (TorqueSlip >= 0)
            {
                Skid = TorqueSlip - frictionPeak;
                if (Skid > 0)
                {
                    wheelSpeed = Mathf.Abs(wheelData.groundSpeed.z) + Skid;
                    if (MotorSlip < 0)
                        wheelSpeed = -wheelSpeed;
                }
                else
                    wheelSpeed = wheelData.groundSpeed.z;
            }
            else
            {
                Skid = Mathf.InverseLerp(frictionMax, frictionPeak, -TorqueSlip);
                wheelSpeed = wheelData.groundSpeed.z * Skid;
            }
            if (m_Car.serviceMode)
                wheelSpeed = RpmToMs(WheelCol.rpm, WheelCol.radius * Wheel.transform.lossyScale.y);
            wheelData.angularVelocity = wheelSpeed / (WheelCol.radius * Wheel.transform.lossyScale.y);
            if (wheelData.lastSkidmark != -1 && wheelData.skidmarkTime < Time.fixedDeltaTime)
                wheelData.skidmarkTime += Time.deltaTime;
            else
            {
                bool isHardSurface = IsHardSurface(Hit.collider);
                bool isStaticSurface = IsStaticSurface(Hit.collider);
                deltaT = wheelData.skidmarkTime;
                if (deltaT == 0.0f)
                    deltaT = Time.deltaTime;
                wheelData.skidmarkTime = 0.0f;
                forwardSkidValue = Mathf.InverseLerp(forwardSkidmarksBegin, forwardSkidmarksBegin + forwardSkidmarksRange, Mathf.Abs(Wheel.getForwardSlipRatio()));
                sidewaysSkidValue = Mathf.InverseLerp(sidewaysSkidmarksBegin, sidewaysSkidmarksBegin + sidewaysSkidmarksRange, Mathf.Abs(Wheel.getSidewaysSlipRatio()));
                wheelData.skidValue = Mathf.Max(forwardSkidValue, sidewaysSkidValue);
                float skidmarksLock = Mathf.Min(forwardSkidmarksBegin, 2.0f);
                if (TorqueSlip < 0 && Mathf.Abs(Wheel.getForwardSlipRatio()) >= skidmarksLock)
                {
                    forwardSkidValue = Mathf.InverseLerp(forwardSkidmarksBegin, forwardSkidmarksBegin + forwardSkidmarksRange, Mathf.Abs(wheelData.groundSpeed.z + skidmarksLock));
                    wheelData.skidValue = Mathf.Max(forwardSkidValue, sidewaysSkidValue);
                }
                if (isHardSurface)
                {
                    float downForceRatio = Mathf.Clamp01(Hit.force / WheelCol.suspensionSpring.spring);
                    wheelData.skidValue *= downForceRatio;
                    float thisSkidMark = wheelData.skidValue;
                    if (alwaysDrawSkidmarks && wheelData.groundSpeed.magnitude > 0.01f)
                        thisSkidMark = downForceRatio;
                    if (thisSkidMark > 0.0f)
                    {
                        if (isStaticSurface && m_skidmarks && isEnabled)
                        {
                            wheelData.lastSkidmark = m_skidmarks.AddSkidMark(Hit.point + wheelData.wheelVelocity * deltaT + tr.right * wheelData.skidmarkOffset,
                                                                                    Hit.normal,
                                                                                    thisSkidMark,
                                                                                    skidmarksWidth,
                                                                                    wheelData.lastSkidmark);
                        }
                        else
                            wheelData.lastSkidmark = -1;
                    }
                    else
                        wheelData.lastSkidmark = -1;
                }
                else
                {
                    wheelData.skidValue = -Mathf.Max(Mathf.Abs(wheelData.angularVelocity) * WheelCol.radius * Wheel.transform.lossyScale.y, wheelData.groundSpeed.magnitude);
                    wheelData.lastSkidmark = -1;
                }
                if (isHardSurface)
                {
                    forwardSkidValue = Mathf.InverseLerp(forwardSmokeBegin, forwardSmokeBegin + forwardSmokeRange, Mathf.Abs(Wheel.getForwardSlipRatio()));
                    sidewaysSkidValue = Mathf.InverseLerp(sidewaysSmokeBegin, sidewaysSmokeBegin + sidewaysSmokeRange, Mathf.Abs(Wheel.getSidewaysSlipRatio())) * Wheel.getDriftFactor();
                }
                else
                {
                    forwardSkidValue = 0.0f;
                    sidewaysSkidValue = 0.0f;
                }
                float skidSmokeValue = Mathf.Max(forwardSkidValue, sidewaysSkidValue);
                float smokeIntensity = wheelData.skidSmokeIntensity;
                if (sidewaysSkidValue > 0.0f && smokeIntensity < sidewaysSkidValue * smokePeakTime)
                    smokeIntensity = sidewaysSkidValue * smokePeakTime;
                float smokeLock = Mathf.Min(forwardSmokeBegin, 2.0f);
                if (isHardSurface && TorqueSlip < 0 && Mathf.Abs(Wheel.getForwardSlipRatio()) >= smokeLock)
                {
                    forwardSkidValue = Mathf.InverseLerp(forwardSmokeBegin, forwardSmokeBegin + forwardSmokeRange, Mathf.Abs(wheelData.groundSpeed.z + smokeLock));
                    skidSmokeValue = Mathf.Max(forwardSkidValue, sidewaysSkidValue);
                    if (smokeIntensity < smokeStartTime)
                        smokeIntensity = smokeStartTime;
                }
                if (skidSmokeValue > 0.0f)
                    smokeIntensity += deltaT;
                else
                    smokeIntensity -= deltaT;
                if (smokeIntensity >= smokeMaxTime)
                    smokeIntensity = smokeMaxTime;
                else if (smokeIntensity < 0.0f)
                    smokeIntensity = 0.0f;
                skidSmokeValue *= Mathf.InverseLerp(smokeStartTime, smokePeakTime, smokeIntensity);
                Vector3 smokePos = Hit.point + tr.up * WheelCol.radius * Wheel.transform.lossyScale.y * 0.5f + tr.right * wheelData.skidmarkOffset;
                if (skidSmokeValue > 0.0f && isEnabled)
                {
                    float emission = Random.Range(m_skidSmoke.minEmission, m_skidSmoke.maxEmission);
                    float lastParticleCount = wheelData.skidSmokeTime * emission;
                    float currentParticleCount = Time.time * emission;
                    int numParticles = Mathf.CeilToInt(currentParticleCount) - Mathf.CeilToInt(lastParticleCount);
                    int lastParticle = Mathf.CeilToInt(lastParticleCount);
                    Vector3 Vel = WheelCol.transform.TransformDirection(m_skidSmoke.localVelocity) + m_skidSmoke.worldVelocity;
                    Vel += Pivot.forward * (wheelData.groundSpeed.z - wheelSpeed) * 0.125f;
                    Vel += wheelData.wheelVelocity * m_skidSmoke.emitterVelocityScale;
                    for (int i = 0; i < numParticles; i++)
                    {
                        float particleTime = Mathf.InverseLerp(lastParticleCount, currentParticleCount, lastParticle + i);
                        Vector3 PosRnd = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
                        Vector3 VelRnd = new Vector3(Random.Range(-m_skidSmoke.rndVelocity.x, m_skidSmoke.rndVelocity.x), Random.Range(-m_skidSmoke.rndVelocity.y, m_skidSmoke.rndVelocity.y), Random.Range(-m_skidSmoke.rndVelocity.z, m_skidSmoke.rndVelocity.z));
                        float Size = Random.Range(m_skidSmoke.minSize, m_skidSmoke.maxSize);
                        float Energy = Random.Range(m_skidSmoke.minEnergy, m_skidSmoke.maxEnergy);
                        float Rotation = m_skidSmoke.rndRotation ? Random.value * 360 : 0;
                        float RotVel = m_skidSmoke.angularVelocity + Random.Range(-m_skidSmoke.rndAngularVelocity, m_skidSmoke.rndAngularVelocity);
                        m_skidSmoke.Emit(Vector3.Lerp(wheelData.skidSmokePos, smokePos, particleTime) + PosRnd, Vel + VelRnd, Size * 1, Energy * skidSmokeValue, new Color(1, 1, 1, 1), Rotation, RotVel);
                    }
                }
                wheelData.skidSmokeTime = Time.time;
                wheelData.skidSmokePos = smokePos;
                wheelData.skidSmokeIntensity = smokeIntensity;
            }
        }
        else
        {
            wheelData.angularVelocity = WheelCol.rpm * 6 * Mathf.Deg2Rad;
            wheelData.suspensionStress = 0.0f - wheelData.lastSuspensionForce;
            wheelData.lastSuspensionForce = 0.0f;
            wheelData.skidValue = 0.0f;
            wheelData.lastSkidmark = -1;
            wheelData.skidSmokeTime = Time.time;
            wheelData.skidSmokePos = Wheel.transform.position - Wheel.transform.up * ((WheelCol.suspensionDistance + WheelCol.radius * 0.5f) * Wheel.transform.lossyScale.y) + tr.right * wheelData.skidmarkOffset;
            wheelData.skidSmokeIntensity -= Time.deltaTime;
        }
        Graphic.Rotate(wheelData.angularVelocity * Mathf.Rad2Deg * Time.deltaTime, 0.0f, 0.0f);
    }
    void DoWheelPosition(CarWheel Wheel, Transform WheelMesh, float steerAngle, WheelVisualData wheelData)
    {
        Vector3 hitPoint = Vector3.zero;
        bool grounded = false;
        WheelCollider WheelCol = Wheel.getWheelCollider();
        if (!disableRaycast)
        {
            RaycastHit HitR = new RaycastHit();
            if (Physics.Raycast(Wheel.transform.position, -Wheel.transform.up, out HitR, (WheelCol.suspensionDistance + WheelCol.radius) * Wheel.transform.lossyScale.y, Layer.levelMask))
            {
                hitPoint = HitR.point + Wheel.transform.up * (WheelCol.radius * Wheel.transform.lossyScale.y - wheelGroundedBias) + tr.right * wheelData.colliderOffset;
                grounded = true;
            }
        }
        else
        {
            WheelHit HitW = new WheelHit();
            if (WheelCol.GetGroundHit(out HitW))
            {
                hitPoint = HitW.point + Wheel.transform.up * (WheelCol.radius * Wheel.transform.lossyScale.y - wheelGroundedBias) + tr.right * wheelData.colliderOffset;
                grounded = true;
            }
        }
        if (grounded)
            WheelMesh.position = hitPoint;
        else
            WheelMesh.position = Wheel.transform.position - Wheel.transform.up * (WheelCol.suspensionDistance * Wheel.transform.lossyScale.y + wheelGroundedBias) + tr.right * wheelData.colliderOffset;
        Vector3 t = WheelMesh.localEulerAngles;
        t.y = Wheel.transform.localEulerAngles.y + steerAngle;
        t.z = Wheel.transform.localEulerAngles.z;
        WheelMesh.localEulerAngles = t;
    }
    float RpmToMs(float Rpm, float Radius)
    {
        return Mathf.PI * Radius * Rpm / 30.0f;
    }
    float MsToRpm(float Ms, float Radius)
    {
        return 30.0f * Ms / (Mathf.PI * Radius);
    }
    private int m_sumImpactCount = 0;
    private int m_sumImpactCountSoft = 0;
    private Vector3 m_sumImpactPosition = Vector3.zero;
    private Vector3 m_sumImpactVelocity = Vector3.zero;
    private float m_lastImpactTime = 0.0f;
    private void ProcessImpacts()
    {
        bool bCanProcessCollisions = Time.time - m_lastImpactTime >= impactInterval;
        if (bCanProcessCollisions && m_sumImpactCount > 0)
        {
            localImpactPosition = m_sumImpactPosition / m_sumImpactCount;
            localImpactVelocity = m_sumImpactVelocity;
            localImpactSoftSurface = m_sumImpactCountSoft > m_sumImpactCount / 2;
            localDragPositionDiscrete = localDragPosition;
            localDragVelocityDiscrete = localDragVelocity;
            m_sumImpactCount = 0;
            m_sumImpactCountSoft = 0;
            m_sumImpactPosition = Vector3.zero;
            m_sumImpactVelocity = Vector3.zero;
            m_lastImpactTime = Time.time + impactInterval * Random.Range(-impactIntervalRandom, impactIntervalRandom);
        }
        else
        {
            localImpactPosition = Vector3.zero;
            localImpactVelocity = Vector3.zero;
            localDragVelocityDiscrete = Vector3.zero;
        }
    }
    private void ProcessDrags(Vector3 dragPosition, Vector3 dragVelocity, bool dragSoftSurface)
    {
        if (dragVelocity.sqrMagnitude > 0.001f)
        {
            localDragPosition = Vector3.Lerp(localDragPosition, dragPosition, 10.0f * Time.deltaTime);
            localDragVelocity = Vector3.Lerp(localDragVelocity, dragVelocity, 20.0f * Time.deltaTime);
            localDragSoftSurface = dragSoftSurface;
        }
        else
        {
            localDragVelocity = Vector3.Lerp(localDragVelocity, Vector3.zero, 10.0f * Time.deltaTime);
        }
    }
    private void ProcessContacts(Collision col, bool forceImpact)
    {
        int colImpactCount = 0;
        int colImpactCountSoft = 0;
        Vector3 colImpactPosition = Vector3.zero;
        Vector3 colImpactVelocity = Vector3.zero;
        int colDragCount = 0;
        int colDragCountSoft = 0;
        Vector3 colDragPosition = Vector3.zero;
        Vector3 colDragVelocity = Vector3.zero;
        float sqrImpactSpeed = impactMinSpeed * impactMinSpeed;
        foreach (ContactPoint contact in col.contacts)
        {
            Collider thisCol = contact.thisCollider;
            Collider otherCol = contact.otherCollider;
            if (thisCol == null || thisCol.attachedRigidbody != rigidbody)
            {
                thisCol = contact.otherCollider;
                otherCol = contact.thisCollider;
            }
            if (thisCol.GetType() != typeof(WheelCollider) && otherCol.GetType() != typeof(WheelCollider))
            {
                Vector3 V = rigidbody.GetPointVelocity(contact.point);
                if (V.magnitude > bs.settings.collDragSparks)
                    _Game.Emit(contact.point, contact.normal, _Game.sparks2);

                if (otherCol && otherCol.attachedRigidbody)
                    V -= otherCol.attachedRigidbody.GetPointVelocity(contact.point);
                float dragRatio = Vector3.Dot(V, contact.normal);


                if (dragRatio < -impactThreeshold || forceImpact && col.relativeVelocity.sqrMagnitude > sqrImpactSpeed)
                {
                    colImpactCount++;
                    colImpactPosition += contact.point;
                    colImpactVelocity += col.relativeVelocity;
                    if (otherCol && !IsHardSurface(otherCol))
                        colImpactCountSoft++;
                }
                else if (dragRatio < impactThreeshold)
                {

                    colDragCount++;
                    colDragPosition += contact.point;
                    colDragVelocity += V;
                    if (otherCol && !IsHardSurface(otherCol))
                        colDragCountSoft++;


                }
            }
        }
        if (colImpactCount > 0)
        {
            colImpactPosition /= colImpactCount;
            colImpactVelocity /= colImpactCount;
            m_sumImpactCount++;
            m_sumImpactPosition += tr.InverseTransformPoint(colImpactPosition);
            m_sumImpactVelocity += tr.InverseTransformDirection(colImpactVelocity);
            if (colImpactCountSoft > colImpactCount / 2)
                m_sumImpactCountSoft++;
        }
        if (colDragCount > 0)
        {
            colDragPosition /= colDragCount;
            colDragVelocity /= colDragCount;
            ProcessDrags(tr.InverseTransformPoint(colDragPosition), tr.InverseTransformDirection(colDragVelocity), colDragCountSoft > colDragCount / 2);
        }
    }
    static float Lin2Log(float value)
    {
        return Mathf.Log(Mathf.Abs(value) + 1) * Mathf.Sign(value);
    }
    static Vector3 Lin2Log(Vector3 value)
    {
        return Vector3.ClampMagnitude(value, Lin2Log(value.magnitude));
    }
}
