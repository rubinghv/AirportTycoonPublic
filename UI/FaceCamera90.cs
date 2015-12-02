using UnityEngine;
using System.Collections;

public class FaceCamera90 : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {

		if (Camera.main.transform.rotation.eulerAngles.y < 45 || Camera.main.transform.rotation.eulerAngles.y > 315) 
			iTween.RotateTo (this.gameObject, iTween.Hash ("y", 0, "time", 0.5));
		else if (Camera.main.transform.rotation.eulerAngles.y > 45 && Camera.main.transform.rotation.eulerAngles.y < 135) 
			iTween.RotateTo (this.gameObject, iTween.Hash ("y", 90, "time", 0.5));	
		else if (Camera.main.transform.rotation.eulerAngles.y > 135 && Camera.main.transform.rotation.eulerAngles.y < 225) 
			iTween.RotateTo (this.gameObject, iTween.Hash ("y", 180, "time", 0.5));
		else if (Camera.main.transform.rotation.eulerAngles.y > 225 && Camera.main.transform.rotation.eulerAngles.y < 315) 
			iTween.RotateTo (this.gameObject, iTween.Hash ("y", 270, "time", 0.5));

	}
}
