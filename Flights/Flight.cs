using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 
 *	If passenger is not on board on time, can decide to leave, but passengers left behind will be subtracted
 *	from previous passengersOnFlight.
 *	So if first 16 passengers, then 2 left behind when leaving with them boarding, next flight will have 14 passengers
 */
public class Flight : MonoBehaviour {

	string origin;
	public int state; // should be private

	public static int STATE_START = 1;
	public static int STATE_SPAWNED = 2;

	// updating for airplane traveling done in Airplane class
	public static int STATE_ON_RUNWAY = 3;
	public static int STATE_TRAVELING_RUNWAY_TO_PARKING = 4;

	// from arriving to disembarking passengers
	public static int STATE_GATE_ARRIVED = 5;
	public static int STATE_GATE_DISEMBARKING = 6;
	public static int STATE_GATE_DISEMBARKING_COMPLETE = 7;

	// cleaning and boarding
	public static int STATE_GATE_CLEANING_AIRPLANE = 19;
	public static int STATE_GATE_BEGIN_BOARDING = 20;
	public static int STATE_GATE_BOARDING = 21;
	public static int STATE_GATE_BOARDING_FINISHED_PENDING = 22;
	public static int STATE_GATE_BOARDING_FINISHED = 23;

	// control back to airplane
	public static int STATE_TRAVELING_PARKING_TO_RUNWAY = 24;
	public static int STATE_RUNWAY_DEPARTING = 25;

	public static int STATE_FLIGHT_EXIT_FINAL = 30;
	public static int STATE_REMOVE = 40;

	// keep track of incoming and ougoing passengers, as well as passengers currently on board
	List<Passenger> passengers;
	List<Passenger> incomingPassengers;
	List<Passenger> outgoingPassengers;

	// all assigned by different classes
	Gate gate;
	ParkingSpot parkingSpot;
	Runway runway;
	Airplane airplane;
	public FlightController flightController;

	// for disembarking
	int secondsStart;
 	int secondsDelayBeforeExit;
 	int secondsSeparation;
 	int secondsVariation;
 	int secondsSeparationComplete;

 	// for after disembarking
 	// should have something for refueling, food, luggage
 	int minutesToBoarding; // minutes between disembarking and embarking // (in future, disembarking should be refueling/food)
 	int minutesBoarding;

	public Flight(Gate _gate, ParkingSpot parking_spot, Runway _runway, 
		int delayBeforeExit, int seperation, int variation,
		int _minutesToBoarding, int _minutesBoarding, FlightController _flightController) 
	{
		gate = _gate;
		parkingSpot = parking_spot;
		runway = _runway;

		secondsDelayBeforeExit = delayBeforeExit;
 		secondsSeparation = seperation;
 		secondsVariation = variation;

 		minutesToBoarding = _minutesToBoarding;
 		minutesBoarding = _minutesBoarding;

 		flightController = _flightController;

		origin = "Sydney"; // need to pull from some sort of database
		state = STATE_START;


	}

	public void ManualUpdate()
	{
		// state updating disembarking takes place in airplane (because it's responsible)
		if (state == STATE_GATE_ARRIVED) {
			StartDisembarking();
		}
		if (state == STATE_GATE_DISEMBARKING) {
			Disembarking();
		} else if (state == STATE_GATE_DISEMBARKING_COMPLETE) {
			state = STATE_GATE_CLEANING_AIRPLANE;
		} else if (state == STATE_GATE_CLEANING_AIRPLANE) { // this is obviously not working yet
			WaitForMinutes (minutesToBoarding, STATE_GATE_BEGIN_BOARDING); // state naem nott working??
		} else if (state == STATE_GATE_BEGIN_BOARDING) {
			StartBoarding();
			state = STATE_GATE_BOARDING;
		} else if (state == STATE_GATE_BOARDING) {
			WaitForMinutes (minutesBoarding, STATE_GATE_BOARDING_FINISHED_PENDING);
		} else if (state == STATE_GATE_BOARDING_FINISHED_PENDING) {
			if (BoardingFinishedUpdate())
				state = STATE_GATE_BOARDING_FINISHED;
		} else if (state == STATE_GATE_BOARDING_FINISHED) {
			// control back to airplane
		}

	}

	public Gate GetGate() { return gate; }
	public ParkingSpot GetParkingSpot() { return parkingSpot; }
	public Runway GetRunway() { return runway; }
	public Airplane GetAirplane() { return airplane; }

	public int GetFlightCapacity() {return airplane.passengerCapacity; }

