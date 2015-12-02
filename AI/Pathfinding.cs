using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

/**
*	Create path
*/

public static bool debugging;

public static Vector2[] CreatePath (Vector3 startPosition, Vector3 endPosition) {
	debugging = false;

    // FEATURE TO IMPLEMENT - IMPORTANT
    /**
     * At the beginning we need to look for four gridCells closest to startPosition, as well as endPosition
     * After we have found the optimal route, connect the second and second to last nodes to these closest position 
     * (as not to make the walk have strange corners at the end)
     * 
     */


	Dictionary<int,GridCell> openDictionaryList = new Dictionary<int,GridCell>();
	Dictionary<int,GridCell> closedDictionaryList = new Dictionary<int,GridCell>();
	
	GridCell endCell = GridHelper.GetGridCell(endPosition);
	GridCell currentCell = GridHelper.GetGridCell(startPosition);

	if (currentCell.itemID == endCell.itemID) {
		return null;
	}

	GridCell[] adjacentCells; 

	// FIRST step, only one time ------------------------------------------------------------------------------------------------------//
	if(closedDictionaryList.Count == 0) {
//		startNode.fakeParent = null;
		
		closedDictionaryList.Add(currentCell.itemID, currentCell);		

		// get adjacent cells
		adjacentCells = ReturnAdjacentNodes(currentCell, closedDictionaryList, openDictionaryList);
		
		// add to open list
		for (var i = 0; i < 8; i++) {
			if (adjacentCells[i] != null) {	// NEED TO DO NULL CHEECKew
								
				// add all to open list
				openDictionaryList[adjacentCells[i].itemID] = adjacentCells[i];

				// parent to start point
				adjacentCells[i].gridCellPathfinding.SetFakeParent(currentCell);
			}
				
	
		}
	} 
	
	if (debugging) {
		print ("openList size: " + openDictionaryList.Count);
		print ("closedList size: " + closedDictionaryList.Count);
	}

		// SECOND step, look for lowest F value in openList --------------------------------------------------------------------------------//
	// 1. Add gridCell this to closedList and remove from closedList.
	// 2. Get cells adjacent to lowest F value 
	//		* don't include ones in closed list. 
	//		* Check if they're already on open list, if so:
	//			o don't include them but check if their G cost can be lower with currentCell with lowest F value) as parent.
	// 		* Get adjacent cells that remain and add them to openList.
	// 3. Then start all over again if we've not reached the target yet.
	
	bool loopingEnabled = true;
	int loopCount = 0;	
	
	// keep looping while curernt cell is not last one
	while(loopingEnabled) {
		
		// calculate the lowest F value and return corresponding gridCell
		currentCell = GetLowestF(endCell, openDictionaryList);
		
		if (currentCell == null) {
			return null;
			print("failed to find path!");
		}

		if (currentCell.itemID != 0);
			if (closedDictionaryList.ContainsKey(currentCell.itemID)) {
				// don't add
			} else {
				// add
				closedDictionaryList.Add(currentCell.itemID, currentCell);
				// remove from open list
				openDictionaryList.Remove(currentCell.itemID);
			}

		// get adjecent cells
		adjacentCells = ReturnAdjacentNodes(currentCell, closedDictionaryList, openDictionaryList);
		
		// add to open list
		for (int j = 0; j < 8; j++) {
//			if (adjacentCells[j]) {	 NEEED TO DO NULL CHEEECK
				
				try {
				// add all to open list
				openDictionaryList[adjacentCells[j].itemID] = adjacentCells[j];
				
				// parent to current cell
				adjacentCells[j].gridCellPathfinding.SetFakeParent(currentCell);
				
				} catch { }
				
//			}
		}
		
		// see if we can leave loop ?? 
		if (currentCell.itemID == endCell.itemID) {
			loopingEnabled = false;
		}  
		
		if (loopCount >= 10000) {
			// can't find path
			print("pathfinding can't find path");
			loopingEnabled = false;
						
			return null;
		}
		
//		print ("openList size: " + openDictionaryList.Count);
//		print ("closedList size: " + closedDictionaryList.Count);	
			
		loopCount++;
	
	}
    

    /**
     * ---------------------- FOR DEBUGGING PURPOSES  -------------------------
     */

	if (debugging) {
	    foreach (KeyValuePair<int, GridCell> gridCell in openDictionaryList)
	    {
	        // do something with entry.Value or entry.Key
	        gridCell.Value.DrawCell(Color.green);
	    }

	    foreach (KeyValuePair<int, GridCell> gc in closedDictionaryList)
	    {
	        // do something with entry.Value or entry.Key
	        gc.Value.DrawCell(Color.red);
	    }
	}

	
    Vector2[] returnPath = GeneratePath(startPosition, endPosition, currentCell, openDictionaryList);
	ResetPathfinding (openDictionaryList, closedDictionaryList);
	return returnPath;


	}
	
	
	
	
	
	
	/**
	*	Called by CreatePath to return adjacent cells
	*
	*	Only return cells that are empty are not in 
	*/
	static GridCell[] ReturnAdjacentNodes(GridCell currentCell, Dictionary<int, GridCell> closedList, Dictionary<int, GridCell> openList) {

		GridCell[] adjacentCells  = currentCell.gridCellPathfinding.GetNeighbours();
		GridCell[] returnList = new GridCell[adjacentCells.Length];
		int index = 0;
	
		for (var x = 0 ; x < adjacentCells.Length; x++) {
			try 
			{	// check if in closed list
				if (closedList.ContainsKey(adjacentCells[x].itemID)) {
					// do nothing
				// check if in open list	
				} else if (openList.ContainsKey(adjacentCells[x].itemID)) {
					// check if g cost can be lower
					// take  g with original parent, save old parent
					float originalG = CalculateG(adjacentCells[x]);
					GridCell oldParent = adjacentCells[x].gridCellPathfinding.GetFakeParent();
					
					// change parent to check if new one works better
					adjacentCells[x].gridCellPathfinding.SetFakeParent(currentCell);
					
					if (originalG < CalculateG(adjacentCells[x])) {	// if cost of G is higher after switching parent
						adjacentCells[x].gridCellPathfinding.SetFakeParent(oldParent);			// then switch it back
					}
				
				// otherwise add to returnable list
				} else {
					returnList[index] = adjacentCells[x];
					index++;
				}
			} catch { }
		}
		
		return returnList;
	}
	
	
	
	public static float HORIZONTAL_VERTICAL_COST = 10;
	public static float DIAGONAL_COST = 14;
	
	static float CalculateG(GridCell curCell) {
		float newG;	
		
		if (!curCell.gridCellPathfinding.HasFakeParent()) { // gridCell.ultimateParent 
			
			print("problems!!!");
			return -999;
	
		// if there is a parent
		} else {
			
			// check if connection is straight or diagonal (ID's because of performance issues)
			if (curCell.gridCellPathfinding.IsFakeParentHorizontal()) 
				newG = HORIZONTAL_VERTICAL_COST + curCell.gridCellPathfinding.GetFakeParent().gridCellPathfinding.gCost;
			else 
				newG = DIAGONAL_COST + curCell.gridCellPathfinding.GetFakeParent().gridCellPathfinding.gCost;
			
			curCell.gridCellPathfinding.gCost = newG;
			return newG;
					
		}
			
	}

	/**
	*	Called by CreatePath to calculate H value for given gridCell
	*
	*/
	static float CalculateH (GridCell curCell, GridCell endCell) {
		
		Vector2 curCellPos, endCellPos;
			
	//	if (curCell.hCost != -1) {
	//		// we have already calculated it			// THIS MIGHT HAVE BEEN CAUSING ERRORS
	//		return curCell.hCost;
	//	} else {
			// first check if smallNode
	
			
		curCellPos = curCell.worldPosition; // THIS SHOULD probably be array position
		endCellPos = endCell.worldPosition; // THIS SHOULD probably be array position
			
		Vector2 calculateHDistance = new Vector2( Mathf.Abs(curCellPos.x - endCellPos.x), Mathf.Abs(curCellPos.y - endCellPos.y) );
		
//		float newH = ((calculateHDistance.x / grid.sizeGridX) * HORIZONTAL_VERTICAL_COST) + ((calculateHDistance.y / grid.sizeGridX) * HORIZONTAL_VERTICAL_COST); MIGHT NEEED THIS!!!!!!!!!
		float newH = ( calculateHDistance.x * HORIZONTAL_VERTICAL_COST) + ( calculateHDistance.y * HORIZONTAL_VERTICAL_COST);
		curCell.gridCellPathfinding.hCost = newH;
		return newH;
		
	}

	static GridCell GetLowestF(GridCell endCell, Dictionary<int, GridCell> openList) {
			
		GridCell lowestFCell = null;
		float lowestF = -1f;
		
		// calculate G, H and thus F for all openList gridCells
		for(var cell = openList.GetEnumerator(); cell.MoveNext();) {
			// first one
			if (lowestF == -1f) {
				lowestF = CalculateG (cell.Current.Value) + CalculateH(cell.Current.Value, endCell);
				lowestFCell = cell.Current.Value;
			} 
			else if(CalculateG(cell.Current.Value) + CalculateH(cell.Current.Value, endCell) < lowestF) {
				lowestF = CalculateG(cell.Current.Value) + CalculateH(cell.Current.Value, endCell);
				lowestFCell = cell.Current.Value;
			} 
		}
		
		return lowestFCell;
	}
