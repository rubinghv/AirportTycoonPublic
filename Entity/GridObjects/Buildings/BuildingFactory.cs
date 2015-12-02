using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingFactory : MonoBehaviour {

	public List<Building> buildingList = new List<Building>();
	public Transform buildingParent;
	
	void Start() {
		// add all buildings to list
		foreach(Transform child in buildingParent) {
			Building component = child.gameObject.GetComponent<Building>();
			if (component != null)
				buildingList.Add(component);
		}
	}

	public Building BuildBuilding(Vector3 position, string buildingType, Vector3 rotation)
	{
		
		foreach(Building building in buildingList) {
			if (building == null) {
				print("building is null");
				continue;
			}

			if (building.GetType() == buildingType) {

				//print("initial rotation = " + rotation);
				building.InitialRotation(rotation);

				building.PlaceBuildingUpdate(position);
				Building new_building = building.CreateBuilding();

				building.ResetRotation();

				return new_building;

			}
		}

		print("shouldn't be getting here for buildingType = " + buildingType);
		return null;


	}




}
