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
// CarTelemetry
//
// Realtime display with all the information available from the internal components. 
// Also shows the graphic tire telemetry (realtime friction curves) and the debugging gizmos (force lines).
//
//========================================================================================================================
using UnityEngine;
using System.Collections;

public class CarTelemetry : MonoBehaviour
{

	public CarControl Target;
	public bool Enabled;
	public bool Gizmos;
	public bool Velocity;
	public bool Curves;
	public GUIStyle m_Style = new GUIStyle ();
	private int SizeX = 160;
	private int SizeY = 64;
	private GUICanvas m_graphF;
	private GUICanvas m_graphS;
	private GUICanvas m_graphStabF;
	private GUICanvas m_graphStabR;
	private GUICanvas m_graphSpring;
	private GUICanvas m_graphBar0;
	private GUICanvas m_graphBar1;
	private GUICanvas m_graphBar2;

	void Start ()
	{
		// Preparar los gráficos de las curvas de fricción
	
		m_graphF = new GUICanvas (SizeX, SizeY, 10, 10);
		m_graphS = new GUICanvas (SizeX, SizeY, 10, 25);
		m_graphF.SetAlpha (0.5f);
		m_graphS.SetAlpha (0.5f);

		m_graphF.Clear (Color.black);
		m_graphF.Grid (1, 2, new Color (0, 0.2f, 0));
		for (int i=10; i<m_graphF.CanvasHeight(); i+=10)
			m_graphF.LineH (i, new Color (0, 0.5f, 0));	
		m_graphF.Save ();
	
		m_graphS.Clear (Color.black);
		m_graphS.Grid (1, 2, new Color (0, 0.2f, 0)); 
		for (int i=10; i<m_graphS.CanvasHeight(); i+=10)
			m_graphS.LineH (i, new Color (0, 0.5f, 0));
		m_graphS.Save ();
	
		// Preparar los gráficos de las barras estabilizadoras
	
		m_graphStabF = new GUICanvas (96, 64, 1, 1);
		m_graphStabR = new GUICanvas (96, 64, 1, 1);
		m_graphStabF.SetAlpha (0.5f);
		m_graphStabR.SetAlpha (0.5f);
	
		m_graphStabF.Clear (Color.black);
		m_graphStabF.Grid (.25f, .25f, new Color (0, 0.2f, 0));
		m_graphStabF.Save ();	
		m_graphStabR.Clear (Color.black);
		m_graphStabR.Grid (.25f, .25f, new Color (0, 0.2f, 0));
		m_graphStabR.Save ();	
	
		if (Target) {
			UpdateFrictionCurves ();
			UpdateStabBarCurves ();
		}
		
		// Preparar los gráficos de la suspensión
		
		m_graphSpring = new GUICanvas (32, 64, 1, 1);
		m_graphSpring.SetAlpha (0.5f);
		m_graphSpring.Clear (Color.black);
		m_graphSpring.Grid (.50f, .25f, new Color (0, 0.2f, 0));
		m_graphSpring.Save ();	
		
		// Gráficos de las barras de valor
	
		m_graphBar0 = new GUICanvas (16, 16, 1, 1);
		m_graphBar0.Clear (new Color (0, 0.5f, 0, 0.5f));
		m_graphBar1 = new GUICanvas (16, 16, 1, 1);
		m_graphBar1.Clear (new Color (0.5f, 0.5f, 0, 0.5f));
		m_graphBar2 = new GUICanvas (16, 16, 1, 1);
		m_graphBar2.Clear (new Color (0.75f, 0, 0, 0.5f));	
	}
	
	private CarFrictionCurve SavedForwardCurve = new CarFrictionCurve ();
	private CarFrictionCurve SavedSidewaysCurve = new CarFrictionCurve ();
	private float savedForwardScale = 1.0f;
	private float SavedStabBarFront = 1.0f;
	private float SavedStabBarRear = 1.0f;

	bool IsSameCurve (CarFrictionCurve c1, CarFrictionCurve c2)
	{
		return c1.grip == c2.grip && c1.gripRange == c2.gripRange && c1.drift == c2.drift && c1.driftSlope == c2.driftSlope;
	}
	
	void CloneCurve (CarFrictionCurve original, CarFrictionCurve cloned)
	{
		cloned.grip = original.grip;
		cloned.gripRange = original.gripRange;
		cloned.drift = original.drift;
		cloned.driftSlope = original.driftSlope;
	}
	
