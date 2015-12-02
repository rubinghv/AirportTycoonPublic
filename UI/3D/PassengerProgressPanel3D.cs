using UnityEngine;
using System.Collections;

public class PassengerProgressPanel3D : MonoBehaviour {

	public UISprite progressSprite;

	public PassengerProgressPanel3D CreatePassengerProgress(Passenger passenger)
	{
		// instantiate, position and parent
		GameObject new_go = (GameObject) Instantiate (this.gameObject);
		new_go.transform.position = new Vector3(passenger.transform.position.x, this.transform.position.y, 
												passenger.transform.position.z);
		new_go.transform.parent = this.transform.parent;

		PassengerProgressPanel3D new_component = new_go.GetComponent<PassengerProgressPanel3D>();
		NGUITools.SetActive(new_component.progressSprite.gameObject, true);

		// update reference so passenger refers to new
		//passenger.progressPanel = new_component;
		return new_component;
	}

	/*
	 *	Update progress sprite, progress is from 0 to 1
	 */
	public void UpdatePassengerProgress(float progress)
	{
		progressSprite.fillAmount = progress;
	}


}