	/*
	 *	Spawn airplane at location
	 *
	 *	Called by flight controller
	 */
	public void SpawnAirplane(Vector2 location, Airplane _airplane, Transform parentObject)
	{
		// instantiate and set to position
		GameObject new_go = (GameObject) Instantiate (_airplane.gameObject);
		new_go.transform.position = new Vector3(location.x, new_go.transform.position.y, location.y);
		new_go.transform.parent = parentObject;

		// get component and link
		airplane = new_go.gameObject.GetComponent<Airplane>();
		airplane.SetFlight(this);

		new_go.SetActive(true);
		state = STATE_SPAWNED;

		// obviously shouldn't do this because it will need to fly first, but for now it's ok
		state = STATE_ON_RUNWAY;
	}

	/*
	 *	When the airplane has arrived at parking spot, start disembarking
	 * 	which means opening the door and starting the Disembarking() procedure
	 */
	public void AddPassengers()
	{
		passengers = PassengerController.GetOutgoingPassengers(this);
		incomingPassengers = PassengerController.GetIncomingPassengers(this);

		outgoingPassengers = passengers; // because they are currently in the airplane, until they exit
	}

	int waintingForTimeInMinutes = 0;
	void WaitForMinutes(int minutes, int exitState)
	{
		// if first call, do setup
		if (waintingForTimeInMinutes == 0) {
			waintingForTimeInMinutes = TimeController.TimeInMinutes + minutes;
		}

		// if current time has surpassed time we were waiting for
		if (TimeController.TimeInMinutes >= waintingForTimeInMinutes) {
			state = exitState;
			waintingForTimeInMinutes = 0;
		}

	}


	/*
	 *	Passengers coming to flight will be spawned by passengerController
	 */
	public void SpawnIncomingPassengers()
	{
		PassengerController.SpawnPassengersAtEntrance(incomingPassengers);
	}

	/*
	 *	When the airplane has arrived at parking spot, start disembarking
	 * 	which means opening the door and starting the Disembarking() procedure
	 */
 	public void StartDisembarking()
 	{
 		airplane.OpenDoor();

 		state = STATE_GATE_DISEMBARKING;
 		secondsStart = TimeController.TimeInSeconds + secondsDelayBeforeExit;
 		secondsSeparationComplete = secondsSeparation;
 	}

 	/*
	 *	Continue disembarking, which means waiting for a certain number of seconds
	 * 	before getting FIFO passenger to start moving out, and removing from list.
	 *	Also setup again for next call in secondsSeparationComplete 
	 */
 	public void Disembarking()
 	{	
 		//print("secondsStart = " + secondsStart + " - separation " + secondsSeparation + 
 		//	" - passengerlist count = " + passengers.Count);

 		if (passengers.Count < 1) {
 			state = STATE_GATE_DISEMBARKING_COMPLETE;
 			return;
 		}

 		if (secondsStart + secondsSeparationComplete > TimeController.TimeInSeconds) 
 			return;
 		

 		Vector3 exitPosition = airplane.GetExitPosition();
		exitPosition.y = passengers[0].transform.position.y;

		// set position, active, and start exit path
 		passengers[0].transform.position = exitPosition;
 		passengers[0].gameObject.SetActive(true);
 		passengers[0].ExitAirplane(airplane.GetExitPath(), gate);

 		// don't forget to remove  passenger from list and reset start time
 		passengers.RemoveAt(0);
 		secondsStart = TimeController.TimeInSeconds;
 		secondsSeparationComplete = Random.Range(secondsSeparation - secondsVariation, secondsSeparation);
 	}

 	/*
 	 *	Start the boarding process
 	 */
 	public void StartBoarding()
 	{
		print("start boarding, list size = " + incomingPassengers.Count);

 		foreach(Passenger passenger in incomingPassengers) {
 			passenger.StartBoarding();
 		}

 	}


 	/* 
 	 *	Called by a passenger when finished boarding
 	 */	
 	public void FinishedBoarding(Passenger passenger)
 	{
 		passengers.Add(passenger);
		PassengerController.RemovePassengers(passenger);
 	}

 	/*
 	 *	Test if all passengers are all on board, if so, return true
 	 */
 	public bool BoardingFinishedUpdate()
 	{
 		if (incomingPassengers.Count == passengers.Count) {
 			airplane.CloseDoor();
 			return true;
 		}
 		else
 			return false;
 	}

 	public void CompleteFlight()
	{	
		state = STATE_REMOVE;
		flightController.FlightComplete(this);
		// destroy airplane and flight
		Object.Destroy(airplane.gameObject);
		Object.Destroy(this);
	}

}
