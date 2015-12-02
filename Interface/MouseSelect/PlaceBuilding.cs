using UnityEngine;
using System.Collections;

public class PlaceBuilding : MouseSelect {

	Building building;
	public BuildingBuildPanel3D buildPanel;

	void Update()
	{
		if (updateOn)
		{
			Hover();
			Select ();
			Rotate();
		}
	}

	public void UpdateOn(Building new_building)
	{
		building = new_building;
		buildPanel.Setup (building);

		base.UpdateOn ();

		// turn off other interfaces
		this.gameObject.GetComponent<SelectEntity>().Pause();
	}

	protected override bool Hover()
	{
		return HoverPosition(HoverMouseMethod, layerMasks);
	}


	protected override bool Select()
	{
		return SelectPosition(LeftMouseClickMethod, 0, layerMasks);
	}

	protected void Rotate()
	{
		if (Input.GetMouseButtonUp(2))
			building.Rotate();
	}

	public delegate bool HoverMouse(Vector3 pos);
	public override bool HoverMouseMethod(Vector3 pos)
	{
		if (pos != Vector3.zero) {
			buildPanel.UpdateDragging(pos);

			//building.PlaceBuildingUpdate(pos);
		}
		
		return true;
	}

	public delegate bool LeftMouseClick(Vector3 pos);
	public override bool LeftMouseClickMethod(Vector3 pos)
	{
		if (pos == Vector3.zero)
			return false;
		else {
			buildPanel.BuildBuildingPress();

			return true;
		}
	}
	

	protected override void Cancel()
	{
		buildPanel.Cancel ();
		building.CancelPlaceBuilding ();
		this.gameObject.GetComponent<SelectEntity>().UnPause();
		updateOn = false;

	}
}