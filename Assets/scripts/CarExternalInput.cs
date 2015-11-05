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
// CarExternalInput
//
// Interface for allowing alternate input methods for the vehicles.
// The class providing the input must inherit from this one so it could be enabled / disabled from CarSettings (CarSettings.externalInput).
//
// This interface allows input methods such IA or network.
// See the example at CarExternalInputRandom, which randomly controls the vehicle.
//
//========================================================================================================================

using UnityEngine;
using System.Collections;

public class CarExternalInput : MonoBehaviour {

	protected CarControl m_CarControl;

	void Start()
	{
		m_CarControl = GetComponent<CarControl>() as CarControl;
	}


	// Override Update or FixedUpdate in the children classes for assigning input values to CarControl
	// Remember you can use the variables from the rigidbody (i.e. rigidbody.velocity) and any other car's component.


/*
	void Update ()
	{
	m_CarControl.steerInput = 0.0f;
	m_CarControl.motorInput = 0.0f;
	m_CarControl.brakeInput = 1.0f;
	m_CarControl.handbrakeInput = 0.0f;
	m_CarControl.gearInput = 1;
	}
*/
}
