using UnityEngine;
using System.Collections;

public class RoadStraight : Road {

	// traffic connections and connections have to be set to protected
	public Vector2 trafficDirection;
 	public Road incomingConnection;
 	public Road outgoingConnection;

 	public PathfindingObject PFObjectTopRight;
 	public PathfindingObject PFObjectBottomLeft;

 	public override int GetRoadWidth()
	{
		if (trafficDirection.x != 0) 
			return (int) size.y;
		else if (trafficDirection.y != 0)
			return (int) size.x;
		else 
			print("GetRoadWidth() doesn't have any traffic direction");
		
		return 0;
	}
	public override int GetRoadLength()
	{
		if (trafficDirection.x != 0) 
			return (int) size.x;
		else if (trafficDirection.y != 0)
			return (int) size.y;
		else 
			print("GetRoadLength() doesn't have any traffic direction");
		
		return 0;
	}

	public void UpdateRoad(Vector2 road_direction)
	{
		SetupPositionAndSize();
		trafficDirection = road_direction;
		//UpdateMaterials()

		base.UpdateRoad();

	}

	public override GameObject CreateRoad(Road oldRoad)
	{
		base.CreateRoad(oldRoad);

		RoadStraight oldRoadStraight = (RoadStraight) oldRoad;
		trafficDirection = oldRoadStraight.trafficDirection;

		return this.gameObject;
	}

	protected override void UpdateConnections(bool updateRecursively)
	{
		// check if road is horizontal
		if (trafficDirection.x != 0 ) {
			UpdateConnectionsHorizontal();
		} else if (trafficDirection.y != 0) {
			UpdateConnectionsVertical();
		} else {
			print("straight road has no direction?! shouldn't happen");
		}

		UpdateConnectionsStraightToStraight();

	}



	void UpdateConnectionsHorizontal()
	{
		//print(this.gameObject.name + " is updating horizontal connections");

		// begin swith saving road at start of loop
		Road foundRoadRight = GridHelper.GetGridCell(grid_position_max.x + 1, grid_position_min.y).GetRoad();
		Road foundRoadLeft = GridHelper.GetGridCell(grid_position_min.x - 1, grid_position_min.y).GetRoad();

		// also parking
		PathfindingObject foundGridObjectRight = GridHelper.GetGridCell(grid_position_max.x + 1, grid_position_min.y).GetPathfindingObject();
		PathfindingObject foundGridObjectLeft = GridHelper.GetGridCell(grid_position_min.x - 1, grid_position_min.y).GetPathfindingObject();

		// y loops same for left and right
		for (int loopY = (int) grid_position_min.y; loopY <= grid_position_max.y; loopY++) {
			// check right
			if (foundRoadRight != null) {
				if (foundRoadRight != GridHelper.GetGridCell(grid_position_max.x + 1, loopY).GetRoad()) 
					foundRoadRight = null;
			}
			if (foundGridObjectRight != null) {
				if (foundGridObjectRight != GridHelper.GetGridCell(grid_position_max.x + 1, loopY).GetPathfindingObject()) 
					foundGridObjectRight = null;
			}

			// check left
			if (foundRoadLeft != null) {
				if (foundRoadLeft != GridHelper.GetGridCell(grid_position_min.x - 1, loopY).GetRoad()) 
					foundRoadLeft = null;
			}
			if (foundGridObjectLeft != null) {
				if (foundGridObjectLeft != GridHelper.GetGridCell(grid_position_min.x - 1, loopY).GetPathfindingObject()) 
					foundGridObjectLeft = null;
			}
		}



		// first update road connections
		rightConnection = foundRoadRight;
		leftConnection = foundRoadLeft;

		if (rightConnection is RoadCorner)
			rightConnection.UpdateRoad();
		if (leftConnection is RoadCorner)
			leftConnection.UpdateRoad();

		if (rightConnection != null) {
			rightConnection.SetConnection(this, new Vector2(1,0));
		} 
		if (leftConnection != null) {
			leftConnection.SetConnection(this, new Vector2(-1,0));
		}
			
		// and also pathfinding objects (part of the road network)
		if (foundGridObjectRight != null) {
			PFObjectTopRight = foundGridObjectRight;
			PFObjectTopRight.SetRoadConnection(this);
		}
		if (foundGridObjectLeft != null) {
			PFObjectBottomLeft = foundGridObjectLeft;
			PFObjectBottomLeft.SetRoadConnection(this);
		}

	}

