using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingPathfinding : MonoBehaviour {

/**
*	Create path
*/

public static bool debugging = false;

public static Vector2[] CreatePath (InternalBuildingNode startObject, InternalBuildingNode endObject) {

	Dictionary<int,InternalBuildingNode> openDictionaryList = new Dictionary<int,InternalBuildingNode>();
	Dictionary<int,InternalBuildingNode> closedDictionaryList = new Dictionary<int,InternalBuildingNode>();
	
	InternalBuildingNode curObject = startObject;
	InternalBuildingNode[] adjacentObjects; 

	// FIRST step, only one time ------------------------------------------------------------------------------------------------------//
	if(closedDictionaryList.Count == 0) {
		
		closedDictionaryList.Add(curObject.GetID(), curObject);		

		// get adjacent roads
		adjacentObjects = ReturnAdjacentNodes(curObject, closedDictionaryList, openDictionaryList);

		// add to open list
		for (var i = 0; i < adjacentObjects.Length; i++) {
			if (adjacentObjects[i] != null) {	
								
				// add all to open list
				openDictionaryList[adjacentObjects[i].GetID()] = adjacentObjects[i];
				// parent to start point
				adjacentObjects[i].SetFakeParent(curObject);
			}
				
	
		}
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
	bool searchingFailure = false;
	int loopCount = 0;	
	
	// keep looping while curernt cell is not last one
	while(loopingEnabled && !searchingFailure) {
		if (debugging) {
		print ("openList size: " + openDictionaryList.Count);
		print ("closedList size: " + closedDictionaryList.Count);
	}

		// calculate the lowest F value and return corresponding gridCell
		curObject = GetLowestF(endObject, openDictionaryList);

		if (curObject == null)
		{
			searchingFailure = true;
			break;
		}

		if (curObject.GetID() != 0);
			if (closedDictionaryList.ContainsKey(curObject.GetID())) {
				// don't add
			} else {
				// add
				closedDictionaryList.Add(curObject.GetID(), curObject);
				// remove from open list
				openDictionaryList.Remove(curObject.GetID());
			}

		// get adjecent cells
		adjacentObjects = ReturnAdjacentNodes(curObject, closedDictionaryList, openDictionaryList);
		
		// add to open list
		for (int j = 0; j < 8; j++) {
//			if (adjacentCells[j]) {	 NEEED TO DO NULL CHEEECK
				
				try {
				// add all to open list
				openDictionaryList[adjacentObjects[j].GetID()] = adjacentObjects[j];
				
				// parent to current cell
				adjacentObjects[j].SetFakeParent(curObject);
				
				} catch { }
				
//			}
		}
		
		// see if we can leave loop ?? 
		if (curObject.GetID() == endObject.GetID()) {
			loopingEnabled = false;
		}  
		
		if (loopCount >= 10000) {
			// can't find path
			print("pathfinding can't find path");
			loopingEnabled = false;
			searchingFailure = true;
						
			return null;
		}
		
//		print ("openList size: " + openDictionaryList.Count);
//		print ("closedList size: " + closedDictionaryList.Count);	
			
		loopCount++;
	
	}
    
    if (searchingFailure)
    {
    	print("search failure");
    	ResetPathfinding (openDictionaryList, closedDictionaryList);
    	return null;
    } else {
    	//print("search sucess");
    	Vector2[] vectorPath = GetPath(endObject);
    	ResetPathfinding (openDictionaryList, closedDictionaryList);
    	return vectorPath;
    }
   

}
	static Vector2[] GetPath(InternalBuildingNode endObject) 
	{
		InternalBuildingNode currentObject = endObject;
    	Vector2[] returnPath = new Vector2[GetPathLength(endObject)];
    	int index = 0;

    	while(currentObject.HasFakeParent() && index < 10000) {
    		Debug.DrawLine(currentObject.position3, currentObject.GetFakeParent().position3, Color.red, 10f);
    		returnPath[index] = currentObject.position2;
    		currentObject = currentObject.GetFakeParent();
    		//print("index = " + index + " and pos = " + returnPath[index]);
    		index++;
    	}

    	if (index > 9990)
    		print("failed to get path, while loop crash!");


//    	print("index length = " + index + " and array length = " + GetPathLength(endObject));
    	
    	
    	return returnPath;
	}

	static int GetPathLength(InternalBuildingNode endObject) 
	{
		int counter = 0;
		InternalBuildingNode currentObject = endObject;
		while(currentObject.HasFakeParent() && counter < 10000) {
			currentObject = currentObject.GetFakeParent();
			counter++;
		}

		if (counter > 9990)
    		print("failed to get path, while loop crash!");

		return counter;
	}

		
	
	/**
	*	Called by CreatePath to return adjacent cells
	*
	*	Only return cells that are empty are not in 
	*/
	static InternalBuildingNode[] ReturnAdjacentNodes(InternalBuildingNode cur_pf_object, Dictionary<int, InternalBuildingNode> closedList, 
											Dictionary<int, InternalBuildingNode> openList) {

		InternalBuildingNode[] adjacentObjects = cur_pf_object.GetNeighbours();
		InternalBuildingNode[] returnList = new InternalBuildingNode[adjacentObjects.Length];
		int index = 0;
	
		for (var x = 0 ; x < adjacentObjects.Length; x++) {
			try 
			{	// check if in closed list
				if (closedList.ContainsKey(adjacentObjects[x].GetID())) {
					// do nothing
				// check if in open list	
				} else if (openList.ContainsKey(adjacentObjects[x].GetID())) {
					// check if g cost can be lower
					// take  g with original parent, save old parent
					float originalG = CalculateG(adjacentObjects[x]);
					InternalBuildingNode oldParent = adjacentObjects[x].GetFakeParent();
					
					// change parent to check if new one works better
					adjacentObjects[x].SetFakeParent(cur_pf_object);
					
					if (originalG < CalculateG(adjacentObjects[x])) {	// if cost of G is higher after switching parent
						adjacentObjects[x].SetFakeParent(oldParent);			// then switch it back
					}
				
				// otherwise add to returnable list
				} else {
					returnList[index] = adjacentObjects[x];
					index++;
				}
			} catch { }
		}
		
		return returnList;
	}
	
	
	
	public static float HORIZONTAL_VERTICAL_COST = 10;
	public static float DIAGONAL_COST = 14;
	
	static float CalculateG(InternalBuildingNode curObject) {
		float newG = 0;	
		
		if (!curObject.HasFakeParent()) { // gridCell.ultimateParent 
			
			print("problems!!!");
			return -999;
	
		// if there is a parent
		} else {
			
			// check if connection is straight or diagonal (ID's because of performance issues)
			newG = (HORIZONTAL_VERTICAL_COST * curObject.GetGCostMultiplier())+ curObject.GetFakeParent().GetGCost();
			
			curObject.SetGCost(newG);
			return newG;
					
		}
			
	}

	/**
	*	Called by CreatePath to calculate H value for given gridCell
	*
	*/
	static float CalculateH (InternalBuildingNode curObject, InternalBuildingNode endObject) {
		
		Vector2 curPos, endPos;
						
		curPos = curObject.position2; // THIS SHOULD probably be array position
		endPos = endObject.position2; // THIS SHOULD probably be array position
			
		Vector2 calculateHDistance = new Vector2( Mathf.Abs(curPos.x - endPos.x), Mathf.Abs(curPos.y - endPos.y) );
		//Vector2 calculateHDistance = Vector2.Distance(curPos, endPos.y);

//		float newH = ((calculateHDistance.x / grid.sizeGridX) * HORIZONTAL_VERTICAL_COST) + ((calculateHDistance.y / grid.sizeGridX) * HORIZONTAL_VERTICAL_COST); MIGHT NEEED THIS!!!!!!!!!
		float newH = (calculateHDistance.x * HORIZONTAL_VERTICAL_COST) + ( calculateHDistance.y * HORIZONTAL_VERTICAL_COST);
		curObject.SetHCost(newH);
		return newH;
		
	}

	static InternalBuildingNode GetLowestF(InternalBuildingNode endObject, Dictionary<int, InternalBuildingNode> openList) {
			
		InternalBuildingNode lowestFObject = null;
		float lowestF = -1f;
		
		// calculate G, H and thus F for all openList gridCells
		for(var obj = openList.GetEnumerator(); obj.MoveNext();) {
			// first one
			if (lowestF == -1f) {
				lowestF = CalculateG (obj.Current.Value) + CalculateH(obj.Current.Value, endObject);
				lowestFObject = obj.Current.Value;
			} 
			else if(CalculateG(obj.Current.Value) + CalculateH(obj.Current.Value, endObject) < lowestF) {
				lowestF = CalculateG(obj.Current.Value) + CalculateH(obj.Current.Value, endObject);
				lowestFObject = obj.Current.Value;
			} 
		}
		
		return lowestFObject;
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
	static Vector2[] GeneratePath (InternalBuildingNode endObject, Dictionary<int, InternalBuildingNode> openList) 
	{
		int index = 0;
		InternalBuildingNode curObject = endObject;
		bool loopEnabled = true;
		Vector3[] waypoints = new Vector3[1024];

		// add position of first cell, then look if that cell has a fakeParent
		// if so, increase index and add in next loop to list
		// otherwise, stop loop
		while (loopEnabled)
		{
			//print ("index = " + index + " and size = " + openList.Count);
			waypoints[index] = curObject.position3;
			
			if (curObject.HasFakeParent())
			{
				curObject = curObject.GetFakeParent();
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
		Vector2[] return_waypoints = new Vector2[index + 1];
		//return_waypoints [0] = new Vector2(endPosition.x, endPosition.z);

		for (int i = 0; i < return_waypoints.Length; i++)
		{
			return_waypoints[i] = new Vector2(waypoints[i].x, waypoints[i].z);

		}


		if (debugging) {
			//DrawPath(return_waypoints, 0.2f);
		}
	
		return return_waypoints;

	}

	static PathfindingObject[] GenerateRoadPath (PathfindingObject endObject, Dictionary<int, PathfindingObject> openList) 
	{
		int index = 0;
		PathfindingObject curObject = endObject;
		bool loopEnabled = true;
		PathfindingObject[] waypoints = new PathfindingObject[1024];

		// add position of first cell, then look if that cell has a fakeParent
		// if so, increase index and add in next loop to list
		// otherwise, stop loop
		while (loopEnabled)
		{
			//print ("index = " + index + " and size = " + openList.Count);
			waypoints[index] = curObject;
			
			if (curObject.HasFakeParent())
			{
				curObject = curObject.GetFakeParent();
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
		PathfindingObject[] return_waypoints = new PathfindingObject[index + 1];
		for (int i = 0; i < return_waypoints.Length; i++)
		{
			return_waypoints[i] = waypoints[i];
		}
	


		// now create new list with all the Vector3 waypoints
		Vector2[] position_waypoints = new Vector2[return_waypoints.Length * 2];
		int vectorIndex = 0;
		for (index = 0; index < return_waypoints.Length; index++) {
			
			Vector2[] temp_waypoints;

			// if not first or last
			if (index != 0 && index != (return_waypoints.Length - 1))
				temp_waypoints = return_waypoints[index].GetWaypoints(return_waypoints[index - 1], return_waypoints[index + 1]);
			else { // if first or last index
				temp_waypoints = new Vector2[1];
				temp_waypoints[0] = return_waypoints[index].GetWaypoint();
			}

			// loop list from pathfindingObject and add to list
			for (int k = 0; k < temp_waypoints.Length; k++) {
				position_waypoints[vectorIndex] = temp_waypoints[k];
				vectorIndex++;
			}

		}

		position_waypoints = Helper.ResizeArray(position_waypoints, vectorIndex);


		if (debugging) {
			DrawPath(position_waypoints, 0.2f);
		}
	
		return return_waypoints;

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
            //print("index = " + i + " and position = " + waypoints[i]);
        }
    }



	static void ResetPathfinding(Dictionary<int,InternalBuildingNode> openDictionaryList, 
								 Dictionary<int,InternalBuildingNode> closedDictionaryList) {

        foreach(KeyValuePair<int, InternalBuildingNode> entry in openDictionaryList)
        {
            entry.Value.SetFakeParent(null); 
        }

		foreach(KeyValuePair<int, InternalBuildingNode> entry in closedDictionaryList)
		{
			entry.Value.SetFakeParent(null); 
		}
		
	}
}
