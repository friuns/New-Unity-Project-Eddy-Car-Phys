using UnityEngine;
using System.Collections;

public class ForwardEvents : MonoBehaviour
{
    public WeaponMachineGun weapon;
    private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];
    public void OnParticleCollision(GameObject other)
    {
        int safeLength = particleSystem.safeCollisionEventSize;
        if (collisionEvents.Length < safeLength)
            collisionEvents = new ParticleSystem.CollisionEvent[safeLength];

        int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
        var cc = other.transform.root.GetComponent<CarControl>();
        int i = 0;
        while (i < numCollisionEvents)
        {
            weapon.OnCollision(collisionEvents[i], cc);
            i++;
        }
    }
}