	void UpdateConnectionsVertical()
	{
		//print(this.gameObject.name + " is updating vertical connections");

		// reset for up and down
		Road foundRoadUp = GridHelper.GetGridCell(grid_position_min.x, grid_position_max.y + 1).GetRoad();
		Road foundRoadDown = GridHelper.GetGridCell(grid_position_min.x, grid_position_min.y - 1).GetRoad();

		// also look for parking spots
		PathfindingObject foundGridObjectUp = GridHelper.GetGridCell(grid_position_min.x, grid_position_max.y + 1).GetPathfindingObject();
		PathfindingObject foundGridObjectDown = GridHelper.GetGridCell(grid_position_min.x, grid_position_min.y - 1).GetPathfindingObject();

		// x loops same for up and down
		for (int loopX = (int) grid_position_min.x; loopX <= grid_position_max.x; loopX++) {
			// check up
			if (foundRoadUp != null) {
				if (foundRoadUp != GridHelper.GetGridCell(loopX, grid_position_max.y + 1).GetRoad()) {
					foundRoadUp = null;
			} if (foundGridObjectUp != null) {			
				if (foundGridObjectUp != GridHelper.GetGridCell(loopX, grid_position_max.y + 1).GetPathfindingObject())
					foundGridObjectUp = null;
			} 


			// check down
			if (foundRoadDown != null) {
				if (foundRoadDown != GridHelper.GetGridCell(loopX, grid_position_min.y - 1).GetRoad()) 
					foundRoadDown = null;
			} if (foundGridObjectDown != null)	
				if (foundGridObjectDown != GridHelper.GetGridCell(loopX, grid_position_min.y - 1).GetPathfindingObject())
					foundGridObjectDown = null;
			}
			//print("UP - look at cells: " + loopX + ", " + (grid_position_max.y + 1));
			//print("DOWN - look at cells: " + loopX + ", " + (grid_position_min.y - 1));

		}

		// uprdate road connections
		topConnection = foundRoadUp;
		bottomConnection = foundRoadDown;


		if (topConnection is RoadCorner)
			topConnection.UpdateRoad();
		if (bottomConnection is RoadCorner)
			bottomConnection.UpdateRoad();
		
		if (topConnection != null) {
			topConnection.SetConnection(this, new Vector2(0,1));
		} 
		if (bottomConnection != null) {
			bottomConnection.SetConnection(this, new Vector2(0,-1));
		}

		// and also pathfinding objects (part of the road network)
		if (foundGridObjectUp != null) {
			PFObjectTopRight = foundGridObjectUp;
			PFObjectTopRight.SetRoadConnection(this);
		}
		if (foundGridObjectDown != null) {
			PFObjectBottomLeft = foundGridObjectDown;
			PFObjectBottomLeft.SetRoadConnection(this);
		}
	}

	/*
	 *	If there are two straights connected end-to-end, or perpendicular,
	 *	make it into one
	 *
	 */
	bool UpdateConnectionsStraightToStraight() 
	{
		RoadStraight connectionRoad;
		// check if road is horizontal
		if (trafficDirection.x != 0 ) {
			// if to the road to the left is also straight?
			if (leftConnection != null && leftConnection is RoadStraight ) {
				connectionRoad = (RoadStraight) leftConnection;
				// are roads end to end?
				if (connectionRoad.trafficDirection.x != 0) {
					CreateTwoStraights(connectionRoad);
				// are roads perpendicular?
				} else if (connectionRoad.trafficDirection.y != 0) {
					CreateTwoStraightsAndCorner(connectionRoad);
				}
			} 

			if (rightConnection != null && rightConnection is RoadStraight) {
					connectionRoad = (RoadStraight) rightConnection;
				if (connectionRoad.trafficDirection.x != 0) {
					CreateTwoStraights(connectionRoad);
				} else if (connectionRoad.trafficDirection.y != 0) {
					CreateTwoStraightsAndCorner(connectionRoad);
				}
			}
		// otherwise if road is vertical
		} else if (trafficDirection.y != 0 ) {
			if (bottomConnection != null && bottomConnection is RoadStraight) {
				connectionRoad = (RoadStraight) bottomConnection;
				if (connectionRoad.trafficDirection.y != 0) {
					CreateTwoStraights(connectionRoad);
				} else if (connectionRoad.trafficDirection.x != 0) {
					CreateTwoStraightsAndCorner(connectionRoad);
				}
			}

			if (topConnection != null && topConnection is RoadStraight) {
				connectionRoad = (RoadStraight) topConnection;
				if (connectionRoad.trafficDirection.y != 0) {
					CreateTwoStraights(connectionRoad);
				} else if (connectionRoad.trafficDirection.x != 0) {
					CreateTwoStraightsAndCorner(connectionRoad);
				}

			}
		}

		return false;
	}

