using UnityEngine;
using System.Collections;

public class EntityInfoPanel : MonoBehaviour {
	[Header("Labels")]
	public UILabel entityNameLabel;
	public UILabel entityTypeLabel;

	public UILabel line_1_left_label;
	public UILabel line_1_right_label;
	public UILabel line_2_left_label;
	public UILabel line_2_right_label;
	public UILabel line_3_left_label;
	public UILabel line_3_right_label;
	public UILabel line_4_left_label;
	public UILabel line_4_right_label;
	public UILabel line_5_left_label;
	public UILabel line_5_right_label;
	public UILabel line_6_left_label;
	public UILabel line_6_right_label;
	public UILabel line_7_left_label;
	public UILabel line_7_right_label;

	[Header("Base classes")]
	public PersonInfoPanel personPanel;
	public PassengerInfoPanel passengerPanel;
	public EmployeeInfoPanel employeePanel;
	public BuildingInfoPanel buildingPanel;

	[Header("Other")]
	public GameObject highlight_rectangle;
	public GameObject highlight_circle;

	Entity lastEntity = null;

	protected virtual void Start () {
		personPanel.Setup(this);
		passengerPanel.Setup(this);
		buildingPanel.Setup(this);
		employeePanel.Setup(this);

		NGUITools.SetActive(this.gameObject, false);		
	}

	/*
	 *	
	 */
	public virtual void Setup(EntityInfoPanel panel) {
		entityNameLabel = panel.entityNameLabel;
		entityTypeLabel = panel.entityTypeLabel;

		line_1_left_label = panel.line_1_left_label;
		line_1_right_label = panel.line_1_right_label;
		line_2_left_label = panel.line_2_left_label;
		line_2_right_label = panel.line_2_right_label;
		line_3_left_label = panel.line_3_left_label;
		line_3_right_label = panel.line_3_right_label;
		line_4_left_label = panel.line_4_left_label;
		line_4_right_label = panel.line_4_right_label;
		line_5_left_label = panel.line_5_left_label;
		line_5_right_label = panel.line_5_right_label;
		line_6_left_label = panel.line_6_left_label;
		line_6_right_label = panel.line_6_right_label;
		line_7_left_label = panel.line_7_left_label;
		line_7_right_label = panel.line_7_right_label;

		highlight_rectangle = panel.highlight_rectangle;
		highlight_circle = panel.highlight_circle;
	}

	void EnablePanel(bool show)
	{	if (show) {
			NGUITools.SetActive(entityNameLabel.gameObject, true);
			NGUITools.SetActive(entityTypeLabel.gameObject, true);
		} else {
			NGUITools.SetActive(entityNameLabel.gameObject, false);
			NGUITools.SetActive(entityTypeLabel.gameObject, false);
			NGUITools.SetActive(line_1_left_label.gameObject, false);
			NGUITools.SetActive(line_1_right_label.gameObject, false);
			NGUITools.SetActive(line_2_left_label.gameObject, false);
			NGUITools.SetActive(line_2_right_label.gameObject, false);
			NGUITools.SetActive(line_3_left_label.gameObject, false);
			NGUITools.SetActive(line_3_right_label.gameObject, false);
			NGUITools.SetActive(line_4_left_label.gameObject, false);
			NGUITools.SetActive(line_4_right_label.gameObject, false);
			NGUITools.SetActive(line_5_left_label.gameObject, false);
			NGUITools.SetActive(line_5_right_label.gameObject, false);
			NGUITools.SetActive(line_6_left_label.gameObject, false);
			NGUITools.SetActive(line_6_right_label.gameObject, false);
			NGUITools.SetActive(line_7_left_label.gameObject, false);
			NGUITools.SetActive(line_7_right_label.gameObject, false);
		}
		NGUITools.SetActive(this.gameObject, show);
	}

	/*
	 *	Called by external click entity interface when an entity is selected
	 */
	public void ShowPanels(Entity entity) {
		// first check if there is a currently selected entity
		if (lastEntity != null) {
			HidePanel();
		}


		if (entity is Person) {
			if (entity is Passenger)
				passengerPanel.ShowPanel(entity);
			else if (entity is Employee)
				employeePanel.ShowPanel(entity);
			else
				personPanel.ShowPanel(entity);

		} else if (entity is Building) {
			//print("selected building!");
			buildingPanel.ShowPanel(entity);
			
		} else {
			print("no behaviour for selecting this type of entity yet!");
			return;
		}

		lastEntity = entity;
	}

	/*
	 *	For the base class entity, set the cost if there are any
	 *	And show the entity name and type
	 */
	protected virtual void ShowPanel(Entity entity) 
	{
		ResetPanel();

		// get cost component if it's there
		Cost cost = entity.gameObject.GetComponent<Cost>();
		if (cost != null) {
			//line_1_right_label.text = "$" + cost.costPerHour;
			//NGUITools.SetActive(line_1_left_label.gameObject, true);
			//NGUITools.SetActive(line_1_right_label.gameObject, true);
			ChangeAndShowLabel(line_1_left_label, "Cost per hour");
			ChangeAndShowLabel(line_1_right_label, "$" + cost.costPerHour);
		}

		entityNameLabel.text = entity.GetName();
		entityTypeLabel.text = entity.GetType();

		EnablePanel(true);
	}

	/*
	 *	Change UIlabel to new text and set to visible
	 */
	protected void ChangeAndShowLabel(UILabel label, string text)
	{
		label.text = text;
		NGUITools.SetActive(label.gameObject, true);
	}

	protected virtual void Highlight(Entity entity, GameObject highlight) {
		print("not imlpemented, need to override!");
	}

	protected virtual void Dehighlight() {
		highlight_circle.SetActive(false);
		highlight_rectangle.SetActive(false);		
	}


	void ResetPanel()
	{
		EnablePanel(false);
	}

	public void HidePanel()
	{	
		EnablePanel(false);
		Dehighlight();
		lastEntity = null;
	}
	
	
}
