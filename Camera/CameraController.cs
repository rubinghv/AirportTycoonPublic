using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Controls what interface is activated at what time
 * 
 * 
 */ 

public class CameraController : MonoBehaviour {

    public CameraParameters cameraParameters;

    public bool cameraDragOn;   // has to be done at start
    public bool cameraZoomOn;
    public bool cameraRotateOn;

    List<CameraObject> cameraObjectList = new List<CameraObject>();

	void Start()
	{
		Setup ();
	}

	public void Setup () {
				if (cameraDragOn)
						cameraObjectList.Add (new CameraDrag (cameraParameters));
				if (cameraZoomOn)
						cameraObjectList.Add (new CameraZoom (cameraParameters));
				if (cameraRotateOn)
						cameraObjectList.Add (new CameraRotate (cameraParameters));
		}
	
	// Update is called once per frame
	void Update () {
		UpdateCamera(Time.deltaTime);
	}
	
	void UpdateCamera (float deltaTime) {
        foreach (CameraObject co in cameraObjectList)
			co.UpdateManual(deltaTime);
	}
	
}




/**
*	Move the camera around by dragging with the right mouse button
*
*/

//var cameraFocusCenter : GameObject;
//var cameraFocusBottom : GameObject;
//function UpdateCameraFocus () {
//	// redo hit
//	ray = Camera.main.ScreenPointToRay (Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0));
//	layerMask = 1 << 12; // only look through draggable layer
//	
//	if (Physics.Raycast (ray, hit, 10000, layerMask)) {
//		cameraFocusCenter.transform.position = Vector3 (hit.point.x, 0, hit.point.z);
//	}
//	
//	// also for front of screen
//	ray = Camera.main.ScreenPointToRay (Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 10, 0));
//	
//	if (Physics.Raycast (ray, hit, 10000, layerMask)) {
//		cameraFocusBottom.transform.position = Vector3 (hit.point.x, 0, hit.point.z);
//	}
//
//}
//
//
//private var mousePos : Vector2;
//var cameraMoveEdgeMargin : int;
//var cameraMoveEdgeSensitivity : int;
//private var cameraMutation : Vector3;
//
//function CameraMoveEdge (deltaTime : float) {
//	mousePos = Input.mousePosition;
//	
//	// check if we're not outside of the screen
//	if (mousePos.x >= -5 && mousePos.x <= Screen.width + 5 && mousePos.y >= -5 && mousePos.y <= Screen.height + 5) {
//		
//		if (mousePos.x < cameraMoveEdgeMargin || mousePos.x > (Screen.width - cameraMoveEdgeMargin) ) {
//			
//			cameraMutation = Vector3(deltaTime * cameraMoveEdgeSensitivity * (Camera.main.transform.position.y / 230),0,0);
//			
//			if (mousePos.x < cameraMoveEdgeMargin) {
//				Camera.main.transform.Translate (-cameraMutation, Space.Self);
//			} else if (mousePos.x > (Screen.width - cameraMoveEdgeMargin) ) {
//				Camera.main.transform.Translate (cameraMutation, Space.Self);
//			}
//		}
//	
//		if (mousePos.y < cameraMoveEdgeMargin || mousePos.y > (Screen.height - cameraMoveEdgeMargin) ) {
//			
//			cameraMutation = Camera.main.transform.forward;
//	
//			Camera.main.transform.position += Camera.main.transform.TransformDirection(cameraMutation) * deltaTime * cameraMoveEdgeSensitivity * (Camera.main.transform.position.y / 230);
//
//		}
//		
//	}
//
//	
////	print(Input.mousePosition);
//}
//
//