using UnityEngine;
using System.Collections;

public class PersonInfoPanel : EntityInfoPanel {

	protected override void Start () {
		// make sure it doesn't call base
	}

	public override void Setup(EntityInfoPanel panel) {
		base.Setup(panel);
	}

	protected override void ShowPanel(Entity entity) 
	{
		base.ShowPanel(entity);

		Highlight(entity, highlight_circle);
	}

	protected override void Highlight(Entity entity, GameObject highlight) {
		Person person = entity.gameObject.GetComponent<Person>();		
		Vector2 size = new Vector2(1.5f, 1.5f);
		
		//highlight.transform.localScale = new Vector3 ((size.x) * GridHelper.GetGridCellSize (), 
		//                                                     highlight.transform.localScale.y, 
		//                                                     (size.y) * GridHelper.GetGridCellSize ());

		highlight.transform.position = new Vector3(person.transform.position.x, highlight.transform.position.y, person.transform.position.z);
		highlight.transform.parent = person.transform;

		highlight.SetActive(true);

	}

}
