using UnityEngine;
using System.Collections;

public class MovingEntity : Entity {
	
	[Header("Moving Entity")]
    //protected SteeringBehaviors steeringBehaviors = new SteeringBehaviors(); // CAN DISAPPEAR replaced by
    protected SteeringCalculate steeringCalculate; // create one or the other
    protected Steering steering;

	//should be part of base entity class?
	public Vector2 position {
		get { return new Vector2 (this.tf.position.x, this.tf.position.z); }
		private set { this.transform.position = new Vector3 (value.x, this.transform.position.y, value.y); }
	}
	
	public Vector2 velocity;
	public Vector2 heading; // normalized vector pointing in the heading direction
//	Vector2 side;	// perpendicular to heading
	
	public float mass;
	public float maxSpeed;
	public float maxForce;	// similar to thrust
	public float maxTurnRate;	// max radians per second 
	
	public Vector2 targetPos;
	public Transform tf; // something something
	public MovingEntity targetVehicle;

	// Update is called once per frame
	protected virtual void Update () {
		if (steeringCalculate == null)
		{
//			print("trying");
        	steeringCalculate = new SteeringCalculateWeightedSum(); 
		}


		// combined steering force from steering behavior
        Vector2 steeringForce = steeringCalculate.CalculateSteering(this);

		Vector2 acceleration = steeringForce / mass; // acceleration = force / mass
		
		// update velocity
		velocity += acceleration * Time.deltaTime * TimeController.GameSpeed;
		
		// make sure it does not exceed maximum velocity ------------- ENABLE THIS LATER
		velocity = truncateSpeed (velocity);
		
		position += velocity * Time.deltaTime * TimeController.GameSpeed; // update position
		
		// update the heading if we have a very small velocity
		if (velocity.magnitude > 0.00001)
		{
			heading = velocity.normalized;
			tf.forward = new Vector3(heading.x, 0, heading.y);
//			side = heading.	 ------------ MIGHT NEED IMPLEMENT SIDE LATER
		}

		if (steering == null)
		{
//			print("trying");
        	steering = new Steering(this, steeringCalculate);
		}


		//if (flee) { steering.Flee (targetVehicle); flee = false; } // TEMPORARY
		//if (pursuit) { steering.Pursuit (targetVehicle); flee = false; } // TEMPORARY
		//if (arrive) { steering.Arrive (new Vector2(targetVehicle.transform.position.x, targetVehicle.transform.position.z)); arrive = false; } // TEMPORARY

        // don't forget steering update
        steering.ManualUpdate();
	}
	

    // temporary members
    //public bool flee = false; // TEMP
    //public bool pursuit = false;
	//public bool arrive = false;
	
	Vector2 truncateSpeed (Vector2 vec2) {
		if (vec2.magnitude > maxSpeed) {
			 vec2.Normalize();
	         vec2 *= maxSpeed;
		} 
		return vec2;		
	}

	// ------------------------------------- Saving -------------------------------------

	public override void Save (string filename, string tag) 
	{
		base.Save(filename, tag);
		
		ES2.Save(this.transform.position, filename + tag + "transformPosition");
		ES2.Save(velocity, filename + tag + "velocity");
		ES2.Save(heading, filename + tag + "heading");

		ES2.Save(mass, filename + tag + "mass");
		ES2.Save(maxSpeed, filename + tag + "maxSpeed");
		ES2.Save(maxForce, filename + tag + "maxForce");
		ES2.Save(maxTurnRate, filename + tag + "maxTurnRate");

		ES2.Save(targetPos, filename + tag + "targetPos");
	}

	public override void Load (string filename, string tag) 
	{
		base.Load(filename, tag);

		this.transform.position = ES2.Load<Vector3>(filename + tag + "transformPosition");
		velocity = ES2.Load<Vector2>(filename + tag + "velocity");
		heading = ES2.Load<Vector2>(filename + tag + "heading");

		mass = ES2.Load<float>(filename + tag + "mass");
		maxSpeed = ES2.Load<float>(filename + tag + "maxSpeed");
		maxForce = ES2.Load<float>(filename + tag + "maxForce");
		maxTurnRate = ES2.Load<float>(filename + tag + "maxTurnRate");

		targetPos = ES2.Load<Vector2>(filename + tag + "targetPos");
	}

}
