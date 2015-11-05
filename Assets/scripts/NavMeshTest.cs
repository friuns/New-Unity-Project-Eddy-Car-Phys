using UnityEngine;

public class NavMeshTest : bs
{
    public NavMeshAgent agent;
    public Transform target;
    public void Update()
    {
        agent.SetDestination(target.position);
        Debug.DrawLine(pos, agent.nextPosition);
    }
}