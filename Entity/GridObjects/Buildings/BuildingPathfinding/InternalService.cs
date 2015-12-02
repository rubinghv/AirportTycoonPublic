using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InternalService : EntityNoMono {

	public string category;

	InternalBuildingNode queue_start;
	InternalBuildingNode queue_end;
	InternalBuildingNode passenger_node;
	InternalBuildingNode employee_node;

	Employee employee;
	Passenger passenger;
	bool employeeReady = false;
	bool passengerReady = false;

	List<Passenger> queue;
	int maxQueueSize;

	public InternalService(PassengerServiceFactory p_service) {
		queue = new List<Passenger>();

		// add all internal nodes
		queue_start = p_service.queueStart.internalNode;
		queue_end = p_service.queueEnd.internalNode;
		passenger_node = p_service.passengerNode.internalNode;
		employee_node = p_service.employeeNode.internalNode;

		category = p_service.category;
		maxQueueSize = p_service.maxQueueSize;
	}

	public Passenger GetPassenger() { return passenger; }

	/*
	 *	Called by PassengerBehaviour for Passenger when joining queue
	 */
	public InternalBuildingNode JoinQueue(Passenger passenger) {
		queue.Add(passenger);
		return queue_start;
	}

	/*
	 *	Caled by PassengerBehaviour when trying to figure out if path is possible
	 */
	public InternalBuildingNode JoinQueueNode() {
		return queue_start;
	}

	/*
	 *	Caled by PassengerBehaviour to find path to next place
	 */
	public InternalBuildingNode PassengerSpotNode() {
		return passenger_node;
	}

	public bool FirstInLine(Person person)
	{
		if (queue.Count > 0)
			if (queue[0].GetID() == person.GetID())
				return true;

		return false;
	}

	/*
	 *	Called by PassengerBehaviour for Passenger when getting first position
	 */
	public Vector2 GetQueuePosition(Passenger passenger) {
		if (queue.Count == 1)
			return passenger_node.position2;

		for (int i = 0; i < queue.Count; i++) {
		//foreach(Passenger _passenger in queue) {
			if (passenger.GetID() == queue[i].GetID()) {

				if (i == 0) // passenger at front of queue
					return passenger_node.position2;

				Vector2 queueTotal = queue_start.position2 - queue_end.position2;
				Vector2 queueDelta = queueTotal / (maxQueueSize - 1);

				Vector2 result = queue_end.position2 + ((i - 1)* queueDelta);
				return result;
			}
		}

		Debug.Log("PROBLEM - Couldn't find queue position!");
		return Vector2.zero;
	}

	/*
	 *	Passenger needs to know which is the best passenger service to visit
	 */
	public int PassengerPriority()
	{
		if (!HasEmployee())
			return 0;

		return maxQueueSize - queue.Count;

	}


	/*
	 *	Called by passengerBehavior for Employee or Passenger when arrived
	 *	at spot
	 */
	public bool ReadyForService(Person person)
	{
		if (employeeReady && passengerReady) {
			passenger = queue[0];
			return true;
		}

		if (person is Employee)
			employeeReady = true;
		else if (person is Passenger)
			passengerReady = true;

		return false;
	}

	public void AssistPassengerComplete()
	{
		//Debug.Log("finished assisting passenger!");
		passengerReady = false;
		// move queue
		MoveQueue();
	}

	void MoveQueue() {
		// first move on finished passenger
		passenger.MoveQueue(true);
		queue.Remove(passenger);
		passenger = null;

		// for each passenger, move to new queue position
		foreach(Passenger _passenger in queue) {
			//Vector2 new_position = GetQueuePosition(_passenger);
			_passenger.MoveQueue(false);
		}

	}

	public void AssignEmployee(Employee new_employee) {
		if (HasEmployee())	
			Debug.Log("assigning while another employee is still assigned");

		employee = new_employee;
	}

	/*
	 *	EmployeeController gets employee priority to determine if its necessary
	 *	to send employee here. Higher number is higher priority. 
	 *
	 *	return 0 if no employees needed	
	 */
	public int EmployeePriority() 
	{
		// TEMPORARY
		if (!HasEmployee())
			return 1;
		else
			return 0;
	}

	public bool HasEmployee() {
		if (employee == null)
			return false;
		else
			return true;
	}

	public InternalBuildingNode GetEmployeeSpot() {
		return employee_node;
	}

	public bool HasPerson(Person person) {
		if (employee != null)
			if (employee.GetID() == person.GetID())
				return true;

		if (passenger != null)
			if (passenger.GetID() == person.GetID())
				return true;

		return false;

	}

}
