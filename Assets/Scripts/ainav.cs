using UnityEngine;
using UnityEngine.AI;
public class ainav : MonoBehaviour
{
    public float wanderRadius = 10f;      // How far the agent can wander
    public float wanderTimer = 5f;        // How often it picks a new point

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        // Only act if the agent is on the NavMesh
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Agent is not on the NavMesh!");
            return;
        }

        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, NavMesh.AllAreas);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int areaMask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, distance, areaMask))
        {
            return navHit.position;
        }

        return origin; // fallback if no valid position
    {
        
    }
}
}
