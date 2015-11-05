using UnityEngine;
using System.Collections;

public class CamSmoothLookAt : MonoBehaviour
{

	public Transform target;
	public float damping = 6.0f;
	public float minFov = 10.0f;
	public float maxFov = 60.0f;
	public float fovSensitivity = 20.0f;
	public float fovDamping = 4.0f;
	public float moveSpeed = 2.0f;	// Usar 0 para anclar a punto fijo o a otra transform en movimiento
	public float moveDamping = 5.0f;
	private Vector3 m_Pos;
	private float m_fov = 0.0f;
	private float m_savedFov = 0.0f;
	private Camera m_Camera;

	void Start ()
	{
		m_Pos = transform.position;
	
		m_Camera = GetComponent<Camera> () as Camera;
		if (m_Camera) {
			m_fov = m_Camera.fieldOfView;
			m_savedFov = m_Camera.fieldOfView;
		}
		
		// Make the rigid body not change rotation
	
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
	
	void OnEnable ()
	{
		m_Pos = transform.position;
	
		if (m_Camera)
			m_fov = m_Camera.fieldOfView;		
	}
	
	void OnDisable ()
	{
		if (m_Camera)
			m_Camera.fieldOfView = m_savedFov;
	}
	
	void LateUpdate ()
	{
		if (!target)
			return;
	
		Vector3 targetpos;
	
		if (target.rigidbody)
			targetpos = target.rigidbody.worldCenterOfMass;
		else
			targetpos = target.position;
		
		// Posición
	
		float stepSize = moveSpeed * Time.deltaTime;
	
		m_Pos += Input.GetAxis ("Sideways") * transform.right * stepSize;
		m_Pos += Input.GetAxis ("Upwards") * transform.up * stepSize;
		m_Pos += Input.GetAxis ("Forwards") * new Vector3 (transform.forward.x, 0.0f, transform.forward.z).normalized * stepSize;
	
		transform.position = Vector3.Lerp (transform.position, m_Pos, moveDamping * Time.deltaTime);
	
		// Orientación

		Quaternion rotation = Quaternion.LookRotation (targetpos - transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * damping);
	
		// Zoom opcional con cámara presente
	
		if (m_Camera) {
			m_fov -= Input.GetAxis ("Mouse ScrollWheel") * fovSensitivity;
			m_fov = Mathf.Clamp (m_fov, minFov, maxFov);
			m_Camera.fieldOfView = Mathf.Lerp (m_Camera.fieldOfView, m_fov, fovDamping * Time.deltaTime);
		}
	}
}
