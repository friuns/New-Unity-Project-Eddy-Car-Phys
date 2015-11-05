
using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class CamSmoothFollow : MonoBehaviour
{
    
    public Transform target;
    public Rigidbody targetrigidbody;
    public float distance = 10.0f;
    public float height = 5.0f;
    public float targetHeightRatio = 0.5f;
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;
    public bool followVelocity = true;
    public float velocityDamping = 5.0f;
    private Vector3 lastPos = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;
    private float wantedRotationAngle = 0.0f;
    [HideInInspector]
    public bool reset = true;
    public void Start()
    {
        if (target)
            targetrigidbody = target.rigidbody;
    }
    void LateUpdate()
    {
        if (!target)
            return;
        if (reset)
        {
            lastPos = target.position;
            wantedRotationAngle = target.eulerAngles.y;
            currentVelocity = target.forward * 2.0f;
            reset = false;
        }
        Vector3 updatedVelocity = (target.position - lastPos) / Time.deltaTime;
        updatedVelocity.y = 0.0f;
        if (updatedVelocity.magnitude > 1.0f)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, updatedVelocity, velocityDamping * Time.deltaTime);
            wantedRotationAngle = Mathf.Atan2(currentVelocity.x, currentVelocity.z) * Mathf.Rad2Deg;
        }
        lastPos = target.position;
        if (!followVelocity)
            wantedRotationAngle = target.transform.eulerAngles.y;
        /*
        var velocity = (target.position - lastPos) / Time.deltaTime;
        velocity.y = 0.0;
        var wantedRotationAngle = target.eulerAngles.y;
        if (velocity.magnitude > 1.0)
            wantedRotationAngle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
        lastPos = target.position;
        */
        if (bsInput.Input2.GetKey(KeyCode.Q))
            wantedRotationAngle += 180;
        float wantedHeight = target.position.y + height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        shake = Mathf.Lerp(shake, 0, Time.deltaTime * 20);
        transform.position = target.position + Random.insideUnitSphere * shake;
        transform.position -= currentRotation * Vector3.forward * distance;
        Vector3 t = transform.position;
        t.y = currentHeight;
        transform.position = t;
        if (targetrigidbody)
        {
            Vector3 CoM = Vector3.Scale(target.rigidbody.centerOfMass, new Vector3(1.0f / target.transform.localScale.x, 1.0f / target.transform.localScale.y, 1.0f / target.transform.localScale.z));
            CoM = target.transform.TransformPoint(CoM);
            transform.LookAt(CoM + Vector3.up * height * targetHeightRatio);
        }
        else
            transform.LookAt(target.position + Vector3.up * height * targetHeightRatio);
        
    }
    public float shake = 0;
}
