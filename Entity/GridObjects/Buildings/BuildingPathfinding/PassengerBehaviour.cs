using UnityEngine;
using System.Collections;

public class PassengerBehaviour : EntityNoMono {

	int currentBehaviour;
	string[] behaviours;
	bool employee;

	BuildingPassenger building;
	InternalBuildingNode currentNode;
	InternalBuildingNode entrance;
	Vector2 finalPosition; // if there is no node to check, like in queue
	
	GameObject door;
	bool doubleDoor = false;
	string currentTask;
	Person person;
	InternalService p_service;

	public PassengerBehaviour (BuildingPassenger _building, Person _person, 
							   string[] behaviourArray) {
		behaviours = behaviourArray;
		building = _building;
		currentBehaviour = 0;
		person = _person;

		if (person is Employee) {
			employee = true;
			behaviours = new string[] {	"visit employee", 
										"work",
										"exit" };
		} 
	}

	public void SetEntrance(InternalBuildingNode _entrance) {
		entrance = _entrance;
		currentNode = entrance;
	}

	public Vector2 GetEntrancePosition()
	{
		return entrance.position2;
	}

	public bool ExecuteBehaviour()
	{
		if (currentBehaviour >= behaviours.Length) {
			return false;
		}

		string behaviourString = behaviours[currentBehaviour];
		string[] data = behaviourString.Split(new char[] {' '});
		//Debug.Log("executing behaviour number: " + currentBehaviour);


		if (data[0] == "visit") { // eg. visit sink
			bool nodeAvailability = false; // will this edge node be available (for others) when visiting?
			if (data.Length > 2) { // eg. visit sink door    or    visit toilet door 2
				if (data[2] == "door") {
					Vector2[] path = Visit(data[1]); // get current node and path
					path[0] = new Vector2(currentNode.door.transform.position.x, 
										  currentNode.door.transform.position.z);
					person.VisitPosition(path);

					// differentiate between single pass through door or double pass
					if (data.Length > 3) 
						doubleDoor = true;
					//else
					//	doubleDoor = false;
					
					if (door == null)
						door = currentNode.door;

					currentTask = data[2]; // current task = door
					return true;				
				} else if (data[2] == "available") 
					nodeAvailability = true;
			}
			Visit(data[1], nodeAvailability);

		} else if (data[0] == "wait") { // eg. wait 10-15 or wait 12
			// if unlimited waiting (wait for )
			if (data.Length == 1) {
				currentTask = "wait infinite";
				return true;
			} else {
				string[] waitTime = data[1].Split(new char[] {'-'});
				if (waitTime.Length == 1)
					Wait(int.Parse(waitTime[0]));
				else if (waitTime.Length > 1) 
					Wait(Random.Range(int.Parse(waitTime[0]), int.Parse(waitTime[1])));
				else 
					Debug.Log("PROBLEM - Don't understand this wait string");
			}
		} else if (data[0] == "exit") { // eg. exit
			Exit();
		} else if (data[0] == "queue") {
			if (p_service == null) {
				// setup p_service and add to queue for service
				p_service = building.GetPassengerService(data[1]);
				InternalBuildingNode endNode = p_service.JoinQueue((Passenger) person);

				// set on way
				Vector2[] path = building.GetPathToEndNode(currentNode, endNode);
				person.VisitPosition(path);
				currentNode = endNode;

				currentTask = "visitQueue";
				return true;
			} else if (currentTask == "visitQueue") {
				finalPosition = p_service.GetQueuePosition((Passenger) person);
				person.VisitPosition(finalPosition);
				currentTask = "enteringQueue";
				return true;
			} else if (currentTask == "enteringQueue") {
				// waiting in queue
			} else {
				Debug.Log("waiting in queue, currentTask = " + currentTask);
				return true;
			}
		} else if (data[0] == "plane") { // go the airplane and enter
			Passenger passenger = (Passenger) person;
			passenger.EnterAirplane();
		} else {

			Debug.Log("can't find this command: " + data[0]);
		}
		
		doubleDoor = false;
		currentTask = data[0];
		//currentBehaviour++;

		return true;
		
	}

	

