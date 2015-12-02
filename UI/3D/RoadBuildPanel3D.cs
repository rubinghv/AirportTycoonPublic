using UnityEngine;
using System.Collections;

public class RoadBuildPanel3D : MonoBehaviour {

	[Header("GameObjects")]
	public GameObject road_straight_one;
	public GameObject road_straight_two;
	public GameObject road_corner;

	public Transform roadParent;

	[Header("Materials")]
	public Material selectionMaterialValid;
	public Material selectionMaterialInvalid;

	[Header("Parameters")]
	public int roadWidth; // needs to be odd number
	public float cornerSnap;

	// for setup and caching purposes
	GridCell minGridCell = null;
	GridCell maxGridCell = null;
	PlaceRoad placeRoad;
	RoadStraight road_straight_one_component;
	RoadStraight road_straight_two_component;
	RoadCorner road_corner_component;

	/*
	 * Setup when place road is first started
	 */
	public void Setup(PlaceRoad place_road)
	{
		placeRoad = place_road;

		// also grab components
		road_straight_one_component = road_straight_one.GetComponent<RoadStraight>();
		road_straight_two_component = road_straight_two.GetComponent<RoadStraight>();
		road_corner_component = road_corner.GetComponent<RoadCorner>();
	}

	public bool IsRoadStraight(Vector3 mouse_down, Vector3 mouse_up)
	{
		Vector3 temp;
		if (mouse_down.x > mouse_up.x) { temp.x = mouse_down.x; mouse_down.x = mouse_up.x; mouse_up.x = temp.x; } 
		if (mouse_down.z > mouse_up.z) { temp.z = mouse_down.z; mouse_down.z = mouse_up.z; mouse_up.z = temp.z; }
		
		minGridCell = GridHelper.GetGridCell (mouse_down);
		maxGridCell = GridHelper.GetGridCell (mouse_up);

		Vector2 size = new Vector2 (maxGridCell.gridPosition.x - minGridCell.gridPosition.x + 1, 
									maxGridCell.gridPosition.y - minGridCell.gridPosition.y + 1);

		// check if first section is horizontal or vertical
		bool horizontal = false;
		bool vertical = false;
		if (maxGridCell.gridPosition.x - minGridCell.gridPosition.x >= maxGridCell.gridPosition.y - minGridCell.gridPosition.y)
			horizontal = true;
		else
			vertical = true;

		// test if this is a straight road, or one with a corner
		bool straightRoad = false;
		if ((horizontal && (maxGridCell.gridPosition.y - minGridCell.gridPosition.y) <= cornerSnap) ||
			(vertical && (maxGridCell.gridPosition.x - minGridCell.gridPosition.x) <= cornerSnap))
			return true;
		else
			return false;
	}