	void UpdateFrictionCurves ()
	{
        if (Target.WheelFL.getWheelCollider() != null && m_graphF!=null)
        {
			// Curva longitudinal
		
			m_graphF.Restore ();
			m_graphF.LineV (Target.WheelFL.getForwardPeakSlip (), Color.gray);
			m_graphF.LineV (Target.WheelFL.getForwardMaxSlip (), Color.gray);		
			CarWheelFriction.DrawScaledFrictionCurve (m_graphF, CarWheelFriction.MCforward, Target.ForwardWheelFriction, Target.WheelFL.getWheelCollider ().forwardFriction.extremumSlip, Color.green);
			CloneCurve (Target.ForwardWheelFriction, SavedForwardCurve);
		
			// Curva lateral
		
			m_graphS.Restore ();
			CarWheelFriction.DrawFrictionCurve (m_graphS, CarWheelFriction.MCsideways, Target.SidewaysWheelFriction, Color.green);
			m_graphS.LineV (Target.WheelFL.getSidewaysPeakSlip (), Color.gray);
			m_graphS.LineV (Target.WheelFL.getSidewaysMaxSlip (), Color.gray);
			CloneCurve (Target.SidewaysWheelFriction, SavedSidewaysCurve);
		}
	}
	
	void UpdateStabBarCurves ()
	{
		m_graphStabF.Restore ();
		m_graphStabF.BiasCurve (Vector2.zero, Vector2.one, Target.AntiRollFront.AntiRollBias, Color.green);
		SavedStabBarFront = Target.AntiRollFront.AntiRollBias;
	
		m_graphStabR.Restore ();
		m_graphStabR.BiasCurve (Vector2.zero, Vector2.one, Target.AntiRollRear.AntiRollBias, Color.green);
		SavedStabBarRear = Target.AntiRollRear.AntiRollBias;	
	}

	void Update ()
	{
		float forwardScale = 1.0f;
	
		if (Target.WheelFL.getWheelCollider () != null)
			forwardScale = Target.WheelFL.getWheelCollider ().forwardFriction.extremumSlip;
	
		if (Target && (!IsSameCurve (Target.ForwardWheelFriction, SavedForwardCurve) || !IsSameCurve (Target.SidewaysWheelFriction, SavedSidewaysCurve) || forwardScale != savedForwardScale)) {
			UpdateFrictionCurves ();
			savedForwardScale = forwardScale;
		}
		
		if (Target.AntiRollFront.AntiRollBias != SavedStabBarFront || Target.AntiRollRear.AntiRollBias != SavedStabBarRear)
			UpdateStabBarCurves ();
		
		// Dibujar los gizmos
	
		if (Gizmos) {
			DoWheelGizmos (Target.WheelFL);
			DoWheelGizmos (Target.WheelFR);
			DoWheelGizmos (Target.WheelRL);
			DoWheelGizmos (Target.WheelRR);
		
			Vector3 CoM = Target.CenterOfMass.position;
			Vector3 F = Target.tr.forward * 0.05f;
			Vector3 U = Target.tr.up * 0.05f;
			Vector3 R = Target.tr.right * 0.05f;
		
			Debug.DrawLine (CoM - F, CoM + F, Color.white, 0, false);
			Debug.DrawLine (CoM - U, CoM + U, Color.white, 0, false);
			Debug.DrawLine (CoM - R, CoM + R, Color.white, 0, false);
		}	
	}
	
	private string m_sDescription = "";

	Color GetSlipColor (float Slip)
	{
		Slip = Mathf.Abs (Slip);
	
		if (Slip <= 1.0f)
			return Color.green;
		else if (Slip < 2.0f)
			return Color.yellow;
		else
			return Color.red;
	}
	
	string DoWheelTelemetry (CarWheel Wheel, float Compression)
	{
		WheelHit Hit = new WheelHit ();
		WheelCollider WheelCol = Wheel.getWheelCollider ();
		bool bGrounded = WheelCol.GetGroundHit (out Hit);

		if (bGrounded) {
			float forwardSlipRatio = Wheel.getForwardSlipRatio (); 
			float sidewaysSlipRatio = Wheel.getSidewaysSlipRatio ();
		
			return string.Format ("{0} RPM:{1,4:0.} S:{2,5:0.00} F:{3,5:0.} FS:{4,5:0.00} SS:{5,5:0.00} FSR:{6,5:0.00} SSR:{7,5:0.00}\n", // SM:{8,5:0.00} FM:{9,5:0.00}\n",
				Wheel.gameObject.name, WheelCol.rpm, Compression, Hit.force, Hit.forwardSlip, Hit.sidewaysSlip, 
				forwardSlipRatio, sidewaysSlipRatio,
				
				WheelCol.sidewaysFriction.stiffness, WheelCol.forwardFriction.stiffness
				);
		} else
			return string.Format ("{0} RPM:{1,4:0.} S:{2,5:0.00}\n", Wheel.gameObject.name, WheelCol.rpm, Compression);
	}
	
