using UnityEngine;
using System.Collections;

public class TestRoadPathfinding : MonoBehaviour {

	public bool activate;

	public PathfindingObject startObject;
	public PathfindingObject endObject;
	
	// Update is called once per frame
	void Update () {
		
		if (activate) {
			TestPathfinding();
			activate = false;
		}
	}

	void TestPathfinding()
	{
		RoadPathfinding.CreatePath(startObject, endObject);
	}
}
