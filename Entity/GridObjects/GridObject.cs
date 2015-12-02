using UnityEngine;
using System.Collections;

public class GridObject : Entity {

	[Header("GridObject")]
	protected Vector2 grid_position_min;
	protected Vector2 grid_position_max;
	public Vector2 size;

	public Vector3 rotation = Vector3.up; 
	public Vector2 wallSnap;
	public BuildingToBuildingSnap buildingSnap;

	public GridCell GetMinGridCell () {
		return GridHelper.GetGridCell (grid_position_min);
	}

	public GridCell GetMaxGridCell () {
		return GridHelper.GetGridCell (grid_position_max);
	}

	public Vector2 GetGridPositionMin()
	{
		return grid_position_min;
	}

	public Vector2 GetGridPositionMax()
	{
		return grid_position_max;
	}

	public Vector2 GetWorldPosition2 () {
		return GridHelper.GetWorldPosition2 (grid_position_min);
	}
	
	public Vector2 GetWorldPosition3 (float height) {
		return GridHelper.GetWorldPosition3 (grid_position_min, height);
	}

	public Vector2 GetPositionCenter2 () {
		Vector3 tempVector = GetPositionCenter();
		return new Vector2(tempVector.x, tempVector.z);
	}

	public Vector3 GetPositionCenter () {
		Vector3 returnVector = GridHelper.GetWorldPosition3(grid_position_min) + ((GridHelper.GetWorldPosition3(grid_position_max) - GridHelper.GetWorldPosition3(grid_position_min)) / 2);
		
		if (size.x % 2 != 0) // x size is odd
			//returnVector.x += GridHelper.GetGridCellSize() / 2;
		if (size.y % 2 != 0) // x size is odd
			returnVector.z += GridHelper.GetGridCellSize() / 2;

		return returnVector;
	}
	
	public Vector2 GetSize() {
		return size;
	}


	// --------------------------- FOR PLACEMENT AND BUILDING ---------------------------

	public virtual void Setup()
	{
		SetupPositionAndSize ();
		//panel = GameObject.Find (panel_name).GetComponent<InfoPanel>();
	}

	/**
	 * 	Seperated so that it can be called to update variables after building has been moved
	 * 
	 */
	public virtual void SetupPositionAndSize()
	{
		// setup array positions and size

		//this.gameObject.GetComponent<BoxCollider> ().enabled = true;
		UpdatePositionAndSize ();
		//this.gameObject.GetComponent<BoxCollider> ().enabled = false;
	}

	/*
	 *	Don't update actual gameObject because this is for testing CanBuild
	 */
	public virtual void UpdatePositionAndSizeManual(int min_x, int min_y)
	{
		grid_position_min = new Vector2(min_x, min_y);
		grid_position_max = grid_position_min + size - Vector2.one;
	}

	protected virtual void UpdatePositionAndSize()
	{
		grid_position_min = GridHelper.GetGridPosition(this.gameObject.GetComponent<BoxCollider>().bounds.min);
		grid_position_max = GridHelper.GetGridPosition(this.gameObject.GetComponent<BoxCollider>().bounds.max);
		size = grid_position_max - grid_position_min + Vector2.one;
	}

	public virtual bool CanBuild()
	{
		return CanBuild(Vector2.zero);
	}

	public virtual bool CanBuild(Vector2 arraySize)
	{
		if (GridHelper.CanBuildGridObject(this, wallSnap))
			return true;
		else 
			return false;
	}

	public void Rotate()
	{
		this.transform.RotateAround(this.transform.position, this.transform.up, 90f);

		// also rotate wall snap so that it will stay correct alligned
		Vector2 new_wall_snap = new Vector2(0,0);

		if (wallSnap.y != 0)
			new_wall_snap.x = wallSnap.y;
			new_wall_snap.y = 0;
		if (wallSnap.x != 0)
			new_wall_snap.y = wallSnap.x * -1;
			if (wallSnap.y == 0)
			new_wall_snap.x = 0;

		wallSnap = new_wall_snap;	

		// and update rotate 
		if (rotation == Vector3.down)
			rotation = Vector3.left;
		else if (rotation == Vector3.left)
			rotation = Vector3.up;
		else if (rotation == Vector3.up)
			rotation = Vector3.right;
		else if (rotation == Vector3.right)
			rotation = Vector3.down;
	}