	/*
	 *	When  
	 *
	 */
	public void ShowPreview(Vector3 mouse_down, Vector3 mouse_up, bool snap)
	{		
		if (snap) {
			// get road we're snapping to 
			Road mouse_up_road = GridHelper.GetGridCell(mouse_up).GetRoad();

			if (IsRoadStraight(mouse_down, mouse_up) && mouse_up_road != null) {
				mouse_up = mouse_up_road.GetSnapGridCell(GetStraightOneDirection(mouse_down, mouse_up), false, mouse_up);
			} else {	
				mouse_up = mouse_up_road.GetSnapGridCell(GetStraightTwoDirection(mouse_down, mouse_up), false, mouse_up);
			}
		} 

		GridCell mouseUpCell = GridHelper.GetGridCell(mouse_up);
		GridCell mouseDownCell = GridHelper.GetGridCell(mouse_down);

		Vector3 temp;
		if (mouse_down.x > mouse_up.x) { temp.x = mouse_down.x; mouse_down.x = mouse_up.x; mouse_up.x = temp.x; } 
		if (mouse_down.z > mouse_up.z) { temp.z = mouse_down.z; mouse_down.z = mouse_up.z; mouse_up.z = temp.z; }
				
		minGridCell = GridHelper.GetGridCell (mouse_down);
		maxGridCell = GridHelper.GetGridCell (mouse_up);
		
		// get gridCell, size, and center for calcuation
		Vector2 size = new Vector2 (maxGridCell.gridPosition.x - minGridCell.gridPosition.x + 1, 
									maxGridCell.gridPosition.y - minGridCell.gridPosition.y + 1);

		// check if first section is horizontal or vertical
		bool horizontal = false;
		bool vertical = false;
		if (maxGridCell.gridPosition.x - minGridCell.gridPosition.x >= maxGridCell.gridPosition.y - minGridCell.gridPosition.y)
			horizontal = true;
		else
			vertical = true;

		// test if this is a straight road, or one with a corner
		bool straightRoad = false;
		if ((horizontal && (maxGridCell.gridPosition.y - minGridCell.gridPosition.y) <= cornerSnap) ||
			(vertical && (maxGridCell.gridPosition.x - minGridCell.gridPosition.x) <= cornerSnap))
			straightRoad = true;

		// do some cool stuff
		if (straightRoad) {
			// disable visiblity of corner preview objects
			road_corner.GetComponent<Renderer>().enabled = false;
			road_straight_two.GetComponent<Renderer>().enabled = false;

			// update road material and preview objects
			UpdateMaterialsStraight();
			UpdateSelectionObjectStraightOne(minGridCell, mouseDownCell, size, horizontal, vertical, !straightRoad);
		} else {
			// update road preview objects
			UpdateMaterialsCorner();
			UpdateSelectionObjectStraightOne(minGridCell, mouseDownCell, size, horizontal, vertical, !straightRoad);
			UpdateSelectionObjectCorner(mouseDownCell, mouseUpCell, horizontal, vertical);
			UpdateSelectionObjectStraightTwo(minGridCell, mouseUpCell, size, horizontal, vertical);

			// enable visibilty
			road_corner.GetComponent<Renderer>().enabled = true;
			road_straight_two.GetComponent<Renderer>().enabled = true;
		}

		if (!this.gameObject.activeSelf)
			NGUITools.SetActive (this.gameObject, true);
	}

	void UpdateSelectionObjectStraightOne(GridCell minGridCell, GridCell mouseDownCell, Vector2 size, 
										  bool horizontal, bool vertical, bool corner)
	{
		Vector3 positionAdd = Vector3.zero;
		if(corner){
			if (horizontal)
				size.x = size.x - ((roadWidth - 1) / 2) - 1;
			else if (vertical)
				size.y = size.y - ((roadWidth - 1) / 2) - 1;
		}

		if (horizontal) {
			road_straight_one.transform.localScale = new Vector3 ((size.x) * GridHelper.GetGridCellSize (), 
			                                                     road_straight_one.transform.localScale.y, 
			                                                     (roadWidth) * GridHelper.GetGridCellSize ());
		} else if (vertical) {
			road_straight_one.transform.localScale = new Vector3 ((roadWidth) * GridHelper.GetGridCellSize (), 
			                                                     road_straight_one.transform.localScale.y, 
			                                                     (size.y) * GridHelper.GetGridCellSize ());
		}

		if (horizontal) {
			road_straight_one.transform.position = new Vector3 (
				GridHelper.GetWorldPosition2 (minGridCell.gridPosition).x + ((size.x * GridHelper.GetGridCellSize ()) / 2.0f) - GridHelper.GetGridCellSize() / 2,
				road_straight_one.transform.position.y,
				mouseDownCell.worldPosition.y);
			if (corner && minGridCell.gridPosition.x < mouseDownCell.gridPosition.x)
				road_straight_one.transform.position += new Vector3(((roadWidth / 2) + 1) * GridHelper.GetGridCellSize(),0,0);

		} else if (vertical) {
			road_straight_one.transform.position = new Vector3 (
				mouseDownCell.worldPosition.x,
				road_straight_one.transform.position.y,
				GridHelper.GetWorldPosition2 (minGridCell.gridPosition).y + ((size.y * GridHelper.GetGridCellSize ()) / 2.0f) - GridHelper.GetGridCellSize() / 2);
			if (corner && minGridCell.gridPosition.y < mouseDownCell.gridPosition.y)
				road_straight_one.transform.position += new Vector3(0,0,((roadWidth /2) + 1) * GridHelper.GetGridCellSize());
		}


	}

