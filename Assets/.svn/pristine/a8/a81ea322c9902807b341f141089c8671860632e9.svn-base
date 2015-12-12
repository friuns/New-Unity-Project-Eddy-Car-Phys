using UnityEngine;
using System.Collections;

public class CamSmoothLookAtFromPos : MonoBehaviour
{

	public Transform pos;
	public Transform target;
	public float damping = 6.0f;
	public float positionZ = -10.0f;
	public float positionY = 2.0f;

	void LateUpdate ()
	{	
		if (!target)
			return;

		Vector3 targetpos;
	
		if (target.rigidbody)
			targetpos = target.rigidbody.worldCenterOfMass;
		else
			targetpos = target.position;

		// Look at and dampen the rotation
		Quaternion rotation = Quaternion.LookRotation (targetpos - transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * damping);
		
		if (pos)
			transform.position = pos.transform.position + Vector3.up * positionY + transform.forward * positionZ;
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
}
