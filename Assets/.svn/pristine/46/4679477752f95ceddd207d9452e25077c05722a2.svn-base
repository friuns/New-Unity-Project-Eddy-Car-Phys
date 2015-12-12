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
// CarSettings
//
// Configures the high-level vehicle specific settings (features) and implements the driving aids.
//
//========================================================================================================================
using UnityEngine;
using System.Collections;

public class CarSettings : MonoBehaviour
{

	public string description;
	public bool hasABS = true;
	public bool hasTC = true;
	public bool hasESP = true;
	public int hasTractionModes = 1;				// 0: eje fijo, 1: fijo + 4x4, 2: intercambiable + 4x4
	public int mainTractionAxle = 1;  			  	// Eje de tracción principal. 0: frontal, 1: trasero, 2: ambos
	public bool hasVariableStabilizer = false;		// Se puede modificar el modo de estabilidad
	public int mainStabilizerMode = 0;				// Modo de estabilidad principal (para el caso de que sea fijo)

	public bool abs = true;
	public bool tc = true;
	public bool esp = true;
	public int tractionAxle = 0;					// 0: frontal, 1: trasero, 2: 4x4

	public int stabilizerMode = 0;					// 0: auto, 1: offroad, 2: comfort, 3: sport, 4: none
	public float stabilizerFactor = 1.0f;
	public float espLevel = 1.0f;
	public bool externalInput = false;				// El coche recibe la entrada de un componente derivado de CarExternalInput. 
													// El componente Debe estar presente. Si no lo está, el coche no recibirá entrada de ningún tipo.
	public bool bypassTC = false;
	public bool bypassABS = false;
	public bool serviceMode = false;
	private CarControl m_Car;
	private CarExternalInput m_ExternalInput;
	private float STAB_OFFROAD = 0.25f;
	private float STAB_COMFORT = 0.80f;
	private float STAB_SPORT = 1.0f;

	void Start ()
	{
		m_Car = GetComponent<CarControl> () as CarControl;
		m_ExternalInput = GetComponent<CarExternalInput> () as CarExternalInput;
	
		if (m_ExternalInput)
			m_ExternalInput.enabled = externalInput;	
	}

	void Update ()
	{
		// En modo de servicio no se toca nada
	
		if (serviceMode)
			return;
	
		// Desactivar elementos que no están disponibles.
		// Así se puede leer el estado desde fuera.
	
		tc = tc && hasTC;
		abs = abs && hasABS;
		esp = esp && hasESP;
	
		// Ajustar tractionAxle según hasTractionModes (nota: valores desconocidos asumen 0 - eje fijo)
	
		// mainTractionAxle = mainTractionAxle && mainTractionAxle;	// Asegurar 0 ó 1
	
		if (hasTractionModes > 2 || hasTractionModes < 0)
			hasTractionModes = 0;
	
		switch (hasTractionModes) {
		case 1:		// Eje fijo + 4x4. Permitir sólo eje principal o 4x4
			if (tractionAxle > 2)
				tractionAxle = mainTractionAxle;
			else if (tractionAxle != mainTractionAxle)
				tractionAxle = 2;
			break;
			
		case 2:		// Eje intercambiable + 4x4. Permitir recorrer todas las opciones
			if (tractionAxle > 2)
				tractionAxle = 0;
			else if (tractionAxle < 0)
				tractionAxle = 2;
			break;
			
		default: // Configuración fija, frontal, trasero o 4x4
			if (mainTractionAxle > 2)
				mainTractionAxle = 2;
			else if (mainTractionAxle < 0)
				mainTractionAxle = 0;
			tractionAxle = mainTractionAxle;
			break;
		}
		
		// Ajustar parámetros del script según los settings dados
		// ABS / TC / ESP
	
		m_Car.autoMotorMax = tc && !bypassTC;
		m_Car.autoBrakeMax = abs && !bypassABS;
		m_Car.autoSteerLevel = esp ? espLevel : 0.0f;
	
		// 4x4 / Traccion
	
		switch (tractionAxle) {
		case 0:		// Eje delantero
			m_Car.motorBalance = 0.0f;
			m_Car.motorForceFactor = 2.0f;
			break;
			
		case 1:		// Eje trasero
			m_Car.motorBalance = 1.0f;
			m_Car.motorForceFactor = 2.0f;
			break;
		
		case 2:		// 4x4
			m_Car.motorBalance = 0.5f;
			m_Car.motorForceFactor = 1.0f;
			break;
			
		default:
			// No puede darse - tractionAxle se a ajustado arriba
			break;
		}
	
		// Ajustar estabilización
		// 0: auto, 1: offroad, 2: comfort, 3: sport, 4: none	
	
		if (!hasVariableStabilizer)
			stabilizerMode = mainStabilizerMode;
	
		float newAntiRoll = 0.0f;
		if (stabilizerMode < 0)
			stabilizerMode = 4;
		else if (stabilizerMode > 4)
			stabilizerMode = 0;
	
		switch (stabilizerMode) {
		case 0:	// auto
			float Speed = m_Car.rigidbody.velocity.magnitude;

			if (Speed < 3.25f)
				newAntiRoll = STAB_OFFROAD;
			else if (Speed < 19.5f)
				newAntiRoll = STAB_COMFORT;
			else
				newAntiRoll = STAB_SPORT;
			break;
			
		case 1:
			newAntiRoll = STAB_OFFROAD;
			break;
		case 2:
			newAntiRoll = STAB_COMFORT;
			break;
		case 3:
			newAntiRoll = STAB_SPORT;
			break;			
		}
		
		m_Car.antiRollLevel = newAntiRoll * stabilizerFactor;
	
		// Ajustar control externo si está disponible

		if (m_ExternalInput) {
			m_ExternalInput.enabled = externalInput;
		
			if (externalInput)
				m_Car.readUserInput = false;
		}
	}
	
	public bool HasExternalInput ()
	{
		return m_ExternalInput != null;
	}
	
	public string getStabilizerModeStr ()
	{
		string sResult = "none";
	
		if (stabilizerFactor == 0.0f)
			return "none";
	
		switch (stabilizerMode) {
		case 0:	// auto
			float level = m_Car.antiRollLevel / stabilizerFactor; 
	
			if (level < STAB_OFFROAD)
				sResult = "none";
			else if (level < STAB_COMFORT)
				sResult = "Offroad";
			else if (level < STAB_SPORT)
				sResult = "Comfort";
			else
				sResult = "Sport";
			break;
			
		case 1:
			sResult = "Offroad";
			break;
		case 2:
			sResult = "Comfort";
			break;
		case 3:
			sResult = "Sport";
			break;
		}
	
		return sResult;
	}
	
	public string getStabilizerModeShortStr ()
	{
		string sResult = "none";
	
		if (stabilizerFactor == 0.0f)
			return "none";
	
		switch (stabilizerMode) {
		case 0:
			sResult = "AUTO";
			break;
		case 1:
			sResult = "offroad";
			break;
		case 2:
			sResult = "comfort";
			break;
		case 3:
			sResult = "sport";
			break;
		}
	
		return sResult;
	}
	
	public string getTractionAxleStr ()
	{
		string sResult = "?";
	
		switch (tractionAxle) {
		case 0:
			sResult = "front";
			break;
		case 1:
			sResult = "rear";
			break;
		case 2:
			sResult = "4x4";
			break;
		}
		
		return sResult;
	}
}
