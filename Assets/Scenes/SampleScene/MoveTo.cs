// MoveTo.class
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour {
    GameObject[] evacPoints;
    NavMeshAgent agent;
    NavMeshObstacle obstacle;
    float updateTimer = 0f;
    float updateInterval = 2f;
    float evacDespawnCount = 0f;

    [TextArea(1,1000)]
    public string README = "1) Add a NavMeshAgent to this game object\n2) Create objects for evac points and other POIs, ensure they are touching the NavMesh\n3) Add tags to these points and update the code with functionality for each tag (see comments). For evac points, include the tag \"EvacPoint\"";

    // Set available classes for your agent here
    public enum AgentClass { None, Student }

    // Public attributes
    [Tooltip("Time before agent is deleted once it reaches the EvacPoint"), Range(1f, 500f)]
    public float evacDespawnDelay = 1f;
    [Tooltip("When false, agent will follow a schedule based off of their class. When true, agent will move to nearest EvacPoint")]
    public bool evacuate = false;
    public AgentClass currentClass;

    // Tags for points
    readonly string evacTag = "EvacPoint";


    Vector3 GetEvacPoint(NavMeshAgent agent) {
        evacPoints = GameObject.FindGameObjectsWithTag(evacTag);

        float minPathLength = Mathf.Infinity;

        GameObject targetGoal = null;

        foreach (GameObject goal in evacPoints) {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(goal.transform.position, path);

            float pathLength = 0.0f;
            if (path.status == NavMeshPathStatus.PathComplete) {
                for (int i = 1; i < path.corners.Length; ++i) {
                    pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
            }

            if (pathLength < minPathLength) {
            minPathLength = pathLength;
            targetGoal = goal;
            }
        }

        return targetGoal.transform.position; 
    }


    Vector3 GetGoal (NavMeshAgent agent) {
        if (evacuate) {
            return GetEvacPoint(agent);
        } else {
            switch (currentClass) {
                case AgentClass.Student:
                    return agent.transform.position;
                case AgentClass.None:
                    return agent.transform.position;
                default:
                    return agent.transform.position;
            }
        }
    }

  
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();

        agent.destination = GetGoal(agent);
    }


    void Update() {
        updateTimer += Time.deltaTime;

        if (updateTimer >= updateInterval) {
            agent.destination = GetGoal(agent);
            if ((agent.remainingDistance <= agent.stoppingDistance + 1) & evacuate) {
                if (evacDespawnCount == evacDespawnDelay) {
                    Destroy(gameObject);
                } else {
                    evacDespawnCount++;
                }
            } else {
                evacDespawnCount = 0;
            }
        }
    }
}
