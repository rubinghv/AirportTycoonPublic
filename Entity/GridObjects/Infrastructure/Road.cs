using UnityEngine;
using System.Collections;

public class Road : PathfindingObject {

	// connections have to be set to protected
	public Road topConnection;
	public Road rightConnection;
	public Road bottomConnection;
	public Road leftConnection;

	public Material roadMaterial;


	/*
	 *	Need to adjust gridObject's size and max array position
	 *
	 */
	protected void SetupPositionAndSize(bool adjust)
	{
		this.gameObject.GetComponent<BoxCollider> ().enabled = true;
		base.SetupPositionAndSize();
		UpdatePositionAndSize(adjust);
	}

	protected void UpdatePositionAndSize(bool adjust)
	{
		this.gameObject.GetComponent<BoxCollider> ().enabled = true;
		base.UpdatePositionAndSize();

		if (adjust) {
			size -= Vector2.one;
			grid_position_max -= Vector2.one;
		}
	}

	public override bool CanBuild(Vector2 arraySize)
	{
		SetupPositionAndSize();
		size -= Vector2.one;

		if (GridHelper.CanBuildGridObject(this, wallSnap))
			return true;
		else 
			return false;
	}

	public virtual GameObject CreateRoad(Road oldRoad)
	{
		//don't forget to set layer
		this.gameObject.layer = InterfaceController.LAYER_ROAD;

		// set old properties
		nameType = oldRoad.GetType();
		roadMaterial = oldRoad.roadMaterial;

		// reflect changes in grid
		SetupPositionAndSize();
		size -= Vector2.one;
		grid_position_max -= Vector2.one;
		GridHelper.BuildGridObject (this, true);

		/*print("min array = " + array_position_min);
		print("max array = " + array_position_max);
		print("size = " + size);*/

		return this.gameObject;
	}

	public virtual GameObject CreateRoad(Transform parent)
	{
		// instantiate new gameObject and then position and parent it
		GameObject new_go = (GameObject) Instantiate (this.gameObject);
		new_go.transform.position = this.transform.position;
		new_go.SetActive (true);
		new_go.transform.parent = parent;

		BoxCollider boxCollider = new_go.GetComponent<BoxCollider>();
		boxCollider.size = new Vector3(0.999f, 5f, 0.999f);

		// setup position and keep box collider enabled
		SetupPositionAndSize (true);

		//don't forget to set layer
		new_go.layer = InterfaceController.LAYER_ROAD;

		// reflect changes in grid
		Road new_road = new_go.GetComponent<Road>();
		new_road.SetupPositionAndSize();
		GridHelper.BuildGridObject (new_road);

		return new_go;
	}

	public virtual int GetRoadWidth()
	{
		print("GetRoadWidth() not implemented, needs override!");
		return 0;
	}
	public virtual int GetRoadLength()
	{
		print("GetRoadLength() not implemented, needs override!");
		return 0;
	}

	public virtual void UpdateRoad()
	{
		UpdateConnections(true);
		//enable box collider
		this.gameObject.GetComponent<BoxCollider> ().enabled = true;
		UpdateMaterials();
	}

	/*
	 *	Rotate texture by rotating the mesh, which means switching x and z and 
	 *	rotating by 90 degrees
	 */ 
 	protected float previousRotation = 999f;
	protected virtual void RotateMesh(float degrees)
	{
		if (previousRotation <= 360f) {
			//reset to original rotation
			this.transform.RotateAround(this.transform.position, this.transform.up, -previousRotation);
		}
		previousRotation = degrees;

		// always rotate
		this.transform.RotateAround(this.transform.position, this.transform.up, degrees);
	}

	protected virtual void UpdateConnections(bool updateRecursively)
	{
		print("need to implement!");
	}

	public void SetConnection(RoadCorner corner, Vector2 direction)
	{
		if (direction == new Vector2(0,1))
			bottomConnection = corner;
		else if (direction == new Vector2(1,0))
			leftConnection = corner;
		else if (direction == new Vector2(0,-1))
			topConnection = corner;
		else if (direction == new Vector2(-1,0))
			rightConnection = corner;
	}

	
	public void SetConnection(RoadStraight straight, Vector2 direction)
	{

		if (direction == new Vector2(0,1))
			bottomConnection = straight;
		else if (direction == new Vector2(1,0))
			leftConnection = straight;
		else if (direction == new Vector2(0,-1))
			topConnection = straight;
		else if (direction == new Vector2(-1,0))
			rightConnection = straight;

		UpdateMaterials();
	}


	/*
	 *	So that one road can call another when connecting, to make sure that connects
	 */
	public virtual void UpdateConnections()
	{
		UpdateConnections(false);
	}

	protected virtual void UpdateMaterials()
	{
		print("not implemented!");
	}

	public virtual Vector3 GetSnapGridCell(Vector2 direction, bool mouseDown, Vector3 pos)
	{
		print("not implemented!");
		return Vector3.zero;
	}
	
	// -------------------------------- for pathfinding --------------------------------

	public override PathfindingObject[] GetNeighbours() {
		PathfindingObject[] returnArray = new PathfindingObject[4];

		returnArray[0] = topConnection;
		returnArray[1] = rightConnection;
		returnArray[2] = bottomConnection;
		returnArray[3] = leftConnection;

		return returnArray;

	}




}
