using UnityEngine;
using System.Collections;

public static class GridBuilder {

	static int entityIDcounter = 1000;

	public static GameObject CreateRectangle(Vector2 minArrayPos, Vector2 maxArrayPos, float height, Transform parent, bool addBoxCollider) 
	{	
		GameObject new_go = GameObject.Instantiate(GameObject.Find("sampleCube"));
		new_go.name = "CreatedObject" + GetEntityID().ToString();

		new_go.transform.parent = parent;

		// set size
		Vector2 size = maxArrayPos - minArrayPos + Vector2.one;

		// set scale
		new_go.transform.localScale = new Vector3 (size.x * GridHelper.GetGridCellSize (), 
			                                    new_go.transform.localScale.y * height, 
			                                   size.y * GridHelper.GetGridCellSize ());
		// set position
		new_go.transform.position = new Vector3 (
				GridHelper.GetWorldPosition3(minArrayPos).x + ((size.x * GridHelper.GetGridCellSize ()) / 2.0f) - GridHelper.GetGridCellSize() / 2,
				new_go.transform.position.y,
				GridHelper.GetWorldPosition3(minArrayPos).z + ((size.y * GridHelper.GetGridCellSize ()) / 2.0f) - GridHelper.GetGridCellSize() / 2);

		new_go.GetComponent<Renderer>().enabled = true;

		// box collider
		if (addBoxCollider) {
			BoxCollider boxCollider = new_go.AddComponent<BoxCollider>();
			boxCollider.size = new Vector3(boxCollider.size.x * 0.999f, boxCollider.size.y * 5, boxCollider.size.z * 0.999f);
		}

		return new_go;
	}

	public static int GetEntityID()
	{
		int returnID = entityIDcounter;
		entityIDcounter++;
		return returnID;
	}

}
