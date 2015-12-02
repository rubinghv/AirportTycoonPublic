using UnityEngine;
using System.Collections;

public class PlacePerson : MouseSelect {
	
	Person person;
	Cost cost;
	public PersonFactory personFactory;

	public Material selectionMaterialValid;
	public Material selectionMaterialInvalid;
	Material originalMaterial;

	void Update()
	{
		if (updateOn)
		{
			Hover();
			Select ();
		}
	}
	
	public void UpdateOn(Person new_person)
	{
		//buildPanel.Setup (building);
		base.UpdateOn ();

		person = new_person;
		cost = person.gameObject.GetComponent<Cost>();

		originalMaterial = person.gameObject.GetComponent<Renderer>().material;
		person.gameObject.SetActive (true);

		// turn off other interfaces
		this.gameObject.GetComponent<SelectEntity>().Pause();
	}
	
	protected override bool Hover()
	{
		return HoverPosition(HoverMouseMethod, layerMask);
	}
	
	
	protected override bool Select()
	{
		return SelectPosition(LeftMouseClickMethod, 0, layerMask);
	}
	
	public delegate bool HoverMouse(Vector3 pos);
	public override bool HoverMouseMethod(Vector3 pos)
	{
		if (pos != Vector3.zero) {
			person.transform.position = new Vector3(pos.x, person.transform.position.y, pos.z);
			if (person.IsPlaceable() && MoneyController.CanBuy(cost)) {
				person.gameObject.GetComponent<Renderer>().material = selectionMaterialValid;
			} else {
				person.gameObject.GetComponent<Renderer>().material = selectionMaterialInvalid;
			}

		}
		
		return true;
	}
	
	public delegate bool LeftMouseClick(Vector3 pos);
	public override bool LeftMouseClickMethod(Vector3 pos)
	{
		if (person.IsPlaceable() && MoneyController.CanBuy(cost)) {
			person.gameObject.GetComponent<Renderer>().material = originalMaterial;
			Person new_person = personFactory.PlacePerson(person.transform.position, person.GetType());
			MoneyController.Buy(cost, new_person);
		} else {
			Tooltip.ShowTooltip("Need to place a person inside a building", 3f);
		}

		return false;

	}
	
	
	protected override void Cancel()
	{
		base.Cancel ();
		person.gameObject.SetActive (false);
		this.gameObject.GetComponent<SelectEntity>().UnPause();
	}
}