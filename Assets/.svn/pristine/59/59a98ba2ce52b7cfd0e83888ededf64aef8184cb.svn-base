using UnityEngine;
using System.Collections;
public class CarCameras : MonoBehaviour
{
    public bool showFixedCams = false;
    public int startupFixedCam = 0;
    public Camera[] FixedCameras;
    public Camera MirrorLeft;
    public Camera MirrorRight;
    public Camera MirrorRear;
    public Transform CameraLookAtPoint;
    public Transform DriverFront;
    public float viewDistance = 10.0f;
    public float viewHeight = 3.5f;
    public float viewDamping = 3.0f;
    public float viewMinDistance = 3.8f;
    public float viewMinHeight = 0.0f;
    private int m_currentFixedCam;
    private Vector3 m_DriverViewAngles;
    public Vector3 getDriverViewAngles()
    {
        return m_DriverViewAngles;
    }
    void Start()
    {
        m_currentFixedCam = startupFixedCam;
        if (m_currentFixedCam >= FixedCameras.Length)
            m_currentFixedCam = -1;
        for (int i = 0; i < FixedCameras.Length; i++)
            FixedCameras[i].enabled = false;
        if (MirrorLeft)
            MirrorLeft.enabled = false;
        if (MirrorRight)
            MirrorRight.enabled = false;
        if (MirrorRear)
            MirrorRear.enabled = false;
        if (DriverFront)
        {
            CamFreeView scrDriverMove = DriverFront.GetComponent<CamFreeView>() as CamFreeView;
            if (scrDriverMove)
                scrDriverMove.enabled = false;
            m_DriverViewAngles = DriverFront.localEulerAngles;
        }
    }
    public void Next()
    {
        if (FixedCameras.Length == 0)
            return;
        if (m_currentFixedCam >= 0)
        {
            FixedCameras[m_currentFixedCam++].enabled = false;
            if (m_currentFixedCam < FixedCameras.Length)
                FixedCameras[m_currentFixedCam].enabled = true && showFixedCams;
            else
                m_currentFixedCam = -1;
        }
        else
        {
            m_currentFixedCam = 0;
            FixedCameras[m_currentFixedCam].enabled = true && showFixedCams;
        }
    }
    void Update()
    {
        if (m_currentFixedCam >= 0)
        {
            if (showFixedCams && !FixedCameras[m_currentFixedCam].enabled)
                FixedCameras[m_currentFixedCam].enabled = true;
            if (!showFixedCams && FixedCameras[m_currentFixedCam].enabled)
                FixedCameras[m_currentFixedCam].enabled = false;
        }
    }
}
