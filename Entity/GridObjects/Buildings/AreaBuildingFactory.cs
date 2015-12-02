using UnityEngine;
using System.Collections;

public class AreaBuildingFactory : GridObject {

	public GameObject airport_go;
	Airport airport;
	public BuildingFactory buildingFactory;

	public Transform areaBuildingContainer;
	public PlaceAreaBuilding placeAreaBuilding;
	public Material areaBuildingMaterial;
	public Vector2 minSize;

	public GameObject selection_object;

	void Start()
	{
		airport = airport_go.GetComponent<Airport> ();
	}

	public void Setup()
	{
		base.Setup ();
	}

	public void SetupPositionAndSize(Vector2 grid_position, Vector2 new_size)
	{
		grid_position_min = grid_position;
		size = new_size;

	}

	public void PlaceBuilding()
	{
		placeAreaBuilding.UpdateOn (this);
	}

	public bool CanBuild(Vector2 arraySize)
	{
		if (CanBuildGrid (arraySize) && CanBuildSize(arraySize))
		    return true;
		else 
		    return false;

	}

	public bool CanBuildGrid(Vector2 arraySize)
	{
		return base.CanBuild (arraySize);
	}

	public bool CanBuildSize(Vector2 arraySize)
	{
		// check i  f minimum size is satisfied
		if (arraySize.x < minSize.x || arraySize.y < minSize.y)
			return false;
		else
			return true;
	}

	
	public AreaBuilding CreateAreaBuilding(	GridCell minGridCell, GridCell maxGridCell, Vector2 size, int type)
	{
		// setup for selection object
		Vector3 oldPosition = selection_object.transform.position;
		SetupPositionAndSize (minGridCell.gridPosition, size);
		selection_object.transform.position = UpdateSelectionObject(minGridCell, maxGridCell, size);

		// instantiate new gameObject and then position and parent it
		GameObject new_go = (GameObject) Instantiate (selection_object);
		new_go.transform.position = selection_object.transform.position;
		new_go.transform.position = new Vector3(selection_object.transform.position.x, 
												selection_object.transform.position.y - 0.05f, 
												selection_object.transform.position.z);
		new_go.transform.parent = areaBuildingContainer;
		new_go.GetComponent<Renderer>().material = areaBuildingMaterial;

		//don't forget to set layer
		new_go.layer = InterfaceController.LAYER_AREA_BUILDING;

		// add components, and add collider
		AreaBuilding new_area = new_go.AddComponent<AreaBuilding> ();
		new_go.AddComponent<BoxCollider> ();
		new_go.GetComponent<BoxCollider> ().size = new Vector3 (0.99f, 0.99f, 0.99f); // otherwise it's counts as outside and size ends up at +1 because of Setup
		new_area.Setup (type, buildingFactory);

		// reflect changes in grid
		GridHelper.BuildGridObject (new_area);

		// and finally add to airport list
		airport.AddAreaBuilding (new_area);

		// reset position
		selection_object.transform.position = oldPosition;

		return new_area;

	}

	/**
	 * 	Helper function for ShowDragging() to update the size of the selection object that shows
	 * 	the size of the AreaBuilding
	 */
	public Vector3 UpdateSelectionObject(GridCell minGridCell, GridCell maxGridCell, Vector2 size)
	{
		// set selection object visiblity, scale, and position
		if (!selection_object.GetComponent<Renderer>().enabled)
			selection_object.GetComponent<Renderer>().enabled = false;
		
		selection_object.transform.localScale = new Vector3 ((size.x) * GridHelper.GetGridCellSize (), 
		                                                     selection_object.transform.localScale.y, 
		                                                    (size.y) * GridHelper.GetGridCellSize ());

		return new Vector3 (
			GridHelper.GetWorldPosition2 (minGridCell.gridPosition).x + ((size.x * GridHelper.GetGridCellSize ()) / 2.0f) - GridHelper.GetGridCellSize() / 2.0f,
			selection_object.transform.position.y,
			GridHelper.GetWorldPosition2 (minGridCell.gridPosition).y + ((size.y * GridHelper.GetGridCellSize ()) / 2.0f) - GridHelper.GetGridCellSize() / 2.0f);

	}
}
