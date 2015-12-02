using UnityEngine;
using System.Collections;

public class PathfindingObject : GridObject {

	PathfindingObject fakeParent;
	float hCost; 
	float gCost;

	// Use this for initialization

	public bool HasFakeParent()
	{
		if (fakeParent == null) 
			return false;
		else
			return true;
	}
	
	public void SetFakeParent(PathfindingObject pf_object) { fakeParent = pf_object;	}
	public PathfindingObject GetFakeParent() { return fakeParent; }

	public virtual PathfindingObject[] GetNeighbours() 
	{
		print("not implemented in base class!");
		return null;
	}

	public virtual void SetRoadConnection(Road road)
	{
		print("not implemented");
	}

	public float GetHCost() { return hCost; }
	public float GetGCost() { return gCost; }

	public void SetHCost(float newCost) { hCost = newCost; }
	public void SetGCost(float newCost) { gCost = newCost; }

	public int GetGCostMultiplier()
	{
		if (size.x >= size.y)
			return (int) size.x;
		else
			return (int) size.y;
	}


	public virtual Vector2 GetWaypoint()
	{
		return GetPositionCenter2(); 
	}

	public virtual Vector2[] GetWaypoints(PathfindingObject previousObject, PathfindingObject nextObject)
	{
		Vector2[] returnArray= new Vector2[1];
		returnArray[0] = GetPositionCenter2(); 
		return returnArray;
	}


}
