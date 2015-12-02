using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Airport : MonoBehaviour {

	public string airportName = "default name";
	public static string airportGoName = "Airport";

	AreaBuildingFactory areaBuildingFactory;

	//List<Building> buildingList = new List<Building>();
	List<AreaBuilding> areaBuildings = new List<AreaBuilding>();
	List<BuildingOutside> outsideBuildings = new List<BuildingOutside>();

	public List<Runway> runwayList = new List<Runway>();
	/**
	 * 	Look for building components as children of the city gameobject and add them to buildingList
	 * 
	 */
	public void Setup (AreaBuildingFactory _areaBuildingFactory) {

		// Setup personList
		PersonList.Setup ();
		BuildingList.Setup (this);
		areaBuildingFactory = _areaBuildingFactory;

	}

	public void AddAreaBuilding(AreaBuilding areaBuilding)
	{
		areaBuildings.Add (areaBuilding);
	}

	public List<Building> GetAllBuildings()
	{
		List<Building> returnList = new List<Building> ();
		foreach (AreaBuilding area_building in areaBuildings) {
			returnList.AddRange (area_building.GetBuildingList());
		}

		//print ("buildingSize = " + returnList.Count);
		return returnList;
	}


	public List<BuildingEmployee> GetAllEmployeeBuildings() 
	{	// DEPRECIATED ( REPLACED BELOW) -- DELETE WHEN WORKING
		List<BuildingEmployee> returnList = new List<BuildingEmployee> ();
		foreach (AreaBuilding area_building in areaBuildings) {
			returnList.AddRange (area_building.GetEmployeeBuildingList());
		}
		
		return returnList;
	}

	public List<BuildingPassenger> GetAllPassengerBuildings()
	{
		List<BuildingPassenger> returnList = new List<BuildingPassenger> ();
		foreach (AreaBuilding area_building in areaBuildings) {
			returnList.AddRange (area_building.GetPassengerBuildingList());
		}
		
		return returnList;
	}

	/*
	 *	Get a list of all entrances from all area buildings
	 */
	 public List<Entrance> GetAllEntrances()
	 {
	 	List<Entrance> entrances = new List<Entrance>();

	 	foreach(AreaBuilding areaBuilding in areaBuildings) {
	 		List<Entrance> area_building_entrances = areaBuilding.GetEntrances();
	 		
	 		foreach(Entrance entrance in area_building_entrances) {
	 			entrances.Add(entrance);
	 		}
	 	}

	 	return entrances;
	 }


	 /*
	 *	Get a list of all entrances from all area buildings
	 */
	 public List<Exit> GetAllExits()
	 {
	 	List<Exit> exits = new List<Exit>();

	 	foreach(AreaBuilding areaBuilding in areaBuildings) {
	 		List<Exit> area_building_exits = areaBuilding.GetExits();
	 		
	 		foreach(Exit exit in area_building_exits) {
	 			exits.Add(exit);
	 		}
	 	}

	 	return exits;
	 }


	 /*
	  *
	  */
	public void AddOutsideBuilding(BuildingOutside outside_building)
	{
		outsideBuildings.Add(outside_building);
	}

	// ------------------------------------- Saving -------------------------------------
	
	public void Save (string filename) {
		ES2.Save(airportName, filename + "?tag=airportName");	
		ES2.Save(areaBuildings.Count, filename + "?tag=areaBuildingListSize");	

		
		int counter = 0;
		foreach (AreaBuilding area_building in areaBuildings) {
			// do something
			area_building.Save(filename, "?tag=areaBuilding" + counter);
			counter++;
		};
	}

	public void Load (string filename) {
		airportName = ES2.Load<string>(filename + "?tag=airportName");
		int areaBuildingListSize = ES2.Load<int>(filename + "?tag=areaBuildingListSize");

		string area_building_tag = filename + "?tag=areaBuilding";

		for (int i = 0; i < areaBuildingListSize; i++)
		{
			Vector2 minGridPos = ES2.Load<Vector2>(area_building_tag + i + "grid_position_min");
			Vector2 maxGridPos = ES2.Load<Vector2>(area_building_tag + i + "grid_position_max");
			Vector2 size = ES2.Load<Vector2>(area_building_tag + i + "grid_size");

			AreaBuilding new_area_building = areaBuildingFactory.CreateAreaBuilding(GridHelper.GetGridCell(minGridPos),
																					GridHelper.GetGridCell(minGridPos),
																					size, 0);
			new_area_building.Load(filename, "?tag=areaBuilding" + i);
		}
	}

}
