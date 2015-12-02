using UnityEngine;
using System.Collections;

public class CameraParameters : MonoBehaviour {

    // drag
    public int dragTreshold = 20;

    // rotate
    public float rotateCameraYSensitivity = 0.2f;
    public float rotateCameraXSensitivity = 0.1f;
    public int maxCameraRotation = 70;
    public int minCameraRotation = 10;

    // zoom
    public float zoomDistanceMax = 100;
    public float zoomDistanceMin = 10;
    public float zoomScrollSpeed = 10;
	
    // other 
    public Vector3 latestHitPoint;

	// keyboard
	public float keyboardMoveMultiplier = 5;
	public float keyboardRotateMultiplier = 20;


}
