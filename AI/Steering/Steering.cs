using UnityEngine;
using System.Collections;

public class Steering : EntityNoMono {

    protected MovingEntity movingEntity;
    protected SteeringCalculate steeringCalculate;

    protected bool stopping = false;
	protected float pathfindingCellDelta = 0.1f;


    public Steering (MovingEntity me, SteeringCalculate sc) 
    {
        movingEntity = me;
        steeringCalculate = sc;
    }

    

    //public float multSeek,
    //                multArrive,
    //                multSeperation;

    protected virtual void DefaultBehavior()
    {
        steeringCalculate.separation = false;
        //steeringCalculate.stopping = false;
    }

    public void ManualUpdate()
    {
        if (pathIndex >= 0)
        {
            Visting();
        }
        if (stopping) // need this for bullshit
        {
            if (Vector2.Distance(movingEntity.targetPos, movingEntity.position) < 0.01)
                FullStop();
        }
    }

	public bool Visit(Vector3 target)
	{
		return Visit (new Vector2 (target.x, target.z));
	}

    public virtual bool Visit(Vector2 target)
    {
        DefaultBehavior();
        path = Pathfinding.CreatePath(movingEntity.transform.position, new Vector3(target.x, 1, target.y));
        
        if (path == null) {
            Debug.Log("Can't visit because path cannot be found!");
            pathIndex = -1;
            return false; 
        } 
        /*
        if (pathIndex <= 0) {
            pathIndex = -1;
            return false;
        }*/

        pathIndex = path.Length - 1;


        movingEntity.targetPos = path[pathIndex];
        steeringCalculate.seek = true;

        return true;
    }

    public virtual void Visit(Vector2[] new_path)
    {
        DefaultBehavior();
        path = new_path;
        pathIndex = path.Length - 1;

        if (path == null) {
            Debug.Log("Can't visit because path cannot be found!");
            pathIndex = -1;
            return; 
        }
        /*
        if (pathIndex <= 0) {
            pathIndex = -1;
            return;
        }*/

        pathIndex = path.Length - 1;

        movingEntity.targetPos = path[pathIndex];
        steeringCalculate.seek = true;

     //   Debug.Log("visiting success (" + steeringCalculate.GetID() + ") seek = " + steeringCalculate.seek);

    }


    public virtual void Visit(PathfindingObject startObject, PathfindingObject endObject)
    {
        Debug.Log("Not implemented!");
    }

    /**
     *  Activated by Visit() and gets called by update when pathlengh is longer than 0. 
     * 
     */
    protected Vector2[] path;

    private int pathIndex = -1;
    protected int PathIndex
    {
        get { return pathIndex; }
        set { pathIndex = value; }
    }

    protected void Visting()
    {
        if (path == null)
        {
            FullStop();
        }

		if (Vector2.Distance(movingEntity.position, path[pathIndex]) < pathfindingCellDelta && pathIndex > 0)
        {
            pathIndex--;
            movingEntity.targetPos = path[pathIndex];
		}

        if (pathIndex == 0)
		{
			steeringCalculate.arrive = true;
			steeringCalculate.seek = false;
		}
    }


    /*
     *  Get final destination from path
     *  
     *  Used by vehicles and people (Person) to determine if final destination has been reached
     */
    public Vector2 GetFinalDestination()
    {
        if (path != null && pathIndex != -1) 
            return path[0];
        else {
            Debug.Log("returning zero, something went wrong");
            return Vector2.zero;
       }
    }

    public void Arrive(Vector3 target)
    {
        Arrive(new Vector2(target.x, target.z));
    }

    public void Arrive(Vector2 target)
    {
        DefaultBehavior();
        movingEntity.targetPos = target;
        steeringCalculate.arrive = true;
    }

    public void Seek(Vector3 target)
    {
         Seek(new Vector2(target.x, target.z));
    }

    public void Seek(Vector2 target)
    {
        DefaultBehavior();
        movingEntity.targetPos = target;
        steeringCalculate.seek = true;
    }

    public void Flee(MovingEntity vehicle)
    {
        DefaultBehavior();
        movingEntity.targetVehicle = vehicle;
        steeringCalculate.flee = true;
    }

    public void Pursuit(MovingEntity vehicle)
    {
        DefaultBehavior();
        movingEntity.targetVehicle = vehicle;
        steeringCalculate.pursuit = true; ;
    }


    public void Stop()
    {// NEEDS ATTENTION// NEEDS ATTENTION// NEEDS ATTENTION// NEEDS ATTENTION
        steeringCalculate.seek = false;
        steeringCalculate.flee = false;
        steeringCalculate.separation = false;           // whatever points to this needs to be redirected
        steeringCalculate.pursuit = false;

        steeringCalculate.arrive = true;
        movingEntity.targetPos = movingEntity.position + movingEntity.heading;
        stopping = true;
    }

    public void FullStop()
    {
        steeringCalculate.arrive = false;
        steeringCalculate.seek = false;
        movingEntity.targetPos = movingEntity.position;
        movingEntity.velocity = Vector2.zero;
        
    }
}
