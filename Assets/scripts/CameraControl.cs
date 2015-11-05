using UnityEngine;
using System.Collections;
public class CameraControl : MonoBehaviour
{
    public Transform Target;
    public Transform Target2;
    public Camera MapCamera;
    public int DefaultCamera;
    public bool showMirrors = true;
    public GUITexture MirrorLeftTex;
    public GUITexture MirrorRightTex;
    public GUITexture MirrorRearTex;
    public bool enableImageEffects = true;
    public MonoBehaviour[] ImageEffects;
    private MonoBehaviour[] m_cameraScript;
    private int m_currCamera;
    private int m_numCameras = 6;
    private CamFixTo m_scrFixTo;
    public CamSmoothFollow m_scrSmoothFollow;
    private CamMouseOrbit m_scrMouseOrbit;
    private CamSmoothLookAt m_scrSmoothLookAt;
    private CamFreeView m_scrFreeView;
    private CamSmoothLookAtFromPos m_scrSmoothLookAtFromPos;
    private CamFreeView m_scrDriverFreeView;
    private CamSmoothFollow m_scrMapFollow;
    private Transform m_lastTarget;
    private bool m_lastShowMirrors;
    private bool m_lastImageEffects;
    private CarCameras m_targetCams;
    private void UpdateTarget()
    {
        if (m_targetCams)
        {
            if (m_targetCams.MirrorLeft)
                m_targetCams.MirrorLeft.enabled = false;
            if (m_targetCams.MirrorRight)
                m_targetCams.MirrorRight.enabled = false;
            if (m_targetCams.MirrorRear)
                m_targetCams.MirrorRear.enabled = false;
            if (m_scrDriverFreeView)
                m_scrDriverFreeView.enabled = false;
        }
        m_targetCams = Target.GetComponent<CarCameras>() as CarCameras;
        if (m_targetCams.DriverFront)
            m_scrDriverFreeView = m_targetCams.DriverFront.GetComponent<CamFreeView>() as CamFreeView;
        else
            m_scrDriverFreeView = null;
        m_scrFixTo.Pos = m_targetCams.DriverFront;
        m_scrSmoothFollow.target = m_targetCams.CameraLookAtPoint;
        m_scrSmoothFollow.distance = m_targetCams.viewDistance;
        m_scrSmoothFollow.height = m_targetCams.viewHeight;
        m_scrSmoothFollow.rotationDamping = m_targetCams.viewDamping;
        m_scrSmoothFollow.reset = true;
        m_scrMouseOrbit.target = m_targetCams.CameraLookAtPoint;
        m_scrMouseOrbit.distance = m_targetCams.viewDistance;
        m_scrMouseOrbit.distMinLimit = m_targetCams.viewMinDistance;
        m_scrMouseOrbit.yMinLimit = m_targetCams.viewMinHeight;
        m_scrSmoothLookAt.target = m_targetCams.CameraLookAtPoint;
        if (m_scrMapFollow)
            m_scrMapFollow.target = m_targetCams.CameraLookAtPoint;
        if (Target2)
        {
            CarCameras Target2Cameras;
            m_scrSmoothLookAtFromPos.pos = m_targetCams.CameraLookAtPoint;
            m_scrSmoothLookAtFromPos.positionZ = -m_targetCams.viewDistance;
            m_scrSmoothLookAtFromPos.positionY = m_targetCams.viewHeight / 2.0f;
            Target2Cameras = Target2.GetComponent<CarCameras>() as CarCameras;
            if (Target2Cameras)
                m_scrSmoothLookAtFromPos.target = Target2Cameras.CameraLookAtPoint;
            else
                m_scrSmoothLookAtFromPos.target = Target2;
        }
        else
        {
            if (m_currCamera == m_numCameras - 1)
                SwitchTo(DefaultCamera);
        }
        UpdateMirrors();
    }
    void ClearMirrorTexture(Camera Cam)
    {
        CameraClearFlags oldClearFlags = Cam.clearFlags;
        Color oldBackgroundColor = Cam.backgroundColor;
        Rect oldRect = Cam.rect;
        int oldCullingMask = Cam.cullingMask;
        Cam.clearFlags = CameraClearFlags.SolidColor;
        Cam.backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        Cam.cullingMask = 0;
        Cam.Render();
        Cam.clearFlags = oldClearFlags;
        Cam.backgroundColor = oldBackgroundColor;
        Cam.rect = oldRect;
        Cam.cullingMask = oldCullingMask;
    }
    void UpdateMirrors()
    {
        if (m_scrDriverFreeView)
            m_scrDriverFreeView.enabled = m_currCamera == 0;
        if (m_currCamera == 0 && showMirrors)
        {
            if (MirrorLeftTex)
                if (m_targetCams.MirrorLeft)
                {
                    m_targetCams.MirrorLeft.targetTexture = MirrorLeftTex.texture as RenderTexture;
                    m_targetCams.MirrorLeft.enabled = true;
                    MirrorLeftTex.enabled = true;
                    ClearMirrorTexture(m_targetCams.MirrorLeft);
                }
                else
                    MirrorLeftTex.enabled = false;
            if (MirrorRightTex)
                if (m_targetCams.MirrorRight)
                {
                    m_targetCams.MirrorRight.targetTexture = MirrorRightTex.texture as RenderTexture;
                    m_targetCams.MirrorRight.enabled = true;
                    MirrorRightTex.enabled = true;
                    ClearMirrorTexture(m_targetCams.MirrorRight);
                }
                else
                    MirrorRightTex.enabled = false;
            if (MirrorRearTex)
                if (m_targetCams.MirrorRear)
                {
                    m_targetCams.MirrorRear.targetTexture = MirrorRearTex.texture as RenderTexture;
                    m_targetCams.MirrorRear.enabled = true;
                    MirrorRearTex.enabled = true;
                    ClearMirrorTexture(m_targetCams.MirrorRear);
                }
                else
                    MirrorRearTex.enabled = false;
        }
        else
        {
            if (m_targetCams.MirrorLeft)
                m_targetCams.MirrorLeft.enabled = false;
            if (m_targetCams.MirrorRight)
                m_targetCams.MirrorRight.enabled = false;
            if (m_targetCams.MirrorRear)
                m_targetCams.MirrorRear.enabled = false;
            if (MirrorLeftTex)
                MirrorLeftTex.enabled = false;
            if (MirrorRightTex)
                MirrorRightTex.enabled = false;
            if (MirrorRearTex)
                MirrorRearTex.enabled = false;
        }
    }
    void Start()
    {
        m_currCamera = DefaultCamera;
        m_lastShowMirrors = showMirrors;
        m_lastImageEffects = enableImageEffects;
        m_currCamera = DefaultCamera;
        m_lastTarget = Target;
        m_scrFixTo = GetComponent<CamFixTo>() as CamFixTo;
        m_scrSmoothFollow = GetComponent<CamSmoothFollow>() as CamSmoothFollow;
        m_scrMouseOrbit = GetComponent<CamMouseOrbit>() as CamMouseOrbit;
        m_scrSmoothLookAt = GetComponent<CamSmoothLookAt>() as CamSmoothLookAt;
        m_scrFreeView = GetComponent<CamFreeView>() as CamFreeView;
        m_scrSmoothLookAtFromPos = GetComponent<CamSmoothLookAtFromPos>() as CamSmoothLookAtFromPos;
        m_cameraScript = new MonoBehaviour[m_numCameras];
        m_cameraScript[0] = m_scrFixTo;
        m_cameraScript[1] = m_scrSmoothFollow;
        m_cameraScript[2] = m_scrMouseOrbit;
        m_cameraScript[3] = m_scrSmoothLookAt;
        m_cameraScript[4] = m_scrFreeView;
        m_cameraScript[5] = m_scrSmoothLookAtFromPos;
        if (MapCamera)
            m_scrMapFollow = MapCamera.GetComponent<CamSmoothFollow>() as CamSmoothFollow;
        if (MirrorLeftTex)
            MirrorLeftTex.enabled = false;
        if (MirrorRightTex)
            MirrorRightTex.enabled = false;
        if (MirrorRearTex)
            MirrorRearTex.enabled = false;
        if (Target)
            UpdateTarget();
        for (int i = 0; i < ImageEffects.Length; i++)
            ImageEffects[i].enabled = enableImageEffects;
        for (int i = 0; i < m_numCameras; i++)
            m_cameraScript[i].enabled = false;
        m_cameraScript[m_currCamera].enabled = true;
    }
    void Update()
    {
        if (m_lastTarget != Target)
        {
            UpdateTarget();
            m_lastTarget = Target;
        }
        if (m_lastShowMirrors != showMirrors)
        {
            UpdateMirrors();
            m_lastShowMirrors = showMirrors;
        }
        if (m_lastImageEffects != enableImageEffects)
        {
            for (int i = 0; i < ImageEffects.Length; i++)
                ImageEffects[i].enabled = enableImageEffects;
            m_lastImageEffects = enableImageEffects;
        }
    }
    public void Next()
    {
        m_cameraScript[m_currCamera++].enabled = false;
        if (m_currCamera >= m_numCameras || (m_currCamera == m_numCameras - 1 && !Target2))
            m_currCamera = 0;
        m_cameraScript[m_currCamera].enabled = true;
        m_scrSmoothFollow.reset = true;
        UpdateMirrors();
    }
    public void SwitchTo(int Cam)
    {
        if (Cam < m_numCameras)
        {
            if (Cam == 0 && Cam == m_currCamera)
            {
                CamFreeView DriverCam = m_targetCams.DriverFront.GetComponent<CamFreeView>() as CamFreeView;
                if (DriverCam)
                    DriverCam.SetLocalEulerAngles(m_targetCams.getDriverViewAngles());
            }
            m_cameraScript[m_currCamera].enabled = false;
            m_cameraScript[Cam].enabled = true;
            m_currCamera = Cam;
            m_scrSmoothFollow.reset = true;
            UpdateMirrors();
        }
    }
    public void ToggleMap()
    {
        if (MapCamera)
            MapCamera.enabled = !MapCamera.enabled;
    }
}