	public bool BehaviorCompleted(float deltaTime) 
	{
		// update door
		if (doorOpening && door != null) {
			if (iTween.Count(door) == 0) {
				CloseDoor(!doubleDoor);
				doorOpening = false;
			}
		}

		if (currentTask == "visit") {
			if (person.HasArrivedStop(0.1f, currentNode.position2)) {
				currentBehaviour++;
				return true;
			}
		} else if (currentTask == "visitQueue") {
			if (person.HasArrivedStop(0.1f, currentNode.position2)) {
				// if have reached final destination (ready for help)
				return true;
			}
		} else if (currentTask == "enteringQueue") {
			if (person.HasArrivedStop(0.1f, finalPosition)) {
				if (p_service.FirstInLine(person)) {
					p_service.ReadyForService(person);
				}
				return true;
			}
		} else if (currentTask == "door") {
			if (person.HasArrivedStop(0.5f)) {
				OpenDoor();
				Wait(30);
				currentTask = "doorWait";
			}
		} else if (currentTask == "doorWait") {
			if (TimeController.WaitForSeconds(this.GetID())) {
				person.VisitPosition(currentNode.position3);
				currentTask = "visit";
			}
		} else if (currentTask == "wait") {
			if (TimeController.WaitForSeconds(this.GetID())) {
				OpenDoor();
				currentNode.available = true;
				currentBehaviour++;
				return true;
			}
		} else if (currentTask == "wait infinite") {
			// wait forever
		} else if (currentTask == "exit") {
			if (person.HasArrivedStop()) {
				currentBehaviour++;
				return true;	
			}
		// ------------------ FOR EMPLOYEE ------------------
		} else if (currentTask == "work") {
			
			if (p_service.ReadyForService(person)) {
				// can help passenger
				Employee employee = (Employee) person;
				//Debug.Log("ready for service");
				if (employee.HelpPassenger(p_service.GetPassenger(), deltaTime)) {
					// helping is complete, notify passengerService
					p_service.AssistPassengerComplete();
				}	
			}
		} else if (currentTask == "queue") {

		} else {
			Debug.Log("don't know if this behaviour is complete: " + currentTask);
		}

		return false;
	}

	/* 
	 *	Called by flight through airplane when boarding starts
	 */
	public void NextBehaviour() {
		currentNode.available = true;

		currentBehaviour++;
		ExecuteBehaviour();
	}

	/* 
	 *	Called by passenger service through passenger to go somewhere after queue and service
	 */
	public void MoveQueue(bool serviceCompleted) {
		if (serviceCompleted)
			currentBehaviour++;
		else 
			currentTask = "visitQueue";

		// make sure currentNode is right one
		currentNode = p_service.PassengerSpotNode();
		ExecuteBehaviour();
	}

	//int canVisitCounter = 0;
	public bool CanVisit(InternalBuildingNode startNode)
	{
		// first get correct category
		string behaviourString = behaviours[currentBehaviour];
		string[] data = behaviourString.Split(new char[] {' '});
		string category = "";
		if (data.Length > 1)
			category = data [1];

		//Debug.Log("Category = " + category);

		// do test path
		Vector2[] path = null;
		InternalBuildingNode curNode = currentNode;
		currentNode = startNode;
		//p_service = building.GetService(person);
		if (employee && category == "employee") {
			p_service = building.GetService(person);
			path = building.GetPathToEndNode(currentNode, p_service.GetEmployeeSpot());
		} else {
			//Debug.Log("data[0] = " + data[0]);
			if (data[0] == "queue") {
				p_service = building.GetPassengerService(category);
				path = building.GetPathToEndNode(currentNode, p_service.JoinQueueNode());
				p_service = null;
				
			} else if (data[0] == "exit") {
//				Debug.Log("from " + curNode.name + " to " + startNode.name);
				path = building.GetPathToEndNode(curNode, startNode); // entrance (startNode) is destination

			} else {
				path = building.GetPathToEndNode(currentNode, category);
			}
		}
		
		currentNode = curNode;

		if (path == null)
			return false;
		else
			return true;
	}

	Vector2[] Visit(string category) {
		return Visit(category, false);
	}

	Vector2[] Visit(string category, bool nodeAvailable) {
		Vector2[] path;

		if (employee && category == "employee") {
			p_service = building.GetService(person);
			path = building.GetPathToEndNode(currentNode, p_service.GetEmployeeSpot());
		} else {
			path = building.GetPathToEndNode(currentNode, category);
		}

		person.VisitPosition(path);

		// set currentNode to next node
		currentNode = building.GetEndNode(path[0]);
		currentNode.available = nodeAvailable;

		return path;
	}

	bool Wait(int seconds) {
		return TimeController.WaitForSeconds(seconds,this.GetID());
		// current node stays the same
	}

	void Exit() {
		entrance = building.GetEntrance(person, this);
		Vector2[] path = BuildingPathfinding.CreatePath(currentNode, entrance);
		person.VisitPosition(path);
	}

	bool doorOpening = false;
	void OpenDoor() {
		if (door == null)
			return;

		iTween.RotateBy(door,iTween.Hash(
    			 	"amount", new Vector3(0,0,-0.23f),
    			 	"easetype", iTween.EaseType.easeInOutQuad,
    			 	"time", 1f));
		doorOpening = true;
	}

	void CloseDoor() { CloseDoor(true); }

	void CloseDoor(bool setNull) {
		if (door == null)
			return;

		iTween.RotateBy(door,iTween.Hash(
    			 	"amount", new Vector3(0,0,0.23f),
    			 	"easetype", iTween.EaseType.easeInOutQuad,
    			 	"time", 0.6f));

		if (setNull)
			door = null;
	}
	


}