	/*
	 *	Reset rotation after placing building
	 */
	public void ResetRotation() {
		if (rotation == Vector3.up) {

		} else if (rotation == Vector3.right) {
			this.transform.RotateAround(this.transform.position, this.transform.up, 270f);
		} else if (rotation == Vector3.down) {
			this.transform.RotateAround(this.transform.position, this.transform.up, 180f);
		} else if (rotation == Vector3.left) {
			this.transform.RotateAround(this.transform.position, this.transform.up, 90f);
		}

		rotation = Vector3.up;
	}

	/*
	 *	Rotate into place after loading
	 */
	public void InitialRotation(Vector3 _rotation) 
	{
		if (_rotation == Vector3.up) {

		} else if (_rotation == Vector3.right) {
			this.transform.RotateAround(this.transform.position, this.transform.up, 90f);
		} else if (_rotation == Vector3.down) {
			this.transform.RotateAround(this.transform.position, this.transform.up, 180f);
		} else if (_rotation == Vector3.left) {
			this.transform.RotateAround(this.transform.position, this.transform.up, 270f);
		}

		rotation = _rotation;
	}


	public Vector3 GetRotation()
	{
		return rotation;
	}

	public int GetWallSnapX()
	{
		if (wallSnap.x == -1)
			return (int) grid_position_min.x;
		else if (wallSnap.x == 1)
			return (int) grid_position_max.x;
		else
			return 0;
	}

	public int GetWallSnapY()
	{
		if (wallSnap.y == -1)
			return (int) grid_position_min.y;
		else if (wallSnap.y == 1)
			return (int) grid_position_max.y;
		else
			return 0;
	}

	public void CancelPlacement()
	{
		this.gameObject.SetActive (false);
	}

	public void PlaceGridObjectUpdate(Vector3 pos)
	{
		if (buildingSnap == null) {
			Vector2 arrayPos = GridHelper.GetGridPosition (pos);
			this.transform.position = GridHelper.GetWorldPosition3(arrayPos, this.transform.position.y);
		} else {
			this.transform.position = buildingSnap.GetNewPosition(pos);
		}

		SetupPositionAndSize ();
	}

	public virtual GridObject CreateGridObject()
	{
		// instantiate new gameObject and then position and parent it
		GameObject new_go = (GameObject) Instantiate (this.gameObject);
		new_go.transform.position = this.transform.position;
		new_go.SetActive (true);

		// reflect changes in grid
		GridObject new_grid_object = new_go.GetComponent<GridObject>();
		new_grid_object.SetupPositionAndSize();
		GridHelper.BuildGridObject (new_grid_object);

		this.gameObject.GetComponent<BoxCollider> ().enabled = true;

		return new_grid_object;

	}

	public virtual bool CanSnap()
	{
		print("not implemented in child class!");
		return false;
	}

	//public void Highlight()
	//{
	//	panel.gameObject.SetActive(true);
	//	panel.Highlight (this);
	//}

	//public void Dehighlight ()
	//{
	//	panel.Dehighlight (this);
	//	panel.gameObject.SetActive(false);
	//}

	// ------------------------------------- Saving -------------------------------------

	public override void Save (string filename, string tag) {
		base.Save(filename, tag);

		ES2.Save(grid_position_min, filename + tag + "grid_position_min");	
		ES2.Save(grid_position_max, filename + tag + "grid_position_max");	
		ES2.Save(size, filename + tag + "grid_size");	

		ES2.Save(rotation, filename + tag + "rotation");	
		ES2.Save(wallSnap, filename + tag + "wallSnap");	
//		print("Save rotation = " + rotation);

	}

	public override void Load (string filename, string tag) {
		base.Load(filename, tag);

		grid_position_min = ES2.Load<Vector2>(filename + tag + "grid_position_min");	
		grid_position_max = ES2.Load<Vector2>(filename + tag + "grid_position_max");	
		size = ES2.Load<Vector2>(filename + tag + "grid_size");	

		rotation = ES2.Load<Vector3>(filename + tag + "rotation");	
		wallSnap = ES2.Load<Vector2>(filename + tag + "wallSnap");	

		this.gameObject.GetComponent<BoxCollider> ().enabled = true;
	}
	

}
