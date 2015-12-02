using UnityEngine;
using System.Collections;

public class InternalBuildingNode : EntityNoMono {

	// type of node
	bool edgeNode;
	bool connectingNode;
	bool entrance;
	//bool employee;

	// node identification
	string category;
	int number;
	public string name;

	public GameObject door;

	// positions
	Vector3 buildingPosition;
	public Vector2 position2 {
		get { return new Vector2(position3.x, position3.z); }
	}
	public Vector3 position3;

	public bool available = true; // can a passenger visit this node?

	public InternalBuildingNode(BuildingNode externalNode, Vector3 building_position, 
								Vector3 nodePosition, GameObject _door, string _name)
	{
		edgeNode = externalNode.edgeNode;
		connectingNode = externalNode.connectingNode;
		entrance = externalNode.entrance;
		//employee = externalNode.employee;

		category = externalNode.category;
		number = externalNode.number;

		buildingPosition = building_position;
		position3 = nodePosition;

		door = _door;
		name = _name;

	}
	public string GetCategory() {return category; }
	public void DrawLines()
	{
		for (int i = 0; i < nodeConnections.Length; i++) {
			if (connectingNode) {
				Debug.DrawLine(position3, nodeConnections[i].position3, Color.red, 10f);
			} else if (edgeNode) {
				Debug.DrawLine(position3, nodeConnections[i].position3, Color.blue, 10f);
			}
		}
	}



	// ------------------------------- pathfinding

	public InternalBuildingNode[] nodeConnections;
	InternalBuildingNode fakeParent;
	float hCost; 
	float gCost;

	public bool HasFakeParent()
	{
		if (fakeParent == null) 
			return false;
		else
			return true;
	}
	
	public void SetFakeParent(InternalBuildingNode pf_object) { fakeParent = pf_object;	}
	public InternalBuildingNode GetFakeParent() { return fakeParent; }

	public virtual InternalBuildingNode[] GetNeighbours() 
	{
		return nodeConnections;
	}

	public float GetHCost() { return hCost; }
	public float GetGCost() { return gCost; }

	public void SetHCost(float newCost) { hCost = newCost; }
	public void SetGCost(float newCost) { gCost = newCost; }

	
	public int GetGCostMultiplier()
	{
		if (HasFakeParent()) {
			Vector2 calculateHDistance = new Vector2(Mathf.Abs(position2.x - fakeParent.position2.x), 
													 Mathf.Abs(position2.y - fakeParent.position2.y));
			
			// convert to grid cell units instead of world position
			return (int) ((calculateHDistance.x + calculateHDistance.y) / GridHelper.GetGridCellSize());

		} else {
			return 0;
		}
	}

}
