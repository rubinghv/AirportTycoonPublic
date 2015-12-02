using UnityEngine;
using System.Collections;

/// <summary>
/// Grid. Base class that is made up of a 2D array of gridCells. Class with static methods, instantiated by GridFactory that is attached to Factory GameObject
/// </summary>


public static class Grid {


	static Transform groundPlane;
    //public Transform groundCube;

	private static int groundPlaneSizeX;
	private static int groundPlaneSizeY;
	private static float gridCellSize;

	public static int GroundPlaneSizeX { get { return groundPlaneSizeX; } }
	public static int GroundPlaneSizeY { get { return groundPlaneSizeY; } }
	public static float GridCellSize { get { return gridCellSize; } }
	
	public static GridCell[,] gridArray;
	public static int GridCellsX { get {return (int) ((float) groundPlaneSizeX / gridCellSize); } } // number of grid cells in X direction
	public static int GridCellsY { get {return (int) ((float) groundPlaneSizeY / gridCellSize); } } // number of grid cells in Y direction
	
	static int gridCellCounter = 1000; // for naming purposes

	
	/**
	 * 	Setup all the different elements:
	 *  ground plane, grid cells, etc.
	 * 
	 */
	public static void Setup (GridProperties gridProperties) 
    {
		groundPlane = gridProperties.groundObject;

		groundPlaneSizeX = gridProperties.groundPlaneSizeX;
		groundPlaneSizeY = gridProperties.groundPlaneSizeY;

		gridCellSize = gridProperties.gridCellSize;

		SetupGroundPlane(gridProperties.gridMaterial);
        SetupGridArray();
	}
	
	
	/**
	 * 	Setup the ground plane according to groundPlaneSizeX and groundPlaneSizeY
	 */
	static void SetupGroundPlane (Material gridMaterial) {
		// regular size = 10. but when size is 10 scale is 0, when size is 20 scale is 1. 
		// so divide by 10 for multiply factor, -1 to correct for starting scale
		//groundPlane.localScale += new Vector3((groundPlaneSizeX / 10) - 1, 0, (groundPlaneSizeY / 10) - 1);
        //groundPlane.localScale += new Vector3((groundPlaneSizeX / 2), 0, (groundPlaneSizeY / 2));
        groundPlane.localScale = new Vector3(groundPlaneSizeX, 1, groundPlaneSizeY);
        groundPlane.transform.position = new Vector3(0, groundPlane.transform.position.y, 0);

		//gridMaterial.mainTextureScale = new Vector2(0.5f, 0.5f);
		groundPlane.GetComponent<Renderer>().material.mainTextureScale = new Vector2(GridCellsX, GridCellsY);
    }
	
	/**
	 * 	Create grid array with corresponding grid cells
	 * 
	 */ 
	static void SetupGridArray () {

            // loop through grid positions
		gridArray = new GridCell[GridCellsX, GridCellsY];

		for (int x = 0; x < GridCellsX; x++)
			for (int y = 0; y < GridCellsY; y++)
                {
                    gridArray[x, y] = new GridCell(new Vector2(x, y), gridCellCounter);
                    gridCellCounter++;
                }

	
	} 

	
	/**
	 * Standard save method
	 */
	public static void Save () {
//		ES2.Save(groundPlaneSizeX, "" + SaveLoadController.fileName + "?tag=gridGroundPlaneSizeX");	
	//	ES2.Save(groundPlaneSizeY, "" + SaveLoadController.fileName + "?tag=gridGroundPlaneSizeY");
	//	ES2.Save(gridCellSize, "" + SaveLoadController.fileName + "?tag=gridGridCellSize");
				
		//SaveGridArray (); temporarily off (befoer saving is fixed)
	}
	
	/**
	 * Standard load method
	 */
	public static void Load () {
		// NEEDS TO BE FIXED, NEW SETUP FUNCTION
		//groundPlaneSizeX = ES2.Load<int>("" + SaveLoadController.fileName + "?tag=gridGroundPlaneSizeX");	
		//groundPlaneSizeY = ES2.Load<int>("" + SaveLoadController.fileName + "?tag=gridGroundPlaneSizeY");	
		//gridCellSize = ES2.Load<float>("" + SaveLoadController.fileName + "?tag=gridGridCellSize");

        //LoadGridArray ();  temporarily off (befoer saving is fixed)

	}
	
    //void SaveGridArray () {
    //    ES2.Save(gridArray.GetLength(0), "" + SaveLoadController.fileName + "?tag=gridGridArraySizeX");	
    //    ES2.Save(gridArray.GetLength(1), "" + SaveLoadController.fileName + "?tag=gridGridArraySizeY");
    //    // loop through array and save objects
    //    for 
    //    for (int x = 0; x < gridArray.GetLength(0); x++)
    //        for (int y = 0; y < gridArray.GetLength(1); y++)
    //            gridArray[floor][x,y].Save(); // save all the individual objects
		
    //}
	
    //void LoadGridArray () {
    //    gridArray = new GridCell[ES2.Load<int>("" + SaveLoadController.fileName + "?tag=gridGridArraySizeX"),
    //                             ES2.Load<int>("" + SaveLoadController.fileName + "?tag=gridGridArraySizeY")];
		
    //    // loop through grid positions
    //    for (int x = 0; x < gridArray.GetLength(0); x++)
    //        for (int y = 0; y < gridArray.GetLength(1); y++)
    //        {
    //            gridArray[x,y] = new GridCell();
    //        }
	
    //}
}
