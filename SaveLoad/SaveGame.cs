using UnityEngine;
using System.Collections;

public class SaveGame : MonoBehaviour {

	public string saveName; 		// shouldn't be public??
	public string saveTimeString;
	
	public string fileName {
		get { return saveName + " @ " + saveTimeString.Replace (':', ';'); }
	}
	
	public SaveGame (string fileName) {
		saveName = GetSaveName(fileName);
		saveTimeString = GetSaveTimeString(fileName).Replace (';', ':');
	}
	
	public SaveGame (string sName, bool current) {
		saveName = sName;	
		saveTimeString = TimeController.GetCurrentDateTimeString();
	}
	
	
	string GetSaveName (string s) {
		string returnString =  s.Split('@')[0];
		return returnString.Remove(returnString.Length - 1);
	}
		
	string GetSaveTimeString (string s) {
		return s.Substring(s.IndexOf('@') + 2);
	}
	
	
}
