#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class TreeSpawn : MonoBehaviour {
	public int trees_amount;
	public float minimum_distance;

	public List<GameObject> treeMeshes = new List<GameObject>();
	public GameObject distributeMesh;
	public Vector3 rotation;

	// temp
	float y_height = 0.85f; // because pivot point is still not in right location

	// private
	List<GameObject> treesSpawned = new List<GameObject> ();
	int tree_mesh_index = 0;


	/**
	#if UNITY_EDITOR
	void OnEnable() {
		if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
			this.enabled = false;
		} else {
			// editor code here!
			//Debug.Log("happening");
		}
	}
	#endif
	*/


	Bounds bounds;

	public void GenerateMeshes() {
		print ("rotate = " + rotation);

		// first get the bounds
		bounds = distributeMesh.GetComponent<MeshCollider>().bounds;

		for (int i = 0; i < trees_amount; i++) {
			Vector2 position_new;
			// try generating up to a 100 times
			for (int k = 0; k < 100; k++) {
				// set new position to a random number somewhere between the min and max bounds of the mesh
				position_new = new Vector2(Random.Range (bounds.min.x, bounds.max.x), Random.Range (bounds.min.z, bounds.max.z));
				if (PositionValid (position_new) && RaycastValid(position_new))
				{	
					// create new mesh and add to list
					CreateMesh(position_new);
					break;
				} 
				else if (k == 99)
				{
					Debug.Log ("Couldn't find a random coordinate (too many trees)");
				}
			}
		
		}


		Debug.Log ("Building Objects with " + trees_amount + " trees!");
	}

	/**
	 * 	Check if @param position is minimum_distance from all the trees in the already
	 * 	generated trees in treeSpawned
	 * 
	 */
	bool PositionValid(Vector2 position) {
		foreach (GameObject tree in treesSpawned) {
			Vector2 treePosition = new Vector2(tree.transform.position.x, tree.transform.position.z);
			if (Vector2.Distance(position, treePosition) < minimum_distance)
				return false;
			else
				continue;
		}

		return true;
	}

	Vector3 hitPos;

	bool RaycastValid(Vector2 position) {
		// reset
		hitPos = Vector3.zero;

		Vector3 start_position = new Vector3 (position.x, 50, position.y);
		RaycastHit hit;
		int layerMask = 1 << 8;



		if (Physics.Raycast (start_position, -transform.up, out hit, 100, layerMask)) {
			hitPos = hit.point;
			Debug.DrawLine(start_position, hitPos, Color.red, 100);
			return true;

		}
		else
			return false;
	}


	void CreateMesh(Vector2 position) {
		// create game object, set position and parent
		GameObject new_mesh = PrefabUtility.InstantiatePrefab (treeMeshes [tree_mesh_index]) as GameObject;
		new_mesh.transform.position = new Vector3 (position.x, hitPos.y + y_height, position.y);
		new_mesh.transform.parent = this.transform;

		print ("y hit =" +  hitPos.y);

		// rotate 
		Vector3 randomRotation = new Vector3 (Random.Range (0, rotation.x), Random.Range (0, rotation.y), Random.Range (0, rotation.z));
		new_mesh.transform.Rotate (randomRotation, Space.World);

		// add to list
		treesSpawned.Add (new_mesh);

		// add to index
		if (tree_mesh_index >= treeMeshes.Count - 1)
			tree_mesh_index = 0;
		else
			tree_mesh_index++;

	}

	public void DestroySpawnedMeshes()
	{
		foreach (GameObject tree in treesSpawned) {
			DestroyImmediate (tree);
		}

		treesSpawned = new List<GameObject> ();
	}
}


#endif
