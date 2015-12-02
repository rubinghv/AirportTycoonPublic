using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

public static class SteeringBehaviors {
	
    //bool 	seek,
    //        flee,
    //        arrive,
    //        separation,
    //        pursuit;

	public enum Deceleration {slow = 3, normal = 2, fast = 1};
	static MovingEntity targetVehicle;
	
	
	public static Vector2 Seek (MovingEntity vehicle) 
	{ 	return Seek (vehicle, vehicle.targetPos); }
	
	public static Vector2 Seek (MovingEntity vehicle, Vector2 targetPosition) 
	{
//        Debug.Log("vehicle position = " + vehicle.position + " - targetPos = " + targetPosition);
		Vector2 desiredVelocity = (targetPosition - vehicle.position).normalized * vehicle.maxSpeed;
		return (desiredVelocity - vehicle.velocity);
	}
	
	public static Vector2 Flee (MovingEntity vehicle)
	{	return Flee (vehicle, vehicle.targetVehicle); }
	
	public static Vector2 Flee (MovingEntity vehicle, MovingEntity target) 
	{
        //flee = true; // NEEDS ATTENTION
        //targetVehicle = target;
        Vector2 desiredVelocity = (vehicle.position - target.position).normalized * vehicle.maxSpeed;
		return (desiredVelocity - vehicle.velocity);
	}

    public static Vector2 Arrive(MovingEntity vehicle)
    {   return Arrive(vehicle, Deceleration.fast); }

	public static Vector2 Arrive (MovingEntity vehicle, Deceleration deceleration)
	{
        //arrive = true; // NEEDS ATTENTION
		float distance = Vector2.Distance(vehicle.targetPos, vehicle.position);
//		savedDeceleration = (double)deceleration;

        if (distance < 0.001)
            return Vector2.zero;
        else if (distance > 0)
        {
            double decelerationTweaker = 0.2;
            double speed = distance / ((double)deceleration * decelerationTweaker);

            speed = Mathf.Min((float)speed, vehicle.maxSpeed); // don't exceed max speed

            Vector2 desiredVelocity = (vehicle.targetPos - vehicle.position) * (float)speed / distance;
            return (desiredVelocity - vehicle.velocity);
        }
        
		
		return Vector2.zero;
	}
	
	public static Vector2 Separation (MovingEntity vehicle)
	{
        //separation = true; // NEEDS ATTENTION
		List<MovingEntity> neighbors = VehicleList.GetNeighbors(vehicle.position, 5);
		Vector2 steeringForce = Vector2.zero;

		for (int i = 0; i < neighbors.Count; i++) 
		{
			if (neighbors[i] != vehicle) // ALSO MAKE SURE that we check for seperation??
			{
				Vector2 toAgent = vehicle.position - neighbors[i].position;
				// scale the force inversely proportional to the agent's distance from it's neighbor
				steeringForce += toAgent.normalized / toAgent.magnitude;
				
			}
			
		}		
		return steeringForce;
	}
	
	public static Vector2 Pursuit (MovingEntity vehicle)
	{ 	return Pursuit (vehicle, vehicle.targetVehicle); }
	
	public static Vector2 Pursuit (MovingEntity vehicle, MovingEntity target)
    {
        //pursuit = true; // NEEDS ATTENTION
        //targetVehicle = target;

		Vector2 toTarget = target.position - vehicle.position;
		float relativeHeading = Vector2.Dot (vehicle.heading, target.heading);
		
		if ((Vector2.Dot (toTarget, vehicle.heading) > 0) &&
									(relativeHeading < -0.95))
			return Seek (vehicle, target.position);
		
		// now time to predict
		float lookAheadTime = toTarget.magnitude / (vehicle.maxSpeed + target.velocity.magnitude);
        return Seek (vehicle, target.position + target.velocity * lookAheadTime);		
	}

    //public static Vector2 WallAvoidance(MovingEntity vehicle)
    //{
    //    // get the feelers
    //    List<Vector2> feelers = new List<Vector2>(); // temporary
    //    List<Vector2> walls = new List<Vector2>(); // TEMPORARY

    //    float DistToThisIP = 0.0f;
    //    float DistToClosestIP = 1000; // has to be smaller than this to start moving away

    //    Wall ClosestWall = null; // index for the vector of the walls

    //    Vector2 SteeringForce,
    //            point,        // used for storing temporary info 
    //            ClosestPoint; // holds the closest intersection point

    //    // examine each feeler
    //    foreach (Vector2 feeler in feelers)
    //    {
    //        foreach (Vector2 wall in walls)
    //        {
    //            if (true) // test if wall and feeler intersect (raycast or something cheaper)
    //            {
    //                if (DistToThisIP < DistToClosestIP)
    //                {
    //                    DistToClosestIP = DistToThisIP;

    //                    ClosestWall = wall; // should be index
    //                    ClosestPoint = point; // need calculating for this one way or anotherr
    //                }
    //            }

    //        }
    //        // if we found a wall with intersection, calculate force that steers away from it
    //        if (ClosestWall >= 0)
    //        {
    //            // calculate the overshoot
    //            Vector2 OverShoot = -ClosestPoint;

    //            // create a force in the direction of the wall normal, with a magnitude of overshoot
    //            SteeringForce = wall * OverShoot.magnitude;
    //        }

    //    }

    //    return SteeringForce;
    //}











	public static void Stop (MovingEntity vehicle)
    {// NEEDS ATTENTION// NEEDS ATTENTION// NEEDS ATTENTION// NEEDS ATTENTION
        //seek = false; 
        //flee = false;
        //separation = false;           // whatever points to this needs to be redirected
        //pursuit = false;	
		
        //arrive = true;
        //vehicle.targetPos = vehicle.position + vehicle.heading;
        //this.Arrive(vehicle, Deceleration.normal);
	}


}
