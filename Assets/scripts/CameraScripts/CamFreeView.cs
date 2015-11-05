using UnityEngine;
using System.Collections;

public class CamFreeView : MonoBehaviour
{

	public float sensitivityH = 5.0f;
	public float sensitivityV = 5.0f;
	public float dampingH = 4.0f;
	public float dampingV = 4.0f;
	public float moveSpeed = 2.0f;	// Usar 0 para anclar a punto fijo o a otra transform en movimiento
	public float moveDamping = 5.0f;
	public float minV = -30.0f;
	public float maxV = 50.0f;
	public float minH = -180.0f;		// Usar -180, 180 para movimiento ilimitado
	public float maxH = 180.0f;
	public float minFov = 10.0f;
	public float maxFov = 60.0f;
	public float fovSensitivity = 20.0f;
	public float fovDamping = 4.0f;
	private float m_rotH = 0.0f;
	private float m_rotV = 0.0f;
	private Vector3 m_Pos = new Vector3 (0, 0, 0);
	private float m_fov = 0.0f;
	private float m_savedFov = 0.0f;
	private Camera m_Camera;


	// Orden de las funciones: 
	//
	// El FreeView de la camara hace OnEnable y OnDisable al arrancar. La primera vez que se activa hace OnEnable, Start, OnDisable. Las siguientes veces hace OnEnable, OnDisable.
	// Los FreeView que van en los puntos DriverFront hacen OnEnable, Start, OnDisable al arrancar. Las siguientes veces OnEnable, OnDisable.


	void Start ()
	{
		SetLocalEulerAngles (transform.localEulerAngles);
	
		m_Camera = GetComponent<Camera> () as Camera;
		if (m_Camera) {
			m_fov = m_Camera.fieldOfView;
			m_savedFov = m_Camera.fieldOfView;
		}
	}
	
	void OnEnable ()
	{
		SetLocalEulerAngles (transform.localEulerAngles);
		m_Pos = transform.localPosition;
	
		if (m_Camera)
			m_fov = m_Camera.fieldOfView;		
	}
	
	void OnDisable ()
	{
		if (m_Camera)
			m_Camera.fieldOfView = m_savedFov;
	}
	
	public void SetLocalEulerAngles (Vector3 Angles)
	{
		Vector3 t = Angles;
		
		m_rotH = t.y;
		m_rotV = t.x;
		transform.localEulerAngles = t;
	}

	void LateUpdate ()
	{
		// Orientación
	
		m_rotH += Input.GetAxis ("Mouse X") * sensitivityH;		
		m_rotV -= Input.GetAxis ("Mouse Y") * sensitivityV;		
		m_rotH = ClampAngle (m_rotH, minH, maxH);
		m_rotV = ClampAngle (m_rotV, minV, maxV);
	
		
		Vector3 t = transform.localEulerAngles;
		
		t.y = Mathf.LerpAngle (transform.localEulerAngles.y, m_rotH, dampingH * Time.deltaTime);
		t.x = Mathf.LerpAngle (transform.localEulerAngles.x, m_rotV, dampingV * Time.deltaTime);
		
		transform.localEulerAngles = t;
		
		// Zoom opcional con cámara presente
	
		if (m_Camera) {
			m_fov -= Input.GetAxis ("Mouse ScrollWheel") * fovSensitivity;
			m_fov = Mathf.Clamp (m_fov, minFov, maxFov);
			m_Camera.fieldOfView = Mathf.Lerp (m_Camera.fieldOfView, m_fov, fovDamping * Time.deltaTime);
		}

		// Movimiento
	
		float stepSize = moveSpeed * Time.deltaTime;
	
		m_Pos += Input.GetAxis ("Sideways") * transform.right * stepSize;
		m_Pos += Input.GetAxis ("Upwards") * transform.up * stepSize;
		m_Pos += Input.GetAxis ("Forwards") * new Vector3 (transform.forward.x, 0.0f, transform.forward.z).normalized * stepSize;
	
		transform.localPosition = Vector3.Lerp (transform.localPosition, m_Pos, moveDamping * Time.deltaTime);
	}
	
	float ClampAngle (float a, float min, float max)
	{
		while (max < min)
			max += 360.0f;
		while (a > max)
			a -= 360.0f;
		while (a < min)
			a += 360.0f;
	
		if (a > max) {
			if (a - (max + min) * 0.5f < 180.0f)
				return max;
			else
				return min;
		} else
			return a;
	}
}
