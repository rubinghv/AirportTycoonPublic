using UnityEngine;
using System.Collections;

public class Bathroom : BuildingPassenger {

	// Use this for initialization
	protected override void Start()
	{
		base.Start ();
		passengerBehaviour = new string[] {	"visit toilet door 2", 
											"wait 120-300",
											"visit sink",
											"wait 80",
											"exit" };
	}
	
	
}
