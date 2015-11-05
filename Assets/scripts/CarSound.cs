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
// CarSound
//
// Configures and controls all the sounds and effects for the vehicle. 
// Data is gathered from CarVisuals and CarSettings scripts.
//
//========================================================================================================================
using UnityEngine;
using System.Collections;

public partial class CarSound : bs
{
    public void Awake()
    {
        //EngineAudio.clip = res.engineAudio;
        //TransmissionAudio.clip = res.transmission;
    }
	public AudioSource EngineAudio;			// Main audio loop for the engine, based on the RPM
    
	public float engineIdleRPM = 600.0f;
	public float engineGearDownRPM = 3000.0f;			// GearDown - GearUp define the range for each gear. 
	public float engineGearUpRPM = 5000.0f;			// Adjust GearDown - GearUp together with transmissionRatio and gearCount to get a proper gear distribution along the speed range of the vehicle.
	public float engineMaxRPM = 6000.0f;
	public int gearCount = 5;
	public float transmissionRatio = 13.6f;
	public float engineGearUpSpeed = 5.0f;			// How fast are the gear shift transitions. Small values can be used to emulate true automated transmissions.
	public float engineGearDownSpeed = 20.0f;
	public float engineAudioIdlePitch = 0.6f;			// Pitch settings for the engine audio source
	public float engineAudioMaxPitch = 3.5f;
	public float engineAudioIdleVolume = 0.4f;		// Volume settings for the engine audio source
	public float engineAudioMaxVolume = 0.55f;
	public AudioSource EngineExtraAudio;		// Customizable extra audio loop for the engine at certain range of the RPM

	public float engineExtraMinRPM = 4000.0f;			// Customizable additional engine audio (i.e. turbo)
	public float engineExtraMaxRPM = 5600.0f;
	public float engineExtraMinPitch = 0.8f;
	public float engineExtraMaxPitch = 1.5f;
	public float engineExtraMinVolume = 0.1f;
	public float engineExtraMaxVolume = 1.0f;
	public AudioSource TransmissionAudio;	// Audio loop based on the transmission, depends on the longitudinal speed of the vehicle.

	public float transmissionMinRPM = 4000.0f;		// Customizable transmision audio (road rumble, single-geared engine sound, ...)
	public float transmissionMaxRPM = 12000.0f;
	public float transmissionMinPitch = 0.5f;
	public float transmissionMaxPitch = 1.6f;
	public float transmissionMinVolume = 0.1f;
	public float transmissionMaxVolume = 0.8f;
	public AudioSource VelocityAudio;		// Audio loop based on the velocity of the vehicle, regardless engine or transmission (ej. wind effects)

	public float velocityMin = 3.0f;					// Velocity-based audio (wind)
	public float velocityMax = 30.0f;					// m/s
	public float velocityMinPitch = 0.5f;
	public float velocityMaxPitch = 1.0f;
	public float velocityMinVolume = 0.0f;
	public float velocityMaxVolume = 0.4f;
	public AudioSource SkidAudio;			// Audio loop for skidding tires / burnouts

	public float skidMin = 0.2f;						// Skid audio
	public float skidMax = 1.0f;						// Min, Max: wheels skidding. 0.0 = no wheels skidding, 1.0 = one wheel skidding full (or 4 wheels skidding 25% each), 4.0 = four wheels skidding full
	public float skidMaxVolume = 1.0f;
	public float skidMinPitch = 1.0f;
	public float skidMaxPitch = 0.9f;
	public AudioSource OffroadAudio;			// Audio loop for wheels rolling offroad

	public float offroadSilent = 0.02f;				// Offroad audio
	public float offroadMin = 1.0f;					// m/s 
	public float offroadMax = 10.0f;
	public float offroadMinPitch = 0.3f;
	public float offroadMaxPitch = 1.5f;
	public float offroadMinVolume = 0.3f;
	public float offroadMaxVolume = 0.8f;
	public AudioClip WheelBumpAudio;			// "bump" impacts at wheels due to suspension stress. AudioClip, will be played as "one shot" when needed.

