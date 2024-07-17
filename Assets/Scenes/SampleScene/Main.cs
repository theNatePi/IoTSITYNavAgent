// using System.Collections;
// using System.Collections.Generic;
// using JetBrains.Annotations;
// using UnityEngine;
// using UnityEngine.AI;

// public class Main : MonoBehaviour
// {
//     // Start is called before the first frame update
//     void Start()
//     {
//         Debug.Log("START");
//         GameObject[] agents;
//         agents = GameObject.FindGameObjectsWithTag("Agent");
//         foreach (GameObject agent in agents) {
//             NavMeshAgent NavAgent = agent.GetComponent<NavMeshAgent>();
//             NavAgent.destination = GetGoal(agent);
//         }
//     }

//     Vector3 GetGoal (GameObject agent) {
//         GameObject[] evacPoints;
//         evacPoints = GameObject.FindGameObjectsWithTag("EvacPoint");

//         float minPathLength = Mathf.Infinity;

//         GameObject targetGoal = null;

//         foreach (GameObject goal in evacPoints) {
//             NavMeshPath path = new NavMeshPath();
//             NavMeshAgent NavAgent = agent.GetComponent<NavMeshAgent>();
//             NavAgent.CalculatePath(goal.transform.position, path);

//             float pathLength = 0.0f;
//             if (path.status == NavMeshPathStatus.PathComplete) {
//                 for (int i = 1; i < path.corners.Length; ++i) {
//                     pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
//                 }
//             }

//             if (pathLength < minPathLength) {
//             minPathLength = pathLength;
//             targetGoal = goal;
//             }
//         }

//         return targetGoal.transform.position; 
//     }
// }
