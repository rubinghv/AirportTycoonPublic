using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 *	Building class that has generic solution for creating
 *	a small pathfinding grid for finding a way within the building 
 *
 *
 */
public class BuildingPassenger : Building {

	[Header("Building Passenger")]

	public float assistRate = 1.0f;

	public Transform edgeNodeParent;
	public Transform connectingParent;

	List<InternalBuildingNode> internalNodes = new List<InternalBuildingNode>(); // all nodes
	List<InternalBuildingNode> edgeNodes = new List<InternalBuildingNode>();	 // only edge nodes
	public List<InternalBuildingNode> entrances = new List<InternalBuildingNode>();
	//public List<InternalBuildingNode> employeeNodes = new List<InternalBuildingNode>();
	List<InternalService> passengerServices = new List<InternalService>();

	//public string[] categories = new string[0]; // initialized in inspector
	public string[] passengerBehaviour;
	//Dictionary<int, PassengerBehaviour> passengerBehaviours = new Dictionary<int, PassengerBehaviour>();

	protected override void Start()
	{
		base.Start ();
		
	}

	// might not need this if a generic BuildingPassenger is impossible
	public override Building CreateBuilding()
	{
		BuildingPassenger building = (BuildingPassenger) base.CreateBuilding ();
		building.SetupNodes();

		return building;
	}

	/*
	 *	Only do this when building is created!
	 */
	void SetupNodes()
	{
		BuildingNode node;
		List<BuildingNode> tempNodes = new List<BuildingNode>();
		List<PassengerServiceFactory> tempPassengerServices = new List<PassengerServiceFactory>();

		// add edge nodes to temp list
		foreach(Transform child in edgeNodeParent.transform) {
			node = child.gameObject.GetComponent<BuildingNode>();
			tempNodes.Add(node);

			// also add passenger service if it exists
			PassengerServiceFactory p_service = child.gameObject.GetComponent<PassengerServiceFactory>();
			if (p_service != null) {
				tempPassengerServices.Add(p_service);
			}
		}

		// add connecting nodes to temp list, and set up edge node connections
		foreach(Transform child in connectingParent.transform) {
			node = child.gameObject.GetComponent<BuildingNode>();
			node.SetupAutoConnection();
			node.SetupEdgeNodeConnections();
			tempNodes.Add(node);
		}

		// for each building node, create internal node to replace old one
		foreach(BuildingNode bNode in tempNodes) {
			bNode.internalNode = new InternalBuildingNode(bNode, this.transform.position, 
										bNode.transform.position, bNode.door, bNode.gameObject.name);
		}

		// loop last time to add internal node connections, and add to final list
		foreach(BuildingNode bNode in tempNodes) {
			bNode.internalNode.nodeConnections = bNode.GetInternalConnections();
			internalNodes.Add(bNode.internalNode);

			if (bNode.edgeNode)
				edgeNodes.Add(bNode.internalNode);
			if (bNode.entrance)
				entrances.Add(bNode.internalNode);

		}

		// also convert passengerService references to internal
		foreach(PassengerServiceFactory p_service in tempPassengerServices) {
			InternalService new_service = new InternalService(p_service);
			passengerServices.Add(new_service);
		}

		// destroy gameObjects
		DestroyOldGameObjects();

	}

	void DestroyOldGameObjects()
	{
		foreach (Transform child in edgeNodeParent.transform) {
     		GameObject.Destroy(child.gameObject);
 		}
 		foreach (Transform child in connectingParent.transform) {
     		GameObject.Destroy(child.gameObject);
 		}
 		GameObject.Destroy(edgeNodeParent.gameObject);
 		GameObject.Destroy(connectingParent.gameObject);
	}

	
	public Vector2[] GetPathToEndNode(InternalBuildingNode startNode, string category) {
		// first get random available end node
		List<InternalBuildingNode> list = GetEdgeNodes(category);

		if (list.Count > 0) {
			InternalBuildingNode randomNode = list[Random.Range(0, list.Count)];
			
			// then get path (SHOULD GET THE RIGHT ENTRANCE WHEN MULTIPLE LIKE IN GATE)
			Vector2[] path = BuildingPathfinding.CreatePath(startNode, randomNode);
			return path;
		}
		else {
			print("could not find any available edge nodes with category = " + category);
			return null;
		}
	}