	void UpdateSelectionObjectCorner(GridCell mouseDownCell, GridCell mouseUpCell, bool horizontal, bool vertical)
	{
		
		road_corner.transform.localScale = new Vector3 ((roadWidth) * GridHelper.GetGridCellSize (), 
			                                             road_corner.transform.localScale.y, 
			                                            (roadWidth) * GridHelper.GetGridCellSize ());
		
		if (horizontal) {
			road_corner.transform.position = new Vector3 (
				mouseUpCell.worldPosition.x,
				road_corner.transform.position.y,
				mouseDownCell.worldPosition.y);
		} else if (vertical) {
			road_corner.transform.position = new Vector3 (
				mouseDownCell.worldPosition.x,
				road_corner.transform.position.y,
				mouseUpCell.worldPosition.y);
		}


	}

	void UpdateSelectionObjectStraightTwo(GridCell minGridCell, GridCell mouseUpCell, Vector2 size, 
										  bool horizontal, bool vertical)
	{
		// adjust size
		size.x = size.x - ((roadWidth - 1) / 2) - 1;
		size.y = size.y - ((roadWidth - 1) / 2) - 1;

		if (vertical) {
			road_straight_two.transform.localScale = new Vector3 ((size.x) * GridHelper.GetGridCellSize (), 
			                                                     road_straight_two.transform.localScale.y, 
			                                                     (roadWidth) * GridHelper.GetGridCellSize ());
		} else if (horizontal) {
			road_straight_two.transform.localScale = new Vector3 ((roadWidth) * GridHelper.GetGridCellSize (), 
			                                                     road_straight_two.transform.localScale.y, 
			                                                     (size.y) * GridHelper.GetGridCellSize ());
		}

		if (vertical) {
			road_straight_two.transform.position = new Vector3 (
				GridHelper.GetWorldPosition2 (minGridCell.gridPosition).x + ((size.x * GridHelper.GetGridCellSize ()) / 2.0f) - GridHelper.GetGridCellSize() / 2,
				road_straight_two.transform.position.y,
				mouseUpCell.worldPosition.y);

			if (mouseUpCell.gridPosition.x > minGridCell.gridPosition.x)
				road_straight_two.transform.position += new Vector3(((roadWidth /2) + 1) * GridHelper.GetGridCellSize(),0,0);

		} else if (horizontal) {
			road_straight_two.transform.position = new Vector3 (
				mouseUpCell.worldPosition.x,
				road_straight_two.transform.position.y,
				GridHelper.GetWorldPosition2 (minGridCell.gridPosition).y + ((size.y * GridHelper.GetGridCellSize ()) / 2.0f) - GridHelper.GetGridCellSize() / 2);
		
			if (mouseUpCell.gridPosition.y > minGridCell.gridPosition.y)
				road_straight_two.transform.position += new Vector3(0,0,((roadWidth /2) + 1) * GridHelper.GetGridCellSize());

		}
	}

	bool UpdateMaterialsStraight()
	{
		if (road_straight_one_component.CanBuild()) {
			road_straight_one.GetComponent<Renderer>().material = selectionMaterialValid;
			return true;
		} else {
			road_straight_one.GetComponent<Renderer>().material = selectionMaterialInvalid;
			return false;
		}

	}

	void UpdateMaterialsCorner()
	{
		if (UpdateMaterialsStraight() && road_straight_two_component.CanBuild() &&
			road_corner_component.CanBuild()) {
			road_straight_one.GetComponent<Renderer>().material = selectionMaterialValid;
			road_straight_two.GetComponent<Renderer>().material = selectionMaterialValid;
			road_corner.GetComponent<Renderer>().material = selectionMaterialValid;
		} else {
			road_straight_one.GetComponent<Renderer>().material = selectionMaterialInvalid;
			road_straight_two.GetComponent<Renderer>().material = selectionMaterialInvalid;
			road_corner.GetComponent<Renderer>().material = selectionMaterialInvalid;
		}

	}

