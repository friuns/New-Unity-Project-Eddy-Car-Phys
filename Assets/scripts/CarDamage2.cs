//using UnityEngine;

//public class CarDamage2 : MonoBehaviour
//{
//    public float minForce = 1.0f;
//    public float multiplier = 0.1f;
//    public float deformRadius = 1.0f;
//    public float deformNoise = 0.1f;
//    public float deformNodeRadius = 0.5f;
//    public float maxDeform = 0.5f;
//    public float maxNodeRotationStep = 10.0f;
//    public float maxNodeRotation = 14.0f;
//    public float bounceSpeed = 0.1f;
//    public float bounceThreshold = 0.002f;
//    public bool autoBounce = false;
//    public bool showRepairingLabel = true;
//    public MeshFilter[] DeformMeshes;
//    public Transform[] DeformNodes;
//    public MeshFilter[] DeformColliders;
//    private CarVisuals m_CarVisuals;
//    private VertexData[] m_meshData;
//    private VertexData[] m_colliderData;
//    private Vector3[] m_permaNodes;
//    private Quaternion[] m_permaNodeAngles;
//    private bool m_doBounce = false;

//    void OnEnable()
//    {
//        m_CarVisuals = GetComponent<CarVisuals>() as CarVisuals;
//        GetComponent<CarControl>().pl.meshTest = meshTest = GetComponentInChildren<MeshTest>();
        
//    }
//    public MeshTest meshTest;
//    void Start()
//    {
//        m_meshData = new VertexData[DeformMeshes.Length];
//        for (int i = 0; i < DeformMeshes.Length; i++)
//        {
//            m_meshData[i] = new VertexData();
//            m_meshData[i].permaVerts = DeformMeshes[i].mesh.vertices;
//        }
//        m_colliderData = new VertexData[DeformColliders.Length];
//        for (int i = 0; i < DeformColliders.Length; i++)
//        {
//            m_colliderData[i] = new VertexData();
//            m_colliderData[i].permaVerts = DeformColliders[i].sharedMesh.vertices;
//        }
//        m_permaNodes = new Vector3[DeformNodes.Length];
//        m_permaNodeAngles = new Quaternion[DeformNodes.Length];
//        for (int i = 0; i < DeformNodes.Length; i++)
//        {
//            m_permaNodes[i] = DeformNodes[i].transform.localPosition;
//            m_permaNodeAngles[i] = DeformNodes[i].transform.localRotation;
//        }
//    }
//    private void DeformMesh(Mesh mesh, Vector3[] originalMesh, Transform localTransform, Vector3 contactPoint, Vector3 contactForce)
//    {
//        Vector3[] vertices = mesh.vertices;
//        float sqrRadius = deformRadius * deformRadius;
//        float sqrMaxDeform = maxDeform * maxDeform;
//        Vector3 localContactPoint = localTransform.InverseTransformPoint(contactPoint);
//        Vector3 localContactForce = localTransform.InverseTransformDirection(contactForce);
//        for (int i = 0; i < vertices.Length; i++)
//        {
//            float dist = (localContactPoint - vertices[i]).sqrMagnitude;
//            if (dist < sqrRadius)
//            {
//                vertices[i] += (localContactForce * (deformRadius - Mathf.Sqrt(dist)) / deformRadius) + Random.onUnitSphere * deformNoise;
//                Vector3 deform = vertices[i] - originalMesh[i];
//                if (deform.sqrMagnitude > sqrMaxDeform)
//                    vertices[i] = originalMesh[i] + deform.normalized * maxDeform;
//            }
//        }
//        mesh.vertices = vertices;
//        mesh.RecalculateNormals();
//        mesh.RecalculateBounds();
//    }
//    private void DeformNode(Transform T, Vector3 originalLocalPos, Quaternion originalLocalRot, Vector3 contactPoint, Vector3 contactVector)
//    {
//        float dist = (contactPoint - T.position).sqrMagnitude;
//        float deformForce = 0;
//        if (dist < deformRadius * deformRadius)
//        {
//            deformForce = (deformRadius - Mathf.Sqrt(dist)) / deformRadius;
//            T.position += contactVector * deformForce + Random.onUnitSphere * deformNoise;
//            Vector3 deform = T.localPosition - originalLocalPos;
//            if (deform.sqrMagnitude > maxDeform * maxDeform)
//                T.localPosition = originalLocalPos + deform.normalized * maxDeform;
//        }
//        if (dist < deformNodeRadius * deformNodeRadius)
//        {
//            Vector3 angles = AnglesToVector(T.localEulerAngles);
//            Vector3 angleLimit = new Vector3(maxNodeRotation, maxNodeRotation, maxNodeRotation);
//            Vector3 angleMax = angles + angleLimit;
//            Vector3 angleMin = angles - angleLimit;
//            angles += deformForce * Random.onUnitSphere * maxNodeRotationStep;
//            T.localEulerAngles = new Vector3(Mathf.Clamp(angles.x, angleMin.x, angleMax.x), Mathf.Clamp(angles.y, angleMin.y, angleMax.y), Mathf.Clamp(angles.z, angleMin.z, angleMax.z));
//        }
//    }
//    private bool BounceMesh(Mesh mesh, Vector3[] originalMesh, float maxSpeed, float sqrThreshold)
//    {
//        bool result = true;
//        Vector3[] vertices = mesh.vertices;
//        for (int i = 0; i < vertices.Length; i++)
//        {
//            vertices[i] = Vector3.MoveTowards(vertices[i], originalMesh[i], maxSpeed);
//            if ((originalMesh[i] - vertices[i]).sqrMagnitude >= sqrThreshold)
//                result = false;
//        }
//        mesh.vertices = vertices;
//        mesh.RecalculateNormals();
//        mesh.RecalculateBounds();
//        return result;
//    }
//    private bool BounceNode(Transform T, Vector3 originalLocalPosition, Quaternion originalLocalRotation, float maxSpeed, float sqrThreshold)
//    {
//        T.localPosition = Vector3.MoveTowards(T.localPosition, originalLocalPosition, maxSpeed);
//        T.localRotation = Quaternion.RotateTowards(T.localRotation, originalLocalRotation, maxSpeed * 50.0f);
//        return (originalLocalPosition - T.localPosition).sqrMagnitude < sqrThreshold &&
//               Quaternion.Angle(originalLocalRotation, T.localRotation) < sqrThreshold;
//    }
//    private void RestoreNode(Transform T, Vector3 originalLocalPos, Quaternion originalLocalAngles)
//    {
//        T.localPosition = originalLocalPos;
//        T.localRotation = originalLocalAngles;
//    }
//    private Vector3 AnglesToVector(Vector3 Angles)
//    {
//        if (Angles.x > 180)
//            Angles.x = -360 + Angles.x;
//        if (Angles.y > 180)
//            Angles.y = -360 + Angles.y;
//        if (Angles.z > 180)
//            Angles.z = -360 + Angles.z;
//        return Angles;
//    }
//    private void RestoreColliders()
//    {
//        if (DeformColliders.Length > 0)
//        {
//            Vector3 CoM = rigidbody.centerOfMass;
//            for (int i = 0; i < DeformColliders.Length; i++)
//            {
//                Mesh mesh = new Mesh();
//                mesh.vertices = m_colliderData[i].permaVerts;
//                mesh.triangles = DeformColliders[i].sharedMesh.triangles;
//                mesh.RecalculateNormals();
//                mesh.RecalculateBounds();
//                DeformColliders[i].sharedMesh = mesh;
//            }
//            rigidbody.centerOfMass = CoM;
//        }
//    }
//    void OnCollisionEnter(Collision collision)
//    {
//        if (autoBounce)
//            m_doBounce = true;
//    }
//    public void DoBounce()
//    {
//        m_doBounce = true;
//    }
//    void OnGUI()
//    {
//        if (showRepairingLabel && m_doBounce)
//            GUI.Label(new Rect(16, Screen.height - 40, 200, 60), "REPAIRING");
//    }
//    void Update()
//    {
//        int i;
//        Vector3 contactForce = Vector3.zero;
//        if (m_CarVisuals.localImpactVelocity.sqrMagnitude > minForce * minForce)
//            contactForce = transform.TransformDirection(m_CarVisuals.localImpactVelocity) * multiplier * 0.2f;
//        else if (m_CarVisuals.localDragVelocityDiscrete.sqrMagnitude > minForce * minForce)
//            contactForce = transform.TransformDirection(m_CarVisuals.localDragVelocityDiscrete) * multiplier * 0.01f;
//        if (contactForce.sqrMagnitude > 0.0f)
//        {
//            Vector3 contactPoint = transform.TransformPoint(m_CarVisuals.localImpactPosition);
//            if (m_meshData != null)
//            {
//                for (i = 0; i < DeformMeshes.Length; i++)
//                    DeformMesh(DeformMeshes[i].mesh, m_meshData[i].permaVerts, DeformMeshes[i].transform, contactPoint, contactForce);
//                if (DeformColliders.Length > 0)
//                {
//                    Vector3 CoM = rigidbody.centerOfMass;
//                    for (i = 0; i < DeformColliders.Length; i++)
//                    {

