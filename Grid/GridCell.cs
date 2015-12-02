using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridCell {
	
	public Vector2 gridPosition;
	public int itemID;

	public AreaBuilding areaBuilding = null;
	public Building building = null; // structure inside of area building
	public GridObject gridObject = null; // outside, road or parking spot etc.
	
	public GridCellPathfinding gridCellPathfinding;

	public Vector2 worldPosition {
		get { return GridHelper.GetWorldPosition2(gridPosition); }
	}

	public Vector3 worldPosition3 {
		get { return GridHelper.GetWorldPosition3(gridPosition); }
	}
	
	string gridCellName { get {return gridPosition.x.ToString () + gridPosition.y.ToString() ;}}
	
	public GridCell (){
		Load();
	}
	
	public GridCell (Vector2 gridPos, int gridCellCounter)
	{
		gridPosition = gridPos;
		//gridCellSize = cellSize;
		itemID = gridCellCounter;

		gridCellPathfinding = new GridCellPathfinding(this);
	}

	public AreaBuilding GetAreaBuilding()
	{
		return areaBuilding;
	}

	public Road GetRoad()
	{
		if (gridObject != null)
			if (gridObject is Road)
				return (Road) gridObject;

		return null;
	}

	/*
	public ParkingSpot GetParkingSpot()
	{
		if (gridObject != null)
			if (gridObject is ParkingSpot)
				return (ParkingSpot) gridObject;

		return null;
	}*/

	/*
	 * gridobject is always outside 'building'
	*/
	public PathfindingObject GetPathfindingObject()
	{
		if (gridObject != null) {
			if (gridObject is Road)
				return null;
			else if (gridObject is PathfindingObject)
				return (PathfindingObject) gridObject;
		}

		return null;
	}

	/**
	 * Standard save method
	 */
	public void Save () 
	{
		//ES2.Save(gridPosition, "" + SaveLoadController.fileName + "?tag=" + this.gridCellName + "gridGroundPlaneSizeX");	
		//ES2.Save(itemID, "" + SaveLoadController.fileName + "?tag=" + this.gridCellName + "griditemID");

	}
	
	/**
	 * Standard load method
	 */
	public void Load ()
	{
		//gridPosition = ES2.Load<Vector2>("" + SaveLoadController.fileName + "?tag=" + this.gridCellName + "gridGroundPlaneSizeX");		
		//itemID = ES2.Load<int>("" + SaveLoadController.fileName + "?tag=" + this.gridCellName + "griditemID");
	}
	
	public override string ToString () {
		return "showing cell: " + gridPosition;	
	}

    public void ShowCell()
    {
        Debug.Log(ToString());
    }

    public void DrawCell()
    {
        DrawCell(Color.green);
    }

    public void DrawCell(Color color)
    {
        Vector3 position = GridHelper.GetWorldPosition3(gridPosition);
        float delta = ((float)Grid.GridCellSize * 0.9f) / 2f;

        // clock wise from top right, so top right to bottom right etc.
        Debug.DrawLine(new Vector3(position.x + delta, position.y, position.z + delta), new Vector3(position.x + delta, position.y, position.z - delta), color, 10);
        Debug.DrawLine(new Vector3(position.x + delta, position.y, position.z - delta), new Vector3(position.x - delta, position.y, position.z - delta), color, 10);
        Debug.DrawLine(new Vector3(position.x - delta, position.y, position.z - delta), new Vector3(position.x - delta, position.y, position.z + delta), color, 10);
        Debug.DrawLine(new Vector3(position.x - delta, position.y, position.z + delta), new Vector3(position.x + delta, position.y, position.z + delta), color, 10);
    }


	public bool CanBuildGridObject (GridObject grid_object)
	{
		if (grid_object is AreaBuildingFactory || grid_object is AreaBuilding) {
			if (areaBuilding == null)
				return true; 
			else
				return false;
		} else if (grid_object is Building) {
			if (areaBuilding != null) {
				Building compare_building = (Building) grid_object;
				if (compare_building.AreaBuildingTypeMatch(areaBuilding.GetBuildingType()) && building == null) {
					return true;
				}
				//Debug.Log("fail 2");
			}
			//Debug.Log("fail 3");

			return false;
		} else if (grid_object is Road) {
			if (areaBuilding == null && gridObject == null) {
				return true;
			} else { 
				return false;
			}
		} else if (grid_object is GridObject) {
			if (areaBuilding == null && gridObject == null) {
				return true;
			} else { 
				return false;
			}
		}

		return false;
	}

	/*
	 *	Give an override option when replacing one road with another
	 */
	public void BuildGridObject (GridObject grid_object, bool ignoreBuildings)
	{
		if (!CanBuildGridObject(grid_object) && !ignoreBuildings)
			Debug.Log ("CRITICAL ERROR - should not be getting to this point");
		else if (grid_object is AreaBuilding)
			areaBuilding = (AreaBuilding) grid_object;
		else if (grid_object is Building)
			building = (Building) grid_object;
		else if (grid_object is GridObject) // for example road
			gridObject = grid_object;
		else
			Debug.Log ("Object not recognized");

	}

	public void BuildGridObject (GridObject grid_object)
	{
		BuildGridObject (grid_object, false);
	}

	/* test if this gridcell is a wall of an area building
	 *	Used to snap buildings to walls of area biulding
	 */
	public bool IsWall()
	{	
		// first test if ;
		if (gridPosition.x > 0 && gridPosition.y > 0 &&
			gridPosition.x < Grid.GridCellsX && gridPosition.x < Grid.GridCellsY)
		{
				int count = 0;
				if (GridHelper.GetGridCell((int)gridPosition.x - 1,(int)gridPosition.y).GetAreaBuilding() != null) 
					count ++;
				if (Grid.gridArray[(int)gridPosition.x + 1,(int)gridPosition.y].GetAreaBuilding() != null) 
					count ++;
				if (Grid.gridArray[(int)gridPosition.x,(int)gridPosition.y - 1].GetAreaBuilding() != null) 
					count ++;
				if (Grid.gridArray[(int)gridPosition.x,(int)gridPosition.y + 1].GetAreaBuilding() != null) 
					count ++;
				
				if (count == 2 || count == 3)
					return true;

		}
		

		return false;
	}


}
