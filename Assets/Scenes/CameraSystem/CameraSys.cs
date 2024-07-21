using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;

//using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;

/*
CAMERA OPERATION:
- Offset for the camera is (0, 30, -40)
- WASD to move the camera around the XZ plane
- SPACE and SHFIT to move up and down, respectively
- QE for rotating the camera left and right (respectively) around the Y-axis
- ZX for rotating the cam up and down around the X-axis
- R to reset the camera position
- EdgeScrolling option available for travseral of XZ plane
*/

public class CameraSys : MonoBehaviour
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame

    [SerializeField] private bool useES = false;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float fovMax = 100;
    [SerializeField] private float fovMin = 10;


    private float goalFOV = 50;
    

    void Update()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;
        if (Input.GetKey(KeyCode.Space)) inputDir.y = +1f;
        if (Input.GetKey(KeyCode.LeftShift)) inputDir.y = -1f;

        if (useES) {
            int edgeScrollSize = 20;

            if (Input.mousePosition.x < edgeScrollSize) inputDir.x = -1f;
            if (Input.mousePosition.y < edgeScrollSize) inputDir.z = -1f;
            if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x = +1f;
            if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDir.z = +1f;
        }
        


        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x + transform.up * inputDir.y;
        float moveSpeed = 50f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float rotateDir = 0f;
        float rotateSpeed = 50f;

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)) {
            if (Input.GetKey(KeyCode.Q)) rotateDir = +1f;
            if (Input.GetKey(KeyCode.E)) rotateDir = -1f;
            transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)) {
            if (Input.GetKey(KeyCode.Z)) rotateDir = +1f;
            if (Input.GetKey(KeyCode.X)) rotateDir = -1f;
            transform.eulerAngles += new Vector3(rotateDir * rotateSpeed * Time.deltaTime, 0, 0);
        }

        if (Input.GetKey(KeyCode.R)) {
            transform.position = new Vector3(0, 0, 0); 
            transform.eulerAngles = new Vector3(0, 0, 0); 
        }

        CameraZoom_FOV();
        
    }

    private void CameraZoom_FOV() {
        if (Input.mouseScrollDelta.y > 0) {
            goalFOV += 5;
        }
        if (Input.mouseScrollDelta.y < 0) {
            goalFOV -= 5;
        }

        goalFOV = Mathf.Clamp(goalFOV, fovMin, fovMax);
        float zoomSpeed = 10f;
        cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, goalFOV, Time.deltaTime * zoomSpeed);
        
    }
}
