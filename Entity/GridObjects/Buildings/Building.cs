using UnityEngine;
using System.Collections;

public class Building : GridObject {

	[Header("Building")]
	public PlaceBuilding placeBuilding;
	public int[] areaBuildingType = new int[4];

	protected virtual void Start()
	{
		Setup ();
	}

	public override void Setup()
	{
		base.Setup ();
		//this.gameObject.GetComponent<BoxCollider> ().enabled = false;
	}


	// -------------- All of this should perhaps be part of building factory

	/**
	 * 	Called by InfoPlotPurchasedPanel (ui event)
	 * 
	 */
	public void PlaceBuilding() 
	{
		this.gameObject.SetActive (true);
		placeBuilding.UpdateOn (this);
	}

	/**
	 * 	Called by PlaceBuilding (interface event)
	 * 
	 */ 
	public void PlaceBuildingUpdate(Vector3 pos)
	{
		Vector2 arrayPos = GridHelper.GetGridPosition (pos);
		this.transform.position = GridHelper.GetWorldPosition3 (arrayPos, this.transform.position.y);

		SetupPositionAndSize ();
	}

	public void CancelPlaceBuilding()
	{
		this.gameObject.SetActive (false);
	}
	
	public virtual Building CreateBuilding()
	{
		// instantiate new gameObject and then position and parent it
		GameObject new_go = (GameObject) Instantiate (this.gameObject);
		new_go.transform.position = this.transform.position;
		new_go.SetActive (true);

		//don't forget to set layer
		new_go.layer = InterfaceController.LAYER_BUILDING;
		this.gameObject.layer = InterfaceController.LAYER_BUILDING;

		// reflect changes in grid
		Building new_building = new_go.GetComponent<Building>();
		new_building.SetupPositionAndSize();
		GridHelper.BuildGridObject (new_building);

		// Add to area building 
		AreaBuilding area_building = GridHelper.GetGridCell (grid_position_min.x, grid_position_min.y).GetAreaBuilding ();
		area_building.AddBuilding (new_go.GetComponent<Building>());

		return new_building;
	}

	/*
	 * Test to see if an area building matches the type number of this building
	 * Used to check if building can be build
	 */
	public bool AreaBuildingTypeMatch(int type_number) 
	{
		for (int i = 0; i < areaBuildingType.Length; i++)
		{
			//Debug.Log (areaBuildingType[i] + " == " + type_number);

			if (areaBuildingType[i] == type_number)
				return true;
		}
		return false;
	}

	// ------------------------------------- Saving -------------------------------------

	public override void Save (string filename, string tag) {
		base.Save(filename, tag);

		ES2.Save(this.transform.position, filename + tag + "realMeshPosition");
			
	}

	public override void Load (string filename, string tag) {
		base.Load(filename, tag);
	}

}
