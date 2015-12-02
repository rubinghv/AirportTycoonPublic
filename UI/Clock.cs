using UnityEngine;
using System.Collections;

public class Clock : MonoBehaviour {

	public UILabel hours;
	public UILabel minutes;
	public UILabel timeInMinutes;

	void Update()
	{
		UpdateClock();
	} 

	void UpdateClock()
	{
		hours.text = TimeController.HoursString;
		minutes.text = TimeController.MinutesString;
		if (timeInMinutes != null)
			timeInMinutes.text = TimeController.TimeInMinutes.ToString();
	}

}
