// MoveTo.class
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public enum Disabilities {
    None,
    Wheelchair,
    ImpairedVision,
    Crutches
}

public abstract class AgentBase : MonoBehaviour {
    public NavMeshAgent _agent;

    

    // Update is called once per frame
    public abstract void Update();

    

    public virtual void AssignAgent (NavMeshAgent agent) {
        _agent = agent;
    }

    public virtual GameObject GetNextPassive() {return new GameObject();}

    public virtual GameObject GetEvacPoint(List<GameObject> evacPoints) {
        float minPathLength = Mathf.Infinity;

        GameObject targetGoal = null;

        foreach (GameObject goal in evacPoints) {
            NavMeshPath path = new NavMeshPath();
            _agent.CalculatePath(goal.transform.position, path);

            float pathLength = 0.0f;
            if (path.status == NavMeshPathStatus.PathComplete) {
                for (int i = 1; i < path.corners.Length; ++i) {
                    pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
                if (pathLength < minPathLength) {
                    minPathLength = pathLength;
                    targetGoal = goal;
                }
            }

        return targetGoal;
    }
}

public class GenericClass : AgentBase {

    public override void AssignAgent (NavMeshAgent agent) {
        _agent = agent;
    }

    // Update is called once per frame
    public override void Update() {
    }

    public override GameObject GetNextPassive() {
        GameObject[] studentPoints;
        studentPoints = GameObject.FindGameObjectsWithTag("GenericBuilding");

        System.Random random = new();

        // Get a random index within the array's length
        int randomIndex = random.Next(studentPoints.Length);

        // Access the random element
        GameObject randomElement = studentPoints[randomIndex];

        return randomElement; 
    }

    // public override GameObject GetEvacPoint(List<GameObject> evacPoints, NavMeshAgent agent) {

    // }
}

public class MoveTo : MonoBehaviour {
    NavMeshAgent agent;
    float updateTimer = 0f;
    float updateInterval = 2f;
    float evacDespawnCount = 0f;
    bool reachedGoal = true;
    public GameObject currentGoal;
    private GameObject TimeSystem;
    private float _timeScale;
    private DateTime _currentTime;

    [TextArea(1,1000)]
    public string README = "1) Add a NavMeshAgent to this game object\n2) Create objects for evac points and other POIs, ensure they are touching the NavMesh\n3) Add tags to these points and update the code with functionality for each tag (see comments). For evac points, include the tag \"EvacPoint\"";

    // Set available classes for your agent here
    public enum AgentClass { None, Generic }

    // Public attributes
    [Tooltip("Time before agent is deleted once it reaches the EvacPoint"), Range(2f, 500f)]
    public float evacDespawnDelay = 10f;
    [Tooltip("When false, agent will follow a schedule based off of their class. When true, agent will move to nearest EvacPoint")]
    public bool evacuate = false;
    [Tooltip("You cannot change this during playback")]
    public AgentClass currentClass;
    AgentBase ClassObject;

    // Tags for points
    readonly string evacTag = "EvacPoint";

    // other
    List<GameObject> evacPoints;

    // List to hold disabilities
    public List<DisabilityStatus> disList = new List<DisabilityStatus>();

    [Serializable]
    public class DisabilityStatus {
        public Disabilities disability;
        public bool isEnabled;
    }

    private void Awake() {
        foreach (Disabilities disability in Enum.GetValues(typeof(Disabilities))) {
            disList.Add(new DisabilityStatus { disability = disability, isEnabled = false });
        }
        foreach (DisabilityStatus disStat in disList) {
            if (disStat.isEnabled) {
                // switch (disStat) {
                //     case disStat.disability.Equals():
                //         agent.speed = 0.5f;
                // }

            }
        }
    }


    Vector3 GetEvacPoint() {
        currentGoal = ClassObject.GetEvacPoint(evacPoints);
        return currentGoal.transform.position;
    }


    Vector3 GetGoal (NavMeshAgent agent) {
        if (evacuate) {
            return GetEvacPoint();
        } else {
            switch (currentClass) {
                case AgentClass.Generic:
                    currentGoal = ClassObject.GetNextPassive();
                    return currentGoal.transform.position;
                case AgentClass.None:
                    return agent.transform.position;
                default:
                    return agent.transform.position;
                
            }
        }
    }


    void Start () {
        agent = GetComponent<NavMeshAgent>();

        evacPoints = GameObject.FindGameObjectsWithTag(evacTag).ToList();

        TimeSystem = GameObject.Find("TimeSystem");
        _timeScale = TimeSystem.GetComponent<TimeSystem>().timeScale;
        _currentTime = TimeSystem.GetComponent<TimeSystem>().simulatedTime;

        switch (currentClass) {
            case AgentClass.Generic:
                ClassObject = gameObject.AddComponent<GenericClass>();
                ClassObject.AssignAgent(agent);
                break;
            case AgentClass.None:
                ClassObject = gameObject.AddComponent<AgentBase>();
                ClassObject.AssignAgent(agent);
                break;
            default:
                ClassObject = gameObject.AddComponent<AgentBase>();
                ClassObject.AssignAgent(agent);
                break;
        }

        agent.destination = GetGoal(agent);
    }


    void Update() {
        updateTimer += Time.deltaTime;

        _timeScale = TimeSystem.GetComponent<TimeSystem>().timeScale;
        _currentTime = TimeSystem.GetComponent<TimeSystem>().simulatedTime;

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
                        AgentAtGoal goalScript = currentGoal.GetComponent<AgentAtGoal>();
                        if (goalScript.population >= goalScript.capacity) {
                            // This evac point is full
                            evacPoints.Remove(currentGoal);

                            if (evacPoints.Count > 0) {
                                agent.destination = GetGoal(agent);
                            } else {
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

            updateTimer = 0f;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MoveTo))]
public class MoveToEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        MoveTo moveTo = (MoveTo)target;

        EditorGUILayout.LabelField("Disabilities", EditorStyles.boldLabel);

        foreach (var disabilityStatus in moveTo.disList) {
            disabilityStatus.isEnabled = EditorGUILayout.Toggle(disabilityStatus.disability.ToString(), disabilityStatus.isEnabled);
        }

        if (GUI.changed) {
            EditorUtility.SetDirty(target);
        }
    }
}
#endif