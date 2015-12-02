using UnityEngine;
using System.Collections;

public class TooltipLabel : MonoBehaviour {

	UILabel label;

	void Start ()
	{
		label = this.gameObject.GetComponent<UILabel> ();
		print ("setting up");
	}

	public void ShowTooltip(string message, float seconds)
	{
		// test if we're already showing message
		if (this.gameObject.GetComponent<iTween>() != null)
		{
			// if so remove current itween and reset alpha
			Destroy(this.gameObject.GetComponent<iTween>());
			ResetAlpha ();
		}

		label.text = message;
		NGUITools.SetActive (label.gameObject, true);
		
		DelayedFadeOut(seconds, 1.0f);
		
	}

	float fade_seconds;

	void DelayedFadeOut(float display_seconds, float new_fade_seconds)
	{
		fade_seconds = new_fade_seconds;

		iTween.ValueTo(this.gameObject, iTween.Hash( // use delay to time displayed before disappearing
						"from", 1, "to", 1,
						"time", display_seconds, "easetype", "linear",
						"onupdate", "SetAlpha", "ignoretimescale", true,
						"oncomplete", "FadeOut"));

	}


	void FadeOut()
	{
		iTween.ValueTo(this.gameObject, iTween.Hash( // use delay to time displayed before disappearing
						"from", 1, "to", 0,
						"time", fade_seconds, "easetype", "linear",
						"onupdate", "SetAlpha", "ignoretimescale", true,
		                "oncomplete", "ResetAlpha"));
		
	}
	
	void SetAlpha(float newAlpha) 
	{
		label.alpha = newAlpha;
		
	}

	void ResetAlpha() 
	{
		label.alpha = 1;
		NGUITools.SetActive (label.gameObject, false);
		
	}



}
