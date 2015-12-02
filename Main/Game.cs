using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public GameObject factoryObject;
	public bool debugging;


	// Use this for initialization
	void Awake () {
		factoryObject.GetComponent<AirportFactory> ().Airport();

	}
	
	// Update is called once per frame
	void Update () {
			Pathfinding.debugging = debugging;
	}
}
