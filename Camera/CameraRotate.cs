using UnityEngine;
using System.Collections;

public class CameraRotate : CameraObject {

    public CameraRotate(CameraParameters cameraParameters) : base(cameraParameters)
    {

	}
	
	static Vector2 rotateCameraInitialMousePosition;
	static float cameraAngleY;
	
    /**
     * 
     * 
     */ 
	public override void UpdateManual(float deltaTime) {
		// if middle mouse button is held down
		RotateFocal();
		RotateKeyboard (deltaTime);
	}

    /**
     *  Allows rotation in any direction around focal point
     */ 
    void RotateFocal()
    {
        if (Input.GetMouseButton(1))
        {

            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));	// send to middle of screen
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                cameraParams.latestHitPoint = hit.point;

                // if this is the first click
                if (rotateCameraInitialMousePosition == Vector2.zero)
                {
                    rotateCameraInitialMousePosition = Input.mousePosition;
                }
                else
                {
                    // left and right rotation (Y Axis)
                    Camera.main.transform.RotateAround(hit.point, Vector3.up, (Input.mousePosition.x - rotateCameraInitialMousePosition.x) * cameraParams.rotateCameraYSensitivity);

                    // save for next loop round
                    rotateCameraInitialMousePosition = Input.mousePosition;
                    cameraAngleY = Camera.main.transform.rotation.eulerAngles.y;
                }

            }

        }
        else
        {
            rotateCameraInitialMousePosition = Vector2.zero;
        }
    }

	void RotateKeyboard(float deltaTime)
	{
		if (Input.GetKey(KeyCode.Q) || Input.GetKey (KeyCode.E)) {

			float delta = 0;

			if (Input.GetKey(KeyCode.Q) && !Input.GetKey (KeyCode.E)) 
				delta = 1;
			else if (Input.GetKey(KeyCode.E) && !Input.GetKey (KeyCode.Q)) 
				delta = -1;

			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));	// send to middle of screen
			RaycastHit hit;
		
			if (Physics.Raycast(ray, out hit))
			{
				// left and right rotation
				Camera.main.transform.RotateAround(hit.point, Vector3.up,
				            delta * cameraParams.keyboardRotateMultiplier * deltaTime);
				
			}
			
		}


	}



     /**
     *  Allows rotation around around central point
     * 
     */ 
    void CenterRotate()
    {
        if (Input.GetMouseButton(1))
        {
            cameraParams.latestHitPoint = Vector3.zero;

            // if this is the first click
            if (rotateCameraInitialMousePosition == Vector2.zero)
            {
                rotateCameraInitialMousePosition = Input.mousePosition;
            }
            else
            {
                // left and right rotation (Y Axis)
                Camera.main.transform.RotateAround(cameraParams.latestHitPoint, Vector3.up, (Input.mousePosition.x - rotateCameraInitialMousePosition.x) * cameraParams.rotateCameraYSensitivity);

                // save for next loop round
                rotateCameraInitialMousePosition = Input.mousePosition;
            }
        }
        else
        {
            rotateCameraInitialMousePosition = Vector2.zero;
        }
    }

}
