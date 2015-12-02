using UnityEngine;
using System.Collections;

public class PathfindingComponent {

	public GridCell fakeParent;
	public float gCost;
	public float hCost;
	public GridCell gridCell;

	public bool HasFakeParent ()
	{
		try {
			int i = fakeParent.itemID;
		} 
		catch 
		{
			return false;	
		}
		return true;
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
	
	public bool IsWalkable ()
	{
		
		//		
		//		if (IsArrayPositionValid(x, y))
		//			try {
		//				GetGridCell (x, y).entity.name = GetGridCell (x, y).entity.name;
		//			} catch { return true; }
		//		
		return false;
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
				//else if (gridCell.canBuild())
				else if (true)
				{
					//					if (x != gridPosition.x && y != gridPosition.y) // diagonal
					//						if (GridHelper.IsGridCellWalkable((int)gridPosition.x, y) && GridHelper.IsGridCellWalkable(x, (int)arrayPosition.y))
					//						{	
					//							returnList[index] = GridHelper.grid.gridArray[x,y];
					//							index++;
					//						}					NEED FOR NOT CUTTING CORNERS
					//					else
					//					{	
					
					returnList[index] = GridHelper.GetGridCell(x, y);
					
					if (returnList[index] == null) // if it's null, leave index the same
						index--;
					
					index++;
					
				}
				else
				{
					Debug.Log("not adding anything here!!!");
				}
			}
		}
		
		return returnList;
	}

}
