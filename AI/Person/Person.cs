using UnityEngine;
using System.Collections;

public class Person : MovingEntity {

	[Header("Person")]
	public int state; // should be protected
	public PlacePerson placePersonInterface;
	public SpeechBubbleController speechBubbleController;

	protected Vector2 finalDestination;

	public static int STATE_WANDERING_WAITING = 1001;


	/*
	protected void Start()
	{	
		// don't do base start
		steeringCalculate = new SteeringCalculateWeightedSum(); // create one or the other
        steering = new SteeringPerson(this, steeringCalculate);
	}*/

	public void Setup() {
		steeringCalculate = new SteeringCalculateWeightedSum(); // create one or the other
        steering = new Steering(this, steeringCalculate);
	}

    protected override void Update()
    {
        base.Update();
        DefaultBehaviour(Time.deltaTime * TimeController.GameSpeed);

    }

    protected virtual void DefaultBehaviour(float deltaTime)
    {
        //Debug.Log("No default behavior implemented!");
    }

	public void PlacePersonButtonPress()
	{
		placePersonInterface.UpdateOn (this);
	}

	public bool IsPlaceable()
	{
		return GridHelper.GetGridCell (this.transform.position).gridCellPathfinding.IsWalkable ();
	}

	public virtual Person CreatePerson(Transform parentObject, Vector3 position)
	{
		// instantiate new gameObject and then position and parent it
		//this.gameObject.SetActive (false);
		GameObject new_go = (GameObject) Instantiate (this.gameObject);
		new_go.transform.position = new Vector3(position.x, this.transform.position.y, position.z);
		new_go.transform.parent = parentObject;
		new_go.SetActive (true);
		
		//don't forget to set layer
		new_go.layer = InterfaceController.LAYER_PERSON;

		// Add to vehicle list
		PersonList.AddPerson (new_go.GetComponent<Person> ());

		return new_go.GetComponent<Person> ();
	}

	/*
	 * 	Test if we have arrived at finalDestination 
	 * 
	 */
	protected bool HasArrived() {
        return HasArrived (GridHelper.GetGridCellSize() / 50f);
    }

    protected bool HasArrived(float distance) {
        return HasArrived(distance, finalDestination);
    }

    protected bool HasArrived(float distance, Vector2 destination) {
        if (Vector2.Distance (destination, this.position) < distance)
            return true;
        else
            return false;
    }

	/* 
	 *	Wait for @param seconds and then change state to exit state
	 *
	 */
	int waintingForTimeInSeconds = -1;
	void WaitForSeconds(int seconds, int exitState)
	{
		// if first call, do setup
		if (waintingForTimeInSeconds == -1) {
			waintingForTimeInSeconds = TimeController.TimeInSeconds + seconds;
		}

		// if current time has surpassed time we were waiting for
		if (TimeController.TimeInSeconds >= waintingForTimeInSeconds) {
			state = exitState;
			waintingForTimeInSeconds = -1;
		}
	}

	bool WaitForSeconds(int seconds)
	{
		// if first call, do setup
		if (waintingForTimeInSeconds == -1) {
			waintingForTimeInSeconds = TimeController.TimeInSeconds + seconds;
			return false;
		}

		// if current time has surpassed time we were waiting for
		if (TimeController.TimeInSeconds >= waintingForTimeInSeconds) {
			waintingForTimeInSeconds = -1;
			return true;
		}

		return false;
	}

	/*
	 *	Start wandering around
	 */
	int wanderingExitState = -1;
	protected void Wandering() { Wandering(0); }
	protected void Wandering(int exitState) 
	{
		// do setup if not initialized
		if (wanderingExitState == -1) {
			wanderingExitState = exitState;
			StartWandering();
			return;
		}

		if (HasArrived()) {
			steering.FullStop();
			if (WaitForSeconds(Random.Range(50,200))) {
				wanderingExitState = -1;
			}
		}



		
	}

	/* 
	 *	Start wandering around, either to a random grid cell around if current gridcell is walkable
	 *	Or straight seeking to nearby gridCell that is walkable
	 */
	void StartWandering()
	{
		float gridcell_range = 3f * GridHelper.GetGridCellSize();
		GridCell gridcell = GridHelper.GetGridCell(this.transform.position);

		// first test if current gridCell is walkable
		if (gridcell.gridCellPathfinding.IsWalkable()) {
			// if walkable, select random cell in vicinity and try to crate a path
			// 1. get new random position, try 10 times
			for (int i = 0; i < 10; i++) {
				float random_world_position_x = Random.Range(gridcell.worldPosition.x - gridcell_range, gridcell.worldPosition.x + gridcell_range);
				float random_world_position_y = Random.Range(gridcell.worldPosition.y - gridcell_range, gridcell.worldPosition.y + gridcell_range);
				Vector2 gridcell_world_position = new Vector2(random_world_position_x, random_world_position_y);


				// 2. check that destination gridcell is walkable
				if (!GridHelper.GetGridCell(new Vector3(gridcell_world_position.x, 0, gridcell_world_position.y)).gridCellPathfinding.IsWalkable())
					return;

				// 3. try to see if creating path is possible
				finalDestination = gridcell_world_position;
				if (steering.Visit(finalDestination))
					return; 

				// if not succesful, loop again
			}
  
		} else {
			// if not walkable, find a nearby cell that is walkable, and seek there
			for (int i = 0; i < 10; i++) {
				int random_grid_position_x = Random.Range((int)gridcell.gridPosition.x - 2, (int)gridcell.gridPosition.x + 2);
				int random_grid_position_y = Random.Range((int)gridcell.gridPosition.y - 2, (int)gridcell.gridPosition.y + 2);
				Vector2 gridcell_grid_position = new Vector2(random_grid_position_x, random_grid_position_y);

				if (GridHelper.GetGridCell(gridcell_grid_position).gridCellPathfinding.IsWalkable()) {
					// seek to this found gridcell
					finalDestination = GridHelper.GetGridCell(gridcell_grid_position).worldPosition;
					steering.Visit(finalDestination);
				}

			}
		}

		print("shouldnt get to this point, couldn't find way to wander");


	}

		// ------------------------------------- MOVING -------------------------------------
	public void VisitPosition(Vector2[] path) 
	{	
		steering.Visit(path);
		finalDestination = path[0];
	}

	public void VisitPosition(Vector3 position) 
	{	steering.Seek(position); }
	public void VisitPosition(Vector2 position) 
	{	finalDestination = position;
		steering.Seek(finalDestination); }

	public bool HasArrivedStop() {
		return HasArrivedStop(0.1f);
	}

	public bool HasArrivedStop(float gridCellMultiplier) {
		return HasArrivedStop(gridCellMultiplier, finalDestination);
	}

	public bool HasArrivedStop(float gridCellMultiplier, Vector2 destination) {
		if (HasArrived (GridHelper.GetGridCellSize() * gridCellMultiplier, destination)) {
			steering.FullStop();
			return true;
		} else {
			return false;
		}
	}



		// ------------------------------------- Saving -------------------------------------

	public override void Save (string filename, string tag) 
	{
		base.Save(filename, tag);

		ES2.Save(state, filename + tag + "state");
		//ES2.Save(personName, filename + tag + "personName");
		ES2.Save(finalDestination, filename + tag + "finalDestination");

	}

	public override void Load (string filename, string tag) 
	{
		base.Load(filename, tag);

		state = ES2.Load<int>(filename + tag + "state");
		//personName = ES2.Load<string>(filename + tag + "personName");
		finalDestination = ES2.Load<Vector2>(filename + tag + "finalDestination");

	}
}
