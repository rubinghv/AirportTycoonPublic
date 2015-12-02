using UnityEngine;
using System.Collections;

public class EntityNoMono {

	protected int id_number;
	
	public EntityNoMono() 
	{
		id_number = GridBuilder.GetEntityID();
	}

	public int GetID() 
	{
		return id_number;
	}

}
