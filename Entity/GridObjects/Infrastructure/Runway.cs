using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Runway : PathfindingObject {

	[Header("Runway")]
	public PlaceGridObject placeGridObject;
	public List<Road> roadConnections = new List<Road>();
	public int maxConnections;
	Airport airport;
	public string airportObjectName;


	void Start()
	{
		base.Start();
		Setup ();
		airport = GameObject.Find(airportObjectName).GetComponent<Airport>();
	}

	public override void Setup()
	{
		base.Setup ();
		//this.gameObject.GetComponent<BoxCollider> ().enabled = false;
	}

	public override GridObject CreateGridObject() 
	{
		Runway new_runway_component = (Runway) base.CreateGridObject();
		// also add to airport

		airport.runwayList.Add(new_runway_component);

		return new_runway_component;
	}

	public void PlaceGridObject() 
	{
		this.gameObject.SetActive (true);
		placeGridObject.UpdateOn (this);
	}

	public override void SetRoadConnection(Road road)
	{
		roadConnections.Add(road);
		// should also check for potentially removed connections
	}



// -------------------------------- for pathfinding --------------------------------

	public override PathfindingObject[] GetNeighbours() {

		PathfindingObject[] returnArray = new PathfindingObject[roadConnections.Count];
		
		for (int i = 0; i < roadConnections.Count; i++) {
			returnArray[i] = roadConnections[i];
		}

		return returnArray;

	}


}
