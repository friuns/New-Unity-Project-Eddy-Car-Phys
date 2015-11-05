using UnityEngine;
using System.Collections;
public class CarExternalInputRandom : CarExternalInput
{
    public float steerInterval = 2.0f;
    public float steerIntervalMargin = 1.0f;
    public float steerSpeed = 4.0f;
    public float steerStraightRandom = 0.4f;
    public float throttleInterval = 5.0f;
    public float throttleIntervalMargin = 2.0f;
    public float throttleSpeed = 3.0f;
    public float throttleForwardRandom = 0.8f;
    public float throttleReverseRandom = 0.6f;
    private float m_targetSteer = 0.0f;
    private float m_nextSteerTime = 0.0f;
    private float m_targetThrottle = 0.0f;
    private float m_targetBrake = 0.0f;
    private int m_targetGear = 0;
    private float m_nextThrottleTime = 0.0f;
    void Awake()
    {
    }
    void Update()
    {
        if (Time.time > m_nextSteerTime)
        {
            if (Random.value < steerStraightRandom)
                m_targetSteer = 0.0f;
            else
                m_targetSteer = Random.Range(-1.0f, 1.0f);
            m_nextSteerTime = Time.time + steerInterval + Random.Range(-steerIntervalMargin, steerIntervalMargin);
        }
        if (Time.time > m_nextThrottleTime)
        {
            float forwardRandom = throttleForwardRandom;
            float speed = rigidbody.velocity.magnitude;
            if (speed < 0.1f && m_targetBrake < 0.001f && m_targetGear != -1)
                forwardRandom *= 0.4f;
            if (Random.value < forwardRandom)
            {
                m_targetGear = 1;
                m_targetThrottle = Random.value;
                m_targetBrake = 0.0f;
            }
            else
            {
                if (speed < 0.5f)
                {
                    m_targetGear = -1;
                    m_targetBrake = 0.0f;
                    m_targetThrottle = Random.value;
                }
                else
                {
                    m_targetThrottle = 0.0f;
                    m_targetBrake = Random.value;
                }
            }
            m_nextThrottleTime = Time.time + throttleInterval + Random.Range(-throttleIntervalMargin, throttleIntervalMargin);
        }
        m_CarControl.steerInput = Mathf.MoveTowards(m_CarControl.steerInput, m_targetSteer, steerSpeed * Time.deltaTime);
        m_CarControl.motorInput = Mathf.MoveTowards(m_CarControl.motorInput, m_targetThrottle, throttleSpeed * Time.deltaTime);
        m_CarControl.brakeInput = m_targetBrake;
        m_CarControl.gearInput = m_targetGear;
        m_CarControl.handbrakeInput = 0.0f;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (enabled && collision.contacts.Length > 0)
        {
            float colRatio = Vector3.Dot(transform.forward, collision.contacts[0].normal);
            if (colRatio > 0.8f || colRatio < -0.8f)
                m_nextThrottleTime -= throttleInterval * 0.5f;
            if (colRatio > -0.4f && colRatio < 0.4f)
                m_nextSteerTime -= steerInterval * 0.5f;
        }
    }
}
