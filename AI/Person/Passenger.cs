using UnityEngine;
using System.Collections;

public class Passenger : Person {
	
	[Header("Passenger")]

	// state above 1000 is reserved for person class
	// outgoing passenger
	// spawn -> checkin -> security -> gate
	public static int STATE_SPAWNED = 1;
	public static int STATE_SECURITY = 2;
	public static int STATE_GATE_SETUP = 3;
	public static int STATE_GATE = 4;
	public static int STATE_GATE_BOARDING_QUEUE = 5;
	public static int STATE_GATE_EXIT = 6;
	public static int STATE_BOARDING_ENTERING_AIRPLANE = 7;
	public static int STATE_BOARDING_COMPLETE = 8;
	public static int STATE_TEST_NEW_UPDATE = 10;

	public static int STATE_TEST = -1;
	public static int STATE_BUILDING = 55;

	// incoming passenger
	public static int STATE_AIRPLANE_WAITING = 20;
	public static int STATE_AIRPLANE_EXITING = 21;
	public static int STATE_GATE_EXITING = 22;
	public static int STATE_GATE_EXITING_COMPLETE = 23;
	public static int STATE_CUSTOMS = 24;
	public static int STATE_CUSTOMS_COMPLETE = 25;
	public static int STATE_EXIT = 50;
	public static int STATE_EXIT_GOTO = 51;


	// passenger building
	public static int STATE_PASSENGER_BUILDING = 99;

	public int state_passenger_building = 0;
	public static int STATE_PASSENGER_BUILDING_GOTO = 91;
	public static int STATE_PASSENGER_BUILDING_GOTO_FINAL = 92;
	public static int STATE_PASSENGER_BUILDING_QUEUE_ENTER = 93;
	public static int STATE_PASSENGER_BUILDING_QUEUE_WAITING = 94;
	public static int STATE_PASSENGER_BUILDING_SPOT_GOTO = 95;
	public static int STATE_PASSENGER_BUILDING_SPOT = 96;
	public static int STATE_PASSENGER_BUILDING_EXIT = 97;

	bool startedBoardingInterrupt = false;
	public static int STATE_GATE_GOTO = 101;
	public static int STATE_GATE_GOTO_ENTER = 102;
	public static int STATE_GATE_GOTO_SEAT = 103;
	public static int STATE_GATE_SEAT_ROTATE = 104;
	public static int STATE_GATE_SEAT = 105;
	public static int STATE_GATE_GOTO_QUEUE = 106;
	public static int STATE_GATE_GOTO_QUEUE_ENTER = 107;

	string[] speechBubble_NoCheckIn = new string[] {"Where can I check-in?",
													"I can't find any place to check-in." };
	string[] speechBubble_NoSecurity = new string[] {"Where can I find security?",
													"I can't find any security." };
	string[] speechBubble_NoCustomns = new string[] {"Where can I find customns?",
													"I can't find any customns." };
	Flight flight;

	public PassengerProgressPanel3D progressPanel;

	// Use this for initialization
	void Start () {
		base.Start ();
	}

	protected override void DefaultBehaviour (float deltaTime)
	{
		OutgoingPassengerBehavior(deltaTime);
		IncomingPassengerBehavior();

		if (state == STATE_BUILDING) {
			VisitBuildingUpdate(deltaTime);
		} else if (state == STATE_TEST)
		{
			base.Start ();
			state = STATE_SPAWNED;
		}

	}