	public float bumpMinForce = 4000.0f;				// Newtons, delta value at the suspension
	public float bumpMaxForce = 18000.0f;
	public float bumpMinVolume = 0.2f;
	public float bumpMaxVolume = 0.6f;
	public AudioSource BodyDragAudio;			// Audio loop for the bodywork being dragged over hard surfaces (also played offroad if no BodyDragOffroadAudio is present)
	public AudioSource BodyDragOffroadAudio;		// Audio loop for the bodywork being dragged over offroad surfaces only

	public float dragSilent = 0.01f;						// m/s
	public float dragMin = 2.0f;
	public float dragMax = 20.0f;
	public float dragMinPitch = 0.9f;
	public float dragMaxPitch = 1.0f;
	public float dragMinVolume = 0.9f;
	public float dragMaxVolume = 1.0f;
	public AudioClip BodyImpactAudio;			// Audio clip for body impacts over hard surfaces (also played offroad if no BodyImpactOffroadAudio is present)
	public AudioClip BodyImpactOffroadAudio;		// Audio clip for body impacts over offroad surfaces only

	public float impactMin = 0.1f;						// m/s
	public float impactMax = 10.0f;
	public float impactMinPitch = 0.3f;
	public float impactMaxPitch = 0.6f;
	public float impactRandomPitch = 0.1f;
	public float impactMinVolume = 0.8f;
	public float impactMaxVolume = 1.0f;
	public float impactRandomVolume = 0.1f;
	public AudioClip BodyScratchAudio;			// Audio clip for random body scratch effects against hard surfaces

	public float scratchMin = 2.0f;						// m/s
	public float scratchRandom = 0.02f;
	public float scratchInterval = 0.2f;
	public float scratchMinPitch = 0.7f;
	public float scratchMaxPitch = 1.1f;
	public float scratchMinVolume = 0.9f;
	public float scratchMaxVolume = 1.0f;
	private float m_lastScratchTime = 0.0f;
	public int currentGear = 0;				// No configuration here - just for debugging purposes on the inspector.
	public float transmissionRPM = 0.0f;
	public float engineRPM = 0.0f;
	public float skidValue = 0.0f;
	public float offroadValue = 0.0f;
	public CarVisuals m_CarVisuals;
    private CarControl m_CarControl;
	private CarSettings m_CarSettings;
	private int m_lastGear = 0;
	private float m_engineDamp;

	void OnEnable ()
	{
		// Retrieve the components which will be used later for gathering the vehicle's settings and the speed at each wheel.
	
		m_CarVisuals = GetComponent<CarVisuals> () as CarVisuals;
		m_CarSettings = GetComponent<CarSettings> () as CarSettings;
	    m_CarControl = GetComponent<CarControl>();
		// Parameter constraints and settings
	
		if (gearCount < 2)
			gearCount = 2;
	
		m_engineDamp = engineGearUpSpeed;
	}

