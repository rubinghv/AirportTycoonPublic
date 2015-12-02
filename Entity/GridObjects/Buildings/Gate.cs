using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gate : BuildingPassenger {

	public Transform[] enterPath;

 	[Header("Benches")]
	public List<Bench> benches;
	public Transform enterBenches;

	ParkingSpot parkingSpot;
	public Transform queueEnter;

	protected override void Start()
	{
		base.Start ();
		//foreach(Bench bench in benches)
		//bench.Setup(this);
	}

	public override Building CreateBuilding()
	{
		return base.CreateBuilding ();
	}

	public void AddParkingSpot(ParkingSpot parking_spot)
	{
		parkingSpot = parking_spot;
		// notify flight controller that this gate + parking spot is ready
		FlightController.FlightSpotAvailable(this, parkingSpot);
	}

	public Vector2 GetEnterBenchesPosition()
	{
		return new Vector2 (enterBenches.position.x, enterBenches.position.z);
	}

	/*
	 *	Add to the bench with the least amount of people on it
	 *
	 *	If nothing available return null
	 */
	public Bench AddToBench(Passenger passenger)
	{
		// find the bench with the lostwest number of passengers on it
		Bench mostAvailableBench = null;
		foreach (Bench bench in benches) {
			if (mostAvailableBench == null)
				mostAvailableBench = bench;
			else if (bench.GetBenchSpaces() > mostAvailableBench.GetBenchSpaces()) {
				mostAvailableBench = bench;
			}
		}

		// make sure there is a bench available
		if (mostAvailableBench != null)
		{
			mostAvailableBench.AddPassenger(passenger);
			return mostAvailableBench;
		}

		return null;
	}

	public override bool CanSnap()
	{
		if (parkingSpot == null)
			return true;
		else
			return false;
	}

	/*
	 *	Enter path, for passengers entering gate from parking spot
	 * 	copy enter path and then add queueEnter as last position
	 */
	public Vector2[] GetEnterPath()
	{
		Vector2[] returnPath = Helper.CopyArrayVector2(enterPath, enterPath.Length + 1);
		returnPath[returnPath.Length - 1] = new Vector2(queueEnter.position.x, queueEnter.position.z);
		returnPath = Helper.InvertArray(returnPath);

		return returnPath;
	}

}
