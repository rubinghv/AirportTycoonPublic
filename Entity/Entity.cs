using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	int id_number;
	public string nameType = "";
	public string entityName = "";

	public string GetType() 
	{
		return nameType;
	}

	public string GetName() 
	{
		return entityName;
	}

	protected void Start()
	{
		id_number = GridBuilder.GetEntityID();
	}

	public int GetID() 
	{
		// if id number hasn't been set for some reason, set it now
		if (id_number == 0)
			id_number = GridBuilder.GetEntityID();

		return id_number;
	}

	// ------------------------------------- Saving -------------------------------------

	public virtual void Save (string filename, string tag) {
		ES2.Save(id_number, filename + tag + "entityID");	
		ES2.Save(nameType, filename + tag + "nametype");
		ES2.Save(entityName, filename + tag + "entityName");
	}

	public virtual void Load (string filename, string tag) {
		id_number = ES2.Load<int>(filename + tag + "entityID");	
		nameType = ES2.Load<string>(filename + tag + "nametype");
		entityName = ES2.Load<string>(filename + tag + "entityName");
	}

}
