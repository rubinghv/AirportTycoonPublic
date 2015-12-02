using UnityEngine;
using System.Collections;

public class SteeringCalculateWeightedSum : SteeringCalculate {

    public override Vector2 CalculateSteering(MovingEntity vehicle)
    {
        Vector2 steeringForce = new Vector2();

        if (seek)
            steeringForce += SteeringBehaviors.Seek(vehicle);
        if (flee)
            steeringForce += SteeringBehaviors.Flee(vehicle);
        if (arrive)
            steeringForce += SteeringBehaviors.Arrive(vehicle, SteeringBehaviors.Deceleration.normal);
        if (separation)
            steeringForce += SteeringBehaviors.Separation(vehicle) * 3;
        if (pursuit)
            steeringForce += SteeringBehaviors.Pursuit(vehicle);

        return steeringForce;
    }



}