	void OutgoingPassengerBehavior(float deltaTime)
	{
		/*
		if (state == STATE_TEST_NEW_UPDATE) {
			//if (VisitBuilding("restaurant", STATE_SPAWNED))
			if (VisitBuilding("Customs", STATE_SPAWNED))
				state = STATE_BUILDING;
			else {
				Wandering(STATE_TEST_NEW_UPDATE);
				speechBubbleController.SendMessageContinuous(speechBubble_NoCheckIn, this, 
											SpeechBubbleController.PASSSENGER_NO_CHECK_IN);
			}
		} */
		if (state == STATE_SPAWNED) {
			//if (VisitPassengerBuilding("Check-in", STATE_SECURITY))
			//	state = STATE_PASSENGER_BUILDING;
			if (VisitBuilding("Check-in", STATE_SECURITY))
				state = STATE_BUILDING;
			else {
				Wandering(STATE_SPAWNED);
				speechBubbleController.SendMessageContinuous(speechBubble_NoCheckIn, this, 
											SpeechBubbleController.PASSSENGER_NO_CHECK_IN);
			}
		//} else if (state == STATE_PASSENGER_BUILDING) {
		//	VisitPassengerBuildingUpdate();
		// 	return;
		} else if (state == STATE_SECURITY) {
			if (VisitBuilding("Security", STATE_GATE))
				state = STATE_BUILDING;
			else {
				Wandering(STATE_SPAWNED);
				speechBubbleController.SendMessageContinuous(speechBubble_NoSecurity, this, 
											SpeechBubbleController.PASSSENGER_NO_SECURITY);
			}

		} else if (state == STATE_GATE) {
			if (VisitBuilding("Gate", STATE_GATE_EXIT))
				state = STATE_BUILDING;


		} else if (state == STATE_GATE_SETUP) {
			state_passenger_building = STATE_GATE_GOTO;
			VisitGateUpdate();
			state = STATE_GATE;
		} else if (state == STATE_GATE) {
			VisitGateUpdate();
		} else if (state == STATE_GATE_BOARDING_QUEUE) {
//			VisitPassengerBuildingUpdate();
		} else if (state == STATE_GATE_EXIT) {
			// dont' forget to leave bench
			//bench.RemovePassenger(this); depreciated
			Vector2[] enterPath = flight.GetAirplane().GetEnterPath();
			steering.Visit(enterPath);
			finalDestination = steering.GetFinalDestination();

			state = STATE_BOARDING_ENTERING_AIRPLANE;
		} else if (state == STATE_BOARDING_ENTERING_AIRPLANE) {
			if (HasArrived()) 
				state = STATE_BOARDING_COMPLETE;
		} else if (state == STATE_BOARDING_COMPLETE) {
			flight.FinishedBoarding(this);
			this.gameObject.SetActive(false);
		}
	}

	void IncomingPassengerBehavior()
	{
		if (state == STATE_AIRPLANE_WAITING)
			; // do nothing, waiting to get exit path from flight
		else if (state == STATE_AIRPLANE_EXITING) {
			if (HasArrived(GridHelper.GetGridCellSize() / 5f)) {
				steering.Visit(gate.GetEnterPath());
				finalDestination = steering.GetFinalDestination();
				state = STATE_GATE_EXITING;
			}
		} else if (state == STATE_GATE_EXITING) {
			if (HasArrived()) {
				state = STATE_CUSTOMS;
				//print("exiting complete!");
			}
		} else if (state == STATE_CUSTOMS) {
			//if (VisitPassengerBuilding("Customs", STATE_CUSTOMS_COMPLETE))
			if (VisitBuilding("Customs", STATE_EXIT))
				state = STATE_BUILDING;
			else {
				Wandering(STATE_SPAWNED);
				speechBubbleController.SendMessageContinuous(speechBubble_NoCustomns, this, 
											SpeechBubbleController.PASSSENGER_NO_CUSTOMNS);
			}
		/*} else if (state == STATE_CUSTOMS_COMPLETE) {
			state = STATE_EXIT;*/
		} else if (state == STATE_EXIT) {
			finalDestination = PassengerController.GetExitPosition(this);
			steering.Visit(finalDestination);

			state = STATE_EXIT_GOTO;
		} else if (state == STATE_EXIT_GOTO) {
			if (HasArrived()) {
				PassengerController.RemovePassengers(this);
				Object.Destroy(this.gameObject);
			}
		}

	}

	// -------- visit Passenger Building
	//public BuildingEmployee visitBuilding;
	public BuildingPassenger visitBuilding;

	int exit_state = -1;

