using UnityEngine;
using System.Collections;

public class BuildingBuildPanel3D : Panel {

	public GameObject selection_object;
	Building building;
	Cost cost;

	public Material selectionMaterialValid;
	public Material selectionMaterialInvalid;

	public BuildingSmoke buildingSmoke;

	// Use this for initialization
	void Start () {
		HidePanel ();
	}

	public void Setup(Building new_building)
	{
		building = new_building;
		cost = building.gameObject.GetComponent<Cost>();
	}


	public void UpdateDragging(Vector3 mousePos)
	{

		building.PlaceBuildingUpdate (mousePos);
		UpdateSelectionObject ();
		UpdateSelectionObjectMaterial ();

		if (!this.gameObject.activeSelf)
			NGUITools.SetActive (this.gameObject, true);

	}

	void UpdateSelectionObject()
	{

		//GridCell minGridCell = building.GetMinGridCell ();
		//GridCell maxGridCell = building.GetMaxGridCell ();
		Vector2 size = building.GetSize ();

		// set selection object visiblity, scale, and position
		if (!selection_object.GetComponent<Renderer>().enabled)
			selection_object.GetComponent<Renderer>().enabled = false;
		
		selection_object.transform.localScale = new Vector3 ((size.x) * GridHelper.GetGridCellSize (), 
		                                                     selection_object.transform.localScale.y, 
		                                                     (size.y) * GridHelper.GetGridCellSize ());

		this.transform.position = new Vector3 (
			GridHelper.GetWorldPosition2 (building.GetGridPositionMin()).x + ((size.x * GridHelper.GetGridCellSize ()) / 2.0f) - GridHelper.GetGridCellSize() / 2,
			selection_object.transform.position.y,
			GridHelper.GetWorldPosition2 (building.GetGridPositionMin()).y + ((size.y * GridHelper.GetGridCellSize ()) / 2.0f) - GridHelper.GetGridCellSize() / 2);

	}

	void UpdateSelectionObjectMaterial()
	{
		if (building.CanBuild(Vector2.zero) && MoneyController.CanBuy(cost)) {
			selection_object.gameObject.GetComponent<Renderer>().material = selectionMaterialValid;
		} else {
			selection_object.gameObject.GetComponent<Renderer>().material = selectionMaterialInvalid;
		}
	}

	public void BuildBuildingPress()
	{
		if (building.CanBuild(Vector2.zero) && MoneyController.CanBuy(cost)) {
			GridObject grid_object = (GridObject) building.CreateBuilding();
			MoneyController.Buy(cost, grid_object);
			buildingSmoke.StartSmoke(building);
		} else 
			print ("no build");

	}

	public void Cancel()
	{
		HidePanel ();
	}
}
