using UnityEngine;
using System.Collections;

public class GridCellPathfinding {

	GridCell fakeParent;
	public float hCost; 
	public float gCost;
	GridCell gridCell;

	public GridCellPathfinding (GridCell _gridCell)
	{
		gridCell = _gridCell;
	}

	public GridCell[] GetNeighbours ()
	{
		GridCell[] returnList = new GridCell[8];
		int index = 0;
		
		for (int x = (int)gridCell.gridPosition.x - 1; x <= gridCell.gridPosition.x + 1; x++)
		{
			for (int y = (int)gridCell.gridPosition.y - 1; y <= gridCell.gridPosition.y + 1; y++)
			{
				if (x == gridCell.gridPosition.x && y == gridCell.gridPosition.y) { } // this cell, SKIP
				else if (IsWalkable())
				{	
					returnList[index] = GridHelper.GetGridCell(x, y);

					/*
					if (returnList[index] == null) // if it's null, leave index the same
						index--;
					
					index++;*/

					if (returnList[index] != null) // if it's null, leave index the same
						index++;
					
				}
				else
				{
					//Debug.Log("not adding anything here!!!");
				}
			}
		}
		
		return returnList;
	}
	
	public bool HasFakeParent()
	{
		if (fakeParent == null)
			return false;
		else
			return true;
	}
	
	public void SetFakeParent(GridCell grid_cell)
	{
		fakeParent = grid_cell;
	}
	
	public GridCell GetFakeParent()
	{
		return fakeParent;
	}
	
	public bool IsFakeParentHorizontal ()
	{
		if (this.HasFakeParent())
			if (gridCell.gridPosition.x != fakeParent.gridPosition.x &&
			    gridCell.gridPosition.y != fakeParent.gridPosition.y)
				return false;
		else
			return true;		
		return false;
	}

	public bool IsWalkable()
	{
		if (gridCell.areaBuilding && !gridCell.building)
			return true;
		else if (gridCell.areaBuilding && gridCell.building is Entrance)
			return true;
		else 
			return false;

	}

}
