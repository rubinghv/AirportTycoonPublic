using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public bool zoomOut = false;
	public bool zoomIn = false;
	public int layerMaskNumber;
	public float switchSeconds;
	public float perspectiveDelay;

	public PerspectiveSwitcher perspectiveSwitch;
	float cameraRotation = 0f;
	Vector3 hitPoint;

	// Update is called once per frame
	void Update () {
		if (zoomOut) {
			zoomOut = false;
			StartCoroutine(ZoomOut());
		} else if (zoomIn) {
			zoomIn = false;
			ZoomIn();
		}
	}

	IEnumerator ZoomOut()
	{
		// shoot ray from middle of screen
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit hit;		
		LayerMask layerMask = 1 << layerMaskNumber;

		//if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) 
		if (Physics.Raycast (ray, out hit, Mathf.Infinity) && hit.transform.gameObject.layer == layerMaskNumber) {
			// get x and z position from hit
			hitPoint = hit.point;
		} else {
			print("Something went wrong with zooming out! FIX THIS!");
			yield return 0;
		}


		// move camera to this x and z position
		iTween.MoveTo(this.gameObject, iTween.Hash(
    			 	"x", hitPoint.x, "z", hitPoint.z,
    			 	"easetype", iTween.EaseType.easeInOutQuad,
    			 	"time", switchSeconds));

		// get rotation
		cameraRotation = 90f - Camera.main.transform.eulerAngles.x;

		// rotate downwards
		iTween.RotateAdd(this.gameObject, iTween.Hash(
    			 		 "amount", new Vector3(cameraRotation,0f,0f),
    			 		 "easetype", iTween.EaseType.easeInOutQuad,
    			 		 "time", switchSeconds));

		yield return new WaitForSeconds(perspectiveDelay);
		perspectiveSwitch.SwitchPerspective(switchSeconds - perspectiveDelay);

	}

	//float cameraAngle = 0;
	void ZoomIn()
	{

		float camera_pos_height = 40;
		this.transform.position = new Vector3(this.transform.position.x, 
											  camera_pos_height,
											  this.transform.position.z);
		
		
		iTween.ValueTo(this.gameObject, iTween.Hash(
		     "from", 0f,
		     "to", -1.6f,
		     "time", switchSeconds,
		     "onupdatetarget", this.gameObject,
		     "onupdate", "tweenOnUpdateCallBack",
		     "easetype", iTween.EaseType.easeInOutQuad
		     )
		 );
		
		perspectiveSwitch.SwitchPerspective(switchSeconds);

		//this.transform.RotateAround(hitPoint, this.transform.right , -cameraRotation);	

		/*
		iTween.MoveTo(this.gameObject, iTween.Hash(
    			 	"y", camera_pos_height,
    			 	"easetype", iTween.EaseType.easeInOutQuad,
    			 	"time", switchSeconds));
		*/

		

		// rotate downwards
		/*
		iTween.RotateAdd(this.gameObject, iTween.Hash(
    			 		 "amount", new Vector3(-cameraRotation,0f,0f),
    			 		 "easetype", iTween.EaseType.easeInOutQuad,
    			 		 "time", switchSeconds));

		perspectiveSwitch.SwitchPerspective(switchSeconds);
		*/
	}

	void tweenOnUpdateCallBack( float newValue )
	{	
		this.transform.RotateAround(hitPoint, this.transform.right, newValue);
	}

}