	bool VisitPassengerBuilding(string type_name, int new_exit_state)
	{
		return VisitPassengerBuilding(type_name, STATE_PASSENGER_BUILDING_GOTO, new_exit_state);
	}

	bool VisitPassengerBuilding(string type_name, int start_state, int new_exit_state)
	{
		visitBuilding = BuildingList.GetPassengerBuilding (this, type_name);

		if (visitBuilding == null) {
			return false;
		} else {
			//finalDestination = visitBuilding.GetEnterQueuePosition(); DISABLED FOR PASSENGER BUILDING
			steering.Visit(finalDestination);
			state_passenger_building = start_state;
			exit_state = new_exit_state;

			// don't forget to remove eventual message
			speechBubbleController.RemoveMessageImmediate(this);

			return true;
		}
	}

	void VisitPassengerBuildingUpdate() 
	{
		if (state_passenger_building == STATE_PASSENGER_BUILDING_GOTO)
		{
			// test one last time if there is no better building to go to
			if (HasArrived(GridHelper.GetGridCellSize() * 10)) {
				VisitPassengerBuilding(visitBuilding.GetType(), STATE_PASSENGER_BUILDING_GOTO_FINAL, exit_state);
			}
		
		}
		if (state_passenger_building == STATE_PASSENGER_BUILDING_GOTO_FINAL)
		{
			if (HasArrived(GridHelper.GetGridCellSize() / 5)) {
				//finalDestination = visitBuilding.EnterQueueAndGetPosition(this); DISABLED FOR PASSENGER BUILDING
				steering.Arrive(finalDestination);
				state_passenger_building = STATE_PASSENGER_BUILDING_QUEUE_ENTER;
			}

		} else if (state_passenger_building == STATE_PASSENGER_BUILDING_QUEUE_ENTER) {
			if (HasArrived()) {
				steering.FullStop ();
				finalDestination = new Vector2 (99999f, 99999f);
				state_passenger_building = STATE_PASSENGER_BUILDING_QUEUE_WAITING;
			}
		} else if (state_passenger_building == STATE_PASSENGER_BUILDING_QUEUE_WAITING) {
			// if (visitBuilding.QueueMove(this)) { DISABLED FOR PASSENGER BUILDING
			//	state_passenger_building = STATE_PASSENGER_BUILDING_SPOT_GOTO; DISABLED FOR PASSENGER BUILDING
			//}

			if (HasArrived(GridHelper.GetGridCellSize() / 5)) {
				steering.FullStop ();
				finalDestination = new Vector2 (99999f, 99999f);
			}

		} else if (state_passenger_building == STATE_PASSENGER_BUILDING_SPOT_GOTO) {
			if (HasArrived(GridHelper.GetGridCellSize() / 5)) {
				state_passenger_building = STATE_PASSENGER_BUILDING_SPOT;
			}
		} else if (state_passenger_building == STATE_PASSENGER_BUILDING_SPOT) {
			// control to building and employee
		}  else if (state_passenger_building == STATE_PASSENGER_BUILDING_EXIT) {
			if (HasArrived(GridHelper.GetGridCellSize() / 5)) {
				state = exit_state;
			}

		}
	}


	// -------- visit Passenger Building
	BuildingPassenger buildingPassenger;
	PassengerBehaviour behaviour;
	int buildingExitState = -1;

	int state_passenger = 0;
	static int STATE_BUILDING_GOTO = 511;
	static int STATE_BUILDING_GOTO_FINAL = 512;
	static int STATE_BUILDING_GET_MOVE = 513;
	static int STATE_BUILDING_MOVE = 514;

	bool VisitBuilding(string type_name, int _exit_state)
	{
		// get building
		buildingPassenger = (BuildingPassenger) BuildingList.GetPassengerBuilding (this, type_name);

		if (buildingPassenger == null) {
			print("could not find building)");
			return false;
		} else  {
			behaviour = buildingPassenger.VisitBuilding(this);
			finalDestination = behaviour.GetEntrancePosition();
			steering.Visit(finalDestination);
			state_passenger = STATE_BUILDING_GOTO;
			buildingExitState = _exit_state;

			// don't forget to remove eventual message
			speechBubbleController.RemoveMessageImmediate(this);
			return true;
		}
	}

