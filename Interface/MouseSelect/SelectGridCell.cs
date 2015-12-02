using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SelectGridCell : MouseSelect
{
    void Update()
    {
        if (updateOn)
        {
            Hover();
			//Select ();
        }
    }


	public override void UpdateOn()
	{
		base.UpdateOn ();
		tempTransform.gameObject.SetActive (true);
	}


    protected override bool Hover()
    {
		return HoverPosition(HoverMouseMethod, layerMask);
    }

    // selection isn't possible for selecting gridcells
    protected override bool Select()
    {
		return SelectPosition(LeftMouseClickMethod, 0, layerMask);
    }

	public Transform tempTransform;

    public delegate bool HoverMouse(Vector3 pos);
    public override bool HoverMouseMethod(Vector3 pos)
    {
        GridCell gridCell = GridHelper.GetGridCell(pos);
		if (pos == Vector3.zero || gridCell == null)
			return false;
		else {
			//Debug.Log ("gridCell pos = " + gridCell.arrayPosition);
			// for temp debugging

			tempTransform.position = new Vector3(gridCell.worldPosition.x, tempTransform.position.y, gridCell.worldPosition.y); 
			if (tempTransform.GetComponentInChildren<UILabel>() != null)
				tempTransform.GetComponentInChildren<UILabel>().text = "  (" + gridCell.gridPosition.x + ", " + gridCell.gridPosition.y + ")";
			//tempTransform.localScale.Scale(new Vector3(GridHelper.GetGridCellSize(),1,GridHelper.GetGridCellSize()));
		}


        return true;
    }

    public delegate bool LeftMouseClick(Vector3 pos);
    public override bool LeftMouseClickMethod(Vector3 pos)
   	{
		if (pos == Vector3.zero) {
			return false;
		} else {
			//Debug.Log ("select grid cell activating");
       		GridCell gridCell = GridHelper.GetGridCell(pos);
			return true;
		}

    }

	protected override void Cancel()
	{
		base.Cancel ();
		tempTransform.gameObject.SetActive (false);
	}

}



