using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingNode : MonoBehaviour {

	[Header("Type of node")]
	public bool entrance;
	public bool connectingNode;
	public bool edgeNode;
	//public bool employee;

	[Header("Identification")]
	public string category;
	public int number;

	[Header("Functions")]
	public bool autoConnect;

	[Header("Links")]
	public GameObject door;

	//public BuildingNode[] nodeConnections;
	public List<BuildingNode> nodeConnections;
	public InternalBuildingNode internalNode;

	public void SetupAutoConnection()
	{
		if (connectingNode && autoConnect) {
			BuildingNode node;
			// loop through all connecting nodes
			foreach(Transform child in this.transform.parent) {
				if (Vector3.Distance(this.transform.position, child.position) < 1.1f * GridHelper.GetGridCellSize() &&
					Vector3.Distance(this.transform.position, child.position) > 0.01f * GridHelper.GetGridCellSize()) {

					node = child.gameObject.GetComponent<BuildingNode>();

					if (node.connectingNode && node.autoConnect) 
						nodeConnections.Add(node);
					
				}
			}
		}
	}

	public void SetupEdgeNodeConnections()
	{	
		//for (int i = 0; i < nodeConnections.Length; i++) {
		foreach(BuildingNode node in nodeConnections) {
			// if there's an edge node

			if (node.edgeNode) {
				node.SetEdgeConnection(this);
			}
		}
	}

	/* 
	 *	Edge connection will be set by connecting nodes
	 *	They will call this method to set the connection in both ways
	 */
	void SetEdgeConnection(BuildingNode connectingNode)
	{
		if (edgeNode) {
			nodeConnections.Add(connectingNode);
		} else {
			print("should not be getting called, is not edge node");
		}
	}

	public InternalBuildingNode[] GetInternalConnections()
	{	
		int i = 0;
		InternalBuildingNode[] internalConnections = new InternalBuildingNode[nodeConnections.Count];

		foreach(BuildingNode node in nodeConnections) {
			internalConnections[i] = node.internalNode;
			i++;
		}

		return internalConnections;
	}


}
