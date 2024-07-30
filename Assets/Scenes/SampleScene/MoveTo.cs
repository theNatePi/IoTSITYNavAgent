// MoveTo.class
using System;
using System.Collections.Generic;
using System.Linq;
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

    public virtual GameObject GetNextPassive() {return _agent.gameObject;}

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
        GameObject[] genericPoints;
        genericPoints = GameObject.FindGameObjectsWithTag("GenericBuilding");

        System.Random random = new();

        // Get a random index within the array's length
        int randomIndex = random.Next(genericPoints.Length);

        // Access the random element
        GameObject randomElement = genericPoints[randomIndex];

        return randomElement; 
    }

    // public override GameObject GetEvacPoint(List<GameObject> evacPoints, NavMeshAgent agent) {

    // }
}

public class MoveTo : MonoBehaviour {
    NavMeshAgent agent;
    float updateTimer = 0f;
    float updateInterval = 2f;
    private bool reachedGoal = false;
    private GameObject currentGoal;
    private GameObject TimeSystem;
    private float _timeScale;
    private DateTime _currentTime;
    private DateTime _departTime;
    public float evacDespawnCount = 0f;

    [TextArea(1,1000)]
    public string README = "1) Add a NavMeshAgent to this game object\n2) Create objects for evac points and other POIs, ensure they are touching the NavMesh\n3) Add tags to these points and update the code with functionality for each tag (see comments). For evac points, include the tag \"EvacPoint\"";

    // Set available classes for your agent here
    public enum AgentClass { AgentBase, GenericClass }

    // Public attributes
    [Range(1, 150)]
    public int age;
    [Range(1, 40)]
    public int maxIncline = 20;
    [Tooltip("Time before agent is deleted once it reaches the EvacPoint"), Range(2f, 500f)]
    public float evacDespawnDelay = 10f;
    [Tooltip("When false, agent will follow a schedule based off of their class. When true, agent will move to nearest EvacPoint")]
    public bool evacuate = false;
    [Tooltip("You cannot change this during playback. You should not assign this to AgentBase")]
    public AgentClass currentClass;
    public AgentBase ClassObject;

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

    Vector3 GetGoal () {
        if (evacuate) {
            currentGoal = ClassObject.GetEvacPoint(evacPoints);
            return currentGoal.transform.position;
        } else {
            currentGoal = ClassObject.GetNextPassive();
            return currentGoal.transform.position;
        }
    }


    void Start () {
        agent = GetComponent<NavMeshAgent>();
        // Set the priority of the agnent, if all agents are the same, they will get stuck on
        // each other
        System.Random random = new();
        int priority = random.Next(1, 10);
        agent.avoidancePriority = priority;

        evacPoints = GameObject.FindGameObjectsWithTag(evacTag).ToList();

        TimeSystem = GameObject.Find("TimeSystem");
        _timeScale = TimeSystem.GetComponent<TimeSystem>().timeScale;
        _currentTime = TimeSystem.GetComponent<TimeSystem>().simulatedTime;

        // TODO: Change this to be a distribution
        age = random.Next(1, 150);

        // Grab the class selected in the dropdown, and assign an object of this class to the agent
        string className = currentClass.ToString();
        Type type = Type.GetType(className);
        ClassObject = gameObject.AddComponent(type) as AgentBase;
        // ClassObject = gameObject.AddComponent<_class>();
        ClassObject.AssignAgent(agent);

        agent.destination = GetGoal();
    }

    void _UpdateEvac() {
        if (!reachedGoal) {
            agent.destination = GetGoal();
            reachedGoal = true;
        }

        if (agent.remainingDistance <= agent.stoppingDistance + 1) {
            AgentAtGoal goalScript = currentGoal.GetComponent<AgentAtGoal>();
            if (goalScript.population >= goalScript.capacity) {
                // This evac point is full
                evacPoints.Remove(currentGoal);
                if (evacPoints.Count > 0) {
                    agent.destination = GetGoal();
                } else {
                    evacuate = false;
                }
            }
            evacDespawnCount++;
        } else {
            evacDespawnCount = 0;
        }
    }

    void _UpdatePassive() {
        if (reachedGoal) {
            agent.destination = GetGoal();
            System.Random random = new();
            int randomDiff = random.Next(15, 120);
            _departTime = _currentTime.AddMinutes(randomDiff);
            reachedGoal = false;
            // somehow make them not collide
            foreach (var collider in agent.GetComponents<Collider>()) {
                collider.enabled = false;
            }
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }

        if (_currentTime >= _departTime) {
            reachedGoal = true;
        }

        if (agent.remainingDistance <= agent.stoppingDistance + 1) {
            // make the agents collide again
            foreach (var collider in agent.GetComponents<Collider>()) {
                collider.enabled = true;
            }
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        } else {
            evacDespawnCount = 0;
        }
    }

    void Update() {
        // Pressing E will command all agents to evacuate
        if (Input.GetKey(KeyCode.E)) evacuate = !evacuate;

        // Increase the update timer and current time
        updateTimer += Time.deltaTime;
        _timeScale = TimeSystem.GetComponent<TimeSystem>().timeScale;
        _currentTime = TimeSystem.GetComponent<TimeSystem>().simulatedTime;

        // If it is time to update the agent
        if (updateTimer >= updateInterval) {
            if (evacuate) {
                _UpdateEvac();
            } else {
                _UpdatePassive();
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