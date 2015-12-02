using UnityEngine;
using System.Collections;

public class PlaceRoad : MouseSelect {

	public RoadBuildPanel3D roadBuild3D;
	bool firstClick = true;
	bool firstClickSnap = false;
	Vector3 firstClickPosition = Vector3.zero;

	void Update()
	{
		if (updateOn)
		{
			if (!HoverRoad())
				Hover();
			
			if (!SelectRoad())
				Select();
		}
	}

	public void UpdateOn()
	{
		base.UpdateOn ();
		roadBuild3D.Setup(this);
	}

	bool Select()
	{
		return SelectPosition(LeftMouseClickMethod, 0, layerMasks);
	}

	bool SelectRoad()
	{
		return SelectPosition(LeftMouseClickMethodToo, 0, InterfaceController.LAYER_ROAD);
	}

	bool Hover()
	{
		return HoverPosition(HoverMouseMethod, layerMask);
	}

	bool HoverRoad()
	{
		return HoverPosition(HoverMouseMethodToo, InterfaceController.LAYER_ROAD);
	}

	public delegate bool HoverMouse(Vector3 pos);
	public override bool HoverMouseMethod(Vector3 pos)
	{
		if (!firstClick && pos != Vector3.zero) {
			
			if (firstClickSnap) {
				GridCell roadGridCell = GridHelper.GetGridCell(firstSnapPosition);
				Vector2 roadDirection = roadBuild3D.GetStraightOneDirection(firstSnapPosition, pos);
				firstClickPosition = roadGridCell.GetRoad().GetSnapGridCell(roadDirection, true, firstClickPosition);

			}
			roadBuild3D.ShowPreview(firstClickPosition, pos, false);
			return true;
		}
			
		return false;
		
	}

	public delegate bool HoverMouseToo(Vector3 pos);
	public bool HoverMouseMethodToo(Vector3 pos)
	{
		if (!firstClick && pos != Vector3.zero) {
			
			if (firstClickSnap) {
				GridCell roadGridCell = GridHelper.GetGridCell(firstSnapPosition);
				Vector2 roadDirection = roadBuild3D.GetStraightOneDirection(firstSnapPosition, pos);
				firstClickPosition = roadGridCell.GetRoad().GetSnapGridCell(roadDirection, true, firstClickPosition);
			}
			roadBuild3D.ShowPreview(firstClickPosition, pos, true);
			return true;
		}
		
		return false;
	}

	public delegate bool LeftMouseClick(Vector3 pos);
	public override bool LeftMouseClickMethod(Vector3 pos)
	{
		// if first click, save position so we can go to hover
		if (firstClick) {
			firstClickPosition = pos;
			firstClick = false;
			return true;
		//} else if (firstClickSnap && pos != Vector3.zero) {
		//	print("trying to connect from snap");
		} else if (pos != Vector3.zero) {
			if (roadBuild3D.IsRoadStraight(firstClickPosition, pos))
				roadBuild3D.CompletePreview(firstClickPosition, pos, false, false);
			else 
				roadBuild3D.CompletePreview(firstClickPosition, pos, true, false);

			return true;
		}
		
		return false;
	}

	Vector3 firstSnapPosition;

	public delegate bool LeftMouseClickToo(Vector3 pos);
	public bool LeftMouseClickMethodToo(Vector3 pos)
	{
		// if first click, save position so we can go to hover
		if (firstClick && pos != Vector3.zero) {
			firstClickPosition = pos;
			firstSnapPosition = pos;
			firstClick = false;
			firstClickSnap = true;
		//} else if (firstClickSnap && pos != Vector3.zero) {
		//	print("trying to connect from snap TWO");
			return true;

		} else if (pos != Vector3.zero) {
			print("made it here too");
			if (roadBuild3D.IsRoadStraight(firstClickPosition, pos))
				roadBuild3D.CompletePreview(firstClickPosition, pos, false, true);
			else 
				roadBuild3D.CompletePreview(firstClickPosition, pos, true, true);

			return true;
		}
		
		return false;
	}


	protected override void Cancel()
	{
		base.Cancel ();
		roadBuild3D.Cancel();
		firstClick = true;
		firstClickSnap = false;
	}

	public void Complete()
	{
		Cancel ();
	}
}
