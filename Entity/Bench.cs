using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // for FirstOrDefault

public class Bench : Entity {

	public List<Transform> benchSpotsLeft;
	public List<Transform> benchSpotsRight;

	public List<Transform> benchLeftPath;
	public List<Transform> benchRightPath;

	Dictionary<int, Passenger> passengersBenchLeft;
	Dictionary<int, Passenger> passengersBenchRight;

	Gate gate;

	void Start()
	{
		passengersBenchLeft = new Dictionary<int, Passenger>();
		passengersBenchRight = new Dictionary<int, Passenger>();
	}

	public void Setup(Gate new_gate)
	{
		gate = new_gate;
	}

	int GetLeftBenchSpaces()
	{
		return benchSpotsLeft.Count - passengersBenchLeft.Count;
	}

	int GetRightBenchSpaces()
	{	
		return benchSpotsRight.Count - passengersBenchRight.Count;
	}

	public int GetBenchSpaces()
	{
		return GetLeftBenchSpaces() + GetRightBenchSpaces();
	}

	public void AddPassenger(Passenger passenger)
	{
		//List<Passenger> currentBench;
		if (GetLeftBenchSpaces() >= GetRightBenchSpaces()) {
			//passengersBenchLeft[GetRandomSeatNumber(passengersBenchLeft)] = passenger;
			passengersBenchLeft.Add(GetRandomSeatNumber(passengersBenchLeft, benchSpotsLeft.Count), passenger);
		}
		else {
			//passengersBenchRight[GetRandomSeatNumber(passengersBenchRight)] = passenger;
			passengersBenchRight.Add(GetRandomSeatNumber(passengersBenchRight, benchSpotsRight.Count), passenger);
		}

		//print("added to bench");	

	}

	/*
	 *	Remove passenger from bench, this is called by passenger
	 *	when moving from gate to airplane
	 */
	public void RemovePassenger(Passenger passenger) 
	{

		if (passengersBenchLeft.ContainsKey(GetPassengerIndexLeftBench(passenger))) 
			passengersBenchLeft.Remove(GetPassengerIndexLeftBench(passenger));
		else if (passengersBenchRight.ContainsKey(GetPassengerIndexRightBench(passenger))) 
			passengersBenchRight.Remove(GetPassengerIndexRightBench(passenger));
		

	}

	int GetRandomSeatNumber(Dictionary<int, Passenger> passengers, int totalSpots)
	{
		List<int> possibleSeatNumbers = new List<int>();

		// fill list with all possible numbers
		for (int i = 0; i < totalSpots; i++) {
			possibleSeatNumbers.Add(i);
		}
		// remove seat numbers that are already taken
		foreach (int key in passengers.Keys) {	
			possibleSeatNumbers.Remove(key);
		}
		// select random one and return
		int randomIndex = Random.Range(0, possibleSeatNumbers.Count);
		return possibleSeatNumbers[randomIndex]; 

	}

	/*
	 *	Get the seat of the passenger on the left bench
	 *	If not on bench, return -1;
	 */
	int GetPassengerIndexLeftBench(Passenger passenger)
	{
		foreach(KeyValuePair<int, Passenger> entry in passengersBenchLeft) {
			if (entry.Value.GetID() == passenger.GetID())
				return entry.Key;
		}

		return -1;
	}

	/*
	 *	Get the passenger index of the passenger on the right bench
	 *	If not on bench, return -1;
	 */
	int GetPassengerIndexRightBench(Passenger passenger)
	{
		foreach(KeyValuePair<int, Passenger> entry in passengersBenchRight) {
			if (entry.Value.GetID() == passenger.GetID())
				return entry.Key;
		}

		return -1;
	}

	public Vector2[] GetBenchExitPath(Passenger passenger) 
	{
		Vector2[] enterPath = GetBenchEnterPath(passenger);
		Vector2[] exitPath = Helper.InvertArray(enterPath);

		return exitPath;
	}

