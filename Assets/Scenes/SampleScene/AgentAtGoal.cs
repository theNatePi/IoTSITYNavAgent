using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AgentAtGoal : MonoBehaviour
{
    public float detectionRadius = 1f;

    [Range(1, 100000)]
    public int capacity = 10;
    public int population = 0;
    HashSet<NavMeshAgent> uniqueAgents = new HashSet<NavMeshAgent>();

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Agent"))
            {
                NavMeshAgent agent = collider.GetComponent<NavMeshAgent>();
                MoveTo agentScript = collider.GetComponent<MoveTo>();
                if (!agentScript.evacuate) {
                    // The agent is not evacuating, don't despawn them
                    continue;
                }
                if (agent != null && !uniqueAgents.Contains(agent)) {
                    if (population < capacity) {
                        uniqueAgents.Add(agent);
                        population++;
                        Destroy(collider.gameObject);
                    }
                }
            }
        }
    }
}
