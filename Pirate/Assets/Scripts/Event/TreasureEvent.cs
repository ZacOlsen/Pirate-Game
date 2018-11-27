using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureEvent : EmptyZoneEvent {

	[SerializeField] private int woodMin = 5;
	[SerializeField] private int woodMax = 12;
	[SerializeField] private int goldMin = 5;
	[SerializeField] private int goldMax = 15;

	private int foodCost {
		get { return Mathf.CeilToInt (playerShip.crewMembers.Count / 2f); }
	}

	public new void CloseReport () {

		base.CloseReport ();

		int woodGain = Random.Range (woodMin, woodMax + 1);
		int goldGain = Random.Range (goldMin, goldMax + 1);

		int foodToSpend = foodCost;
		if (!playerShip.CanAfford (foodCost, 0, 0)) {
			foodToSpend = playerShip.GetFood ();
			woodGain = woodGain * foodToSpend / foodCost;
			goldGain = goldGain * foodToSpend / foodCost;
		}

		message.text = "We have found " + woodGain + " wood and " + goldGain + 
			" gold and it cost us " + foodCost + " food to get. Proceed to next sector.";

		playerShip.AddInventory (0, woodGain, goldGain);
	}
}
