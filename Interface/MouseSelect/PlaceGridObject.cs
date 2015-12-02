using UnityEngine;
using System.Collections;

public class PlaceGridObject : MouseSelect {

	GridObject gridObject;
	public GridObjectBuildPanel3D buildPanel;

	void Update()
	{
		if (updateOn)
		{
			Hover();
			Select ();
			Rotate();
		}
	}

	public void UpdateOn(GridObject grid_object)
	{
		gridObject = grid_object;
		buildPanel.Setup (gridObject);
		base.UpdateOn ();

		// turn off other interfaces
		this.gameObject.GetComponent<SelectEntity>().Pause();
	}

	protected override bool Hover()
	{
		return HoverPosition(HoverMouseMethod, layerMask);
	}


	protected override bool Select()
	{
		return SelectPosition(LeftMouseClickMethod, 0, layerMask);
	}

	protected void Rotate()
	{
		if (Input.GetMouseButtonUp(2))
			gridObject.Rotate();
	}

	public delegate bool HoverMouse(Vector3 pos);
	public override bool HoverMouseMethod(Vector3 pos)
	{
		if (pos != Vector3.zero) {
			buildPanel.UpdateDragging(pos);
		}
		
		return true;
	}

	public delegate bool LeftMouseClick(Vector3 pos);
	public override bool LeftMouseClickMethod(Vector3 pos)
	{
		if (pos == Vector3.zero)
			return false;
		else {
			buildPanel.BuildGridObjectPress();

			return true;
		}
	}
	

	protected override void Cancel()
	{
		buildPanel.Cancel ();
		gridObject.CancelPlacement ();
		this.gameObject.GetComponent<SelectEntity>().UnPause();
		updateOn = false;
	}
}