	/*
	 *	Called by UpdateConnectionsStraightToStraight() when two straights need to be 
	 *	replaced by one, and the old roads deleted
	 */
	void CreateTwoStraights(RoadStraight otherRoad)
	{
		Vector2 min_array = new Vector2(Mathf.Min(grid_position_min.x,otherRoad.grid_position_min.x), 
									  	Mathf.Min(grid_position_min.y,otherRoad.grid_position_min.y)); 
		Vector2 max_array = new Vector2(Mathf.Max(grid_position_max.x,otherRoad.grid_position_max.x), 
										Mathf.Max(grid_position_max.y,otherRoad.grid_position_max.y));

		GameObject new_go = GridBuilder.CreateRectangle(min_array, max_array, this.transform.localScale.y, 
														this.transform.parent, true);

		// create component, createRoad() and then update
		RoadStraight road_component = new_go.AddComponent<RoadStraight>();
		road_component.CreateRoad(this);
		
		/*
		print(this.gameObject.name + "is creating two straights");
		StartCoroutine(DelayedUpdateRoad(road_component, otherRoad));
		*/

		road_component.UpdateRoad(this.trafficDirection);

		Object.Destroy(otherRoad.gameObject);
		Object.Destroy(this.gameObject);
	}

	
    /*
     *	Need to delay updating new road, so other roads being constructed can finish
     * 	their new road connections first (and then be overridden )
     */
    IEnumerator DelayedUpdateRoad(RoadStraight road_component, RoadStraight otherRoad)
    {      
        yield return new WaitForSeconds(0.0001f);    //Wait one frame
        road_component.UpdateRoad(this.trafficDirection);
        print("updated new road!");

		// delete two old gameObjects
        Object.Destroy(otherRoad.gameObject);
		Object.Destroy(this.gameObject);
    }

	/*
	 *	Called by UpdateConnectionsStraightToStraight() when one straight connects to 
	 * 	another perpendicularly, and there needs to be a corner and two straights to
	 *	replace the old road, which is deleted
	 */
	void CreateTwoStraightsAndCorner(RoadStraight otherRoad)
	{
		RoadStraight straightUpper, straightLower;
		Road corner;

		if (otherRoad.trafficDirection.y != 0) {
			straightLower = (RoadStraight) CreateRoad(otherRoad.grid_position_min, 
								new Vector2(otherRoad.grid_position_max.x, grid_position_min.y - 1), false);

			straightUpper = (RoadStraight) CreateRoad(new Vector2(otherRoad.grid_position_min.x, 
								grid_position_max.y + 1), otherRoad.grid_position_max, false);

			corner = CreateRoad(new Vector2(otherRoad.grid_position_min.x, grid_position_min.y),
								 new Vector2(otherRoad.grid_position_max.x, grid_position_max.y), true);
		} else {
			straightLower = (RoadStraight) CreateRoad(otherRoad.grid_position_min, 
								new Vector2(grid_position_min.x - 1, otherRoad.grid_position_max.y), false);

			straightUpper = (RoadStraight) CreateRoad(new Vector2(grid_position_max.x + 1, 
								otherRoad.grid_position_min.y), otherRoad.grid_position_max, false);

			corner = CreateRoad(new Vector2(grid_position_min.x, otherRoad.grid_position_min.y),
								 new Vector2(grid_position_max.x, otherRoad.grid_position_max.y), true);
		}


		if (straightLower != null)
			straightLower.UpdateRoad(otherRoad.trafficDirection);
		if (straightUpper != null)
			straightUpper.UpdateRoad(otherRoad.trafficDirection);
		corner.UpdateRoad();

		Object.Destroy(otherRoad.gameObject);
		//UpdateRoad();
	}

