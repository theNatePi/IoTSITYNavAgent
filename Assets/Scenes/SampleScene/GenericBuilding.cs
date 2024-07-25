using System;
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
        // Set the min and max age dynamically
        System.Random random = new System.Random();
        minimumAge = random.Next(1, 80); // [min, max)
        int ageDifference = random.Next(10, 150);
        maximumAge = Math.Min(minimumAge + ageDifference, 150);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
