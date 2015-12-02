using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {

	public bool flipAround = false;
		
	void Update()
	{
		transform.LookAt(Camera.main.transform);

		if (flipAround)
			transform.Rotate(new Vector3(0,180,0));
	}
}