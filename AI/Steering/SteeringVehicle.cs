using UnityEngine;
using System.Collections;

public class SteeringVehicle : Steering {

    protected PathfindingObject[] objectPath;

	public SteeringVehicle(MovingEntity me, SteeringCalculate sc) : base(me, sc)
 	{
 		// don't need anything here
 	}

 	protected override void DefaultBehavior()
    {
        steeringCalculate.separation = false;
    }


	public override void Visit(PathfindingObject startObject, PathfindingObject endObject)
    {
    	objectPath = RoadPathfinding.CreatePath(startObject, endObject);

    	if (objectPath == null)
    	{
    		 Debug.Log("Can't visit because path cannot be found!");
    		return; 
    	}

    	// -- create list from object path, all at once
    	// in the future this will have to be REPLACED because road might disappear.
    	// So instead it would search by getting waypoints from PathfindingObjects as
    	// the vehicle went along, so if a road disappears, we could reroute

		Vector2[] position_waypoints = new Vector2[objectPath.Length * 2];
		int vectorIndex = 0;
		for (int index = 0; index < objectPath.Length; index++) {
			
			Vector2[] temp_waypoints = new Vector2[0];

			// if not first or last
			if (index != 0 && index != (objectPath.Length - 1))
				temp_waypoints = objectPath[index].GetWaypoints(objectPath[index - 1], objectPath[index + 1]);
			else if (index == 0) { // if first
				temp_waypoints = new Vector2[1];
				temp_waypoints[0] = objectPath[index].GetWaypoint();
			} // do nothing if first

			// loop list from pathfindingObject and add to list
			for (int k = 0; k < temp_waypoints.Length; k++) {
				position_waypoints[vectorIndex] = temp_waypoints[k];
				vectorIndex++;
			}

		}

		path = Helper.ResizeArray(position_waypoints, vectorIndex);


		// ---------------------------------------

		PathIndex = path.Length - 1;
        movingEntity.targetPos = path[PathIndex];
        steeringCalculate.seek = true;

        // set distance for visiting; when path moves on to next waypoint
        pathfindingCellDelta = GridHelper.GetGridCellSize();

        //Debug.Log("path index = " + PathIndex);

    }





}
