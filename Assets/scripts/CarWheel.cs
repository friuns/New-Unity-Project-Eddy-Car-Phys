//========================================================================================================================
// Edy Vehicle Physics - (c) Angel Garcia "Edy" - Oviedo, Spain
// http://www.edy.es/dev/vehicle-physics
// 
// Terms & Conditions:
//  - Use for unlimited time, any number of projects, royalty-free.
//  - Keep the copyright notices on top of the source files.
//  - Resale or redistribute as anything except a final product to the end user (asset / library / engine / middleware / etc.) is not allowed.
//  - Put me (Angel Garcia "Edy") in your game's credits as author of the vehicle physics.
//
// Bug reports, improvements to the code, suggestions on further developments, etc are always welcome.
// Unity forum user: Edy
//========================================================================================================================
//
// CarWheel
//
// Calculates the friction curves and their limits based on the parameters.
// Also provides the corrective code for the WheelCollider to work in the clamped model, and adjusts the tire behavior
// based on the terrain.
//
//========================================================================================================================
using UnityEngine;
using System.Collections;

// Tipos de datos
[System.Serializable]
public class CarFrictionCurve : System.Object
{
	public float grip = 800.0f;
	public float gripRange = 5.0f;
	public float drift = 150.0f;
	public float driftSlope = 0.0f;
}

public class CarWheel : MonoBehaviour
{

	
	// Propiedades públicas
	
	public float motorInput = 0.0f;					// Entradas para el motor y el freno, directamente en SLIP.
	public float brakeInput = 0.0f;					// Para ABS y TC se pueden obtener los puntos máximos con getFrictionPeak

	public float motorForceFactor = 1.0f;			// Factor de amplificación de la tracción (ej. 2.0 al pasar de 4x4 a traccion 2 ruedas)
	public float brakeForceFactor = 1.0f;			// Factor de amplificación del frenado (NO es adecuado para el balance de frenos - balancear usando brakeInput)
	public float sidewaysForceFactor = 1.0f;		// Factor de ajuste del agarre lateral (ej. 0.9 para las ruedas que soportan la masa del motor, acentuando el understeer)
	public float sidewaysDriftFriction = 0.35f;		// Factor de agarre lateral cuando las ruedas entran en pérdida longitudinal (burnout - handbrake)

	public float performancePeak = 2.8f;			// Máximo rendimiento de las ruedas del vehículo (aceleración aproximada)
	public float staticFrictionMax = 1500.0f;		// Máxima pendiente para aplicar en fricción estática. 0 (o menos que extremumValue) para desactivar. A más valor mayor pendiente sin deslizar, pero más posibilidad de volcar en impactos.

	public CarFrictionCurve ForwardWheelFriction = new CarFrictionCurve ();
	public CarFrictionCurve SidewaysWheelFriction = new CarFrictionCurve ();
	public bool optimized = false;					// Con TRUE es necesario llamar a RecalculateStuff cada vez que se modifiquen las curvas de fricción.
	public bool serviceMode = false;


	// Datos privados

	private WheelCollider m_wheel;
	private Rigidbody m_rigidbody;
	private Vector2 m_forwardFrictionPeak = new Vector2 (1.0f, 1.0f);		// x = punto de valor máximo; y = valor máximo
	private Vector2 m_sidewaysFrictionPeak = new Vector2 (1.0f, 1.0f);		// Necesario valor inicial distinto de 0.

	private float m_forwardSlipRatio = 0.0f;
	private float m_sidewaysSlipRatio = 0.0f;
	private float m_driftFactor = 1.0f;
	private WheelHit m_wheelHit;
	internal bool m_grounded = false;
	private float m_DeltaTimeFactor = 1.0f;

	public float getForwardPeakSlip ()
	{

		return m_wheel.forwardFriction.extremumSlip * m_forwardFrictionPeak.x;
	}

	public float getForwardMaxSlip ()
	{
		return m_wheel.forwardFriction.asymptoteSlip;
	}

	public float getSidewaysPeakSlip ()
	{
		return m_wheel.sidewaysFriction.extremumSlip * m_sidewaysFrictionPeak.x;
	}  // extremumSlip aquí siempre es 1.0
	public float getSidewaysMaxSlip ()
	{
		return m_wheel.sidewaysFriction.asymptoteSlip;
	}

