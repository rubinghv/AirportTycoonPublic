using UnityEngine;
using System.Collections;

public class Depot : Building {

	void Start ()
	{
		this.gameObject.SetActive (false);
	}

	public Depot ()
	{
		nameType = "Depot";
	}
	
	public override void Setup()
	{
		base.Setup ();
		//panel = GameObject.Find (panel_name).GetComponent<InfoHousePanel>(); NEED TO FILL THIS IN
		
	}

}
