using UnityEngine;

public partial class CarVisuals
{

    public AccessTimer<Player> colidesWith = new AccessTimer<Player>() { frameCount = 10 };
    //internal Vector3 oldAng;
    //public void FixedUpdate()
    //{
    //    oldAng = rigidbody.angularVelocity;
    //}


    public void OnCollisionEnter(Collision collision)
    {
        ProcessContacts(collision, true);
        PostProcess(collision, true);

        //if (m_Car.pl == _Player && collision.contacts.Length > 0 && !(collision.contacts[0].thisCollider is WheelCollider))
        //    _MainCamera.m_scrSmoothFollow.shake = Mathf.Min(2, (collision.relativeVelocity - rigidbody.velocity).magnitude*.05f) * .3f;
    }
    public void OnCollisionStay(Collision collision)
    {        
        ProcessContacts(collision, false);
        PostProcess(collision, false);
    }
    CarControl cc;

    
    public void PostProcess(Collision collision, bool enter)
    {
        Rigidbody otherRig = collision.rigidbody;
        if (enter && otherRig)
            cc = otherRig.GetComponent<CarControl>();

        if (otherRig && cc)
        {
            float f = Mathf.Clamp((otherRig.velocity.magnitude - rigidbody.velocity.magnitude) * Mathf.Max(1, otherRig.mass / rigidbody.mass), .5f, 2);
            m_sumImpactVelocity *= f;
            if (!android && room.collFix)
            {
                //ZeroY();
                rigidbody.angularVelocity = new Vector3(0, rigidbody.angularVelocity.y, 0);
            }
            colidesWith.value = cc.pl;
            if (enter && (colidesWith.value == _Player || m_Car.pl == _Player))
                colidesWith.value.collisionTime = m_Car.pl.collisionTime = Time.time;

        }

    }
    private void ZeroY()
    {
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, Mathf.Clamp(rigidbody.velocity.y, 0, -1), rigidbody.velocity.z);
    }
    
}