using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PassengerServiceFactory : MonoBehaviour {

	public BuildingNode queueStart;
	public BuildingNode queueEnd;
	public BuildingNode passengerNode;
	public BuildingNode employeeNode;

	public string category;

	InternalBuildingNode queue_start;
	InternalBuildingNode queue_end;
	InternalBuildingNode passenger_node;
	InternalBuildingNode employee_node;

	Employee employee;
	Passenger passenger;

	List<Passenger> queue = new List<Passenger>();
	public int maxQueueSize = 4;


	/*
	 *	Convert this node so the references are to internal nodes
	 */
	 /*
	public void ConvertNode()
	{
		queue_start = queueStart.internalNode;
		queue_end = queueEnd.internalNode;
		passenger_node = passengerNode.internalNode;
		employee_node = employeeNode.internalNode;
	}
	*/
	
	/*
	 *	Called by PassengerBehaviour for Passenger when joining queue
	 
	public InternalBuildingNode JoinQueue(Passenger passenger) {
		queue.Add(passenger);
		return queue_start;
	}

	
	 *	Passenger needs to know which is the best passenger service to visit
	 *
	public int PassengerPriority()
	{
		if (!HasEmployee())
			return 0;

		return maxQueueSize - queue.Count;

	}

	public void AssignEmployee(Employee new_employee) {
		if (HasEmployee())	
			print("assigning while another employee is still assigned");

		employee = new_employee;
	}

	
	 *	EmployeeController gets employee priority to determine if its necessary
	 *	to send employee here. Higher number is higher priority. 
	 *
	 *	return 0 if no employees needed	
	 
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

	*/



}
