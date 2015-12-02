using UnityEngine;
using System.Collections;

public class Entrance : Building {

	protected override void Start()
	{
		base.Start ();
	}

	
	public override Building CreateBuilding()
	{
		Entrance entrance = (Entrance) base.CreateBuilding ();

		// gridCell -> areaBuilding -> add entrance to area building
		GetMinGridCell().GetAreaBuilding().AddEntrance(entrance	);

		return entrance;
	}


}
