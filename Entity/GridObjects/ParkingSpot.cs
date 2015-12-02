using UnityEngine;
using System.Collections;

public class ParkingSpot : PathfindingObject {


	[Header("ParkingSpot")]
	public PlaceGridObject placeGridObject;
	Gate gate;
	public Road roadConnection;

	void Start()
	{
		base.Start();
		Setup ();
	}

	public override void Setup()
	{
		base.Setup ();
	}

	public override GridObject CreateGridObject() 
	{
		ParkingSpot new_parking_spot_component = (ParkingSpot) base.CreateGridObject();

		gate = (Gate) buildingSnap.GetSnapObject();
		gate.AddParkingSpot(new_parking_spot_component);
		new_parking_spot_component.gate = gate;

		return new_parking_spot_component;
		
	}

	public void PlaceGridObject() 
	{
		this.gameObject.SetActive (true);
		placeGridObject.UpdateOn (this);
	}

	/*
  	 *	Does this parking spot have a road connection?
  	 */
	public bool HasRoadConnection()
	{
		if (roadConnection != null)
			return true;
		else
			return false;
	}

	public override void SetRoadConnection(Road road)
	{
		roadConnection = road;
	}

// -------------------------------- for pathfinding --------------------------------

	public override PathfindingObject[] GetNeighbours() {
		PathfindingObject[] returnArray = new PathfindingObject[1];
		returnArray[0] = roadConnection;
		return returnArray;
	}


}