	void Update ()
	{
		float averageWheelRate;
	
		// Retrieve the average wheel spin rate of the drive wheels
	
		switch (m_CarSettings.tractionAxle) {
		case 0:
			averageWheelRate = (m_CarVisuals.spinRateFL + m_CarVisuals.spinRateFR) * 0.5f;
			break;	// Front drive
		case 1:
			averageWheelRate = (m_CarVisuals.spinRateRL + m_CarVisuals.spinRateRR) * 0.5f;
			break;	// Rear drive
		default: 																						// Full 4x4 drive
			averageWheelRate = (m_CarVisuals.spinRateFL + m_CarVisuals.spinRateFR + m_CarVisuals.spinRateRL + m_CarVisuals.spinRateRR) * 0.25f;
			break;
		}
	
		// Get the RPM at the output of the gearbox. spinRate is rads/s, need RPM.

		transmissionRPM = averageWheelRate * Mathf.Rad2Deg / 6.0f;			// 6.0 = 360.0 / 60.0
		transmissionRPM *= transmissionRatio;
	
		// Calculate the engine RPM according to three possible states:
		// - Stopped
		// - Moving forward. The top gear can increase the sound pitch until its limit
		// - Reverse. Single gear, sound pitch is increased until its limit
	
		float updatedEngineRPM;
	
		if (Mathf.Abs (averageWheelRate) < 1.0f) {
			currentGear = 0;
			updatedEngineRPM = engineIdleRPM + Mathf.Abs (transmissionRPM);
		} else if (transmissionRPM >= 0) {
			// First gear goes from idle to gearUp
		
			float firstGear = engineGearUpRPM - engineIdleRPM;	
		
			if (transmissionRPM < firstGear) {
				currentGear = 1;
				updatedEngineRPM = transmissionRPM + engineIdleRPM;
			} else {
				// Second gear and above go from gearDown to gearUp
				
				float gearWidth = engineGearUpRPM - engineGearDownRPM;
			
				currentGear = 2 + (int) ((transmissionRPM - firstGear) / gearWidth);
			
				if (currentGear > gearCount) {
					currentGear = gearCount;
					updatedEngineRPM = transmissionRPM - firstGear - (gearCount - 2) * gearWidth + engineGearDownRPM;
				} else
					updatedEngineRPM = Mathf.Repeat (transmissionRPM - firstGear, gearWidth) + engineGearDownRPM;
			}
		} else {
			// Reverse gear
		
			currentGear = -1;
			updatedEngineRPM = Mathf.Abs (transmissionRPM) + engineIdleRPM;
		}
		
		updatedEngineRPM = Mathf.Clamp (updatedEngineRPM, 10.0f, engineMaxRPM);	

		// Calculate engine damp according to latest shift up or shift down
	
		if (currentGear != m_lastGear) {
            if (currentGear > m_lastGear && m_CarControl.pl)
                m_CarControl.pl.PlayOneShot(res.gearChange.Random(), forcePlay: false);
			m_engineDamp = currentGear > m_lastGear ? engineGearUpSpeed : engineGearDownSpeed;
			m_lastGear = currentGear;
		}
	
		// Final engine RPM
	
		engineRPM = Mathf.Lerp (engineRPM, updatedEngineRPM, m_engineDamp * Time.deltaTime);
	
		// Engine audio pitch and volume according to the configured range and engine RPM
	
		if (EngineAudio)
			ProcessContinuousAudio (EngineAudio, engineRPM, engineIdleRPM, engineMaxRPM, engineAudioIdlePitch, engineAudioMaxPitch, engineAudioIdleVolume, engineAudioMaxVolume);
		
		// Extra engine audio
	
		if (EngineExtraAudio)
			ProcessContinuousAudio (EngineExtraAudio, engineRPM, engineExtraMinRPM, engineExtraMaxRPM, engineExtraMinPitch, engineExtraMaxPitch, engineExtraMinVolume, engineExtraMaxVolume);
		
		// Transmission audio
	
		if (TransmissionAudio)
			ProcessContinuousAudio (TransmissionAudio, Mathf.Abs (transmissionRPM), transmissionMinRPM, transmissionMaxRPM, transmissionMinPitch, transmissionMaxPitch, transmissionMinVolume, transmissionMaxVolume);
        
		// Velocity audio
	
		if (VelocityAudio)
			ProcessContinuousAudio (VelocityAudio, rigidbody.velocity.magnitude, velocityMin, velocityMax, velocityMinPitch, velocityMaxPitch, velocityMinVolume, velocityMaxVolume);

		// Skid values from CarVisuals:
		//   > 0 = skidding over asphalt / hard surfaces. we use the sum of all wheels (a single wheel skidding to the top causes maximum skid)
		//   < 0 = rolling / skidding over offroad surfaces. we use the average value of all wheels.
		
		float asphaltSkid = 0.0f;
		float offroadSkid = 0.0f;
		int offroadWheels = 0;
	
		if (m_CarVisuals.skidValueFL >= 0.0f)
			asphaltSkid += m_CarVisuals.skidValueFL;
		else {
			offroadSkid -= m_CarVisuals.skidValueFL;
			offroadWheels++;
		}
		if (m_CarVisuals.skidValueFR >= 0.0f)
			asphaltSkid += m_CarVisuals.skidValueFR;
		else {
			offroadSkid -= m_CarVisuals.skidValueFR;
			offroadWheels++;
		}
		if (m_CarVisuals.skidValueRL >= 0.0f)
			asphaltSkid += m_CarVisuals.skidValueRL;
		else {
			offroadSkid -= m_CarVisuals.skidValueRL;
			offroadWheels++;
		}
		if (m_CarVisuals.skidValueRR >= 0.0f)
			asphaltSkid += m_CarVisuals.skidValueRR;
		else {
			offroadSkid -= m_CarVisuals.skidValueRR;
			offroadWheels++;
		}
		
		if (offroadWheels > 1)
			offroadSkid /= offroadWheels;
		
		// Skid audio
	
		skidValue = Mathf.Lerp (skidValue, asphaltSkid, 40.0f * Time.deltaTime);
		if (SkidAudio)
			ProcessContinuousAudio (SkidAudio, skidValue, skidMin, skidMax, skidMinPitch, skidMaxPitch, 0.0f, skidMaxVolume);
		
		// Offroad audio
	
		offroadValue = Mathf.Lerp (offroadValue, offroadSkid, 20.0f * Time.deltaTime);
		if (OffroadAudio) {
			ProcessSpeedBasedAudio (OffroadAudio, offroadValue, offroadSilent, offroadMin, offroadMax, 0.0f, offroadMinPitch, offroadMaxPitch, offroadMinVolume, offroadMaxVolume);
		}
		
		// Wheel bumps
	
		if (WheelBumpAudio) {
			ProcessWheelBumpAudio (m_CarVisuals.suspensionStressFL, m_CarVisuals.PivotFL);
			ProcessWheelBumpAudio (m_CarVisuals.suspensionStressFR, m_CarVisuals.PivotFR);
			ProcessWheelBumpAudio (m_CarVisuals.suspensionStressRL, m_CarVisuals.PivotRL);
			ProcessWheelBumpAudio (m_CarVisuals.suspensionStressRR, m_CarVisuals.PivotRR);
		}
		
		// Body drag audio
	
		float dragSpeed = m_CarVisuals.localDragVelocity.magnitude;
	
		if (BodyDragAudio)
			ProcessSpeedBasedAudio (BodyDragAudio, !m_CarVisuals.localDragSoftSurface || !BodyDragOffroadAudio ? dragSpeed : 0.0f, dragSilent, dragMin, dragMax, dragMinPitch, dragMinPitch, dragMaxPitch, dragMinVolume, dragMaxVolume);

		if (BodyDragOffroadAudio)
			ProcessSpeedBasedAudio (BodyDragOffroadAudio, m_CarVisuals.localDragSoftSurface ? dragSpeed : 0.0f, dragSilent, dragMin, dragMax, dragMinPitch, dragMinPitch, dragMaxPitch, dragMinVolume, dragMaxVolume);
		
		// Body impacts audio
	
		float impactSpeed = m_CarVisuals.localImpactVelocity.magnitude;
	
		if (BodyImpactAudio || BodyImpactOffroadAudio) {		
			if (impactSpeed > impactMin) {
				float impactRatio = Mathf.InverseLerp (impactMin, impactMax, impactSpeed);
                //AudioClip clip = null;
			
                //if (BodyImpactAudio && (!m_CarVisuals.localImpactSoftSurface || !BodyImpactOffroadAudio))
                //    clip = BodyImpactAudio;
                //else if (m_CarVisuals.localImpactSoftSurface)
                //    clip = BodyImpactOffroadAudio;
				
                //if (clip)
			    var volume = Mathf.Lerp(impactMinVolume, impactMaxVolume, impactRatio) + Random.Range(-impactRandomVolume, impactRandomVolume)*.1f;
			    PlayOneTime(res.hitSound.Random(), tr.TransformPoint(m_CarVisuals.localImpactPosition), volume, Mathf.Lerp(impactMinPitch, impactMaxPitch, impactRatio) + Random.Range(-impactRandomPitch, impactRandomPitch));
			}
		}
		
		// Random body scratch on drags
		
        //if (BodyScratchAudio) 
        {
            
			if (dragSpeed > scratchMin && !m_CarVisuals.localDragSoftSurface && Random.value < scratchRandom && Time.time - m_lastScratchTime > scratchInterval) {
			    var volume = Random.Range(scratchMinVolume, scratchMaxVolume)* .1f;
			    PlayOneTime(res.hitSoundBig.Random(), tr.TransformPoint(m_CarVisuals.localDragPosition), volume, Random.Range(scratchMinPitch, scratchMaxPitch));
				m_lastScratchTime = Time.time;
			}
		}
	}
	
