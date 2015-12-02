using UnityEngine;
using System.Collections;

public class AreaBuildPanel3D : Panel {

	public GameObject selection_object;
	public UILabel size_label;

	public UILabel main_label;
	public UIButton buttonDepartures;
	public UIButton buttonArrivals;
	public UIButton buttonTerminal;
	public UIButton buttonSecurity;
	public UIButton buttonBuild;

	public BuildingSmoke buildingSmoke;

	// to pass onto factory:
	GridCell minGridCell;
	GridCell maxGridCell;
	Vector2 size;

	protected AreaBuildingFactory area_building_factory;
	protected PlaceAreaBuilding place_area_building;

	void Start()
	{
		SelectionUpdateLabels (false);
		HidePanel ();
	}

	public void Setup(AreaBuildingFactory areaBuildingFactory, PlaceAreaBuilding placeAreaBuilding)
	{
		area_building_factory = areaBuildingFactory;
		place_area_building = placeAreaBuilding;
	}

	/*
	 * Show dragging UI when trying to build an AreaBuilding
	 * @param mouse_down is mouse position when mouse pressed
	 * @param mouse_up is mouse position when mouse released
	 */
	public void ShowDragging(Vector3 mouse_down, Vector3 mouse_up)
	{

		// flip order of mouseUp and mouseDown if necessary so that lowest x and y are in mouseDown
		Vector3 temp;
		if (mouse_down.x > mouse_up.x) { temp.x = mouse_down.x; mouse_down.x = mouse_up.x; mouse_up.x = temp.x; } 
		if (mouse_down.z > mouse_up.z) { temp.z = mouse_down.z; mouse_down.z = mouse_up.z; mouse_up.z = temp.z; }
		
		// get gridCell, size, and center for calcuation
		minGridCell = GridHelper.GetGridCell (mouse_down);
		maxGridCell = GridHelper.GetGridCell (mouse_up);
		size = new Vector2 (maxGridCell.gridPosition.x - minGridCell.gridPosition.x + 1, 
									maxGridCell.gridPosition.y - minGridCell.gridPosition.y + 1);


		if (size != area_building_factory.GetSize ())
		{
			area_building_factory.SetupPositionAndSize (minGridCell.gridPosition, size);
			this.transform.position = area_building_factory.UpdateSelectionObject (minGridCell, maxGridCell, size);

			DraggingUpdateLabels (minGridCell, size);
		
			if (!this.gameObject.activeSelf)
				NGUITools.SetActive (this.gameObject, true);

		}

	}

	/**
	 * 	Helper function for ShowDragging() to update the labels that provide help with
	 * 	the size of an AreaBuilding
	 */
	void DraggingUpdateLabels(GridCell minGridCell, Vector2 size)
	{	
		if (size.x < 3 || size.y < 3) {
			NGUITools.SetActive (size_label.gameObject, false);
		} else if (!area_building_factory.CanBuildGrid (size)) {
			size_label.text = "building is overlapping, draw again";
			//main_label.transform.position = new Vector3(center.x, main_label.transform.position.y, center.y);
			NGUITools.SetActive (size_label.gameObject, true);
	    } else if (!area_building_factory.CanBuildSize (size)) {
			size_label.text = "size needs to be at least " + area_building_factory.minSize.x + ". currently: " + size;
			//main_label.transform.position = new Vector3(center.x, main_label.transform.position.y, center.y);
			NGUITools.SetActive (size_label.gameObject, true);

		} else {
			size_label.text = "release mouse button to see building options";
		}

		if (main_label.gameObject.activeSelf) 
			SelectionUpdateLabels(false);

	}

	/*
	 * Show UI when mouse is released whentrying to build an AreaBuilding
	 * @param mouse_down is mouse position when mouse pressed
	 * @param mouse_up is mouse position when mouse released
	 */
	public void ShowOnRelease(Vector3 mouse_down, Vector3 mouse_up)
	{
		// flip order of mouseUp and mouseDown if necessary so that lowest x and y are in mouseDown
		Vector3 temp;
		if (mouse_down.x > mouse_up.x) { temp.x = mouse_down.x; mouse_down.x = mouse_up.x; mouse_up.x = temp.x; } 
		if (mouse_down.z > mouse_up.z) { temp.z = mouse_down.z; mouse_down.z = mouse_up.z; mouse_up.z = temp.z; }
		
		// get gridCell, size, and center for calcuation
		GridCell minGridCell = GridHelper.GetGridCell (mouse_down);
		GridCell maxGridCell = GridHelper.GetGridCell (mouse_up);
		Vector2 size = new Vector2 (maxGridCell.gridPosition.x - minGridCell.gridPosition.x + 1, maxGridCell.gridPosition.y - minGridCell.gridPosition.y + 1);
		Vector2 center = GridHelper.GetWorldPosition2 (GridHelper.GetGridCell (
					new Vector3 (	mouse_down.x + ((mouse_up.x - mouse_down.x) / 2), 0, 
		             mouse_down.z + ((mouse_up.z - mouse_down.z) / 2))).gridPosition);


		area_building_factory.SetupPositionAndSize (minGridCell.gridPosition, size);
		
		if (area_building_factory.CanBuild(size)) {

			if (size.x >= area_building_factory.minSize.x && size.y >= area_building_factory.minSize.y) {
				SelectionUpdateLabels(true);
			}
		} else {
			//DraggingUpdateLabels(minGridCell, size, center);
		}

	}

	/*
	 * Helper function to flip between pre-build and build phase by showing and hiding
	 * labels and menu
	 */
	protected void SelectionUpdateLabels(bool show)
	{
		NGUITools.SetActive (size_label.gameObject, !show);
		
		NGUITools.SetActive (main_label.gameObject, show);
		NGUITools.SetActive (buttonArrivals.gameObject, show);
		NGUITools.SetActive (buttonDepartures.gameObject, show);
		NGUITools.SetActive (buttonSecurity.gameObject, show);
		NGUITools.SetActive (buttonTerminal.gameObject, show);
		NGUITools.SetActive (buttonBuild.gameObject, show);
		
		if (show)
			EnableAllButtons ();
	}

	public void Cancel()
	{
		HidePanel ();
	}
	
	protected void EnableAllButtons()
	{
		buttonDepartures.isEnabled = true;
		buttonArrivals.isEnabled = true;
		buttonTerminal.isEnabled = true;
		buttonSecurity.isEnabled = true;
	}

	public void BuildingSelectionButtonPress(UIButton button)
	{
		EnableAllButtons ();
		button.isEnabled = false;
	}

	public void BuildButtonPress()
	{
		if (!buttonDepartures.isEnabled) 
			area_building_factory.CreateAreaBuilding (minGridCell, maxGridCell, size, AreaBuilding.TYPE_DEPARTURES);
		else if (!buttonArrivals.isEnabled)
			area_building_factory.CreateAreaBuilding (minGridCell, maxGridCell, size, AreaBuilding.TYPE_ARRIVALS);
		else if (!buttonTerminal.isEnabled)
			area_building_factory.CreateAreaBuilding (minGridCell, maxGridCell, size, AreaBuilding.TYPE_TERMINAL);
		else if (!buttonSecurity.isEnabled)
			area_building_factory.CreateAreaBuilding (minGridCell, maxGridCell, size, AreaBuilding.TYPE_SECURITY);
		else
		{
			return;
		}

		buildingSmoke.StartSmoke(size);
		place_area_building.Complete ();
	}

}
