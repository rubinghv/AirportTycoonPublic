using UnityEngine;
using System.Collections;

public class AutoResize : MonoBehaviour {

	public Transform uiRoot;

	void Start () {
		this.transform.localScale = new Vector3 (1.0f / uiRoot.transform.localScale.x, 
			                                     1.0f / uiRoot.transform.localScale.y, 
			                                   1.0f / uiRoot.transform.localScale.z);
	}
	
	
}
