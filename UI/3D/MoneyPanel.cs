using UnityEngine;
using System.Collections;

public class MoneyPanel : MonoBehaviour {

	public UILabel moneyLabel;
	
	// Update is called once per frame
	void Update () {
		moneyLabel.text = "$" + MoneyController.GetTotalMoney();
	}
}
