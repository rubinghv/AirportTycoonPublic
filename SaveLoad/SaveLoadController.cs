using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveLoadController : MonoBehaviour {
	
	public string airportGoName;
	Airport airport;

	public string fileName;
	public string fileType;
	public string saveDir = "Saves/";
	string fullPath {
		get {return saveDir + fileName + fileType; }
	}


	static public List<SaveGame> saveList = new List<SaveGame>(); // filename and date

	static public List<string> fileNameList  {
		get { 
			List<string> returnList = new List<string>();
			foreach (SaveGame sGame in saveList)
				returnList.Add (sGame.saveName);
			return returnList;
		}
	}
	
	static public List<string> saveTimeList {
		get { 
			List<string> returnList = new List<string>();
			foreach (SaveGame sGame in saveList)
				returnList.Add (sGame.saveTimeString);
			return returnList;
		}
	}
	
	void Start()
	{
		airport = GameObject.Find(airportGoName).GetComponent<Airport>();
	}

	// temporary variables
	public bool saveGame = false;
	public bool loadGame= false;

	void Update()
	{
		if (saveGame) {
			saveGame = false;
			tempSaveGame();
		} else if (loadGame) {
			loadGame = false;
			tempLoadGame();
		}

	}

	void tempSaveGame()
	{
		SaveGame("tempSaveFile");
	}

	void tempLoadGame()
	{
		Load(fullPath);
	}


	/**
	 * Look in the default search folder for all the saves
	 * then add them to the saveList array.
	 */ 
	public static void LoadInitialSavedGames () {
		// don't forget to update assets
		//AssetDatabase.Refresh();
		
		string[] filesInFolder = ES2.GetFiles("");
		
		for (int i = 0; i < filesInFolder.Length; i++) {
			saveList.Add ( new SaveGame (filesInFolder[i]));

			print (saveList[i].saveName + "|       |" + saveList[i].saveTimeString);
		}
		
		print (    System.DateTime.Now);
		
	}
	
	public void SaveGame (string saveName) {
		SaveGame sGame = new SaveGame(saveName, false);
		saveList.Add(sGame);
		
		Save(fullPath);

		// NEED TO CHECK IF IT"S ALREDY HERE
	}
	
	public void LoadGame (string saveName) { 
		for (int i = 0; i < saveList.Count; i++) 
			if (saveList[i].saveName == saveName) 
				fileName = saveList[i].fileName;
				
		
		Load(fileName);
		
		print ("loaded: " + saveName);

	}

	public void Save (string filename) {

	//	SaveGrid();
		airport.Save(filename);
		EmployeeController.Save(filename, "?tag=");
		TimeController.Save(filename, "?tag=");
	}
	
	//public static void SaveGrid () {
	//	Grid.Save();	
	//
	
	public void Load (string filename) {

		airport.Load(filename);
		EmployeeController.Load(filename, "?tag=");
		TimeController.Load(filename, "?tag=");

	}
	
	//public static void LoadGrid () {
	//	Grid.Load();
	//}
	
	/**
	 * 	Delete game by removing it from list, and then removing file
	 * 
	 * 
	 */ 
	public static void DeleteGame (string saveName) {		
		for (int i = 0; i < saveList.Count; i++) {

			if (saveList[i].saveName == saveName) {
				//AssetDatabase.Refresh();
				//if (AssetDatabase.DeleteAsset("Assets/" + saveDir + saveList[i].fileName))
				//	print ("|" + "Assets/" + saveDir + saveList[i].fileName + "|");
				//else 
				////	print ("failure!!");
				//	print ("|" + "Assets/" + saveDir + saveList[i].fileName + "|");
				
				//saveList.RemoveAt (i);
				//AssetDatabase.Refresh();
				break;
				
			}
		}

	}
	



}
























