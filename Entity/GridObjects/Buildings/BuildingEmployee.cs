using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingEmployee : Building  {
	
	protected override void Start()
	{
		base.Start ();
	}

	public override Building CreateBuilding()
	{
		BuildingEmployee building_employee = (BuildingEmployee) base.CreateBuilding ();

		employeeList = new List<Employee> ();
		queueList = new List<Passenger> ();

		return building_employee;
	}

	// ------------------------------ Employee ----------------------------------
	 [Header("Employee")]

	public List<Employee> employeeList; //shouldn't be public (protected instead)
	public List<Transform> employeeSpots; //
	public Employee employeeType;

	public int NeedsEmployees()
	{
		return employeeSpots.Count - employeeList.Count;
	}

	public void AssignEmployee(Employee employee)
	{
		employeeList.Add (employee);
	}
	
	public Vector3 GetWorkDestination(Employee employee)
	{
		int index = employeeList.IndexOf (employee);

		foreach (Transform child in employeeSpots [index])
		{
			//print ("work destination = " + child.position);
			return child.position;
		}
		return Vector3.zero;
	}

	public Vector3 GetFinalWorkDestination(Employee employee)
	{
		int index = employeeList.IndexOf (employee);
		//print ("final work destination = " + employeeSpots [index].position);
		return employeeSpots[index].position;
	}

	// ------------------------------ Passengers ----------------------------------

	// queue
	 [Header("Queue")]

	public Transform queueEnter;
	public Transform queueStart;
	public Transform queueEnd;
	public int queuePositions;
	public List<Passenger> queueList;
	Vector2 queueDelta;

	// passenger spot and assitance
	[Header("Passenger")]

	public Transform passengerSpot;
	public Passenger passenger; // passenger that is in help position
	public float assistRate = 1.0f;

	// exit
	public Transform[] exitPath;

	public int GetQueueSize()
	{
		return queueList.Count;
	}

	public bool HasQueueSpace()
	{
		//Debug.Log ("queue size = " + queueList.Count + " and queue positions = " + queuePositions);
		if (queueList.Count < queuePositions)
			return true;
		else
			return false;
	}

	public virtual Vector2 GetEnterQueuePosition()
	{
		return new Vector2 (queueEnter.position.x, queueEnter.position.z);
	}

	public Vector2 EnterQueueAndGetPosition(Passenger passenger)
	{
		// get queue position, then add to queue
		float index = (float) queueList.Count;
		queueList.Add (passenger); 

		// calculate queueDelta
		queueDelta = new Vector2(queueStart.position.x, queueStart.position.z) - new Vector2(queueEnd.position.x, queueEnd.position.z);
		queueDelta = queueDelta / ((float)queuePositions - 1); 

		Vector2 returnPosition = new Vector2(queueEnd.position.x, queueEnd.position.z) + (queueDelta * index);

		return returnPosition;
	}

	
	public bool QueueMove(Passenger new_passenger)
	{
		if (passenger == null && queueList.Count > 0 && 
			queueList[0] == new_passenger && employeeList.Count > 0) {

			passenger = queueList[0];
			queueList.Remove(passenger);
			passenger.MoveToNextQueueSpot(new Vector2(passengerSpot.position.x, passengerSpot.position.z));

			for(int index = 0; index < queueList.Count; index++) {
				Vector2 new_position = new Vector2(queueEnd.position.x, queueEnd.position.z) + (queueDelta * index);
				queueList[index].MoveToNextQueueSpot(new_position);
			}
			return true;
		} else {
			return false;
		}
	}
	
	/*
	 *	Called by employee when ready to help a passenger
	 *	return passenger if ready for help, otherwise return null
	 */
	public Passenger AssistPassenger(Employee employee) 
	{
		if (passenger != null && employeeList.Count > 0)
			if (passenger.ReadyForAssitance()) 
				return passenger;
			
		return null;
	}

	/*
	 *	Called by employee when passenger assistance has been
	 * 	completed
	 *
	 */
	public void AssistPassengerComplete() 
	{
		passenger.ExitBuilding();
		passenger = null;
	}

	/*
	 *	Called by passenger to get exit path out of building
	 */
	public Vector2[] GetExithPath() 
	{
		Vector2[] returnPath = new Vector2[exitPath.Length];
		int k = 0;
		for (int i = exitPath.Length - 1; i >= 0; i--) {
			returnPath[i] = new Vector2(exitPath[k].position.x, exitPath[k].position.z);
			k++;
		}

		return returnPath;
	}

}