	public Vector2[] GetPathToEndNode(InternalBuildingNode startNode, InternalBuildingNode endNode) {
		Vector2[] path = BuildingPathfinding.CreatePath(startNode, endNode);
		return path;
	}

	/*
	 *	Get edge node with position2
	 */
	public InternalBuildingNode GetEndNode(Vector2 position2) {
		foreach(InternalBuildingNode node in edgeNodes) {
			if (Vector2.Distance(position2, node.position2) < GridHelper.GetGridCellSize() * 0.01f) {
				return node;
			}
		}
		print("couldn't find internal building node with position = " + position2);
		return null;
	}


	public InternalBuildingNode GetEntrance(Person person, PassengerBehaviour behaviour) {
		InternalBuildingNode return_entrance = null;

		if (entrances.Count == 1)
			return entrances[0];

		for (int i = 0; i < entrances.Count; i++) {
			if (behaviour.CanVisit(entrances[i])) {
				// can visit this entrance, return it.
//				print("returning entrance = " + i);
				return entrances[i];
				// otherwise continue
			}
		}

		if (return_entrance == null)
			print("do not know what entrance to get");
		
		return return_entrance;
	}


	/*
	 *	EmployeeController gets employee priority to determine if its necessary
	 *	to send employee here. Higher number is higher priority. 
	 *
	 *	return 0 if no employees needed	
	 */
	public int EmployeePriority() 
	{
		int highestPriority = 0;
		foreach(InternalService p_service in passengerServices) {
			if (p_service.EmployeePriority() > highestPriority) {
				highestPriority = p_service.EmployeePriority();
			}
		}

		return highestPriority;
	}

	/*
	 *	EmployeeController assigns employee
	 */
	public void AssignEmployee(Employee employee)
	{
		//print("assigning employee");
		InternalService highestPriority = null;

		foreach(InternalService p_service in passengerServices) {
			if (!p_service.HasPerson(employee)) // make sure it hasn't been assigned yet to this service
				if (!p_service.HasEmployee()) {
					p_service.AssignEmployee(employee);
					return;
				}
		}
	}



	public int PassengerPriority()
	{
		int highestPriority = 0;
		foreach(InternalService p_service in passengerServices) 
			if (p_service.PassengerPriority() > highestPriority) 
				highestPriority = p_service.PassengerPriority();
		

		return highestPriority;
	}

	/*
	 *	Return service that has smallest queue and active employee
	 *	Also has to match category
	 */
	public InternalService GetPassengerService(string category)
	{
		InternalService service = null;
		foreach(InternalService p_service in passengerServices) {
			if (p_service.category == category) {
				if (service == null)
					service = p_service;
				else if (p_service.PassengerPriority() > service.PassengerPriority())
					service = p_service;
			}
		}


		if (service == null)
			print("no service found for passenger at building" + this.gameObject.name);

		return service;
	}

	/* 
	 *	Get the employee spot that needs an employee the most 
	 *	(should be based on queue of people)
	 */
	 public InternalService GetService(Person person) {
		foreach(InternalService p_service in passengerServices) 
			if (p_service.HasPerson(person)) 
				return p_service;
		
		print("Couldn't find passenger service, returning null");
	 	return null;
	 }

	/* 
	 *	Get all the edge nodes with certain category
	 *	that are also available
	 */
	protected List<InternalBuildingNode> GetEdgeNodes(string category) {
		List<InternalBuildingNode> returnlist = new List<InternalBuildingNode>();
		foreach(InternalBuildingNode node in edgeNodes) {
			if (node.GetCategory() == category && node.available) {
				returnlist.Add(node);
			}
		}
		return returnlist;
	}

	// --------------------------------- FOR PASSENGERS ---------------------------------
	
	public PassengerBehaviour VisitBuilding(Person person) {
		PassengerBehaviour behaviour = new PassengerBehaviour(this, person, passengerBehaviour);
		InternalBuildingNode entranceNode = GetEntrance(person, behaviour);
//		print("entrance node = " + entranceNode.position2);
		behaviour.SetEntrance(entranceNode);
		return behaviour;
	} 
	
}