using UnityEngine;
using System.Collections;

public class GridObjectBuildPanel3D : Panel {

	public GameObject selection_object;
	GridObject gridObject;
	Cost cost;

	public Material selectionMaterialValid;
	public Material selectionMaterialInvalid;

	public BuildingSmoke buildingSmoke;


	// Use this for initialization
	void Start () {
		HidePanel ();
	}

	public void Setup(GridObject grid_object)
	{
		gridObject = grid_object;
		cost = gridObject.gameObject.GetComponent<Cost>();
	}


	public void UpdateDragging(Vector3 mousePos)
	{

		gridObject.PlaceGridObjectUpdate (mousePos); // UPDATE THIS
		UpdateSelectionObject ();
		UpdateSelectionObjectMaterial ();

		if (!this.gameObject.activeSelf)
			NGUITools.SetActive (this.gameObject, true);

	}

	void UpdateSelectionObject()
	{

		//GridCell minGridCell = building.GetMinGridCell ();
		//GridCell maxGridCell = building.GetMaxGridCell ();
		Vector2 size = gridObject.GetSize ();

		// set selection object visiblity, scale, and position
		if (!selection_object.GetComponent<Renderer>().enabled)
			selection_object.GetComponent<Renderer>().enabled = false;
		
		selection_object.transform.localScale = new Vector3 ((size.x) * GridHelper.GetGridCellSize (), 
		                                                     selection_object.transform.localScale.y, 
		                                                     (size.y) * GridHelper.GetGridCellSize ());

		this.transform.position = new Vector3 (
			GridHelper.GetWorldPosition2 (gridObject.GetGridPositionMin()).x + ((size.x * GridHelper.GetGridCellSize ()) / 2.0f) - GridHelper.GetGridCellSize() / 2,
			selection_object.transform.position.y,
			GridHelper.GetWorldPosition2 (gridObject.GetGridPositionMin()).y + ((size.y * GridHelper.GetGridCellSize ()) / 2.0f) - GridHelper.GetGridCellSize() / 2);

	}

	void UpdateSelectionObjectMaterial()
	{
		if (gridObject.CanBuild(Vector2.zero) && MoneyController.CanBuy(cost)) {
			selection_object.gameObject.GetComponent<Renderer>().material = selectionMaterialValid;
		} else {
			selection_object.gameObject.GetComponent<Renderer>().material = selectionMaterialInvalid;
		}
	}

	public void BuildGridObjectPress()
	{
		if (gridObject.CanBuild(Vector2.zero) && MoneyController.CanBuy(cost)) {
			GridObject grid_object = gridObject.CreateGridObject();
			MoneyController.Buy(cost, grid_object);
			buildingSmoke.StartSmoke(gridObject);
		} else 
			print ("no build");

	}

	public void Cancel()
	{
		HidePanel ();
	}
}
