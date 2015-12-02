using UnityEngine;
using System.Collections;

public class HidingWalls : MonoBehaviour {

	public GameObject NW_wall;
	public GameObject NE_wall;
	public GameObject SE_wall;
	public GameObject SW_wall;

	public Transform cameraTransform;
	public GridObject gridObject;

	void Start() {
		UpdateWalls();
	}
	// Update is called once per frame
	void Update () {
		UpdateWalls();
	}

	void UpdateWalls() {
		if (cameraTransform.eulerAngles.y > 0f && cameraTransform.eulerAngles.y < 90f) {
			HideExcept("SW");
		} else if (cameraTransform.eulerAngles.y > 90f && cameraTransform.eulerAngles.y < 180f) {
			HideExcept("NW");
		} else if (cameraTransform.eulerAngles.y > 180f && cameraTransform.eulerAngles.y < 270f) {
			HideExcept("NE");
		} else if (cameraTransform.eulerAngles.y > 270) {
			HideExcept("SE");
		}
	}

	void HideExcept(string wall) {
		NW_wall.SetActive(false);
		NE_wall.SetActive(false);
		SE_wall.SetActive(false);
		SW_wall.SetActive(false);

		if (wall == "SW") {
			SW_wall.SetActive(true);
		} else if (wall == "NW") {
			NW_wall.SetActive(true);
		} else if (wall == "NE") {
			NE_wall.SetActive(true);
		} else if (wall == "SE") {
			SE_wall.SetActive(true);
		}
	}


}