	/*
	 *	Create a road with minimal instructions, helper method for above functions
	 */
	Road CreateRoad(Vector2 min_array, Vector2 max_array, bool corner) {
		Road road_component;
		// if there is a corner, one straight will have size 0, if so return null
		if (max_array.x - min_array.x <= 0 || max_array.y - min_array.y <= 0)
			return null;

		GameObject new_go = GridBuilder.CreateRectangle(min_array, max_array, this.transform.localScale.y, 
														this.transform.parent, true);

		// create component, createRoad() and then update
		if (corner) {
			road_component = new_go.AddComponent<RoadCorner>();
		} else {
			road_component = new_go.AddComponent<RoadStraight>();
		}

		road_component.CreateRoad(this);

		return road_component;
	}

	/* 
	 * update scale and rotation of materials
	 */
	protected override void UpdateMaterials()
	{
		Vector2 tiling = Vector2.one;

		if (trafficDirection.y != 0) { // vertical
			// don't change orientation but do change tiling
			if (trafficDirection.y == -1)
				RotateMesh(180f);

			tiling.y = (int) size.y;
		} else if (trafficDirection.x != 0) { // horizontal
			if (trafficDirection.x == 1)
				RotateMesh(90f);
			else if (trafficDirection.x == -1)
				RotateMesh(270f);

			tiling.y = (int) size.y;
		}

		this.gameObject.GetComponent<Renderer>().material = roadMaterial;
		Material tempMaterial = this.gameObject.GetComponent<Renderer>().material;

		tempMaterial.SetTextureScale("_MainTex", tiling);
	}

	/*
	 *	In addition to rotating, also scale the mesh
	 */
	protected override void RotateMesh(float degrees)
	{
		// save old scale
		float scaleX = this.transform.localScale.x;
		float scaleZ = this.transform.localScale.z;

		//new scale (if a horizontal road) 
		if (degrees == 90f || degrees == 270f) {
			this.transform.localScale = new Vector3(scaleZ, this.transform.localScale.y, scaleX);

			float old_x = size.x;
			size.x = size.y;
			size.y = old_x;
		} 
		
		base.RotateMesh(degrees);

		//SetupPositionAndSize(false);
	}

