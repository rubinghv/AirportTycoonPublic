using UnityEngine;
using System.Collections;

public class MoneyController : MonoBehaviour {

	public int startingMoney;
	public int maxCostNodes;
	static int money;

	static int costPerHour;
	static int incomePerHour;

	static HeapPriorityQueue<CostNode> costs; 

	void Start()
	{
		money = startingMoney;
		costs = new HeapPriorityQueue<CostNode>(maxCostNodes);
	}

	void Update()
	{
		UpdateCost();
	}

	/*
	 *	Update costs by going through costs items, retrieving minPriority (lowest time)
	 *	and checking if that time has arrived yet.
	 *	If so, dequeue, subtract hourly cost, and put back in queue
	 */
	void UpdateCost()
	{	
		if (costs.Count > 0)
			if (costs.First.TimeInMinutes <= TimeController.TimeInMinutes) {
				// dequeue and subtract cost
				CostNode costNode = costs.Dequeue();
				money -= costNode.CostPerHour;

				// reset time in minutes for next hour and enqueue
				costNode.TimeInMinutes = TimeController.TimeInMinutes + 60;
				costs.Enqueue(costNode, costNode.TimeInMinutes);
			}
	}

	public static int GetTotalMoney() { return money; }

	/*
	 *	Called by building panels 3D when trying to buy a building/employee/grid object etc.
	 */
	public static bool CanBuy(int cost)
	{
		if (cost <= money)
			return true;
		else 
			return false;
	}

	public static bool CanBuy(Cost cost)
	{
		return CanBuy(cost.costInitial);
	}

	/* 
	 *	Called by building panels 3d when finally purchasing an entity
	 */
	public static void Buy(Cost cost, Entity entity)
	{
		money -= cost.costInitial;

		// only add costs when there is a cost per hour
		if (cost.costPerHour > 0)
			AddCost(cost, entity);
	}

	public static void AddCost(Cost cost, Entity entity)
	{	
		CostNode node = new CostNode(cost, entity, TimeController.TimeInMinutes + 60);
		costs.Enqueue(node, node.TimeInMinutes);

		// also add to cost per hour
		costPerHour += cost.costPerHour;
	}
}