	void DoWheelGizmos (CarWheel Wheel)
	{
		WheelHit Hit = new WheelHit ();
		WheelCollider WheelCol = Wheel.getWheelCollider ();
		
		if (WheelCol.GetGroundHit (out Hit)) {
			float forwardSlipRatio = Wheel.getForwardSlipRatio (); 
			float sidewaysSlipRatio = Wheel.getSidewaysSlipRatio ();

			float extension = (-WheelCol.transform.InverseTransformPoint (Hit.point).y - WheelCol.radius) / WheelCol.suspensionDistance;
		
			RaycastHit RayHit = new RaycastHit ();	
			if (Physics.Raycast (Wheel.transform.position, -Wheel.transform.up, out RayHit, (WheelCol.suspensionDistance + WheelCol.radius) * Wheel.transform.lossyScale.y))
				Hit.point = RayHit.point;
		
			Debug.DrawLine (Hit.point, Hit.point + Wheel.transform.up * (Hit.force / 10000.0f), extension <= 0 ? Color.magenta : Color.white, 0, false);
			Debug.DrawLine (Hit.point, Hit.point - Wheel.transform.forward * Hit.forwardSlip, GetSlipColor (forwardSlipRatio), 0, false);
			Debug.DrawLine (Hit.point, Hit.point - Wheel.transform.right * Hit.sidewaysSlip, GetSlipColor (sidewaysSlipRatio), 0, false);
			if (Velocity)
				Debug.DrawLine (Hit.point, Hit.point + Target.rigidbody.GetPointVelocity (Hit.point), Color.blue, 0, false);
		}
	}

	private float m_LastSpeedMS = 0;
	private float m_LastSpeedLatMS = 0;
	private Vector3 m_MaxAngularVelocity = new Vector3 (0, 0, 0);

