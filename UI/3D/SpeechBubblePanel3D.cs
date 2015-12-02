using UnityEngine;
using System.Collections;

public class SpeechBubblePanel3D : Entity {

	public UILabel speechLabel;
	public UISprite speechSprite;
	public int fadeInSeconds;
	Entity sendingEntity;
	SpeechBubbleController controller;

	public SpeechBubblePanel3D CreateNewSpeechBubblePanel(Transform parent, string message, 
			Entity sendingEntity, SpeechBubbleController _controller)
	{

		GameObject new_go = (GameObject) Instantiate (this.gameObject);
		new_go.transform.parent = parent;
		new_go.transform.position = new Vector3(parent.position.x, new_go.transform.position.y, parent.position.z);

		// get new component, set message, and show
		SpeechBubblePanel3D new_component = new_go.GetComponent<SpeechBubblePanel3D>();
		new_component.StartSpeechBubble(message, fadeInSeconds, sendingEntity, _controller);

		return new_component;
	}

	void Update()
	{
		// if it's initialized
		UpdateSpeechBubble();
	}

	public void StartSpeechBubble(string message, int seconds, Entity _sendingEntity, SpeechBubbleController _controller)
	{
		speechLabel.text = message;
		fadeInSeconds = seconds;
		sendingEntity = _sendingEntity;
		controller = _controller;
		
		this.gameObject.SetActive(true);
	}

	void UpdateSpeechBubble()
	{
		if (TimeController.WaitForSeconds(fadeInSeconds, GetID())) {
			controller.RemoveMessageDelayed(sendingEntity);
			RemoveSpeechBubble();
		}
	}

	public void RemoveSpeechBubble()
	{
		Object.Destroy(speechSprite.gameObject);
		Object.Destroy(speechLabel.gameObject);
		Object.Destroy(this.gameObject);
	}

}
