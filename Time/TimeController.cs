using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TimeController : MonoBehaviour {

	static float gameSpeed = 1.0f; // 1 is default
 	float gameTimeMultiplier = 50f;

	public static float GameSpeed {  
		get {return gameSpeed; }
		set {gameSpeed = value; }
	}

	// game speed that is either 0 or 1, for UI elements that need to be paused but not speed up otherwise
	public static float GameSpeedUI {  
		get {if (gameSpeed >= 1)
				return 1f;
			 else 
			 	return 0f; }
	}

	// all time units
	static float seconds; 
	public static float Seconds { get {return seconds; } }

	static int minutes;
	public static float Minutes { get {return minutes; } }
	public static string MinutesString { get {
			if (minutes < 10)
				return "0" + minutes;
			else 
				return "" + minutes;
		}
	}

	static int hours;
	public static float Hours { get {return hours; } }
	public static string HoursString { get {
			if (hours < 10)
				return "0" + hours;
			else 
				return "" + hours;
		}
	}
	static int days = 1;
	public static float Days { get {return days; } }
	
	public static int TimeInSeconds {
		get { return (int) seconds + (TimeInMinutes * 60);}
	}

	public static int TimeInMinutes {
		get { return minutes + (hours * 60) + (days * 60 * 25);}
	}

	
	void Update()
	{
		UpdateTime(Time.deltaTime);
	}	

	void UpdateTime(float updateDeltaTime)
	{
		// add delta to seconds and update other time uinits accordingly
		seconds += updateDeltaTime * gameSpeed * gameTimeMultiplier;

		if (seconds >= 60f) {
			seconds -= 60f;
			minutes += 1;
		}
		if (minutes >= 60) {
			minutes -= 60;
			hours += 1;
		}
		if (hours >= 24) {
			hours -= 24;
			days += 1;
		}
	}

	static Dictionary<int, int> waitForSecondsLeft = new Dictionary<int, int>();

	public static bool WaitForSeconds(int minSeconds, int maxSeconds, int entity_ID)
	{	return WaitForSeconds(UnityEngine.Random.Range(minSeconds, maxSeconds), entity_ID); }

	public static bool WaitForSeconds(int entity_ID)
	{ 	return WaitForSeconds(0, entity_ID); }

	public static bool WaitForSeconds(int seconds, int entity_ID)
	{
		// if first call, do setup
		if (!waitForSecondsLeft.ContainsKey(entity_ID)) {
			waitForSecondsLeft.Add(entity_ID, TimeController.TimeInSeconds + seconds);
			return false;
		}

		// if current time has surpassed time we were waiting for
		if (TimeController.TimeInSeconds >= waitForSecondsLeft[entity_ID]) {
			waitForSecondsLeft.Remove(entity_ID);
			return true;
		}

		return false;
	}




	// ------------------------------ for game save purposes ------------------------------

	static DateTime centuryBegin = new DateTime(2001, 1, 1);
	static DateTime currentDate = DateTime.Now;

	public static string GetCurrentDateTimeString () {				
		return "" + DateTime.Now.Year + "-" + TwoDigitNumber(DateTime.Now.Month) + "-" + TwoDigitNumber(DateTime.Now.Day) +
			   " " + TwoDigitNumber(DateTime.Now.Hour) + ":" + TwoDigitNumber(DateTime.Now.Minute);
	}
	/**
	 * 	Make sure the string is two digits (so adds a 0)
	 */  
	public static string TwoDigitNumber (int number) {
		if (number == 0) 
			return "00";
		else if (number < 10)
			return "0" + number;
		else 
			return number.ToString();
	}
	
	// ------------------------------------- Saving -------------------------------------
	public static void Save (string filename, string tag) 
	{
		ES2.Save(seconds, filename + tag + "TimeController" + "seconds");
		ES2.Save(minutes, filename + tag + "TimeController" + "minutes");
		ES2.Save(  hours, filename + tag + "TimeController" + "hours");
		ES2.Save(   days, filename + tag + "TimeController" + "days");
	}

	public static void Load (string filename, string tag) 
	{
		seconds = ES2.Load<float>(filename + tag + "TimeController" + "seconds");
		minutes = ES2.Load<int>(filename + tag + "TimeController" + "minutes");
		hours 	= ES2.Load<int>(filename + tag + "TimeController" + "hours");
		days 	= ES2.Load<int>(filename + tag + "TimeController" + "days");
	}


	
}
