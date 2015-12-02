using UnityEngine;
using System.Collections;

public class PassengerInfoPanel : EntityInfoPanel {

	protected override void Start () {
		// make sure it doesn't call base
	}

	public override void Setup(EntityInfoPanel panel) {
		base.Setup(panel);
	}

	protected override void ShowPanel(Entity entity) 
	{
		base.ShowPanel(entity);

		Passenger passenger = entity.gameObject.GetComponent<Passenger>();
		print("clicked passenger!");
		ChangeAndShowLabel(line_2_left_label, "Needs");
		ChangeAndShowLabel(line_3_left_label, "Food :");
		ChangeAndShowLabel(line_3_right_label, "" + passenger.need_food);
		ChangeAndShowLabel(line_4_left_label, "Shopping : ");
		ChangeAndShowLabel(line_4_right_label, "" + passenger.need_shopping);
		ChangeAndShowLabel(line_5_left_label, "Services :");
		ChangeAndShowLabel(line_5_right_label, "" + passenger.need_services);
		ChangeAndShowLabel(line_6_left_label, "Restroom : ");
		ChangeAndShowLabel(line_6_right_label, "" + passenger.need_restroom);
		ChangeAndShowLabel(line_7_left_label, "Impatience :");
		ChangeAndShowLabel(line_7_right_label, "" + passenger.need_impatience);

	}
}