	void VisitBuildingUpdate(float deltaTime) 
	{
		if (state_passenger == STATE_BUILDING_GOTO)
		{
			// test one last time if there is no better building to go to
			//if (HasArrived(GridHelper.GetGridCellSize() * 10)) {
			//	VisitBuilding(buildingPassenger.GetType(), buildingExitState);
				state_passenger = STATE_BUILDING_GOTO_FINAL;
			//}
		} else if (state_passenger == STATE_BUILDING_GOTO_FINAL) {
			if (HasArrived()) {
				state_passenger = STATE_BUILDING_GET_MOVE;
				//behaviour = buildingPassenger.VisitBuilding(this);
			}
		} else if (state_passenger == STATE_BUILDING_GET_MOVE) {
			if (behaviour.ExecuteBehaviour())
				state_passenger = STATE_BUILDING_MOVE;
			else // it's finished
				state = buildingExitState;
		} else if (state_passenger == STATE_BUILDING_MOVE) {
			if (behaviour.BehaviorCompleted(deltaTime)){
				state_passenger = STATE_BUILDING_GET_MOVE;
			}
		}
	}

	/* 
	 *	Called by passenger service when needing to move queue along
	 */
	public void MoveQueue(bool serviceCompleted) {
		behaviour.MoveQueue(serviceCompleted);
	}


	// OBSOLETE
	public void MoveToNextQueueSpot(Vector2 destination)
	{
		finalDestination = destination;
		steering.Arrive(destination);
	}

	/*
	 *	Check if the passenger is ready to be helped by employee
	 * 	
	 *	Employee Building and employee need to know this
	 */
	public bool ReadyForAssitance() {
		if (state_passenger_building == STATE_PASSENGER_BUILDING_SPOT)
			return true;
		else
			return false;

	}

	public void ExitBuilding() {
		Vector2[] exitPath = null; // visitBuilding.GetExithPath(); DISABLED FOR PASSENGER BUILDING
		steering.Visit(exitPath);
		finalDestination = exitPath[0];
		state_passenger_building = STATE_PASSENGER_BUILDING_EXIT;
	}

	public Gate gate;
	public Bench bench;