	private void ProcessContinuousAudio (AudioSource Audio, float audioValue, float audioMin, float audioMax, float minPitch, float maxPitch, float minVolume, float maxVolume)
	{
		float audioRatio = Mathf.InverseLerp (audioMin, audioMax, audioValue);
	
		Audio.pitch = Mathf.Lerp (minPitch, maxPitch, audioRatio);
        Audio.volume = Mathf.Lerp(minVolume, maxVolume, audioRatio) * res.volumeFactor;
	
		if (!Audio.isPlaying)
			Audio.Play ();
		Audio.loop = true;
	}

	private void ProcessSpeedBasedAudio (AudioSource Audio, float audioValue, float audioSilent, float audioMin, float audioMax, float silentPitch, float minPitch, float maxPitch, float minVolume, float maxVolume)
	{
		if (audioValue < audioSilent) {
			if (Audio.isPlaying)
				Audio.Stop ();
		} else {
			
			float audioRatio;
			if (audioValue < audioMin) {
				audioRatio = Mathf.InverseLerp (audioSilent, audioMin, audioValue);
			
				Audio.pitch = Mathf.Lerp (silentPitch, minPitch, audioRatio);
                Audio.volume = Mathf.Lerp(0.0f, minVolume, audioRatio) * res.volumeFactor;
			} else {
				audioRatio = Mathf.InverseLerp (audioMin, audioMax, audioValue);
			
				Audio.pitch = Mathf.Lerp (minPitch, maxPitch, audioRatio);
				Audio.volume = Mathf.Lerp (minVolume, maxVolume, audioRatio)*res.volumeFactor;
			}
			
			if (!Audio.isPlaying)
				Audio.Play ();
			Audio.loop = true;
		}
	}
	
	private void ProcessWheelBumpAudio (float suspensionStress, Transform suspensionPoint)
	{
		float bumpRatio = Mathf.InverseLerp (bumpMinForce, bumpMaxForce, suspensionStress);
		if (bumpRatio > 0.0f)
			PlayOneTime (WheelBumpAudio, suspensionPoint.position, Mathf.Lerp (bumpMinVolume, bumpMaxVolume, bumpRatio)*res.volumeFactor);
	}
		
	void PlayOneTime (AudioClip clip, Vector3 position, float volume)
	{
		PlayOneTime (clip, position, volume, 1.0f);
	}

	void PlayOneTime (AudioClip clip, Vector3 position, float volume, float pitch)
	{
		if (clip == null)
			return;
	
		GameObject go = new GameObject ("One shot audio");
		go.transform.parent = tr;
		go.transform.position = position;
		AudioSource source = go.AddComponent <AudioSource>() as AudioSource;
		source.clip = clip;
		source.volume = volume;
        source.pitch = pitch;
		source.Play ();
		Destroy (go, clip.length*Time.timeScale);
	}
}
