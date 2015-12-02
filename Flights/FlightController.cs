using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 *	In CreateFlight() need to create method for spawning airplanes if there are multiple runways
 *
 */
public class FlightController : MonoBehaviour {

	/*
	 *	A flight that cannot be created right now because there is no connection from
	 * 	runway to parking spot, or some other reason
	 */
	protected class FutureFlight {
		public Gate gate; public ParkingSpot parkingSpot;
		public FutureFlight(Gate _gate, ParkingSpot parking_spot) {
			gate = _gate; parkingSpot = parking_spot;
		}
	}

	Airport airport;
	public Transform airplaneParent;
	public Airplane airplane; // need to be able to spawn different types of airplanes at some point
	public int waitSecondsManageFutureFlights;

	static List<FutureFlight> futureFlights= new List<FutureFlight>(); // flights before they are added to flightList
	static List<Flight> flights = new List<Flight>();	

	[Header("Flight Passenger Exit ")]
 	public int secondsDelayBeforeExit = 200; // delay plain arriving and first passenger leaving
 	public int secondsSeparation = 30; // max gap between passengers
 	public int secondsVariation = 15; // separation - variation = min gap between passengers
	
	[Header("Gate parameters ")]
 	public int minutesToBoarding; // minutes between disembarking and embarking // (in future, disembarking should be refueling/food)
 	public int minutesBoarding;

	void Start()
	{
		airport = GameObject.Find("Airport").GetComponent<Airport>();
	}

	void Update() 
	{
		if (futureFlights.Count > 0) {
			ManageFutureFlights(Time.deltaTime);
		}
	
	}

	float timeSinceManageFlight = 0;
	void ManageFutureFlights(float deltaTime)
	{
		timeSinceManageFlight += deltaTime;

		// test if we have waited for 2 seconds yet
		if (timeSinceManageFlight >= waitSecondsManageFutureFlights)
			timeSinceManageFlight = 0;
		else 
			return;

		// loop through all future flights and see if they can now work
		FutureFlight createFlight = null;
		foreach(FutureFlight futureFlight in futureFlights) {
			// if no road correction, don't bother looking for path
			if (ReadyForFlightCreation(futureFlight.parkingSpot, futureFlight)) {
				createFlight = futureFlight;
				break;
			}

		}

		if (createFlight != null)
		{
			CreateFlight(createFlight);
			futureFlights.Remove(createFlight);
		}

	}

	/*
	 *	Test if all the conditions are verified so that passengers and airplanes can be spawned
	 *	in the form of a Flight being created
	 */
	bool ReadyForFlightCreation(PathfindingObject pathObject, FutureFlight futureFlight)
	{
		// test if the parking spot has a road connection
		if (!futureFlight.parkingSpot.HasRoadConnection()) {
			return false;
		}
		// test if runways are reachable
		else if (!AreRunwaysReachable(futureFlight.parkingSpot)) {
			return false;
		}
		// test if there are entrances
		else if (airport.GetAllEntrances().Count < 1)
			return false;

		return true;
	}

	/*
	 *	Is it possible to travel from @param pathObject to any of the
	 * 	runways? 
	 */
	bool AreRunwaysReachable(PathfindingObject pathObject)
	{
		foreach(Runway runway in airport.runwayList) {
			PathfindingObject[] path = RoadPathfinding.CreatePath (pathObject, runway);
			
			if (path != null) {
				return true;
			} 
			
		}

		return false;
	}

	/*
	 *	Create a new flight and add to flight list
	 */
	void CreateFlight(FutureFlight futureFlight) {
		Runway curRunway = airport.runwayList[0]; // very temporary, need to able to chose runway

		// create flight and add to list
		Flight newFlight = new Flight(futureFlight.gate, futureFlight.parkingSpot, curRunway, 
			secondsDelayBeforeExit, secondsSeparation, secondsVariation,
			minutesToBoarding, minutesBoarding, this);
		
		CreateFlightFinal(newFlight, curRunway);
	}

	void CreateFlight(Flight flight) {
		Runway curRunway = airport.runwayList[0]; // very temporary, need to able to chose runway

		// create flight and add to list
		Flight newFlight = new Flight(flight.GetGate(), flight.GetParkingSpot(), curRunway, 
			secondsDelayBeforeExit, secondsSeparation, secondsVariation,
			minutesToBoarding, minutesBoarding, this);
		
		CreateFlightFinal(newFlight, curRunway);
	}

	void CreateFlightFinal(Flight flight, Runway runway) {
		flights.Add(flight);

		// need to create method for spawning airplanes if there are different types of airplanes
		flight.SpawnAirplane(runway.GetPositionCenter2(), airplane, airplaneParent);
		flight.AddPassengers();	// adds both incoming and outgoing passengers

		// TEMPORARY needs to be delayed somehow?
		flight.SpawnIncomingPassengers();

		Debug.Log("created flight!");
	}

	/*
	 *	Called by flight when it is completed
	 */
	public void FlightComplete(Flight flight)
	{	
		// remove from current flights
		flights.Remove(flight);
		CreateFlight(flight);
	}


	/* ----------------------------------- static methods -----------------------------------
	/*
	 *	When a parking spot is build (connected to gate) this function will 
	 * 	add the flight to the futureFlightList so it can be tested for validity
	 */
	public static void FlightSpotAvailable(Gate gate, ParkingSpot parking_spot)
	{	
		FutureFlight futureFlight = new FutureFlight(gate, parking_spot);
		futureFlights.Add(futureFlight);
	}

		
		
	


	
}
