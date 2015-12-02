using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 
 *	This class will be called by entities (passengers, employees, flights) when trying
 *	to communicate something to the player.
 *
 *	This class will manage all the speech bubbles, make sure there aren't too many, and that
 *	there is a list of messages available that can be shown on top of the screen, 
 *	and filtered as well
 */

public class SpeechBubbleController : MonoBehaviour {

	protected class SpeechBubble {
		public string message;
		int category;
		public int messageType;
		public Entity sendingEntity;
		public SpeechBubblePanel3D panel;

		public bool waitingForRemoval = false;
		public int removalTimeInSeconds = 0;

		public bool bubbleCreated {
		get { if (panel == null) return false;
			  else return true; }
		}

		public SpeechBubble(string _message, int message_type, Entity sending_entity) {
			message = _message; category = GetCategory(sending_entity); messageType = message_type; sendingEntity = sending_entity;
		}

		/*
		 *	Based on who is trying to send message/create speech bubble, get the
		 *	matching category
		 */
		int GetCategory(Entity sendingEntity)
		{
			if (sendingEntity is Passenger)
				return CATEGORY_PASSENGER;
			else if (sendingEntity is Employee)
				return CATEGORY_EMPLOYEE;			
			else if (sendingEntity is Flight || sendingEntity is Airplane)
				return CATEGORY_FLIGHT;
			else {
				print("can't find category for this speech bubble message");
				return 0;
			}
		}
	}

	// message categories
	public static int CATEGORY_PASSENGER = 1;
	public static int CATEGORY_EMPLOYEE = 2;
	public static int CATEGORY_FLIGHT = 3;

	// messageType per entity
	public static int EMPLOYEE_NO_WORK_BUILDING = 201;

	public static int PASSSENGER_NO_CHECK_IN = 301;
	public static int PASSSENGER_NO_SECURITY = 302;
	public static int PASSSENGER_NO_CUSTOMNS = 303;


	public SpeechBubblePanel3D speechBubblePanel;
	public int minGridDistanceSameMessageType;
	public int speechRemovalTimeDelayInSeconds;

	Dictionary<int, SpeechBubble> speechBubbles = new Dictionary<int, SpeechBubble>();	// first int is entity id
	Dictionary<int, int> removeBubbles = new Dictionary<int, int>(); // first int is timeInMinutes, second is entityID
	Queue<SpeechBubble> removeQueue = new Queue<SpeechBubble>();

	// Update is called once per frame
	void Update () {
		// need to manage speechBubbles in here


		// take care of delayed removal
		RemoveMessageFinal();
	}

	/*
	 *	Create a new speech bubble and add to list
	 */
	public void SendMessage(string[] messages, Entity sendingEntity, int messageType) {

		// choose what message to use
		int randomMessageIndex = Random.Range(0, messages.Length);
		string message = messages[randomMessageIndex];

		// create speech bubble, set parameters
		SpeechBubble speechBubble = new SpeechBubble(message, messageType, sendingEntity);
		speechBubbles.Add(sendingEntity.GetID(), speechBubble);

		// THIS IS TEMPORARY - this should be managed by looking at the list
		// for now, send to panel to create new speech bubble panel
		if (ShouldCreateSpeechBubble(messageType, sendingEntity.transform.position) && 
									 !speechBubbles[sendingEntity.GetID()].bubbleCreated) {
			//print("creating speech bubble! size = " + speechBubbles.Count + " entity ID = " + sendingEntity.GetID());

			speechBubbles[sendingEntity.GetID()].panel = speechBubblePanel.CreateNewSpeechBubblePanel(sendingEntity.transform, 
																			  message,
																			  sendingEntity,
																			  this);
		} else 
			RemoveMessageDelayed(sendingEntity);
		
	}

	/*	Can be called many times by entity (in update) because this will check
	 * 	if speechBubble has already been created
	 */	
	public void SendMessageContinuous(string message, Entity sendingEntity, int messageType) 
	{	SendMessageContinuous(new string[]{message}, sendingEntity, messageType); } 

	public void SendMessageContinuous(string[] messages, Entity sendingEntity, int messageType) {
		if (speechBubbles.ContainsKey(sendingEntity.GetID())) 
			return;
		else 
			SendMessage(messages, sendingEntity, messageType);
	}

	/*
	 *	A person (or entity) can remove speech after completing showing speech bubble
	 *	(called from speech bubble panel)
	 *
	 *	Delay removal (therefore introducing timeout to next continuous speech bubble)
	 */
	public void RemoveMessageDelayed(Entity entity) 
	{
		
		if (speechBubbles.ContainsKey(entity.GetID())) {
			// make sure we're not already waiting for removal
			if (!speechBubbles[entity.GetID()].waitingForRemoval) {
				// setup delayed removal
				speechBubbles[entity.GetID()].waitingForRemoval = true;
				
				int removalTimeInSeconds = TimeController.TimeInSeconds + speechRemovalTimeDelayInSeconds;
				speechBubbles[entity.GetID()].removalTimeInSeconds = removalTimeInSeconds;

				removeQueue.Enqueue(speechBubbles[entity.GetID()]);

			}
		}
	}

	/*
	 *	A person (or entity) can remove speech after moving onto another state
	 *
	 */
	public void RemoveMessageImmediate(Entity entity)
	{
		if (speechBubbles.ContainsKey(entity.GetID())) {
			SpeechBubble speechBubble = speechBubbles[entity.GetID()];

			if (speechBubble.panel != null)
				speechBubble.panel.RemoveSpeechBubble();

			speechBubbles.Remove(entity.GetID());


		}
	}

	void RemoveMessageFinal() 
	{
		// check if queue size > 0?
		if (removeQueue.Count == 0)
			return;

		int time = TimeController.TimeInSeconds;
		// check removal list for time key
		if (removeQueue.Peek().removalTimeInSeconds <= time) {

			int entity_ID = removeQueue.Peek().sendingEntity.GetID();
			speechBubbles.Remove(entity_ID);
			removeQueue.Dequeue();

		}

	}

	/*
	 *	Test if we should add the speech bubble based on:
	 *	type -> if message type is in list, and
	 *	range -> if message nearby
	 *	then return false, else return true
	 */
	bool ShouldCreateSpeechBubble(int messageType, Vector3 messageLocation)
	{
		// loop through all speech bubbles (with panel or not)
		foreach(KeyValuePair<int, SpeechBubble> entry in speechBubbles) {
		    // check if type matches and panel is deployed
		    if (entry.Value.messageType == messageType && entry.Value.bubbleCreated) {
		    	// now look if this is close to messageLocation
		    	if (Vector3.Distance(messageLocation, entry.Value.sendingEntity.transform.position) < 
		    		minGridDistanceSameMessageType * GridHelper.GetGridCellSize())
		    		return false;
		    }
		}

		return true;
	}


	



}
