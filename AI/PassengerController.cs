using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PassengerController : MonoBehaviour {

	public static Passenger passenger;
	public string PassengerGoName;
	public static Airport airport;
	public string AirportGoName;
	public static Transform passengerParent;
	public string passengerParentGoName;

	static Dictionary<int,Passenger> passengersToSpawn = new Dictionary<int,Passenger>();
	static List<Passenger> allPassengers = new List<Passenger>();

	[Header("Flight Passenger Entrance ")]
	public static int spawnTotalMinutes = 60; // spawn time is total minutes + random.range /(0, variation seconds)
	public static float spawnVariationMinutes = 3;

	void Start() {
		passenger = GameObject.Find(PassengerGoName).GetComponent<Passenger>();
		airport = GameObject.Find(AirportGoName).GetComponent<Airport>();
		passengerParent = GameObject.Find(passengerParentGoName).transform;

		passenger.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		SpawnPassengers();
	}

	void SpawnPassengers()
	{
		int curTimeInMinutes = TimeController.TimeInMinutes;
		// test if there is a passenger that is suppsed to spawn now
		if (passengersToSpawn.ContainsKey(curTimeInMinutes)) {
			//print("spawn passenger!!");
			passengersToSpawn[curTimeInMinutes].SpawnAtEntrance();

			// add to lsit
			allPassengers.Add(passengersToSpawn[curTimeInMinutes]);

			passengersToSpawn.Remove(curTimeInMinutes);
		}



	}

	/*
	 *	When passenger has reached exit and needs to be deleted
	 */
	public static void RemovePassengers(Passenger passenger)
	{
		allPassengers.Remove(passenger);
	}

	/*
	 *	With a list of passengers, spawn them at an entrance, and get a time for them
	 *	to start being enabled and start moving
	 *	
	 *	Called by Flight
	 */
	public static void SpawnPassengersAtEntrance(List<Passenger> passengers)
	{
		float currentTime = TimeController.TimeInMinutes;
		float timeDelta = spawnTotalMinutes / passengers.Count; // get delta for individual passengers in minuts

		float passengerCount = 0;
		//print("passengers size = " + passengers.Count);

		foreach (Passenger passenger in passengers)
		{
			// first move passenger to correct position
			passenger.transform.position = GetEntrancePosition(passenger);

			int spawnTime = (int) (currentTime + (timeDelta * passengerCount) + Random.Range(0f, spawnVariationMinutes));
			passengersToSpawn.Add(spawnTime, passenger);

			//print("spawn time = " + spawnTime);
			passengerCount++;
		}
	}
	/*
	 *	Get a list of passengers that are ready to spawn when flight
	 *	is disembarking
	 */
	public static List<Passenger> GetOutgoingPassengers(Flight flight)
	{
		List<Passenger> passengers = new List<Passenger>();

		// set position for spawning
		Vector3 position = flight.GetParkingSpot().GetPositionCenter();
		position.y = passenger.transform.position.y;

		for (int i = 0; i < flight.GetFlightCapacity(); i++) {
			Passenger new_passenger = CreateNewPassenger(flight, position);
			new_passenger.SpawnAndWait(flight, flight.GetGate()); 
			passengers.Add(new_passenger);
		}

		return passengers;
	}

	/*
	 *	Get a list of passengers that are ready to spawn when flight
	 *	is disembarking
	 */
	public static List<Passenger> GetIncomingPassengers(Flight flight)
	{
		List<Passenger> passengers = new List<Passenger>();

		for (int i = 0; i < flight.GetFlightCapacity(); i++) {
			Passenger new_passenger = CreateNewPassenger(flight);
			new_passenger.SetupPassenger(flight, flight.GetGate());
			passengers.Add(new_passenger);
		}

		return passengers;
	}

	/* 
	 *	Create new passenger for flight at specified position
	 */
	public static Passenger CreateNewPassenger(Flight flight)
	{
		return CreateNewPassenger(flight, passenger.transform.position);
	}
	public static Passenger CreateNewPassenger(Flight flight, Vector3 position)
	{
		// instantiate new passenger
		GameObject new_go = (GameObject) Instantiate (passenger.gameObject);
		new_go.transform.position = position;
		new_go.transform.parent = passengerParent;

		Passenger new_passenger = new_go.GetComponent<Passenger>();
		new_passenger.Setup();

		return new_passenger;
	}



	/* 
	 *	Get the right entrance position for passenger, right now just the first one
	 *
	 *	NOT IMPLEMENTED/TEMPORARY - need to add checks to make sure that 
	 *	check-in and gate are reachable from entrance
	 */
	public static Vector3 GetEntrancePosition(Passenger passenger)
	{
		Vector3 returnVector = Vector3.zero;

		foreach (Entrance entrance in airport.GetAllEntrances())
		{
			returnVector = entrance.GetPositionCenter();
			returnVector.y = passenger.transform.position.y;
		}

		return returnVector;

	}

	/* 
	 *	Get the right entrance position for passenger, right now just the first one
	 *
	 *	NOT IMPLEMENTED/TEMPORARY - need to get the closest exit
	 */
	public static Vector2 GetExitPosition(Passenger passenger)
	{
		Vector2 returnVector = Vector2.zero;

		foreach (Exit exit in airport.GetAllExits())
		{
			returnVector = exit.GetPositionCenter2();
		}

		return returnVector;

	}
}
