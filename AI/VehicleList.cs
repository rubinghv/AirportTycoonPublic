using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class VehicleList : MonoBehaviour {
	
	public Transform vehicleContainer;
	static List<MovingEntity> MovingEntityList = new List<MovingEntity>(); // should be static
	
	// Use this for initialization
	void Start () {
		foreach (Transform child in vehicleContainer)
		{
			MovingEntityList.Add ((MovingEntity)child.gameObject.GetComponent(typeof(MovingEntity)));
		}
	}
	
	static public List<MovingEntity> GetNeighbors (Vector2 centerPos, float radius)
	{
		List<MovingEntity> neighbors = new List<MovingEntity>();
		
		foreach (MovingEntity vehicle in MovingEntityList)
			if (vehicle.position != centerPos)
				if (ContainsEllipse(centerPos, radius, vehicle.position) )
					neighbors.Add (vehicle);
		
		return neighbors;
	}
	
	
	static bool ContainsEllipse(Vector2 centerPoint, float radius, Vector2 currentPoint)
    {
        // (y - Yc)^2 / b^2 + (x - Xc)^2 / a^2 = 1
        float xDelta = (float)(Mathf.Pow((float)currentPoint.x - (float)centerPoint.x, 2) / Mathf.Pow(radius / 2.0f, 2));
        float yDelta = (float)(Mathf.Pow((float)currentPoint.y - (float)centerPoint.y, 2) / Mathf.Pow(radius / 2.0f, 2));

        if (xDelta + yDelta < 1)
            return true;
        else 
            return false; 
    }

    static public List<MovingEntity> GetMovingEntityList()
    {
        return MovingEntityList;
    }
}
