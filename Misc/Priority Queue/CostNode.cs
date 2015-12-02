using UnityEngine;
using System.Collections;

public class CostNode : PriorityQueueNode {

	public int CostPerHour { get; set; }
    public int EntityID { get; private set; }
    public Entity Entity { get; private set; }
    public int TimeInMinutes { get; set; } // time of next cost account

    public CostNode(Cost cost, Entity entity, int time) {
    	Entity = entity;
    	EntityID = Entity.GetID();

        CostPerHour = cost.costPerHour;
        TimeInMinutes = time;
    }



}