//                        Mesh mesh = new Mesh();
//                        mesh.vertices = DeformColliders[i].sharedMesh.vertices;
//                        mesh.triangles = DeformColliders[i].sharedMesh.triangles;
//                        DeformMesh(mesh, m_colliderData[i].permaVerts, DeformColliders[i].transform, contactPoint, contactForce);
//                        DeformColliders[i].sharedMesh = mesh;
//                    }
//                    rigidbody.centerOfMass = CoM;
//                }
//            }
//            if (meshTest)
//            {
//                meshTest.Hit(contactPoint, rigidbody.velocity * .5f, Mathf.Min(contactForce.magnitude * 10));
//                meshTest.Damage(contactPoint,contactForce);
//            }
//            contactForce *= 0.5f;
//            for (i = 0; i < DeformNodes.Length; i++)
//                DeformNode(DeformNodes[i], m_permaNodes[i], m_permaNodeAngles[i], contactPoint, contactForce);
//        }
//        if (m_doBounce)
//        {
//            float speed = bounceSpeed * Time.deltaTime;
//            float sqrBounceLimit = bounceThreshold * bounceThreshold;
//            bool completed = true;
//            for (i = 0; i < DeformMeshes.Length; i++)
//                completed = BounceMesh(DeformMeshes[i].mesh, m_meshData[i].permaVerts, speed, sqrBounceLimit) && completed;
//            for (i = 0; i < DeformNodes.Length; i++)
//                completed = BounceNode(DeformNodes[i], m_permaNodes[i], m_permaNodeAngles[i], speed, sqrBounceLimit) && completed;
//            if (completed)
//            {
//                m_doBounce = false;
//                for (i = 0; i < DeformNodes.Length; i++)
//                    RestoreNode(DeformNodes[i], m_permaNodes[i], m_permaNodeAngles[i]);
//                RestoreColliders();
//            }
//        }
//    }
//    public class VertexData : System.Object
//    {
//        public Vector3[] permaVerts;
//    }
//}