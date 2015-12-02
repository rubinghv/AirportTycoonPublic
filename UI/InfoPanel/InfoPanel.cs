/**
 *	Panel for displaying information on bottom left  
 * 
 * 	Attach InfoPanelParameters to this object.
 */

using UnityEngine;
using System.Collections;

public class InfoPanel : MonoBehaviour {

	protected InfoPanelParameters parameters;

	protected Transform selectionObject;
	protected UIPanel panel;
	protected UISprite background;
	protected UILabel type_name;
	protected UISprite type_underline;

	protected UILabel property_one_title;
	protected UILabel property_one_data;
	protected UIButton property_one_button;
	protected UILabel property_two_title;
	protected UILabel property_two_data;
	protected UIButton property_two_button;
	protected UILabel property_three_title;
	protected UILabel property_three_data;
	protected UIButton property_three_button;

	protected UIButton single_button;


	protected void Start()
	{
		parameters = this.gameObject.GetComponent<InfoPanelParameters> ();
		panel = this.gameObject.GetComponent<UIPanel> ();

		// use the parameters object to setup all the variables
		background = parameters.background;
		type_name = parameters.type_name;
		type_underline = parameters.type_underline;

		property_one_title = parameters.property_one_title;
		property_one_data = parameters.property_one_data;
		property_one_button = parameters.property_one_button;
		property_two_title = parameters.property_two_title;
		property_two_data = parameters.property_two_data;
		property_two_button = parameters.property_two_button;
		property_three_title = parameters.property_three_title;
		property_three_data = parameters.property_three_data;
		property_three_button = parameters.property_three_button;

		single_button = parameters.single_button;
		selectionObject = parameters.selectionObject;

		// also hide all
		SetVisible (false);

		// don't forget to disable until required
		this.enabled = false;
	}


	public virtual void Highlight(GridObject gridObject)
	{
		//gridObject = gridOb;
		UpdateSelectionObject(gridObject);
		UpdateLabels(gridObject);
		SetVisible(true);

	}

	protected virtual void UpdateLabels(GridObject gridObject)
	{
		type_name.text = gridObject.GetType();
	}

	void UpdateSelectionObject(GridObject gridObject)
	{
		if (!selectionObject.GetComponent<Renderer>().enabled)
			selectionObject.GetComponent<Renderer>().enabled = true;
		
		selectionObject.transform.localScale = new Vector3(gridObject.GetSize ().x * GridHelper.GetGridCellSize(), 
		                                                   selectionObject.transform.localScale.y, 
		                                                   gridObject.GetSize ().y * GridHelper.GetGridCellSize());
		selectionObject.transform.position = new Vector3 (
			gridObject.GetWorldPosition2 ().x + ((gridObject.GetSize ().x * GridHelper.GetGridCellSize()) / 2.0f) - (GridHelper.GetGridCellSize() / 2.0f),
			selectionObject.transform.position.y,
			gridObject.GetWorldPosition2 ().y + ((gridObject.GetSize ().y * GridHelper.GetGridCellSize()) / 2.0f) - (GridHelper.GetGridCellSize() / 2.0f));
	}

	public virtual void Dehighlight (GridObject gridObject)
	{
		//gridObject = gridOb;
		selectionObject.transform.position = new Vector3(9999, selectionObject.transform.position.y, 9999);
		selectionObject.GetComponent<Renderer>().enabled = false;
		SetVisible(false);

	}

	protected virtual void SetVisible(bool enabled)
	{
		NGUITools.SetActive (background.gameObject, enabled);
		NGUITools.SetActive (type_name.gameObject, enabled);
		NGUITools.SetActive (type_underline.gameObject, enabled);
	}
}
