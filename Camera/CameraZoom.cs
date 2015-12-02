using UnityEngine;
using System.Collections;

public class CameraZoom : CameraObject {

	public CameraZoom(CameraParameters cameraParameters) : base(cameraParameters)
    {

	}
	
	float currentZoom = 0;
	float previousZoom = 0;
    //static float cloudFadeDistanceStart;
    //static float cloudFadeDistanceRange;	// how far fade starts before min cloud height 
	
	public override void UpdateManual(float deltaTime) {

        currentZoom = Vector3.Distance(Camera.main.transform.position, cameraParams.latestHitPoint);
				
	 	// if smaller than min distance
        if (Vector3.Distance(Camera.main.transform.position, cameraParams.latestHitPoint) < cameraParams.zoomDistanceMin)
        {
	 		// only zoom out
	 		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                Camera.main.transform.Translate(new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * cameraParams.zoomScrollSpeed), Space.Self);
                //Camera.main.transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * cameraParams.zoomScrollSpeed);
	 		}
	 		// otherwise do nothing
	 		
	 	}
	 	
	 	// else if bigger than max distance
        else if (Vector3.Distance(Camera.main.transform.position, cameraParams.latestHitPoint) > cameraParams.zoomDistanceMax)
        {
	 		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                Camera.main.transform.Translate(new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * cameraParams.zoomScrollSpeed), Space.Self);
                //Camera.main.transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * cameraParams.zoomScrollSpeed);
	 		}
	 	}
	 	
	 	// no constraints
	 	else { 	
	 		// set camera position
            //Camera.main.transform.Translate(new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * cameraParams.zoomScrollSpeed), Space.Self);
            Camera.main.transform.position += Camera.main.transform.forward * Input.GetAxis("Mouse ScrollWheel") * cameraParams.zoomScrollSpeed;
	 	}
	 	
//	 	// also fade clouds if height is higher than min cloud height
//	 	
//	 	if (MainCamera.transform.position.y > (landscapeController.cloudHeight - landscapeController.cloudHeightVariation - cloudFadeDistanceStart) ) {
//	 		// we need to be fading, decide how much
//	 		cloudAlpha = ((MainCamera.transform.position.y - (landscapeController.cloudHeight - landscapeController.cloudHeightVariation - cloudFadeDistanceStart)) 
//	 			/ - cloudFadeDistanceRange) + 1;
//	 		cloudAlpha = Mathf.Clamp(0, cloudAlpha, 1);
//	
//			cloudMaterial.color.a = cloudAlpha;
//	 		
//	 	} 
	 	
		previousZoom = currentZoom;
			
	}
	
}
