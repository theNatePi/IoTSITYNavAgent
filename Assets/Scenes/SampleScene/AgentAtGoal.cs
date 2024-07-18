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
    // public List<NavMeshAgent> agentPopulation = new List<NavMeshAgent>();
    HashSet<NavMeshAgent> uniqueAgents = new HashSet<NavMeshAgent>();

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Agent"))
            {
                // Agent is within 3 units
                Debug.Log("Agent detected within 3 units");
                // Do something with the agent, e.g.,
                NavMeshAgent agent = collider.GetComponent<NavMeshAgent>();
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