	public void CompletePreview(Vector3 mouse_down, Vector3 mouse_up, bool corner, bool snap)
	{
		ShowPreview(mouse_down, mouse_up, snap);
		
		GameObject new_straight_one = null;
		GameObject new_straight_two = null;
		GameObject new_corner = null;

		// have to verify if it can be build first
		if (CanBuild(corner)) {
			new_straight_one = road_straight_one_component.CreateRoad(roadParent);
			if (corner) {
				new_straight_two = road_straight_two_component.CreateRoad(roadParent);
				new_corner = road_corner_component.CreateRoad(roadParent);
			} 

			// update road, which updates connections, materials, etc.
			new_straight_one.GetComponent<RoadStraight>().UpdateRoad(GetStraightOneDirection(mouse_down, mouse_up));
			if (corner) {
				new_straight_two.GetComponent<RoadStraight>().UpdateRoad(GetStraightTwoDirection(mouse_down, mouse_up));
				new_corner.GetComponent<RoadCorner>().UpdateRoad();
			}
			placeRoad.Complete();
		}
		
 	}

 	public Vector2 GetStraightOneDirection(Vector3 mouse_down, Vector3 mouse_up)
 	{
 		Vector2 returnVector = Vector2.zero;
 		GridCell mouseDownCell = GridHelper.GetGridCell(mouse_down);
		GridCell mouseUpCell = GridHelper.GetGridCell(mouse_up);

		Vector3 temp;
		if (mouse_down.x > mouse_up.x) { temp.x = mouse_down.x; mouse_down.x = mouse_up.x; mouse_up.x = temp.x; } 
		if (mouse_down.z > mouse_up.z) { temp.z = mouse_down.z; mouse_down.z = mouse_up.z; mouse_up.z = temp.z; }
				
		minGridCell = GridHelper.GetGridCell (mouse_down);
		maxGridCell = GridHelper.GetGridCell (mouse_up);

		if (maxGridCell.gridPosition.x - minGridCell.gridPosition.x >= maxGridCell.gridPosition.y - minGridCell.gridPosition.y) {
			// horizontal
			if (mouseDownCell.gridPosition.x < mouseUpCell.gridPosition.x)
				returnVector = new Vector2(1,0);
			else
				returnVector = new Vector2(-1,0);
		} else {
			// vertical
			if (mouseDownCell.gridPosition.y < mouseUpCell.gridPosition.y)
				returnVector = new Vector2(0,1);
			else
				returnVector = new Vector2(0,-1);
		}

		//print ("direction vector = " + returnVector);
 		return returnVector;
 	}

 	Vector2 GetStraightTwoDirection(Vector3 mouse_down, Vector3 mouse_up)
 	{
 		Vector2 returnVector = Vector2.zero;
 		GridCell mouseDownCell = GridHelper.GetGridCell(mouse_down);
		GridCell mouseUpCell = GridHelper.GetGridCell(mouse_up);

		Vector3 temp;
		if (mouse_down.x > mouse_up.x) { temp.x = mouse_down.x; mouse_down.x = mouse_up.x; mouse_up.x = temp.x; } 
		if (mouse_down.z > mouse_up.z) { temp.z = mouse_down.z; mouse_down.z = mouse_up.z; mouse_up.z = temp.z; }
				
		minGridCell = GridHelper.GetGridCell (mouse_down);
		maxGridCell = GridHelper.GetGridCell (mouse_up);

		if (maxGridCell.gridPosition.x - minGridCell.gridPosition.x >= maxGridCell.gridPosition.y - minGridCell.gridPosition.y) {
			// vertical
			if (mouseDownCell.gridPosition.y < mouseUpCell.gridPosition.y)
				return new Vector2(0,1);
			else
				return new Vector2(0,-1);
		} else {
			// horizontal
			if (mouseDownCell.gridPosition.x < mouseUpCell.gridPosition.x)
				return new Vector2(1,0);
			else
				return new Vector2(-1,0);
		}

 		return returnVector;
 	}

 	bool CanBuild(bool corner)
 	{
		if (road_straight_one_component.CanBuild()) {
			if (corner) {
				if (road_straight_two_component.CanBuild() && road_corner_component.CanBuild()) {
					return true;
				} else {
					return false;
				}
			} else { // only have to check one straight which was already done
				return true;
			}
		} 
		return false;
 	}

	public void Cancel()
	{
		// hide
		NGUITools.SetActive (this.gameObject, false);

		road_corner.GetComponent<Renderer>().enabled = true;
		road_straight_one.GetComponent<Renderer>().enabled = true;
		road_straight_two.GetComponent<Renderer>().enabled = true;

	}

}
