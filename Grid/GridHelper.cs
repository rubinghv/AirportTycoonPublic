using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GridHelper : MonoBehaviour {

	public static float GetGridCellSize()
	{
		return Grid.GridCellSize;
	}
 	
	/**
	 * 	Get the real world center position from the array position
	 */ 
	public static Vector2 GetWorldPosition2 (Vector2 arrayPos) {	
		return new Vector2( (arrayPos.x * ((float) Grid.GroundPlaneSizeX / Grid.GridCellsX)) - (Grid.GroundPlaneSizeX / 2) + (Grid.GridCellSize / 2),
		                   (arrayPos.y * ((float) Grid.GroundPlaneSizeY / Grid.GridCellsY)) - (Grid.GroundPlaneSizeY / 2) + (Grid.GridCellSize / 2));
	}

	public static Vector3 GetWorldPosition3 (Vector2 arrayPos) {	
		return GetWorldPosition3 (arrayPos, 0);
	}

    public static Vector3 GetWorldPosition3(Vector2 arrayPos, float height)
    {
		return new Vector3((arrayPos.x * ((float) Grid.GroundPlaneSizeX / Grid.GridCellsX)) - (Grid.GroundPlaneSizeX / 2) + (Grid.GridCellSize / 2), height,
		                   (arrayPos.y * ((float) Grid.GroundPlaneSizeY / Grid.GridCellsY)) - (Grid.GroundPlaneSizeY / 2) + (Grid.GridCellSize / 2));
    }
	
	public static Vector2 GetGridPosition (Vector3 position) {
		return new Vector2(GetGridPositionX(position), GetGridPositionY(position));
	}
	
	private static int GetGridPositionX (Vector3 position) {
		//return (int) ((position.x + (Grid.GroundPlaneSizeX / 2) - (Grid.GridCellSize / 2))/ (Grid.GroundPlaneSizeX / Grid.GridCellsX));
		//Debug.Log (position.x + " + " + Grid.GroundPlaneSizeX / 2 + " / " + ((float) Grid.GroundPlaneSizeX / Grid.GridCellsX));
		return (int) ((position.x + (Grid.GroundPlaneSizeX / 2)) / ((float) Grid.GroundPlaneSizeX / Grid.GridCellsX));
	}
	
	private static int GetGridPositionY (Vector3 position) {
		//return (int) ((position.z + (Grid.GroundPlaneSizeY / 2) - (Grid.GridCellSize / 2))/ (Grid.GroundPlaneSizeY / Grid.GridCellsY));
		return (int) ((position.z + (Grid.GroundPlaneSizeY / 2))/ ((float) Grid.GroundPlaneSizeY / Grid.GridCellsY));

	}
		

    public static GridCell GetGridCell(Vector3 position)
    {
        try
        {
            return Grid.gridArray[GetGridPositionX(position), GetGridPositionY(position)];
        }
        catch (Exception e)
        {
            return null;
        }

    }
	
	public static GridCell GetGridCell (int array_x, int array_y) {
		if (array_x >= 0 && array_x < Grid.GridCellsX && array_y >= 0 && array_y < Grid.GridCellsY)
			return Grid.gridArray[array_x, array_y];
		else
			return null;
	}

	public static GridCell GetGridCell (float array_x, float array_y) {
		return GetGridCell((int)array_x, (int) array_y);
	}

	public static GridCell GetGridCell (Vector2 array_pos) {
		return GetGridCell(array_pos.x, array_pos.y);
	}
	
	public static bool IsArrayPositionValid (int x, int y)
	{
		if (x >= 0 && x < Grid.GridCellsX && y >= 0 && y < Grid.GridCellsY)
			return true;
		else
			return false;
	}

	public static void BuildGridObject(GridObject gridObject)
	{
		// get arrayPos of bottomLeft
		BuildGridObject(gridObject, false);
	}

	public static void BuildGridObject(GridObject gridObject, bool ignoreBuildings)
	{
		// get arrayPos of bottomLeft
		int loopX = (int) gridObject.GetGridPositionMin().x;
		int loopY = (int) gridObject.GetGridPositionMin().y;
		for (int x = loopX; x < loopX + gridObject.GetSize ().x; x++) {
			for (int y = loopY; y < loopY + gridObject.GetSize ().y; y++) {
				GetGridCell (x, y).BuildGridObject(gridObject, ignoreBuildings);
			}
		}
	}

	public static bool CanBuildGridObject(GridObject grid_object)
	{   
		return CanBuildGridObject(grid_object, Vector2.zero);
	}

	public static bool CanBuildGridObject(GridObject grid_object, Vector2 wallSnap)
	{        
		// get arrayPos of bottomLeft
		int loopX = (int) grid_object.GetGridPositionMin().x;
		int loopY = (int) grid_object.GetGridPositionMin().y;
		
		if (grid_object.GetGridPositionMin().x < -(Grid.GroundPlaneSizeX / 2) || grid_object.GetGridPositionMin().y < -(Grid.GroundPlaneSizeY / 2))
			return false;
		
		//print("bottomLeft arrayPos = (" + loopX + ", " + loopY + ")");
		GridCell gridCell;

		for (int x = loopX; x < loopX + grid_object.GetSize().x; x++) {
			for (int y = loopY; y < loopY + grid_object.GetSize().y; y++) {
				
				gridCell = GetGridCell(x, y);
				
				if (gridCell != null) {

					if (!gridCell.CanBuildGridObject(grid_object)) {
						//Debug.Log("Fails on: " + gridCell.ToString());
						return false;
					}

					// now also test for wall snap, incase building is supposed to attach to wall
					if (wallSnap != Vector2.zero) {
						// test if this snap is happening along the x axis (so y is stable)
						if (wallSnap.y != 0 && y == grid_object.GetWallSnapY() ) {
							// if so, make sure this is wall, otherwise return false;
							if(!gridCell.IsWall())
								 return false;
						} 
						if (wallSnap.x != 0 && x == grid_object.GetWallSnapX()) {
							// if so, make sure this is wall, otherwise return false;
							if(!gridCell.IsWall())
								 return false;
						}
					} 

				} else // invalid cell so don't build
					return false;
			}
		}
		return true;
	}



    public static Vector2 GetArrayFloat(Vector3 position)
    {
		return new Vector2(((position.x + (Grid.GroundPlaneSizeX / 2)) / Grid.GridCellSize),
		                   ((position.z + (Grid.GroundPlaneSizeY / 2)) / Grid.GridCellSize));
	}
	

}

