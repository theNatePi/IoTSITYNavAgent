using UnityEngine;
using UnityEngine.UI;

public class EvacuateButtonHandler : MonoBehaviour
{
    public Button evacuateButton;

    void Start()
    {
        if (evacuateButton != null)
        {
            evacuateButton.onClick.AddListener(OnEvacuateButtonClicked);
        }
    }

    void OnEvacuateButtonClicked()
    {
        MoveTo[] agents = FindObjectsOfType<MoveTo>();
        foreach (var agent in agents)
        {
            agent.evacuate = true;
        }
    }
}

