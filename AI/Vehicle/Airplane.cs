using UnityEngine;
using System.Collections;

public class Airplane : Vehicle {

	[Header("Airplane")]

	public static int STATE_SPAWNED = 1;

	public int passengerCapacity;	
	public GameObject door;
	public Transform[] entrancePath;
	/*
	public static int STATE_PATHFINDING_TEST = 99;
	public static int STATE_TRAVELING = 100;

	public PathfindingObject startBuilding; // temporary
	public PathfindingObject endBuilding; // temporary*/

	Flight flight;

	// Use this for initialization
	void Start () {
		base.Start ();
		state = STATE_SPAWNED;

	}

	protected override void DefaultBehaviour (float deltaTime)
	{
		flight.ManualUpdate();

		if (flight != null)
			state = flight.state;

		else if (flight.state == Flight.STATE_ON_RUNWAY)
		{
			steering.Visit(flight.GetRunway(), flight.GetParkingSpot());
			finalDestination = steering.GetFinalDestination();

			flight.state = Flight.STATE_TRAVELING_RUNWAY_TO_PARKING;
		}
		else if (flight.state == Flight.STATE_TRAVELING_RUNWAY_TO_PARKING) {
			if (HasArrived(GridHelper.GetGridCellSize() / 10f)) {
				steering.FullStop();

				// this state gives control back to Flight object
				flight.state = Flight.STATE_GATE_ARRIVED;
			}
		}
		else if (flight.state == Flight.STATE_GATE_BOARDING_FINISHED) {
			steering.Visit(flight.GetParkingSpot(), flight.GetRunway());
			finalDestination = steering.GetFinalDestination();

			flight.state = Flight.STATE_TRAVELING_PARKING_TO_RUNWAY;
		}
		else if (flight.state == Flight.STATE_TRAVELING_PARKING_TO_RUNWAY) {
			if (HasArrived(GridHelper.GetGridCellSize() / 10f)) {
				flight.state = Flight.STATE_RUNWAY_DEPARTING;
			}
		} else if (flight.state == Flight.STATE_RUNWAY_DEPARTING) {
			print("on runway ready to depart!");
			flight.state = Flight.STATE_FLIGHT_EXIT_FINAL;
		} else if (flight.state == Flight.STATE_FLIGHT_EXIT_FINAL) {
			print("departed, ready to be removed and respawn!");
			flight.CompleteFlight();
		}
	
	}

	public void SetFlight(Flight _flight)
	{
		flight = _flight;
	}

	/* 
	 *	Open door so that passengers can get out
	 */
	public void OpenDoor()
	{
		iTween.RotateBy(door,iTween.Hash(
    			 	"amount", new Vector3(0.416f,0,0),
    			 	"easetype", iTween.EaseType.easeInOutQuad,
    			 	"time", 5f));
	}

	/* 
	 *	Close door so that airplane can leave
	 */
	public void CloseDoor()
	{
		iTween.RotateBy(door,iTween.Hash(
    			 	"amount", new Vector3(-0.416f,0,0),
    			 	"easetype", iTween.EaseType.easeInOutQuad,
    			 	"time", 5f));
	}

	/*
	 *	Exit position, where passenger spawns when leaving airplane, is the
	 *	first waypoint from the exit path
	 */
	public Vector3 GetExitPosition()
	{
		return entrancePath[entrancePath.Length - 1].transform.position;
	}

	public Vector2[] GetExitPath()
	{
		Vector2[] returnPath = new Vector2[entrancePath.Length - 1];

		returnPath[0] = new Vector2(entrancePath[0].position.x, entrancePath[0].position.z);
		returnPath[1] = new Vector2(entrancePath[1].position.x, entrancePath[1].position.z);

		return returnPath;
	}

	public Vector2[] GetEnterPath()
	{
		Vector2[] returnPath = Helper.CopyArrayVector2(entrancePath);
		returnPath = Helper.InvertArray(returnPath);
//		print("returnPath size = " + returnPath.Length + " - " + returnPath);

		return returnPath;
	}

	
}
