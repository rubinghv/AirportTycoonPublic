using UnityEngine;
using System.Collections;

public class Exit : Building {

	protected override void Start()
	{
		base.Start ();
	}

	
	public override Building CreateBuilding()
	{
		Exit exit = (Exit) base.CreateBuilding ();

		// gridCell -> areaBuilding -> add entrance to area building
		GetMinGridCell().GetAreaBuilding().AddExit(exit	);

		return exit;
	}


}