	public Vector2 getSidewaysPeak ()
	{
		return m_sidewaysFrictionPeak;
	}

	public Vector2 getSidewaysMax ()
	{
		return new Vector2 (m_wheel.sidewaysFriction.asymptoteSlip, CarWheelFriction.GetValue (CarWheelFriction.MCsideways, m_wheel.sidewaysFriction.asymptoteValue, m_wheel.sidewaysFriction.asymptoteSlip));
	}

	public float getForwardSlipRatio ()
	{
		return m_forwardSlipRatio;
	}

	public float getSidewaysSlipRatio ()
	{
		return m_sidewaysSlipRatio;
	}

	public float getDriftFactor ()
	{
		return m_driftFactor;
	}

	public WheelCollider getWheelCollider ()
	{
		return m_wheel;
	}
    void Awake()
    {
        m_wheel = GetComponent<WheelCollider>() as WheelCollider;
    }
	void  Start ()
	{
		// Acceso al WheelCollider
	
		
		m_rigidbody = m_wheel.attachedRigidbody;
	
		// En modo optimizado, recalcular los datos cotosos ahora. 
		// Si se cambian los parámetros de las curvas de fricción es necesario invocar a RecalculateStuff manualmente.
	
		if (optimized)
			RecalculateStuff ();	
	}

	public void RecalculateStuff ()
	{
		// Punto de máximo rendimiento de la curva longitudinal original
	
		m_forwardFrictionPeak = CarWheelFriction.GetPeakValue (CarWheelFriction.MCforward, ForwardWheelFriction.grip, ForwardWheelFriction.gripRange, ForwardWheelFriction.drift);
	
		// Punto de máximo rendimiento de la curva lateral
	
		m_sidewaysFrictionPeak = CarWheelFriction.GetPeakValue (CarWheelFriction.MCsideways, SidewaysWheelFriction.grip, SidewaysWheelFriction.gripRange, SidewaysWheelFriction.drift);
	}
	

