using UnityEngine;
using System.Collections;

public class InterfaceController : MonoBehaviour {

	public static int LAYER_AREA_BUILDING = 11;
	public static int LAYER_BUILDING = 12;
	public static int LAYER_UI_3D = 31;
	public static int LAYER_UI_2D = 5;
	public static int LAYER_ROAD = 14;
	public static int LAYER_PERSON = 17;

    SelectGridCell selectGridCell;
	public TooltipLabel tooltipLabel;

	void Start()
	{
		Tooltip.Setup(tooltipLabel);
	}

	public void Setup()
    {
        selectGridCell = (SelectGridCell)FindObjectOfType(typeof(SelectGridCell));

    }

	void Update () {
		CheckMouseOverUI();
	}
	
//	UICamera uiCamera;
	static public bool mouseOverUI;

    public void UpdateEnable(bool enabled)
    {
        if (enabled)
        {
            // first check if we're raycasting anything: people/structures/gridcells
            selectGridCell.UpdateOn();
        }
        else
        {
            selectGridCell.UpdateOff();
        }
    }




    /**
     *  
     * 
     */ 
	void CheckMouseOverUI() 
	{	
		if (UICamera.hoveredObject != null) {
			if (UICamera.hoveredObject.gameObject.layer != LAYER_UI_2D)
			{
				mouseOverUI = false;
				return;
			}

			if (UICamera.hoveredObject.GetComponent<BoxCollider>()) {
				mouseOverUI = false;		
			} else {
				mouseOverUI = true;	
			}

			if (UICamera.hoveredObject.GetComponent<Collider>() != null)
			{
				if (UICamera.hoveredObject.GetComponent<Collider>().isTrigger) {
					mouseOverUI = true;	
				}
			} else {
				mouseOverUI = false;
			}
				
		} else {
			mouseOverUI = false;
		}


	}
	

	
}
