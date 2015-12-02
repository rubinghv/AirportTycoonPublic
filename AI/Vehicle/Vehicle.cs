    using UnityEngine;
using System.Collections;

public class Vehicle : MovingEntity {

    public int state; // should be protected
	protected Vector2 finalDestination;
    //protected SteeringVehicle steering; // changed this from Steering to SteeringVehicle

	protected void Start()
	{
		// don't do base start
		steeringCalculate = new SteeringCalculateWeightedSum(); // create one or the other
        steering = new SteeringVehicle(this, steeringCalculate);
	}

    protected override void Update()
    {
        base.Update();
        DefaultBehaviour(Time.deltaTime);
    }

    protected virtual void DefaultBehaviour(float deltaTime)
    {
        //Debug.Log("No default behavior implemented!");
    }


    /*
     *  Test if we have arrived at finalDestination 
     * 
     */
    protected bool HasArrived() {
        return HasArrived (GridHelper.GetGridCellSize() / 100f);
    }

    protected bool HasArrived(float distance) {
        return HasArrived(distance, finalDestination);
    }

    protected bool HasArrived(float distance, Vector2 destination) {
        if (Vector2.Distance (destination, this.position) < distance)
            return true;
        else
            return false;
    }
    
    

}