	public Vector2[] GetBenchEnterPath(Passenger passenger) 
	{
		// First find which bench passenger is at
		bool leftBench = false;
		bool rightBench = false;
		int passengerIndex = -2;
		if (GetPassengerIndexLeftBench(passenger) >= 0) {
			passengerIndex = GetPassengerIndexLeftBench(passenger);
			leftBench = true;
		} else if (GetPassengerIndexRightBench(passenger) >= 0) {
			passengerIndex = GetPassengerIndexRightBench(passenger);
			rightBench = true;
		}


		// create path to return (and appropriate size)
		Vector2[] returnPath = null; 
		if (leftBench)
			returnPath = new Vector2[benchLeftPath.Count + 2];
		else if (rightBench)
			returnPath = new Vector2[benchRightPath.Count + 2];

		int reverse = -1;
		if (leftBench) {
			// add final passenger spot
			returnPath[0] = new Vector2(benchSpotsLeft[passengerIndex].position.x, benchSpotsLeft[passengerIndex].position.z);
			// loop already defined path
			reverse = benchLeftPath.Count + 1;
			for (int l = 0; l < benchLeftPath.Count; l++) {
				returnPath[reverse] = new Vector2(benchLeftPath[l].position.x, benchLeftPath[l].position.z);
				reverse--;
			}
			// add final spot before seat, depends on orientation of building
			if (gate.GetRotation() == Vector3.down || gate.GetRotation() == Vector3.up) 
				returnPath[1] = new Vector2(benchLeftPath[benchLeftPath.Count - 1].position.x, returnPath[0].y);
			else if (gate.GetRotation() == Vector3.left || gate.GetRotation() == Vector3.right) 
				returnPath[1] = new Vector2(returnPath[0].x, benchLeftPath[benchLeftPath.Count - 1].position.z);
			

		} else if (rightBench) {
			returnPath[0] = new Vector2(benchSpotsRight[passengerIndex].position.x, benchSpotsRight[passengerIndex].position.z);
			reverse = benchRightPath.Count + 1;
			for (int r = 0; r < benchRightPath.Count; r++) {
				returnPath[reverse] = new Vector2(benchRightPath[r].position.x, benchRightPath[r].position.z);
				reverse--;
			}

			if (gate.GetRotation() == Vector3.down || gate.GetRotation() == Vector3.up) 
				returnPath[1] = new Vector2(benchRightPath[benchRightPath.Count - 1].position.x, returnPath[0].y);
			else if (gate.GetRotation() == Vector3.left || gate.GetRotation() == Vector3.right) 
				returnPath[1] = new Vector2(returnPath[0].x, benchRightPath[benchRightPath.Count - 1].position.z);
		}

		return returnPath;

	}

	/*
	 *	Based on what the passenger's bench and buildings rotation,
	 *	return rotation so that passenger will face forward
	 */
	public Vector3 GetSeatRotation(Passenger passenger)
	{
		bool leftBench = false;
		bool rightBench = false;
		int passengerIndex = 0;
		if (GetPassengerIndexLeftBench(passenger) >= 0) {
			passengerIndex = GetPassengerIndexLeftBench(passenger);
			leftBench = true;
		} else if (GetPassengerIndexRightBench(passenger) >= 0) {
			passengerIndex = GetPassengerIndexRightBench(passenger);
			rightBench = true;
		}

		// get rotation based on bench and building position
		if (gate.GetRotation() == Vector3.up || gate.GetRotation() == Vector3.down) {
			if ((gate.GetRotation() == Vector3.up && leftBench) || 
				(gate.GetRotation() == Vector3.down && rightBench)) {
				return new Vector3(0, 270f, 0); //rotate to left
			} else if ((gate.GetRotation() == Vector3.up && rightBench) || 
				(gate.GetRotation() == Vector3.down && leftBench)) {
				return new Vector3(0, 90f, 0); // rotate to right
			}
		} else if (gate.GetRotation() == Vector3.right || gate.GetRotation() == Vector3.right) {
			if ((gate.GetRotation() == Vector3.right && leftBench) || 
				(gate.GetRotation() == Vector3.left && rightBench)) {
				return new Vector3(0, 0, 0);// rotate up
			} else if ( (gate.GetRotation() == Vector3.right && rightBench) || 
						(gate.GetRotation() == Vector3.left && leftBench)) {
				return new Vector3(0, 180f, 0);// rotate down
			}

		}

		return Vector3.zero;
	}


}
