using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(TreeSpawn))]

public class TreeSpawnInspector : Editor {

	TreeSpawn treeSpawn;
	//Vector3 rotation;

	public void OnEnable()
	{
		treeSpawn = (TreeSpawn)target;
	}

	public override void OnInspectorGUI()
	{
		//treeSpawn.trees_amount = EditorGUILayout.IntField ("Total trees:", treeSpawn.trees_amount);
		DrawDefaultInspector();

		//rotation = EditorGUILayout.Vector3Field("GameObject 1:", rotation);
		

		if(GUILayout.Button("Generate Meshes")) {
			treeSpawn.GenerateMeshes();
		}

		if (GUILayout.Button ("Destroy Instantiated Objects")) {
			treeSpawn.DestroySpawnedMeshes();
		}
	}
	/**
	void OnInspectorUpdate() {
		treeSpawn.rotateVector = rotation;
	}*/

}
