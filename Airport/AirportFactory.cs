using UnityEngine;
using System.Collections;

public class AirportFactory : MonoBehaviour {

	public GameObject airportGameObject;
	public GridProperties gridProperties;
	public EmployeeController employeeController;
	public AreaBuildingFactory areaBuildingFactory;
	// Update is called once per frame
	public void Airport () {
		Grid.Setup(gridProperties);
		Airport airport = airportGameObject.AddComponent<Airport> ();
		airport.Setup (areaBuildingFactory);

		employeeController.Setup (airport, this.gameObject);
	}
}