	/*	
	
	// DEBUG
	
	private var m_lastVelocity = Vector3.zero;
	private var m_velocity = Vector3.zero;
	private var m_acceleration = Vector3.zero;
	private var m_loadMass = 0.0;
	private var m_force = Vector3.zero;
	
	static function Lin2Log(value : float) : float
		{
		return Mathf.Log(Mathf.Abs(value)+1) * Mathf.Sign(value);	
		}
		
	static function Lin2Log(value : Vector3) : Vector3
		{
		return Vector3.ClampMagnitude(value, Lin2Log(value.magnitude));
		}
	
		
	function Update ()
		{
		var hit : RaycastHit;
		
		if (Physics.Raycast(m_wheel.transform.position, -m_wheel.transform.up, hit, (m_wheel.suspensionDistance + m_wheel.radius) * m_wheel.transform.lossyScale.y))
			{	
			Debug.DrawLine(hit.point, hit.point + m_velocity * 1, Color.gray);
			Debug.DrawLine(hit.point, hit.point + m_acceleration * 1, Color.yellow);
			Debug.DrawLine(hit.point, hit.point + Lin2Log(m_force) * 0.1, Color.magenta);
			}
		}
	*/

	
	void FixedUpdate ()
	{
		/*
		// DATOS Debug
		
		m_grounded = m_wheel.GetGroundHit(m_wheelHit);	
		if (m_grounded)
			{
			m_velocity = m_rigidbody.GetPointVelocity(m_wheelHit.point);
			m_acceleration = (m_velocity - m_lastVelocity) / Time.deltaTime;
			m_lastVelocity = m_velocity;
			m_loadMass = m_wheelHit.force / -Physics.gravity.y;
			m_force = m_acceleration * m_loadMass;
			}
		else
			{
			m_velocity = Vector3.zero;
			m_lastVelocity = Vector3.zero;
			m_acceleration = Vector3.zero;
			m_loadMass = 0.0;
			m_force = Vector3.zero;
			}
		
		//-----------
		*/
	
		// Calcular el ajuste temporal
	
		m_DeltaTimeFactor = Time.fixedDeltaTime * Time.fixedDeltaTime * 2500.0f;   // equivale a (fixedDeltaTime/0.02)^2
	
		// Determinar el estado de la rueda
	
		m_grounded = m_wheel.GetGroundHit (out m_wheelHit);	
	
		// Calcular los elementos costosos (picos de las curvas de fricción)
	
		if (!optimized)
			RecalculateStuff ();
	
		//======================================================================
		// 1. Fricción longitudinal
		//======================================================================
	
		// Calcular el rendimiento neto aceleración-freno aplicado sobre la rueda. Determinará el factor de amplificación (slopeFactor) a aplicar.
	
		float resultInput = Mathf.Abs (motorInput) - brakeInput;
	
		// Calcular parámetros de ajuste de la curva:
		// - fslipFactor escalará la curva de forma que el punto de máximo rendimiento coincida (mas o menos) con el valor dado.
		// - fSlopeFactor es el stiffnes que multiplicará a grip y a drift.
		//   Si es != 1.0 se desplaza ligeramente el punto de máximo rendimiento, pero no se nota mucho.
	
		float fSlipFactor = serviceMode ? 1.0f : performancePeak / m_forwardFrictionPeak.y;		
		float fSlopeFactor = resultInput >= 0 ? motorForceFactor : brakeForceFactor;
	
		// Calcular la curva correcta para el WheelCollider
	
		m_wheel.forwardFriction = GetWheelFrictionCurve (ForwardWheelFriction, fSlopeFactor, fSlipFactor);
		m_wheel.motorTorque = SlipToTorque (motorInput);
		m_wheel.brakeTorque = SlipToTorque (brakeInput);

		//======================================================================
		// 2. Fricción lateral
		//======================================================================
		
		// Calcular la pérdida de agarre lateral en función del rendimiento longitudinal de la rueda
		// Si la rueda está en el suelo se usa el slip longitundinal real, si no se usa el de la entrada.
	
		m_driftFactor = Mathf.Lerp (1.0f, sidewaysDriftFriction, Mathf.InverseLerp (m_wheel.forwardFriction.extremumSlip * m_forwardFrictionPeak.x, m_wheel.forwardFriction.asymptoteSlip, Mathf.Abs (m_grounded ? m_wheelHit.forwardSlip : resultInput)));
		
		// Calcular y aplicar la curva de fricción lateral en el WheelCollider

		fSlopeFactor = serviceMode ? 1.0f : m_driftFactor * sidewaysForceFactor;
		m_wheel.sidewaysFriction = GetWheelFrictionCurve (SidewaysWheelFriction, fSlopeFactor, 1.0f);

		//======================================================================
		// 3. Datos, ajustes & correcciones
		//======================================================================	
	
		if (m_grounded) {
			// Datos para telemetría
		
			WheelFrictionCurve sF = m_wheel.sidewaysFriction;
			WheelFrictionCurve fF = m_wheel.forwardFriction;
			
			m_forwardSlipRatio = GetWheelSlipRatio (m_wheelHit.forwardSlip, fF.extremumSlip * m_forwardFrictionPeak.x, fF.asymptoteSlip);	
			m_sidewaysSlipRatio = GetWheelSlipRatio (m_wheelHit.sidewaysSlip, sF.extremumSlip * m_sidewaysFrictionPeak.x, sF.asymptoteSlip);
		
			// Corregir el diseño de las curvas de fricción de WheelFrictionCurve
			// - Fricción lateral
		
			float absSlip = Mathf.Abs (m_wheelHit.sidewaysSlip);		
			if (staticFrictionMax > sF.extremumValue && absSlip < sF.extremumSlip) {   // Baja velocidad - reforzar ligeramente el control estático del WheelCollider
				
				sF.extremumValue = GetFixedSlope (CarWheelFriction.MCsideways, absSlip, sF.extremumSlip, sF.extremumValue, 0);
				if (sF.extremumValue > staticFrictionMax)
					sF.extremumValue = staticFrictionMax;
			}

			if (absSlip > sF.asymptoteSlip)
				sF.asymptoteValue = GetFixedSlope (CarWheelFriction.MCsideways, absSlip, sF.asymptoteSlip, sF.asymptoteValue, SidewaysWheelFriction.driftSlope);		
			
			// - Fricción longitudinal
				
			absSlip = Mathf.Abs (m_wheelHit.forwardSlip);
			if (absSlip > fF.asymptoteSlip)
				fF.asymptoteValue = GetFixedSlope (CarWheelFriction.MCforward, absSlip, fF.asymptoteSlip, fF.asymptoteValue, ForwardWheelFriction.driftSlope);
		
			// Ajustar la curva en función del terreno
		
			if (m_wheelHit.collider.sharedMaterial) {
				fF.stiffness *= (m_wheelHit.collider.sharedMaterial.dynamicFriction + 1.0f) * 0.5f;
				sF.stiffness *= (m_wheelHit.collider.sharedMaterial.dynamicFriction + 1.0f) * 0.5f;
			
				// Aplicar fuerza de resistencia fuera de la carretera

				Vector3 wheelV = m_rigidbody.GetPointVelocity (m_wheelHit.point);
				m_rigidbody.AddForceAtPosition (wheelV * wheelV.magnitude * -m_wheelHit.force * m_wheelHit.collider.sharedMaterial.dynamicFriction * 0.001f, m_wheelHit.point);
			
			}
			
			m_wheel.sidewaysFriction = sF;
			m_wheel.forwardFriction = fF;
			
		} else {
			m_forwardSlipRatio = 0;
			m_sidewaysSlipRatio = 0;
		}
	}

