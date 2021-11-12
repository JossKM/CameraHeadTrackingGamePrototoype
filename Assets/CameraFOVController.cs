using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Corners
{
    TopLeft = 0,
    TopRight,
    BottomRight,
    BottomLeft
}

[RequireComponent(typeof(Camera))]
public class CameraFOVController : MonoBehaviour
{
    private Camera cam;

    [SerializeField] 
    private GameObject targetPlane;

    [SerializeField] 
    private GameObject[] cornerMarkerGameObjects;

    [SerializeField] 
    private Vector3[] corners;

    [SerializeField] 
    private Vector2 tiltScalars = new Vector2(1.0f, 1.0f);

    private Vector3 planeDimensions = new Vector3(10, 0, 10);

    private WebcamCameraControl camControl;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        camControl = GetComponent<WebcamCameraControl>();
    }

    // Update is called once per frame
    void Update()
    {
        corners[(int)Corners.TopLeft] =     targetPlane.transform.TransformPoint(new Vector3(-planeDimensions.x*0.5f,  planeDimensions.y*0.5f,  planeDimensions.z*0.5f));
        corners[(int)Corners.TopRight] =    targetPlane.transform.TransformPoint(new Vector3( planeDimensions.x*0.5f,  planeDimensions.y*0.5f,  planeDimensions.z*0.5f));
        corners[(int)Corners.BottomRight] = targetPlane.transform.TransformPoint(new Vector3(-planeDimensions.x*0.5f,  planeDimensions.y*0.5f, -planeDimensions.z*0.5f));
        corners[(int)Corners.BottomLeft] =  targetPlane.transform.TransformPoint(new Vector3( planeDimensions.x*0.5f,  planeDimensions.y*0.5f, -planeDimensions.z*0.5f));

        for (int i = 0; i < corners.Length; i++)
        {
            cornerMarkerGameObjects[i].transform.position = corners[i];
        }

        Vector3 displacementToScreen = targetPlane.transform.position - transform.position;
        float dist = displacementToScreen.magnitude;


       // cam.lensShift = new Vector2(camControl.smoothedPosition.x * -0.3f, camControl.smoothedPosition.y * -0.3f);
        

        //cam.nearClipPlane = dist - 0.5f;
        cam.lensShift = new Vector2(transform.localPosition.x * -tiltScalars.x, transform.localPosition.y * -tiltScalars.y);



        // transform.rotation = Quaternion.LookRotation(displacementToScreen.normalized, Vector3.up);
        //cam.fieldOfView = Mathf.Lerp(20, 5, (dist - 0.80f) * (1.0f / 0.8f));
    }
}
