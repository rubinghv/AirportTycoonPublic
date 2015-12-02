using UnityEngine;
using System.Collections;

public class InfoBuildBuildingPanel : InfoPanel {

	Building building;
	
	// Use this for initialization
	protected void Start () {
		base.Start ();
	}
	
	public override void Highlight(GridObject gridObject)
	{
		building = (Building) gridObject;
		base.Highlight (building);
	}
	
	protected override void UpdateLabels(GridObject gridObject)
	{
		property_one_title.text = "building panel";

		//EventDelegate.Add (single_button.onClick, Purchase);
		//single_button.isEnabled = true;
		
		base.UpdateLabels (gridObject);
	}
	
	public override void Dehighlight (GridObject GridObject)
	{
		building = (Building) GridObject;
		base.Dehighlight (GridObject);
	}
	
	protected override void SetVisible(bool enabled)
	{
		base.SetVisible (enabled);
		
		NGUITools.SetActive (property_one_title.gameObject, enabled);
		NGUITools.SetActive (single_button.gameObject, enabled);

	}
	
	void Purchase()
	{
		//
	}
}
