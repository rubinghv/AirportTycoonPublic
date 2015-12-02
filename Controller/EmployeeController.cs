using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmployeeController : MonoBehaviour {

	Airport airport;
 	static PersonFactory personFactory; // satic for load

	public void Setup(Airport new_airport, GameObject factoryGo)
	{
		airport = new_airport;
		personFactory = factoryGo.GetComponent<PersonFactory>();
	}

	void Update () {
		//ManageEmployees ();
		ManageEmployeesNew();
	}

	/*
	void ManageEmployees()
	{
		// first find any buildings that may need employees
		//List<BuildingEmployee> buildingList = airport.GetAllEmployeeBuildings ();

		Employee employee = GetFreeEmployee();

		if (employee == null) // if there is no free employees, return
			return;

		List<BuildingEmployee> needEmployeeBuildingList = new List<BuildingEmployee> ();

		foreach (BuildingEmployee building_employee in airport.GetAllEmployeeBuildings ()) {
			if (building_employee.NeedsEmployees() > 0) {
				needEmployeeBuildingList.Add (building_employee);
			}
		}

		if (needEmployeeBuildingList.Count == 0) // if there are no buildings that need employees, return
			return;

		// now find building that is closest
		float lowestDistance = 99999f;
		BuildingEmployee lowestDistanceBuilding = null;;

		foreach (BuildingEmployee building_employee in needEmployeeBuildingList) {
			if (Vector3.Distance (building_employee.transform.position, employee.transform.position) < lowestDistance) {
				lowestDistance = Vector3.Distance (building_employee.transform.position, employee.transform.position);
				lowestDistanceBuilding = building_employee;
			}
		}

		lowestDistanceBuilding.AssignEmployee(employee);
		employee.AssignWorkBuilding(lowestDistanceBuilding);
		print ("Assigned employee to building");

	}*/


	void ManageEmployeesNew()
	{
		// first find any buildings that may need employees
		//List<BuildingEmployee> buildingList = airport.GetAllEmployeeBuildings ();

		Employee employee = GetFreeEmployee();

		if (employee == null) // if there is no free employees, return
			return;

		List<BuildingPassenger> needEmployeeBuildingList = new List<BuildingPassenger> ();
		BuildingPassenger highestPriorityBuilding = null;
		int highestPriority = 0;

		foreach (BuildingPassenger building in airport.GetAllPassengerBuildings ()) {
			if (building.EmployeePriority() > 0) {
				needEmployeeBuildingList.Add (building);
			}

			if (building.EmployeePriority() > highestPriority) {
				highestPriority = building.EmployeePriority();
				highestPriorityBuilding = building;

			// if there is a building with same priority as current highest, 
			//there is not just one with highest priority, so search for closest
			} else if (building.EmployeePriority() == highestPriority)
				highestPriorityBuilding = null;
		}

		if (needEmployeeBuildingList.Count == 0) // if there are no buildings that need employees, return
			return;
		else if (highestPriorityBuilding != null) { // if there is only one building
			highestPriorityBuilding.AssignEmployee(employee);
			employee.AssignWorkBuilding(highestPriorityBuilding);
			print ("Assigned employee to building");
		} else {
			// there are multiple buildings
			// now find building that is closest
			float lowestDistance = 99999f;
			BuildingPassenger lowestDistanceBuilding = null;;

			foreach (BuildingPassenger building in needEmployeeBuildingList) {
				if (Vector3.Distance (building.transform.position, employee.transform.position) < lowestDistance) {
					lowestDistance = Vector3.Distance (building.transform.position, employee.transform.position);
					lowestDistanceBuilding = building;
				}
			}

			lowestDistanceBuilding.AssignEmployee(employee);
			employee.AssignWorkBuilding(lowestDistanceBuilding);
			print ("Assigned employee to building");
		}

	}


		
	Employee GetFreeEmployee()
	{
		// loop through all employees
		foreach(Employee employee in PersonList.GetEmployeeList())
			if (!employee.HasWork ()) 
				return employee;
		
		return null;
	}

	List<Employee> GetFreeEmployees()
	{
		List<Employee> returnList = new List<Employee> ();

		// loop through all employees
		foreach(Employee employee in PersonList.GetEmployeeList())
			if (!employee.HasWork ()) 
				returnList.Add (employee);
		
		return returnList;
	}

	// ------------------------------------- Saving -------------------------------------

	public static void Save (string filename, string tag) 
	{
		ES2.Save(PersonList.GetEmployeeList().Count, filename + tag + "employeeListSize");

		int counter = 0;
		foreach (Employee employee in PersonList.GetEmployeeList())  {
			employee.Save(filename, tag + "employee" + counter);
			counter++;
		}	

	
	}

	public static void Load (string filename, string tag) 
	{
		int employeeCount = ES2.Load<int>(filename + tag + "employeeListSize");

		for (int i = 0; i < employeeCount; i++) {
			string type = ES2.Load<string>(filename + tag + "employee" + i + "nametype");
			Vector3 transformPosition = ES2.Load<Vector3>(filename + tag + "employee" + i + "transformPosition");

			Employee employee = (Employee) personFactory.PlacePerson(transformPosition, type);
			employee.Load(filename, tag + "employee" + i);

		}
	}

}