	void VisitGateUpdate() 
	{	
		// if the boarding process has started, skip ahead past benches 
		if (startedBoardingInterrupt && (state_passenger_building != STATE_GATE_GOTO_ENTER && 
				   state_passenger_building != STATE_GATE_GOTO_SEAT &&
				   state_passenger_building != STATE_GATE_SEAT_ROTATE)) {

			// if already entering benches, wait to get to seating part
			if (state_passenger_building == STATE_GATE_SEAT) {
				StartBoarding();
			} else {
				state_passenger_building = STATE_GATE_GOTO_QUEUE;
				finalDestination = new Vector2(this.transform.position.x, this.transform.position.z);
			}
			startedBoardingInterrupt = false;
		}

		if (state_passenger_building == STATE_GATE_GOTO) {
			finalDestination = gate.GetEnterBenchesPosition();
			steering.Visit(finalDestination);
			state_passenger_building = STATE_GATE_GOTO_ENTER;
		} else if (state_passenger_building == STATE_GATE_GOTO_ENTER) {
			if (HasArrived(GridHelper.GetGridCellSize() / 5)) {
				bench = gate.AddToBench(this);
				if (bench != null) {
					Vector2[] exitPath = bench.GetBenchEnterPath(this);
					steering.Visit(exitPath);
					finalDestination = exitPath[0];
					state_passenger_building = STATE_GATE_GOTO_SEAT;
				} 
			}
		} else if (state_passenger_building == STATE_GATE_GOTO_SEAT) {
			if (HasArrived(GridHelper.GetGridCellSize() / 10f)) {
				steering.FullStop ();
				finalDestination = new Vector2 (99999f, 99999f);
				state_passenger_building = STATE_GATE_SEAT_ROTATE;
			}
		} else if (state_passenger_building == STATE_GATE_SEAT_ROTATE) {
			 iTween.RotateTo(gameObject,iTween.Hash(
			 	"rotation", bench.GetSeatRotation(this),
			 	"easetype", iTween.EaseType.easeInOutQuad,
			 	"time",1.0f));
			state_passenger_building = STATE_GATE_SEAT;
		} else if (state_passenger_building == STATE_GATE_SEAT) {
	 		// on seat, waiting for boarding
	 	} else if (state_passenger_building == STATE_GATE_GOTO_QUEUE) {
	 		if (HasArrived(GridHelper.GetGridCellSize())) {
	 			// go to enter queue position
	 			//finalDestination = gate.GetEnterQueuePosition(); DISABLED FOR PASSENGER BUILDING
				steering.Seek(finalDestination);

	 			state_passenger_building = STATE_GATE_GOTO_QUEUE_ENTER;
	 		}
	 	} else if (state_passenger_building == STATE_GATE_GOTO_QUEUE_ENTER) {
	 		
	 		hasArrivedCounter++;
	 		
	 		if (hasArrivedCounter > 200) {
	 			print("stuck on has arrived (" + GetID() + ")");
	 			print("this position = " + this.position + " | targetPos = " + targetPos + " | finalDestination = " + finalDestination);
	 			finalDestination = this.position;
	 		}

	 		if (HasArrived(GridHelper.GetGridCellSize())) {
	 			//print("has arrived");
				// send back to visitPassengerUpdate() because that will do the whole queue thing (except for exit path?)
	 			visitBuilding = gate;
	 			exit_state = STATE_GATE_EXIT;

	 			//finalDestination = visitBuilding.EnterQueueAndGetPosition(this); DISABLED FOR PASSENGER BUILDING
				steering.Arrive(finalDestination);

				state_passenger_building = STATE_PASSENGER_BUILDING_QUEUE_ENTER;
				state = STATE_GATE_BOARDING_QUEUE;
	 		}


	 	} 
	}



	int hasArrivedCounter = 0;
	/* 
	 *	Setup passenger after creating a new passenger
	 */
	public void SetupPassenger(Flight _flight, Gate _gate)
	{
		flight = _flight;
		gate = _gate;
	}

	/* 
	 *	Passenger is already in place at entrance, make visible
	 *	and set state to start finding check in
	 */
	public void SpawnAtEntrance()
	{
		this.gameObject.SetActive(true);
		state = STATE_SPAWNED;
	}

	/*
	 *	Called by flight when gate starts boarding and passenger needs to go to
	 *	beginning of benches
	 */
	public void StartBoarding()
	{ 
		if (behaviour == null) 
			print("Passenger does not have behaviour, probably because it isn't at building yet");
		else
			behaviour.NextBehaviour();

	}

	/*
	 * Called by passenger behaviour so that passenger can start entering airplane
	 */
	public void EnterAirplane()
	{ 
		state = STATE_GATE_EXIT;
	}

	// ----------------------------------- outgoing -----------------------------------

	/*
	 *	Create flight connection and wait until Flight tell spassenger to leave airplane
	 */
	public void SpawnAndWait(Flight _flight, Gate _gate)
	{
		SetupPassenger(_flight, _gate);
		state = STATE_AIRPLANE_WAITING;
	}

	/*
	 *	Set gate to exit gate, then leave airplane through airplane exit path
	 */
	public void ExitAirplane(Vector2[] exitPath, Gate _gate)
	{
		Setup();
		gate = _gate;

		steering.Visit(exitPath);
		finalDestination = steering.GetFinalDestination();

		state = STATE_AIRPLANE_EXITING;

	}

	/*
	 *	Called by StartBoarding() if passenger not currently on seat
	 */
	public void StartedBoarding()
	{	
		startedBoardingInterrupt = true;
	}


	// ----------------------------------- needs and happiness -----------------------------------

	public int need_food { get; private set; }
	public int need_shopping { get; private set; }
	public int need_services { get; private set; }

	public int need_restroom { get; private set; }
	public int need_impatience { get; private set; }


}