	// Convertir parámetros de fricción en una curva de WheelCollider
	WheelFrictionCurve GetWheelFrictionCurve (CarFrictionCurve Friction, float Stiffness, float SlipFactor)
	{
		WheelFrictionCurve Curve = new WheelFrictionCurve ();

		Curve.extremumSlip = 1.0f * SlipFactor;
		Curve.extremumValue = Friction.grip * m_DeltaTimeFactor;
		Curve.asymptoteSlip = (1.0f + Friction.gripRange) * SlipFactor;
		Curve.asymptoteValue = Friction.drift * m_DeltaTimeFactor;
	
		Curve.stiffness = Stiffness;
		return Curve;
	}
		
	
	// Convertir un valor de slip en torque para WheelCollider

	float SlipToTorque (float Slip)
	{
		return (Slip * m_wheel.mass) / (m_wheel.radius * m_wheel.transform.lossyScale.y * Time.deltaTime);
	}


	// Calcular  el estado relativo del deslizamiento de la rueda
	//
	// 0..1 -> en agarre completo, antes de Peak
	// 1..2 -> comenzando a deslizar, entre Peak y Max
	// 2... -> deslizando completamente, más de Max

	float GetWheelSlipRatio (float Slip, float PeakSlip, float MaxSlip)
	{
		float slipAbs = Slip >= 0 ? Slip : -Slip;
		float result;
	
		if (slipAbs < PeakSlip)
			result = slipAbs / PeakSlip;
		else if (slipAbs < MaxSlip)
			result = 1.0f + Mathf.InverseLerp (PeakSlip, MaxSlip, slipAbs);
		else
			result = 2.0f + slipAbs - MaxSlip;

		return Slip >= 0 ? result : -result;
	}
	
		
	// Obtener la pendiente corregida de la curva en la asíntota para que la rueda se comporte correctamente
	// Requerimientos:
	//  - Rueda en el suelo
	//	- absSlip es el slip actual en valor absoluto
	
	float GetFixedSlope (float[] Coefs, float absSlip, float asymSlip, float asymValue, float valueFactor)
	{	
		float Slope;
	
		// Valor en el punto de la asíntota. Es el que mantendremos idependientemente de la cantidad de desplazamiento.
		
		float Value = CarWheelFriction.GetValue (CarWheelFriction.MCsideways, asymValue / m_DeltaTimeFactor, asymSlip);
		
		// Nueva pendiente que mantiene el valor con el desplazamiento actual de la rueda aplicando la pendiente controlada indicada (valueVactor)
	
		Slope = CarWheelFriction.GetSlope (CarWheelFriction.MCsideways, absSlip, Value + (absSlip - asymSlip) * valueFactor) * m_DeltaTimeFactor;
	
		return Slope;
	}
}
