using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SelectEntity : MouseSelect
{
	//public GameObject buildingHighlight;
	public EntityInfoPanel infoPanel;
	//public GameObject panel;
	//public UILabel label_type_name;

	void Start() {
		updateOn = true;
	}

	void Update()
	{
		if (updateOn)
		{
			//Hover();
			Select ();
		}
	}

	public void Pause() {
		updateOn = false;
	}

	public void UnPause() {
		updateOn = true;
	}
	protected override bool Hover()
	{
		return HoverPosition(HoverMouseMethod, layerMasks);
	}
	
	// selection isn't possible for selecting gridcells
	protected override bool Select()
	{
		return SelectPosition(LeftMouseClickMethod, 0, layerMasks);
	}

	public delegate bool HoverMouse(Vector3 pos);
	public override bool HoverMouseMethod(Vector3 pos)
	{
		GridCell gridCell = GridHelper.GetGridCell(pos);
		//print ("current position = " + pos);
		if (gridCell != null) {
//			Debug.Log ("gridCell pos = " + gridCell.arrayPosition);
		}
		else{
		}
		
		return true;
	}
	
	public delegate bool LeftMouseClick(Vector3 pos);
	public override bool LeftMouseClickMethod(Vector3 pos)
	{
//		Debug.Log ("selecting building! pos = " + pos);
		if (pos == Vector3.zero)
			return false;
		else {
//			Debug.Log ("clicking building!");
			infoPanel.ShowPanels(lastHitObject.GetComponent<Entity>());

			return true;
		}
	}
	
	protected override void Cancel()
	{
		if (lastHitObject) {
			//lastHitObject.GetComponent<GridObject> ().Dehighlight();
		}
		infoPanel.HidePanel();
	}
	
}



