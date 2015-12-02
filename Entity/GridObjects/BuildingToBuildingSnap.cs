using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingToBuildingSnap : MonoBehaviour {

	public GridObject currentObject;
	public string snapToObjectType;
	public int searchCellRange;

	// for caching
	GridObject snapToObject = null;
	List<Vector3> savedSnapSpots = new List<Vector3>();



	public GridObject GetSnapObject()
	{
		return snapToObject;
	}

	/*
	 *	Get new snap position from mousePos
	 */
	public Vector3 GetNewPosition(Vector3 mousePos) {

		// get searchList
		List<Vector3> searchList = GetSnapSpots(mousePos);

		float shortestDistance = 99999f;
		Vector3 returnPos = mousePos;

		foreach (Vector3 pos in searchList) {
			if (Vector3.Distance(pos, mousePos) < shortestDistance) {
				returnPos = pos;
				shortestDistance = Vector3.Distance(pos, mousePos);
			}
		}

		if (returnPos == mousePos) {
			return GridHelper.GetWorldPosition3(GridHelper.GetGridPosition (mousePos));
		}

		return returnPos;
	}

	/*
	 *	Get all the possible new snap positions
	 */
	public List<Vector3> GetSnapSpots(Vector3 mousePos)
	{
		List<Vector3> returnList = new List<Vector3>();	
		// first get closest snapToObject, because we just have a type
		GridObject foundSnapToObject = BuildingList.GetClosestBuildingOfType(snapToObjectType, mousePos);

		// if there are no buildings of that type, return null;
		if (foundSnapToObject == null) {
			return returnList;
		}
		// if the object closest is same we found before, return saved list
		else if  (foundSnapToObject == snapToObject) {
			return savedSnapSpots;
		}
		// otherwise it's a new building, or first time using this one
		else {
			snapToObject = foundSnapToObject;
		}

		// entire search area 
		// <------------------->
		//
		//			------------
		//			|  snapTo  |
		//			|  Object  |
		//	|		------------
		//	|		
		//	|	
		//	<------>
		//		=
		// searchCellRange
		
		for (int bottomLeftX = (int) snapToObject.GetGridPositionMin().x - searchCellRange; bottomLeftX <= snapToObject.GetGridPositionMax().x + searchCellRange + 1; bottomLeftX++) {
			for (int bottomLeftY = (int) snapToObject.GetGridPositionMin().y - searchCellRange; bottomLeftY <= snapToObject.GetGridPositionMax().y + searchCellRange + 1; bottomLeftY++) {
				currentObject.UpdatePositionAndSizeManual(bottomLeftX, bottomLeftY);
				if (GridHelper.CanBuildGridObject(currentObject))
				{

					Vector2 arrayPos = new Vector2(bottomLeftX + (currentObject.GetSize().x / 2f), bottomLeftY + (currentObject.GetSize().y / 2f));

					if (Vector3.Distance(GridHelper.GetWorldPosition3(arrayPos), snapToObject.GetPositionCenter()) < 
										searchCellRange * GridHelper.GetGridCellSize()) {
						returnList.Add(GridHelper.GetGridCell(arrayPos).worldPosition3);
						//print("can build at = " + bottomLeftX + ", " + bottomLeftY);

					} else {
						//print("too far away " + bottomLeftX + ", " + bottomLeftY + "!");
					}
				} else {
					//print("can't build at " + bottomLeftX + ", " + bottomLeftY + "!");
				}

			}
		}

		// save list so we don't have to make another list when we're still closest to same building
		savedSnapSpots = returnList;
		return returnList;

	}



}
