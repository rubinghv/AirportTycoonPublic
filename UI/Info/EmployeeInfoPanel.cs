using UnityEngine;
using System.Collections;

public class EmployeeInfoPanel : PersonInfoPanel {

	protected override void Start () {
		// make sure it doesn't call base
	}

	public override void Setup(EntityInfoPanel panel) {
		base.Setup(panel);
	}

	protected override void ShowPanel(Entity entity) 
	{
		base.ShowPanel(entity);

		Employee employee = entity.gameObject.GetComponent<Employee>();
		print("clicked employee!");
		ChangeAndShowLabel(line_2_left_label, "----");
		ChangeAndShowLabel(line_3_left_label, "Quality :");
		ChangeAndShowLabel(line_3_right_label, "" + employee.serviceQuality);
		ChangeAndShowLabel(line_4_left_label, "Speed : ");
		ChangeAndShowLabel(line_4_right_label, "" + employee.serviceSpeed);

	}
}
