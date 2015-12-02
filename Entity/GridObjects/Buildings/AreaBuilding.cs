using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AreaBuilding : GridObject {

	public static int TYPE_DEPARTURES = 1;
	public static int TYPE_ARRIVALS = 2;
	public static int TYPE_TERMINAL = 3;
	public static int TYPE_SECURITY = 4;

	protected int buildingType;
	protected List<Building> buildingList = new List<Building> ();
	protected List<Entrance> entrances = new List<Entrance> ();
	protected List<Exit> exits = new List<Exit> ();

	BuildingFactory buildingFactory;

	public void Setup(int building_type, BuildingFactory _buildingFactory)
	{
		base.Setup ();
		buildingType = building_type;
		buildingFactory = _buildingFactory;
	}

	public int GetBuildingType()
	{
		return buildingType;
	}

	public void AddBuilding(Building building)
	{
		buildingList.Add (building);
		building.transform.parent = this.transform;
	}

	public List<Building> GetBuildingList()
	{
		return buildingList;
	}

	public List<BuildingEmployee> GetEmployeeBuildingList()
	{	// DEPRECIATED ( REPLACED BELOW) -- DELETE WHEN WORKING
		List<BuildingEmployee> returnList = new List<BuildingEmployee> ();

		foreach (Building building in buildingList) 
			if (building is BuildingEmployee)
				returnList.Add ((BuildingEmployee)building);

		return returnList;
	}

	public List<BuildingPassenger> GetPassengerBuildingList()
	{
		List<BuildingPassenger> returnList = new List<BuildingPassenger> ();

		foreach (Building building in buildingList) 
			if (building is BuildingPassenger)
				returnList.Add ((BuildingPassenger)building);

		return returnList;
	}


	/* 
	 *	Called by entrance when it's build
	 */
	public void AddEntrance(Entrance entrance) { entrances.Add(entrance); }
	public List<Entrance> GetEntrances() { return entrances; }

	/* 
	 *	Called by exit when it's build
	 */
	public void AddExit(Exit exit) { exits.Add(exit); }
	public List<Exit> GetExits() { return exits; }


	// ------------------------------------- Saving -------------------------------------

	public override void Save (string filename, string tag) {
		base.Save(filename, tag);

		ES2.Save(buildingType, filename + tag + "buildingType");
		ES2.Save(buildingList.Count, filename + tag + "buildingListSize");

		int counter = 0;
		foreach (Building building in buildingList)  {
			building.Save(filename, tag + "building" + counter);
			counter++;
		}	

		// don't forget entrances and exits
		foreach (Building building in entrances)  {
			building.Save(filename, tag + "building" + counter);
			counter++;
		}

		// don't forget entrances and exits
		foreach (Building building in exits)  {
			building.Save(filename, tag + "building" + counter);
			counter++;
		}
	
	}

	public override void Load (string filename, string tag) {
		base.Load(filename, tag);

		buildingType = ES2.Load<int>(filename + tag + "buildingType");	
		int buildingListSize = ES2.Load<int>(filename + tag + "buildingListSize");

		string type;
		Vector3 middlePosition, rotation;
		for (int i = 0; i < buildingListSize; i++) {
			// load building type and position so we can create building
			type = ES2.Load<string>(filename + tag + "building" + i + "nametype");
			middlePosition = ES2.Load<Vector3>(filename + tag + "building" + i + "realMeshPosition");
			rotation = ES2.Load<Vector3>(filename + tag + "building" + i + "rotation");

			Building building = buildingFactory.BuildBuilding(middlePosition, type, rotation);
			building.Load(filename, tag + "building" + i);
		}


	
	}
}
