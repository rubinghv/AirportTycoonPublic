using UnityEngine;
using System.Collections;

public class TimePanel : MonoBehaviour {

	// Use this for initialization
	public void PressButtonPause () {
		TimeController.GameSpeed = 0;
	}
	
	public void PressButtonSpeedOne () {
		TimeController.GameSpeed = 1;
	}

	public void PressButtonSpeedTwo () {
		TimeController.GameSpeed = 2;
	}

	public void PressButtonSpeedThree () {
		TimeController.GameSpeed = 5;
	}

	

}
