using UnityEngine;
using System.Collections;

public class CameraDrag : CameraObject {
	
	Vector3 previousCameraPosition;
	Vector2 firstMousePosition;

    public  CameraDrag(CameraParameters cameraParameters) : base(cameraParameters)
    {

	}
	
	public override void UpdateManual(float deltaTime) {

		MouseDrag (2);
		KeyboardDrag (Time.deltaTime);
	}


	void MouseDrag(int mouseButton)
	{
		// if the left mouse button is held down
		if (Input.GetMouseButton(mouseButton)) {
			// Construct a ray from the current mouse coordinates
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;		
			
			if (Physics.Raycast (ray, out hit)) {
				
				cameraParams.latestHitPoint = hit.point;
				
				// if this is the first click
				if (previousCameraPosition == Vector3.zero) {
					previousCameraPosition = hit.point;
					firstMousePosition = Input.mousePosition;
				} else {
					// check if we've moved a minimum distance from beginning
					if (Vector3.Distance(firstMousePosition, Input.mousePosition) > cameraParams.dragTreshold) {
						//						Camera.main.transform.position.x += (previousCameraPosition.x - hit.point.x);
						//						Camera.main.transform.position.z += (previousCameraPosition.z - hit.point.z);
						
						Camera.main.transform.position = Camera.main.transform.position +  new Vector3 (  
															(previousCameraPosition.x - hit.point.x), 0,
															(previousCameraPosition.z - hit.point.z) );
						
					} else {
						previousCameraPosition = hit.point;
					}
				}
			}
			
			//			UpdateCameraFocus();
			
		} else {	// if not stop moving
			previousCameraPosition = Vector3.zero;
		}

	}

	/*
	 * 	Move camera by pressing the keys
	 * 
	 */
	void KeyboardDrag(float deltaTime)
	{
		float delta = 0;

		// forward and backward
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S)) {
			if (Input.GetKey (KeyCode.W) && !Input.GetKey(KeyCode.S))
				delta = 1;
			else if (Input.GetKey (KeyCode.S) && !Input.GetKey(KeyCode.W))
				delta = -1;

			// not sure how to get forward action while disregarding y,
			// instead just reset y after every forward translate
			float previousY = Camera.main.transform.position.y;
			Camera.main.transform.Translate(Vector3.forward * (deltaTime * cameraParams.keyboardMoveMultiplier * delta));
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, previousY, Camera.main.transform.position.z);

			//Camera.main.transform.position = Camera.main.transform.localPosition +  new Vector3 (  
			//			0, 0, (previousCameraPosition.z + (deltaTime * cameraParams.keyboardMoveMultiplier * delta)));
		}

		// left and right
		if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.A)) {
			if (Input.GetKey (KeyCode.D) && !Input.GetKey(KeyCode.A))
				delta = 1;
			else if (Input.GetKey (KeyCode.A) && !Input.GetKey(KeyCode.D))
				delta = -1;
			
			float previousY = Camera.main.transform.position.y;
			Camera.main.transform.Translate(Vector3.right * (deltaTime * cameraParams.keyboardMoveMultiplier * delta));
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, previousY, Camera.main.transform.position.z);		}

	}



}