	void DoTelemetry ()
	{
		m_sDescription = "";

		if (!Enabled || !Target)
			return;
	
		// Datos de las barras estabilizadoras, si hay
	
		float extFL = 0.0f; 
		float extFR = 0.0f;
		float extRL = 0.0f;
		float extRR = 0.0f;
		float antiRollForceF = 0.0f;
		float antiRollForceR = 0.0f;
	
		if (Target.AntiRollFront) {
			extFL = Target.AntiRollFront.getExtensionL ();
			extFR = Target.AntiRollFront.getExtensionR ();
			antiRollForceF = Target.AntiRollFront.getAntiRollRatio ();
		}
		
		if (Target.AntiRollRear) {
			extRL = Target.AntiRollRear.getExtensionL ();
			extRR = Target.AntiRollRear.getExtensionR ();
			antiRollForceR = Target.AntiRollRear.getAntiRollRatio ();
		}		

		// Telemetría de las ruedas

		m_sDescription += DoWheelTelemetry (Target.WheelFL, extFL);
		m_sDescription += DoWheelTelemetry (Target.WheelFR, extFR);
		m_sDescription += DoWheelTelemetry (Target.WheelRL, extRL);
		m_sDescription += DoWheelTelemetry (Target.WheelRR, extRR);
	
		m_sDescription += string.Format ("Friction F: [{0,5:0.000}, {1,5:0.000}] S: [{2,5:0.000}, {3,5:0.000}]\n", Target.WheelFL.getForwardPeakSlip (), Target.WheelFL.getForwardMaxSlip (), Target.WheelFL.getSidewaysPeakSlip (), Target.WheelFL.getSidewaysMaxSlip ());
		m_sDescription += string.Format ("Grip F:{0,4:0.00} R:{1,4:0.00} Stab F:{2,5:0.000} R:{3,5:0.000} Steer L:{4,5:0.0} R:{5,5:0.0}\n", Target.WheelFL.getDriftFactor (), Target.WheelRL.getDriftFactor (), antiRollForceF, antiRollForceR, Target.getSteerL (), Target.getSteerR ());

		// Telemetría del vehículo

		float SpeedMS = Vector3.Dot (Target.rigidbody.velocity, Target.tr.forward);
		float SpeedLatMS = Vector3.Dot (Target.rigidbody.velocity, Target.tr.right);
	
		float roll = Target.tr.localEulerAngles.z;
		if (roll > 180.0f)
			roll -= 360.0f;
		float pitch = Target.tr.localEulerAngles.x;
		if (pitch > 180.0f)
			pitch -= 360.0f;
	
		Vector3 AngV = Target.rigidbody.angularVelocity;
		if (Mathf.Abs (AngV.x) > m_MaxAngularVelocity.x)
			m_MaxAngularVelocity.x = Mathf.Abs (AngV.x);
		if (Mathf.Abs (AngV.y) > m_MaxAngularVelocity.y)
			m_MaxAngularVelocity.y = Mathf.Abs (AngV.y);
		if (Mathf.Abs (AngV.z) > m_MaxAngularVelocity.z)
			m_MaxAngularVelocity.z = Mathf.Abs (AngV.z);

		m_sDescription += string.Format ("\nSpeed:{0,6:0.00} m/s {1,5:0.0} km/h {2,5:0.0} mph\n  Abs:{3,6:0.00} m/s Lat:{4,5:0.00} m/s\n  Acc:{5,5:0.00} m/s2 Lat:{6,5:0.00} m/s2\nPitch:{7,6:0.00} Roll:{8,6:0.00}  Max:{9,6:0.00}\n",
						SpeedMS, SpeedMS * 3.6, SpeedMS * 2.237, Target.rigidbody.velocity.magnitude, SpeedLatMS, (SpeedMS - m_LastSpeedMS) / Time.deltaTime, (SpeedLatMS - m_LastSpeedLatMS) / Time.deltaTime, pitch, roll, Target.getMaxRollAngle ());
		m_sDescription += string.Format (" AngV: {0,5:0.00},{2,5:0.00} Max:{3,5:0.00},{5,5:0.00}\n", AngV.x, AngV.y, AngV.z, m_MaxAngularVelocity.x, m_MaxAngularVelocity.y, m_MaxAngularVelocity.z);

		m_LastSpeedMS = SpeedMS;
		m_LastSpeedLatMS = SpeedLatMS;

		// Telemetría del script de control

		m_sDescription += string.Format ("\nGear: {0} Accel:{1,5:0.00} Brake:{2,5:0.00} Handbrake:{3,5:0.00} Steer:{4,5:0.00}\nMotorMax:{5,4:0.0} BrakeMax:{6,4:0.0}\n",
						Target.getGear (), Target.getMotor (), Target.getBrake (), Target.getHandBrake (), Input.GetAxis ("Horizontal"),
						Target.motorMax, Target.brakeMax);
	
		// Telemetría de las configuraciones

		CarSettings Settings = Target.GetComponent <CarSettings>() as CarSettings;
		if (Settings)
			m_sDescription += string.Format ("StabMode: {0}{1}{2}\n{3}{4}{5}{6}{7}\n",
						Settings.stabilizerMode == 0 ? "Auto (" : "",
						Settings.getStabilizerModeStr (),
						Settings.stabilizerMode == 0 ? ") " : "",
						Settings.abs ? "ABS " : "",
						Settings.tc ? "TC " : "",
						Settings.esp ? "ESP " : "",
						Settings.getTractionAxleStr () + " ",
						Time.timeScale < 1.0 ? "Slow-Motion " : "");
	}

	void FixedUpdate ()
	{
		DoTelemetry ();
	}
	
	void DrawCurvePerformance (int x, int y, GUICanvas graph, float slip, float slipRatio)
	{
		GUICanvas graphBar;
	
		slip = Mathf.Abs (slip);
		slipRatio = Mathf.Abs (slipRatio);
	
		if (slip > 0.0f) {
			if (slip > graph.CanvasWidth ())
				slip = graph.CanvasWidth ();
		 
			if (slipRatio < 1.0f)
				graphBar = m_graphBar0;
			else if (slipRatio < 2.0f)
				graphBar = m_graphBar1;
			else
				graphBar = m_graphBar2;
		
			graphBar.GUIStretchDraw (x, y, (int) (slip * graph.ScaleX ()), (int) graph.PixelsHeight ());
		}
	}

	void DrawWheelCurves (int x, int y, GUICanvas forwardCurve, GUICanvas sidewaysCurve, CarWheel Wheel)
	{
		forwardCurve.GUIDraw (x, y);
		sidewaysCurve.GUIDraw (x, y + (int) (forwardCurve.PixelsHeight () * 1.1f));
	
		WheelHit Hit = new WheelHit ();
		WheelCollider WheelCol = Wheel.getWheelCollider ();
		bool bGrounded = WheelCol.GetGroundHit (out Hit);

		if (bGrounded) {
			DrawCurvePerformance (x, y, forwardCurve, Hit.forwardSlip, Wheel.getForwardSlipRatio ());
			DrawCurvePerformance (x, y + (int) (forwardCurve.PixelsHeight () * 1.1f), sidewaysCurve, Hit.sidewaysSlip, Wheel.getSidewaysSlipRatio ());
		}
	}
	
