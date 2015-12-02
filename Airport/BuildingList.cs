using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This class exists because we don't want to setup references with Airport for every class (Employee or Person)
 * that needs to find a building.
 * 
 * There is still duplicate functionality with buildings stored in areaBuildings, which are in turn stored in Airport
 * Should leave areaBuildings in Airport but buildings should only really need to exist here.
 * 
 */

public static class BuildingList {
	
	static List<Building> buildingList;
	static List<BuildingPassenger> employeeBuildingList;

	static Airport airport;

	public static void Setup(Airport new_airport)
	{
		buildingList = new List<Building>();
		employeeBuildingList = new List<BuildingPassenger> ();
		airport = new_airport;
	}

	/*
	 * 	Get buildings with Entity's variable: nameType
	 * 	This way there is no need to create a different class for every type of building, 
	 * 	which seems overkill
	 */
	static List<BuildingPassenger> GetEmployeeBuildingsOfType(string type_name) {
		List<BuildingPassenger> return_list = new List<BuildingPassenger>();
		// loop through all buildings
		foreach(Building loop_building in airport.GetAllBuildings ()) {
			if (loop_building.GetType () == type_name && 
				 	(loop_building is BuildingPassenger || loop_building is BuildingPassenger)) {
				return_list.Add ((BuildingPassenger) loop_building);
			}
		}

		if (return_list.Count == 0)
			Debug.Log("Couldnt find employee building of type: " + type_name);
	
		return return_list;
	}



	/*
	 * Return building of type that is closest to person
	 * 1) look for all buildings in search treshold (between min and max)
	 * 		a) if multiple buildings, find one with shortest queue size
	 * 		b) if one building, return that building
	 * 		c) if no buildings, multiply search treshold by 5 and search again
	 * 			
	 */
	public static BuildingPassenger GetPassengerBuilding(Person person, string type_name)
	{
		// parameters
		float searchTresholdMax = GridHelper.GetGridCellSize() * 50f;
		float searchTresholdMin = 0;
		float tresholdMultiplier = 5;

		List<BuildingPassenger> buildingEmployeeList = GetEmployeeBuildingsOfType (type_name);
		List<BuildingPassenger> queueCompareList = new List<BuildingPassenger> ();
		//Debug.Log ("Found " + buildingEmployeeList.Count + " buildings of type " + type_name);

		for (int i = 1; i <= 3; i++) { // 3 treshold search stages
			// loop through all biuldings
			foreach(BuildingPassenger loop_building in buildingEmployeeList) {
				// add if within min and max search treshold
				if (Vector2.Distance (loop_building.GetWorldPosition2(), person.position) < searchTresholdMax &&
				    Vector2.Distance (loop_building.GetWorldPosition2(), person.position) >= searchTresholdMin) {
					// also make sure there is space
					if (loop_building.PassengerPriority() >= 0) {
						queueCompareList.Add (loop_building);
					}
				}
			}

			//Debug.Log("loop = " + i + " found " + queueCompareList.Count + " buildings");

			if (queueCompareList.Count == 0) {
				searchTresholdMin = searchTresholdMax;
				searchTresholdMax = searchTresholdMax * tresholdMultiplier;
			} else if (queueCompareList.Count == 1) {
				return queueCompareList[0];
			} else if (queueCompareList.Count > 1) {
				// loop through buildings to find shortest queue
				BuildingPassenger shortestQueueBuilding = queueCompareList[0];
				foreach(BuildingPassenger loop_building in buildingEmployeeList) {
					// WRITE DEBUG HERE
					if (loop_building.PassengerPriority() > shortestQueueBuilding.PassengerPriority()) {
						shortestQueueBuilding = loop_building;
					}

					/*
					if (loop_building.GetQueueSize() < shortestQueueBuilding.GetQueueSize()) {
						shortestQueueBuilding = loop_building;
					}*/
				}
				return shortestQueueBuilding;
			} else {
				Debug.Log ("something went horribly wrong");
			}

		}

		return null;
	}

	/*
	 * 	Get buildings with Entity's variable: nameType
	 * 	This way there is no need to create a different class for every type of building, 
	 * 	which seems overkill
	 */
	static List<Building> GetBuildingsOfType(string type_name) {
		List<Building> return_list = new List<Building>();
		// loop through all buildings
		foreach(Building loop_building in airport.GetAllBuildings ()) {
			if (loop_building.GetType () == type_name) {
				return_list.Add (loop_building);
			}
		}

		return return_list;
	}

	public static Building GetClosestBuildingOfType(string type_name, Vector3 curPos)
	{
		List<Building> search_list = GetBuildingsOfType(type_name);
		float shortestDistance = 99999f;
		Building returnBuilding = null;

		foreach(Building building in search_list) {
			if (Vector3.Distance(building.transform.position, curPos) < shortestDistance) {
				shortestDistance = Vector3.Distance(building.transform.position, curPos);
				returnBuilding = building;
			}
		}

		return returnBuilding;

	}

	public static Building GetClosestBuildingOfType(string type_name, Vector3 curPos, bool Snappable)
	{
		List<Building> search_list = GetBuildingsOfType(type_name);
		float shortestDistance = 99999f;
		Building returnBuilding = null;

		foreach(Building building in search_list) {
			if (Vector3.Distance(building.transform.position, curPos) < shortestDistance) {
				if (building.CanSnap()) {
					shortestDistance = Vector3.Distance(building.transform.position, curPos);
					returnBuilding = building;
				}
			}
		}

		return returnBuilding;

	}

}
