// MoveTo.class
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public abstract class AgentBase : MonoBehaviour {
    // Start is called before the first frame update
    public abstract void Start();

    // Update is called once per frame
    public abstract void Update();
}

public class StudentClass : AgentBase {
    [SerializeField]
    public int test = 5;
    
    public override void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    public override void Update()
    {
        Debug.Log("Update");
    }
}

public class MoveTo : MonoBehaviour {
    NavMeshAgent agent;
    NavMeshObstacle obstacle;
    float updateTimer = 0f;
    float updateInterval = 2f;
    float evacDespawnCount = 0f;
    bool reachedGoal = true;
    public GameObject currentGoal;

    [TextArea(1,1000)]
    public string README = "1) Add a NavMeshAgent to this game object\n2) Create objects for evac points and other POIs, ensure they are touching the NavMesh\n3) Add tags to these points and update the code with functionality for each tag (see comments). For evac points, include the tag \"EvacPoint\"";

    // Set available classes for your agent here
    public enum AgentClass { None, Student }

    // Public attributes
    [Tooltip("Time before agent is deleted once it reaches the EvacPoint"), Range(2f, 500f)]
    public float evacDespawnDelay = 10f;
    [Tooltip("When false, agent will follow a schedule based off of their class. When true, agent will move to nearest EvacPoint")]
    public bool evacuate = false;
    public AgentClass currentClass;

    // Tags for points
    readonly string evacTag = "EvacPoint";
    readonly string studentTag = "StudentTest";

    // other
    List<GameObject> evacPoints;

    AgentBase ClassObject;


    Vector3 GetEvacPoint(NavMeshAgent agent) {
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

        currentGoal = targetGoal;
        return targetGoal.transform.position; 
    }

    Vector3 GetStudentGoal(NavMeshAgent agent) {
        GameObject[] studentPoints;
        studentPoints = GameObject.FindGameObjectsWithTag(studentTag);

        System.Random random = new();

        // Get a random index within the array's length
        int randomIndex = random.Next(studentPoints.Length);

        // Access the random element
        GameObject randomElement = studentPoints[randomIndex];

        currentGoal = randomElement;
        return randomElement.transform.position; 
    }


    Vector3 GetGoal (NavMeshAgent agent) {
        if (evacuate) {
            return GetEvacPoint(agent);
        } else {
            switch (currentClass) {
                case AgentClass.Student:
                    return GetStudentGoal(agent);
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
        MeshRenderer Mesh;
        Mesh = GetComponent<MeshRenderer>();
        Vector3 a = Mesh.transform.position;

        evacPoints = GameObject.FindGameObjectsWithTag(evacTag).ToList();

        switch (currentClass) {
            case AgentClass.Student:
                ClassObject = gameObject.AddComponent<StudentClass>();
                break;
            case AgentClass.None:
                ClassObject = gameObject.AddComponent<AgentBase>();
                break;
            default:
                ClassObject = gameObject.AddComponent<AgentBase>();
                break;
        }

        agent.destination = GetGoal(agent);
    }


    void Update() {
        updateTimer += Time.deltaTime;

        if (updateTimer >= updateInterval) {
            if (reachedGoal & !evacuate) {
                agent.destination = GetGoal(agent);
                reachedGoal = false;
            } else if (evacuate) {
                agent.destination = GetGoal(agent);
            }
            if (agent.remainingDistance <= agent.stoppingDistance + 1) {
                if (evacuate) {
                    if (evacDespawnCount == evacDespawnDelay) {
                        // Destroy(gameObject);
                        AgentAtGoal goalScript = currentGoal.GetComponent<AgentAtGoal>();
                        if (goalScript.population >= goalScript.capacity) {
                            // This evac point is full
                            Debug.Log("evac point full");
                            evacPoints.Remove(currentGoal);

                            if (evacPoints.Count > 0) {
                                agent.destination = GetGoal(agent);
                            } else {
                                Debug.Log("No evacuation points available!");
                                evacuate = false;
                            }
                        }
                    } else {
                        evacDespawnCount++;
                    }
                } else {
                    reachedGoal = true;
                }
            } else {
                evacDespawnCount = 0;
            }
        }
    }
}
