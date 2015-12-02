using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadCorner : Road {

	// connections have to be set to protected
	public List<Road> incomingConnections;
 	public List<Road> outgoingConnections;

	public Material roadMaterialCornerTwo;
	public Material roadMaterialCornerThree;
	public Material roadMaterialCornerFour;
	
	public override GameObject CreateRoad(Road oldRoad)
	{
		GameObject go = base.CreateRoad(oldRoad);

		RoadCorner sample_corner = GameObject.Find("buildRoadCornerHighlight").GetComponent<RoadCorner>();

		roadMaterialCornerTwo = sample_corner.roadMaterialCornerTwo;
		roadMaterialCornerThree = sample_corner.roadMaterialCornerThree;
		roadMaterialCornerFour = sample_corner.roadMaterialCornerFour;

		return go;
	}

	public override int GetRoadWidth()
	{
		return (int) size.x;
	}

	public override int GetRoadLength()
	{
		return GetRoadWidth();
	}

	public void UpdateRoad()
	{

		SetupPositionAndSize();
		base.UpdateRoad();
	}

	protected override void UpdateConnections(bool updateRecursively)
	{
		

	}
	
	int GetConnections()
	{
		int count = 0;
		if (topConnection != null)
			count++;
		if (rightConnection != null)
			count++;
		if (bottomConnection != null)
			count++;
		if (leftConnection != null)
			count++;

		return count;
	}

	/* update scale and rotation of materials
	 *
	 *
	 */
	protected override void UpdateMaterials()
	{
		if (GetConnections() == 2)
		{
			this.gameObject.GetComponent<Renderer>().material = roadMaterialCornerTwo;

			if (topConnection != null & rightConnection!= null )
				RotateMesh(90f);
			else if (rightConnection != null & bottomConnection != null)
				RotateMesh(180f);
			else if (bottomConnection != null & leftConnection != null)
				RotateMesh(-90f);
			else if (leftConnection != null & topConnection != null)
				RotateMesh(0f);
			// otherwise don't rotate
		} else if (GetConnections() == 3) {
			this.gameObject.GetComponent<Renderer>().material = roadMaterialCornerThree;

			if (leftConnection == null)
				RotateMesh(90f);
			else if (topConnection == null)
				RotateMesh(180f);
			else if (rightConnection == null)
				RotateMesh(-90f);
			else if (bottomConnection == null)
				RotateMesh(0f);
		} else if (GetConnections() == 4) {
			this.gameObject.GetComponent<Renderer>().material = roadMaterialCornerFour;
		} else {
			//print("something went horribly wrong");
		}



	}
	/*
	 *	Get the point where an incoming road should snap to the current road
	 */
	public override Vector3 GetSnapGridCell(Vector2 direction, bool mouseDown, Vector3 pos)
	{
		Vector2 arrayPosition= Vector2.zero;

		if (direction.x != 0) { // horizontal road
			arrayPosition.y = grid_position_min.y + (int) (GetRoadWidth() / 2);

			if ((direction.x == -1 && !mouseDown) || (direction.x == 1 && mouseDown))
				arrayPosition.x = grid_position_max.x + 1;
		 	else if ((direction.x == 1&& !mouseDown) || (direction.x == -1 && mouseDown))
		 		arrayPosition.x = grid_position_min.x - 1;
		} else if (direction.y != 0) {// vertical road
			arrayPosition.x = grid_position_min.x + (int) (GetRoadWidth() / 2);

			if ((direction.y == -1 && !mouseDown) || (direction.y == 1 && mouseDown))
				arrayPosition.y = grid_position_max.y + 1;
			else if ((direction.y == 1 && !mouseDown) || (direction.y == -1 && mouseDown))
				arrayPosition.y = grid_position_min.y - 1;
		}

		//print ("snap grid cell = " + arrayPosition);
		return GridHelper.GetWorldPosition3(arrayPosition);
		//return GridHelper.GetGridCell(returnVector.x, returnVector.y);
	}


	public override Vector2[] GetWaypoints(PathfindingObject previousObject, PathfindingObject nextObject)
	{
		return new Vector2[0];;
	}
}
