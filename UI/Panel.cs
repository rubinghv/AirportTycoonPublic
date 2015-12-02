using UnityEngine;
using System.Collections;

public class Panel : MonoBehaviour {

	public void ShowPanel()
	{
		NGUITools.SetActive (this.gameObject, true);

	}
	
	public void HidePanel()
	{
		NGUITools.SetActive (this.gameObject, false);
	}

	public void SwitchPanel()
	{
//		print ("switching?");

		if (this.gameObject.activeSelf)
			NGUITools.SetActive (this.gameObject, false);
		else
			NGUITools.SetActive (this.gameObject, true);
	}
}
