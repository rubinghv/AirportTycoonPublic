using UnityEngine;
using System.Collections;

public class Employee : Person {


	public static int STATE_AVAILABLE = 1;
	public static int STATE_MOVING_TO_WORK = 2;
	public static int STATE_MOVING_TO_FINAL = 3;
	public static int STATE_WORKING = 4;
	public static int STATE_WORKING_IDLE = 5;

	public static int STATE_SERVICE_PASSENGER = 23;

	public static int STATE_MOVE_INSIDE_BUILDING = 6;
	public static int STATE_BUILDING_GET_MOVE = 7;
	public static int STATE_BUILDING_MOVE = 8;

	//[Header("Employee")]


	public int serviceQuality { get; private set; }
	//public int serviceQuality {
	//	get { return serviceQuality; }
	//	private set { serviceQuality = value; }
	public int serviceSpeed { get; private set; }
	public float assistRate = 1.0f;

	BuildingEmployee work_building;
	BuildingPassenger building;
	PassengerBehaviour behaviour;

	Passenger passenger;
	float passengerAssistPercentage = 0f; 

	string[] speechBubble_NoWorkBuilding = new string[] {"Where am I supposed to work?",
												  		 "I guess I'll wait around until I'm needed somewhere",
												  		 "I'm not needed anywhere!",
												  		 "I thought you had job openings..."};

	protected void Start()
	{
		Setup();
	}

    protected override void DefaultBehaviour(float deltaTime)
    {
        if (state == 0) {
			print ("something has gone wrong, not supposed to have state = 0");
		} else if (state == STATE_AVAILABLE) {
			// wait for employeeController to assign work
			Wandering(STATE_AVAILABLE);
			speechBubbleController.SendMessageContinuous(speechBubble_NoWorkBuilding, this, SpeechBubbleController.EMPLOYEE_NO_WORK_BUILDING);
		} else if (state == STATE_MOVING_TO_WORK) {
			speechBubbleController.RemoveMessageImmediate(this);
			/*
			if (Vector2.Distance(finalDestination, position) < GridHelper.GetGridCellSize() / 3) {
				finalDestination = new Vector2(work_building.GetFinalWorkDestination(this).x, work_building.GetFinalWorkDestination(this).z);
				steering.Arrive (finalDestination);
				state = STATE_MOVING_TO_FINAL;
			}*/

			if (HasArrived()) {
				state = STATE_MOVE_INSIDE_BUILDING;
			}


		} else if (state == STATE_MOVING_TO_FINAL) {
			if (HasArrived()) {
				steering.FullStop();
				state = STATE_WORKING;
			}
		} else if (state == STATE_WORKING) {
			if (passenger == null) {
				//passenger = work_building.AssistPassenger(this);
				//passengerAssistPercentage = 0;

			 } else {
				if(HelpPassenger(deltaTime)) {			
					// reset passenger and delete panel
					passenger = null;
					Object.Destroy(progressPanel.gameObject);

					// tell work buliding all about it
					work_building.AssistPassengerComplete();
				}
			}
		} else if (state == STATE_MOVE_INSIDE_BUILDING) {
			//behaviour = building.VisitBuilding(this);
			state = STATE_BUILDING_GET_MOVE;
		} else if (state == STATE_BUILDING_GET_MOVE) {
			if (behaviour.ExecuteBehaviour())
				state = STATE_BUILDING_MOVE;
			else // it's finished
				state = 0; // TEMPORARY
		} else if (state == STATE_BUILDING_MOVE) {
			if (behaviour.BehaviorCompleted(deltaTime)){
				state = STATE_BUILDING_GET_MOVE;
			}
		}

		if (state == STATE_SERVICE_PASSENGER) {
			if(HelpPassenger(deltaTime)) {			
				// reset passenger and delete panel
				passenger = null;
				Object.Destroy(progressPanel.gameObject);

				// tell work buliding all about it
				work_building.AssistPassengerComplete();
			}

		}

    }

	public override Person CreatePerson(Transform parentObject, Vector3 position)
	{
		Employee employee = (Employee) base.CreatePerson (parentObject, position);
		employee.ResetState ();
		return employee;
	}

	public void ResetState()
	{
		state = STATE_AVAILABLE;
	}

	public bool HasWork()
	{

		if (work_building == null && state == STATE_AVAILABLE)
			return false;
		else
			return true;
	}

	/*
	public void AssignWorkBuilding(BuildingEmployee building_employee)
	{
		work_building = building_employee;

		finalDestination = new Vector2(work_building.GetWorkDestination (this).x, work_building.GetWorkDestination (this).z);
		steering.Visit (finalDestination);

		state = STATE_MOVING_TO_WORK;
	}*/

	public void AssignWorkBuilding(BuildingPassenger building_employee)
	{
		building = building_employee;
		behaviour = building.VisitBuilding(this);

		finalDestination = behaviour.GetEntrancePosition();
		steering.Visit(finalDestination);

		state = STATE_MOVING_TO_WORK;
	}


	/*
	 *	Help passenger that has been assigned	
	 *	return true if helping is complete, otherwise return false
	 */
	PassengerProgressPanel3D progressPanel;
	public bool HelpPassenger(float deltaTime) {
		// if no progress panel, create
		//if (progressPanel == null && passenger != null)
		//	progressPanel = passenger.progressPanel.CreatePassengerProgress(passenger);

		passengerAssistPercentage += deltaTime * building.assistRate;
		//print("helping percentage = " + passengerAssistPercentage);

		// dont' forget to update progressPanel
		progressPanel.UpdatePassengerProgress(passengerAssistPercentage / 100f);

		if (passengerAssistPercentage >= 100) {
			passenger = null;
			Object.Destroy(progressPanel.gameObject);
			return true;
		} else
			return false;
	}

	public bool HelpPassenger(Passenger _passenger, float deltaTime) {
		if (passenger == null) {
			passenger = _passenger;
			passengerAssistPercentage = 0;
			progressPanel = passenger.progressPanel.CreatePassengerProgress(passenger);
		}

		return HelpPassenger(deltaTime);
	}


	// ------------------------------------- Saving -------------------------------------

	public override void Save (string filename, string tag) 
	{
		base.Save(filename, tag);

		//ES2.Save(state, filename + tag + "state");


	}

	public override void Load (string filename, string tag) 
	{
		base.Load(filename, tag);

		//state = ES2.Load<int>(filename + tag + "state");

	}
}
