using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingShipEvent : ShipEvent {

	protected override ShipDecision GenerateShipDecision (bool offerBonus) {

		float fleeBreakpoint = -1.3f * playerShip.trustworthiness * 100f + 100f;
		float parleyBreakpoint = -1.3f * playerShip.trustworthiness * 100f + 120f;

		float point = Random.Range (0f, 100f);

		if (offerBonus) {
			point += 5;
		}

		if (point > parleyBreakpoint) {
			return ShipDecision.PARLEY;

		} else if (point < fleeBreakpoint) {
			return ShipDecision.FLEE;
		}

		return Random.Range (0, 2) > 0 ? ShipDecision.PARLEY : ShipDecision.FLEE;
	}

	protected override bool IsThreatenSuccessful () {

		float engageBreakpoint = -playerShip.lethality * 100f + 90f;
		float giveBreakpoint = -playerShip.lethality * 100f + 110f;

		float point = Random.Range (0f, 100f);

		if (point > giveBreakpoint) {
			return true;
		} else if (point < engageBreakpoint) {
			return false;
		}

		return Random.Range (0, 2) > 0;
	}
}