//
//
//function GetPathLength(curNode : Node) : int {
//	if (curNode.fakeParent == null) {
//		return 0;
//	} else {
//		return 1 + GetPathLength(curNode.fakeParent);
//	}
//}
//
/**
*	Generic return path function
*/
	static Vector2[] GeneratePath (	Vector3 startPosition, Vector3 endPosition, GridCell endCell, 
	                                Dictionary<int, GridCell> openList) 
	{
		int index = 0;
		GridCell curCell = endCell;
		bool loopEnabled = true;
		Vector3[] waypoints = new Vector3[openList.Count + 1];

		// add position of first cell, then look if that cell has a fakeParent
		// if so, increase index and add in next loop to list
		// otherwise, stop loop
		while (loopEnabled)
		{
			waypoints[index] = GridHelper.GetWorldPosition3(curCell.gridPosition);
			
			if (curCell.gridCellPathfinding.HasFakeParent())
			{
				curCell = curCell.gridCellPathfinding.GetFakeParent();
				index++;
			}
			else
				loopEnabled = false;
			
			// while loop protection
			if (index > 999999)
			{
				loopEnabled = false;
				print("while loop crash");
			}

		}

		// create returnable list with correct size and add end position at start
		Vector2[] return_waypoints = new Vector2[index];
		return_waypoints [0] = new Vector2(endPosition.x, endPosition.z);

		for (int i = 1; i < return_waypoints.Length; i++)
		{
			return_waypoints[i] = new Vector2(waypoints[i].x, waypoints[i].z);

		}


		if (debugging) {
			DrawPath(return_waypoints, 0.3f);
			Debug.DrawLine(new Vector3(return_waypoints[return_waypoints.Length - 1].x, 0.3f, return_waypoints[return_waypoints.Length - 1].y), startPosition, Color.blue, 10); // dont't forget to 
		}
	
		return return_waypoints;

	}


    // don't really need this, only if shit goes bad
    static void DrawPathFakeParent(GridCell gridCell, Dictionary<int, GridCell> openList)
    {

        int index = 0;
        GridCell curCell = gridCell;
        bool loopEnabled = true;
        Vector3[] waypoints = new Vector3[openList.Count + 1];

        print("generatePath openList size: " + openList.Count + 1);

        while (loopEnabled)
        {
			print ("index = " + index + " - position = " + curCell.gridPosition);
            waypoints[index] = GridHelper.GetWorldPosition3(curCell.gridPosition);

            if (curCell.gridCellPathfinding.HasFakeParent())
            {
                curCell = curCell.gridCellPathfinding.GetFakeParent();
                index++;
            }
            else
                loopEnabled = false;

            // while loop protection
            if (index > 999999)
            {
                loopEnabled = false;
                print("while loop crash");
            }


        }


        for (int i = 0; i < index; i++)
        {
            Debug.DrawLine(waypoints[i], waypoints[i + 1], Color.blue, 10);
        }
    }

    static void DrawPath(Vector3[] waypoints)
    {
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Debug.DrawLine(waypoints[i], waypoints[i + 1], Color.blue, 10);
        }
    }

    static void DrawPath(Vector2[] waypoints, float height)
    {
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Debug.DrawLine( new Vector3(waypoints[i].x, height, waypoints[i].y),
                            new Vector3(waypoints[i + 1].x, height, waypoints[i + 1].y), Color.blue, 10);
        }
    }

	static void ResetPathfinding(Dictionary<int,GridCell> openDictionaryList, Dictionary<int,GridCell> closedDictionaryList) {

        foreach(KeyValuePair<int, GridCell> entry in openDictionaryList)
        {
            entry.Value.gridCellPathfinding.SetFakeParent(null); 
        }

		foreach(KeyValuePair<int, GridCell> entry in closedDictionaryList)
		{
			entry.Value.gridCellPathfinding.SetFakeParent(null); 
		}
		
	}
}
