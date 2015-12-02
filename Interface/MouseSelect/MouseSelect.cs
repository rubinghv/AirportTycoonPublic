using UnityEngine;
using System.Collections;

public abstract class MouseSelect : MonoBehaviour {
	
    //protected Vector3 position;
	public int layerMask;
	public int[] layerMasks;

	protected bool updateOn;
	public virtual void UpdateOn() { updateOn = true; }
	public virtual void UpdateOff() { Cancel(); }

	protected virtual bool Hover () { return false; } 
	protected virtual bool Select () { return false; } 
		
	protected GameObject lastHitObject; 

	protected Vector3 GetPosition (int layerMaskNumber) // very temporary here
	{	// Construct a ray from the current mouse coordinates
		return GetPosition(new int[] {layerMaskNumber});

	}

	protected Vector3 GetPosition (int[] layerMasks) // very temporary here
	{	// Construct a ray from the current mouse coordinates
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;		
		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
			//Debug.Log("hit layer of " + hit.transform.gameObject.name + " = " + hit.transform.gameObject.layer);

			if (compareLayerMask(hit.transform.gameObject.layer, layerMasks)) {
				lastHitObject = hit.transform.gameObject;
				return hit.point;
			}

		}
		return Vector3.zero;
	}

	bool compareLayerMask(int hitLayer, int[] layerMasks) {
		for (int i = 0; i < layerMasks.Length; i++) {
			if (layerMasks[i] == hitLayer)
				return true;
		}
		return false;
	}

	bool compareLayerMask(int hitLayer, int layerMaskNumber) {
		if (hitLayer == layerMaskNumber)
			return true;
		else return false;
	}
	
	

	protected bool HoverPosition (HoverMouse hm, int layerMaskNumber) {
		return HoverPosition(hm, new int[] {layerMaskNumber});
	}

	protected bool HoverPosition (HoverMouse hm, int[] layerMasks) {
		RightClickCancel();
		if (!InterfaceController.mouseOverUI)
			return hm(GetPosition(layerMasks));
		else return false;
	}
	
	protected bool SelectPosition (LeftMouseClick lmc, int mouseButton, int layerMaskNumber)  {	
		return SelectPosition(lmc, mouseButton, new int[] {layerMaskNumber});
	}

	protected bool SelectPosition (LeftMouseClick lmc, int mouseButton, int[] layerMasks) 
	{	// if the left mouse button is held down
		if (Input.GetMouseButtonUp(mouseButton) && !InterfaceController.mouseOverUI) 
			return lmc(GetPosition(layerMasks));
		
		RightClickCancel();
		return false;
	}

    GridCell mouseDownGridCell;
	Vector2 mouseDownPos;
    Vector3 mouseDownPos3;

    /**
     *  Returns true when dragging
     * 
     */
	protected bool DragPosition(DragMouse dm, DragMouseUpdate dmu, int mouseButton, int layerMaskNumber)
    {
		RightClickCancel ();

        if (Input.GetMouseButtonDown(mouseButton) && !InterfaceController.mouseOverUI)
        {
			mouseDownPos3 = GetPosition(layerMaskNumber);
            mouseDownGridCell = GridHelper.GetGridCell(mouseDownPos3);
            return true;
        }

		if (Input.GetMouseButton(mouseButton) && GridHelper.GetGridCell(GetPosition(layerMaskNumber)).gridPosition != mouseDownGridCell.gridPosition)
        {
            //print("dragging!!!");
			return dmu(mouseDownPos3, GetPosition(layerMaskNumber));
        }

		if (mouseDownGridCell != null)
			if (Input.GetMouseButtonUp(mouseButton) && !InterfaceController.mouseOverUI && GridHelper.GetGridCell(GetPosition(layerMaskNumber)).gridPosition != mouseDownGridCell.gridPosition)
				return dm(mouseDownPos3, GetPosition(layerMaskNumber));

        return false;
    }


	protected bool DragUI (DragMouse dm, int mouseButton)
	{
		if (Input.GetMouseButtonDown(mouseButton) && !InterfaceController.mouseOverUI)
			mouseDownPos = Input.mousePosition;

		if (Input.GetMouseButtonUp(mouseButton) && !InterfaceController.mouseOverUI)
			return dm (mouseDownPos, Input.mousePosition);			
		
		RightClickCancel();
		return false;
	}
	
	public delegate bool HoverMouse(Vector3 pos);
	public virtual bool HoverMouseMethod(Vector3 pos) { print ("nothing happening!"); return false; }
	
	public delegate bool LeftMouseClick(Vector3 pos);
	public virtual bool LeftMouseClickMethod(Vector3 pos) { print ("nothing happening!"); return false; }
	
	public delegate bool DragMouse(Vector3 mouseDown, Vector3 mouseUp);
	public virtual bool DragMouseMethod(Vector3 mouseDown, Vector3 mouseUp) { print ("nothing happening!"); return false; }

	public delegate bool DragMouseUpdate(Vector3 mouseDown, Vector3 mouseUp);
	public virtual bool DragMouseUpdateMethod(Vector3 mouseDown, Vector3 mouseUp) { print ("nothing happening!"); return false; }

	//-------------
	// also need right click cancel for everything
	
	float shortClickTimeCancel;
	protected virtual void RightClickCancel ()
	{
		if ( Input.GetMouseButtonDown (1) )
            shortClickTimeCancel = Time.time;

        if (Input.GetMouseButtonUp(1) && (Time.time - shortClickTimeCancel) < 0.2f)
	        Cancel();
	}
	
	protected virtual void Cancel ()
	{
		updateOn = false;
	}
	
	
	
	
}
