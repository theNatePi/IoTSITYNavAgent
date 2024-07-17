using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAtGoal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("s");
        // Check if colliding object is an agent
        if (other.CompareTag("Agent"))
        {
            Debug.Log("contact");
            GameObject agent = other.gameObject;
            // Remove the agent (choose your desired removal method)
            Destroy(agent);  // Destroys the entire GameObject
            // Alternatively, you can disable the agent and its components:
            // agent.SetActive(false);
            // agent.GetComponent<NavMeshAgent>().enabled = false;
        }
    }
}
