using UnityEngine;
using System.Collections;

public class PlaceAreaBuilding : MouseSelect {

	public AreaBuildPanel3D areaBuild3D;
	public SelectGridCell selectGridCell;

	void Update()
	{
		if (updateOn)
		{
			Drag ();
		}
	}

	public void UpdateOn(AreaBuildingFactory areaBuilding)
	{
		areaBuild3D.Setup (areaBuilding, this);
		base.UpdateOn ();
		selectGridCell.UpdateOn ();
	}

	bool Drag()
	{
		return DragPosition(DragMouseMethod, DragMouseUpdateMethod, 0, layerMask);
	}

	public delegate bool DragMouseUpdate(Vector3 mouseDown, Vector3 mouseUp);
	public override bool DragMouseUpdateMethod(Vector3 mouseDown, Vector3 mouseUp) 
	{ 
		selectGridCell.UpdateOn ();
		areaBuild3D.ShowDragging (mouseDown, mouseUp);
		return true;
	}

	public delegate bool DragMouse(Vector3 mouseDown, Vector3 mouseUp);
	public override bool DragMouseMethod(Vector3 mouseDown, Vector3 mouseUp) 
	{ 
		//print ("finishing from cell: " + GridHelper.GetGridCell (mouseDown).arrayPosition + " to position " + GridHelper.GetGridCell (mouseUp).arrayPosition);
		selectGridCell.UpdateOff ();
		areaBuild3D.ShowOnRelease (mouseDown, mouseUp);
		return true;
		
	}

	protected override void Cancel()
	{
		base.Cancel ();
		selectGridCell.UpdateOff ();
		areaBuild3D.Cancel ();
	}

	public void Complete()
	{
		Cancel ();
	}

}
