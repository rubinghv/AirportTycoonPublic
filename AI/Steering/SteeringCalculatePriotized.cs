using UnityEngine;
using System.Collections;

public class SteeringCalculatePrioritized : SteeringCalculate {

    public override Vector2 CalculateSteering(MovingEntity vehicle)
    {
        Vector2 steeringForce = Vector2.zero;
        Vector2 force;

        if (separation)
        {
            force = SteeringBehaviors.Separation(vehicle) * 1.0f;
            if (AccumulateForce(vehicle, ref steeringForce, force) && force != Vector2.zero) return steeringForce;
        }
        if (seek)
        {
            force = SteeringBehaviors.Seek(vehicle) * 1.0f;
            if (AccumulateForce(vehicle, ref steeringForce, force)) return steeringForce;
        }
        if (flee)
        {
            force = SteeringBehaviors.Flee(vehicle) * 1.0f;
            if (AccumulateForce(vehicle, ref steeringForce, force)) return steeringForce;
        }         if (arrive)
        {
            force = SteeringBehaviors.Arrive(vehicle) * 1.0f;
            if (AccumulateForce(vehicle, ref steeringForce, force)) return steeringForce;
        }
        if (pursuit)
        {
            force = SteeringBehaviors.Pursuit(vehicle) * 1.0f;
            if (AccumulateForce(vehicle, ref steeringForce, force)) return steeringForce;
        }

        return steeringForce;       
    }

    bool AccumulateForce(MovingEntity vehicle, ref Vector2 runningTotal, Vector2 forceToAdd)
    {
        //		print ("runningTotal: " + runningTotal + " . forceToAdd: " + forceToAdd);

        float magnitudeSoFar = runningTotal.magnitude; // how much steering force has been used?
        float magnitudeRemaining = (float)1.0f - magnitudeSoFar;

        //		print ("magnitudeSoFar: " + magnitudeSoFar + " . magnitudeRemaining: " + magnitudeRemaining);
        if (magnitudeRemaining <= 0.0001f) return false;

        float magnitudeToAdd = forceToAdd.magnitude;

        if (magnitudeToAdd < magnitudeRemaining)
            runningTotal += forceToAdd;
        else
            runningTotal += forceToAdd.normalized * magnitudeRemaining;

        return true;
    }

}
