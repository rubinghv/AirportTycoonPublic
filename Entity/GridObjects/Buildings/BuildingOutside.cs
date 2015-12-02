using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingOutside : GridObject {

	[Header("Building Outside")]
	public PlaceGridObject placeGridObject;
	Airport airport;
	//public string airportObjectName;


	void Start()
	{
		base.Start();
		Setup ();
		airport = GameObject.Find(Airport.airportGoName).GetComponent<Airport>();
	}

	public override void Setup()
	{
		base.Setup ();
		//this.gameObject.GetComponent<BoxCollider> ().enabled = false;
	}

	public override GridObject CreateGridObject() 
	{
		BuildingOutside new_outside_building_component = (BuildingOutside) base.CreateGridObject();
		
		//also add to airport
		airport.AddOutsideBuilding(new_outside_building_component);

		return new_outside_building_component;
	}

	public void PlaceGridObject() 
	{
		this.gameObject.SetActive (true);
		placeGridObject.UpdateOn (this);
	}

	public override void SetupPositionAndSize()
	{
		// setup array positions and size

		this.gameObject.GetComponent<BoxCollider> ().enabled = true;
		UpdatePositionAndSize ();
	}

}
