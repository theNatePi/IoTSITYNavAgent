using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericResidential : MonoBehaviour
{
    [Range(0, 10000)]
    public int genericResidents = 10;
    
    [Range(0, 10000)]
    public int seniorResidents = 10;

    private GameObject genericAgent;

    // Start is called before the first frame update
    void Start()
    {
        genericAgent = GameObject.Find("GenericAgent");

        for (int i = 0; i < genericResidents; i++)
        {
            GameObject newAgent = Instantiate(genericAgent, transform.position, Quaternion.identity);
            newAgent.GetComponent<MoveTo>().currentClass = MoveTo.AgentClass.Generic;
            // Add any additional setup for the new agent here, e.g., parenting, naming
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
