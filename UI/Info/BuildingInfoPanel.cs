using UnityEngine;
using System.Collections;

public class BuildingInfoPanel : EntityInfoPanel {

	protected override void Start () {
		// make sure it doesn't call base
	}

	public override void Setup(EntityInfoPanel panel) {
		base.Setup(panel);
	}

	protected override void ShowPanel(Entity entity) 
	{
		base.ShowPanel(entity);

		Highlight(entity, highlight_rectangle);
	}

	protected override void Highlight(Entity entity, GameObject highlight) {
		Building building = entity.gameObject.GetComponent<Building>();		
		Vector2 size = building.GetSize ();
		
		highlight.transform.localScale = new Vector3 ((size.x) * GridHelper.GetGridCellSize (), 
		                                                     highlight.transform.localScale.y, 
		                                                     (size.y) * GridHelper.GetGridCellSize ());

		highlight.transform.position = new Vector3(building.transform.position.x, 0, building.transform.position.z);
		highlight.SetActive(true);

	}

}
