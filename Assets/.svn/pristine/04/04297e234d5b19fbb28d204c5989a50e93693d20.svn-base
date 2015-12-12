using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class CarDamage : bs
{
    public MeshTest meshTest;
    private CarVisuals m_CarVisuals;
    public float minForce = 1.0f;
    public float multiplier = 0.1f;

    void Start()
    {
        meshTest = GetComponentInChildren<MeshTest>();
        //var player = GetComponent<CarControl>().pl;
        //if (player != null)
        //    player.meshTest = GetComponentInChildren<MeshTest>();
        m_CarVisuals = GetComponent<CarVisuals>() as CarVisuals;

    }

    void Update()
    {
        Vector3 contactForce = Vector3.zero;
        if (m_CarVisuals.localImpactVelocity.sqrMagnitude > minForce * minForce)
            contactForce = tr.TransformDirection(m_CarVisuals.localImpactVelocity) * multiplier * 0.2f;
        else if (m_CarVisuals.localDragVelocityDiscrete.sqrMagnitude > minForce * minForce)
            contactForce = tr.TransformDirection(m_CarVisuals.localDragVelocityDiscrete) * multiplier * 0.01f;
        if (contactForce.sqrMagnitude > 0.0f)
        {
            Vector3 contactPoint = tr.TransformPoint(m_CarVisuals.localImpactPosition);
            CarControl cc = tr.root.GetComponent<CarControl>();
            if (cc && cc.pl)
            {

                contactForce = contactForce * 10 / Mathf.Max(3, rigidbody.velocity.magnitude);
                Player player = m_CarVisuals.colidesWith.value;
                if (player != null)
                    cc.pl.OnPlayerCollision(contactForce, contactPoint, player);

                if (player == null || player == _Player || cc.pl == _Player || _AutoQuality.destrQuality >= 2)
                {
                    cc.pl.OnCollision(contactPoint, contactForce.magnitude * (player == _Player || player == null ? 3 : 1));
                    _Game.Emit(contactPoint, contactForce, _Game.sparks);
                }

            }
        }

    }

}