	/*
	 *	Get the point where an incoming road should snap to the current road
	 */
	public override Vector3 GetSnapGridCell(Vector2 direction, bool mouseDown, Vector3 pos)
	{
		Vector2 arrayPosition= Vector2.zero;

		if (direction.x != 0 && trafficDirection.y != 0) { // horizontal road to corner
			arrayPosition.y = GridHelper.GetGridPosition(pos).y;

			// set array positions
			if ((direction.x == 1 && mouseDown) || (direction.x == -1 && !mouseDown))
				arrayPosition.x = grid_position_max.x + 1;
			else if ((direction.x == -1 && mouseDown) || (direction.x == 1 && !mouseDown))
				arrayPosition.x = grid_position_min.x - 1;

			// however if at end of straight snap to corner
			if (arrayPosition.y > grid_position_max.y - GetRoadWidth() - 1) {
				arrayPosition.y = grid_position_max.y - (GetRoadWidth() / 2);
			} else if (arrayPosition.y < grid_position_min.y + GetRoadWidth() + 1) {
				arrayPosition.y = grid_position_min.y + (GetRoadWidth() / 2);
			}

		} else if (direction.x != 0 && trafficDirection.x != 0) { // horizontal road to horizontal road (end-to-end)
			// if connecting road is also horizontal
				// then connect end to end
				arrayPosition.y = grid_position_min.y + (GetRoadWidth() / 2);
				arrayPosition.x = GridHelper.GetGridPosition(pos).x;

			if (arrayPosition.x >= grid_position_max.x - GetRoadWidth() - 1) {
				arrayPosition.x = grid_position_max.x + 1;
			} else if (arrayPosition.x <= grid_position_min.x + GetRoadWidth() - 1) {
				arrayPosition.x = grid_position_min.x - 1;
			}

		} else if (direction.y != 0 && trafficDirection.x != 0) { //  vertical road to horizontal road
			arrayPosition.x = GridHelper.GetGridPosition(pos).x;

			if ((direction.y == 1 && mouseDown) || (direction.y == -1 && !mouseDown))
				arrayPosition.y = grid_position_max.y + 1;
			else if ((direction.y == -1 && mouseDown) || (direction.y == 1 && !mouseDown))
				arrayPosition.y = grid_position_min.y - 1;

			if (arrayPosition.x > grid_position_max.x - GetRoadWidth() - 1) {
				arrayPosition.x = grid_position_max.x - (GetRoadWidth() / 2);
			} else if (arrayPosition.x < grid_position_min.x + GetRoadWidth() + 1) {
				arrayPosition.x = grid_position_min.x + (GetRoadWidth() / 2);
			}	

		} else if (direction.y != 0 && trafficDirection.y != 0) { // vertical road to vertical road (end-to-end)
				// connect end to end
				arrayPosition.x = grid_position_min.x + (GetRoadWidth() / 2);
				arrayPosition.y = GridHelper.GetGridPosition(pos).y;

			if (arrayPosition.y >= grid_position_max.y - GetRoadWidth() - 1) {
				arrayPosition.y = grid_position_max.y + 1;
			} else if (arrayPosition.y <= grid_position_min.y + GetRoadWidth() - 1) {
				arrayPosition.y = grid_position_min.y - 1;
			}

		} else {
			print("return pos!");
			return pos;
		}

		return GridHelper.GetWorldPosition3(arrayPosition);
	}

	/*
	 *	For pathfinding purposes, return a list of possible connections (some could be null)
	 */
	public override PathfindingObject[] GetNeighbours() {
		PathfindingObject[] returnArray = new PathfindingObject[6];

		returnArray[0] = topConnection;
		returnArray[1] = rightConnection;
		returnArray[2] = bottomConnection;
		returnArray[3] = leftConnection;
		returnArray[4] = PFObjectTopRight;
		returnArray[5] = PFObjectBottomLeft;

		return returnArray;
	}

	/*
	 *	For pathfinding purposes, get two waypoints from road
	 */
	public override Vector2[] GetWaypoints(PathfindingObject previousObject, PathfindingObject nextObject)
	{
		Vector2 upRight, downLeft, middle;

		if (trafficDirection.x != 0) { // horizontal
			upRight.y = GetPositionCenter2().y;
			downLeft.y = GetPositionCenter2().y;

			upRight.x = GridHelper.GetWorldPosition2(grid_position_max).x;
			downLeft.x = GridHelper.GetWorldPosition2(grid_position_min).x;
		} else if (trafficDirection.y != 0) { // vertical
			upRight.x = GetPositionCenter2().x;
 			downLeft.x = GetPositionCenter2().x;

 			upRight.y = GridHelper.GetWorldPosition2(grid_position_max).y;
			downLeft.y = GridHelper.GetWorldPosition2(grid_position_min).y;
		} else {
			upRight = Vector2.zero;
			downLeft = Vector2.zero;
			print("fatal error, shouldn't go here!");
		}

		middle = GetPositionCenter2();

		// create array to return and get world position from previous object
		Vector2[] returnArray= new Vector2[2];
		Vector2 previousWorldPosition2 = previousObject.GetPositionCenter2();

		// determine which waypoint is closer, put that first in array
		if (Vector2.Distance(previousWorldPosition2, upRight) <
			Vector2.Distance(previousWorldPosition2, downLeft)) { // if right side is closer, put it first
			returnArray[0] = upRight;
			returnArray[1] = downLeft;
		} else {
			returnArray[0] = downLeft;
			returnArray[1] = upRight;
		}

		// if road length is less than width, remove middle array
		if (GetRoadWidth() + 2 < GetRoadLength()) {
			Vector2[] tempArray = new Vector2[3];
			tempArray[0] = returnArray[0];
			tempArray[2] = returnArray[1];
			tempArray[1] = middle;

			return tempArray;
		}


		return returnArray;
	}
}





















