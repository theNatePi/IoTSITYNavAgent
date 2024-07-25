using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GenericBuilding : MonoBehaviour
{
    [Range(1, 149)]
    public int minimumAge = 10;
    [Range(2, 150)]
    public int maximumAge = 80;

    [Range(0, 24)]
    public int timeOpen = 8;
    [Range(0, 24)]
    public int timeClose = 18;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
