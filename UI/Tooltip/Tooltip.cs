using UnityEngine;
using System.Collections;

public class Tooltip {

	static TooltipLabel tooltipLabel;

	public static void Setup(TooltipLabel tooltip_label)
	{
		tooltipLabel = tooltip_label;
		NGUITools.SetActive (tooltipLabel.gameObject, false);
	}

	public static void ShowTooltip(string message, float seconds)
	{
		tooltipLabel.ShowTooltip (message, seconds);
	}



}