	void DrawStabBarCurve (int x, int y, GUICanvas graph, CarAntiRollBar Bar)
	{
		GUICanvas graphBar = m_graphBar0;
		float ratio = Mathf.Abs (Bar.getAntiRollRatio ());
		float travel = Mathf.Abs (Bar.getAntiRollTravel ());
	
		if (ratio > 0.75f)
			graphBar = m_graphBar2;
		else if (ratio > 0.5f)
			graphBar = m_graphBar1;	
	
		graph.GUIDraw (x, y);	
		graphBar.GUIStretchDraw (x, y, (int) (travel * graph.ScaleX ()), (int) graph.PixelsHeight ());
	}
	
	void DrawSuspensionPerformance (int x, int y, GUICanvas graph, WheelCollider WheelCol, float travel)
	{
		GUICanvas graphBarTravel;
		GUICanvas graphBarForce;
	
		WheelHit Hit = new WheelHit ();
		bool bGrounded = WheelCol.GetGroundHit (out Hit);
	
		travel = 1.0f - travel; // Representar compresión: 0.0 = extendido, 1.0 = comprimido
		float forceRatio = Hit.force / WheelCol.suspensionSpring.spring;
	
		if (bGrounded) {
			if (travel >= 1.0f)
				graphBarTravel = m_graphBar2;
			else
				graphBarTravel = m_graphBar0;
			
			if (forceRatio >= 1.0f)
				graphBarForce = m_graphBar1;
			else
				graphBarForce = m_graphBar0;
			
			travel = Mathf.Clamp01 (travel);
			forceRatio = Mathf.Clamp01 (forceRatio);
		
			graphBarTravel.GUIStretchDraw (x, y + (int) ((1.0f - travel) * graph.PixelsHeight ()), (int) (0.5f * graph.ScaleX ()), (int) (travel * graph.PixelsHeight ()));
			graphBarForce.GUIStretchDraw (x + (int) (0.5f * graph.ScaleX ()), y + (int) ((1.0f - forceRatio) * graph.PixelsHeight ()), (int) (0.5f * graph.ScaleX ()), (int) (forceRatio * graph.PixelsHeight ()));
		}
	}

	void DrawSuspension (int x, int y, CarAntiRollBar Bar)
	{
		m_graphSpring.GUIDraw (x, y);
		m_graphSpring.GUIDraw (x + (int) (m_graphSpring.PixelsWidth ()) + 4, y);
		DrawSuspensionPerformance (x, y, m_graphSpring, Bar.WheelL, Bar.getExtensionL ());
		DrawSuspensionPerformance (x + (int) (m_graphSpring.PixelsWidth ()) + 4, y, m_graphSpring, Bar.WheelR, Bar.getExtensionR ());
	}
	
	void OnGUI ()
	{
		if (!Enabled)
			return;

		GUI.Box (new Rect (8, 8, 440, 231), "Telemetry (B to hide)");
		GUI.Label (new Rect (16, 28, 600, 205), m_sDescription, m_Style);
	
		if (Curves) {
			DrawWheelCurves (460, 4, m_graphF, m_graphS, Target.WheelFL);
			DrawWheelCurves (460 + (int) (m_graphF.PixelsWidth () * 1.1f), 4, m_graphF, m_graphS, Target.WheelFR);
		
			DrawSuspension (460 + (int) (m_graphF.PixelsWidth () * 2.2f) + 14, 4, Target.AntiRollFront);
			DrawStabBarCurve (460 + (int) (m_graphF.PixelsWidth () * 2.2f), 4 + (int) (m_graphF.PixelsHeight () * 1.1f), m_graphStabF, Target.AntiRollFront);
		
			DrawWheelCurves (460, 4 + (int) (m_graphF.PixelsHeight () * 2.2f), m_graphF, m_graphS, Target.WheelRL);
			DrawWheelCurves (460 + (int) (m_graphF.PixelsWidth () * 1.1f), 4 + (int) (m_graphF.PixelsHeight () * 2.2f), m_graphF, m_graphS, Target.WheelRR);		
		
			DrawSuspension (460 + (int) (m_graphF.PixelsWidth () * 2.2f) + 14, 4 + (int) (m_graphF.PixelsHeight () * 2.2f), Target.AntiRollRear);
			DrawStabBarCurve (460 + (int) (m_graphF.PixelsWidth () * 2.2f), 4 + (int) (m_graphF.PixelsHeight () * 3.3f), m_graphStabR, Target.AntiRollRear);
		}
	}
}
