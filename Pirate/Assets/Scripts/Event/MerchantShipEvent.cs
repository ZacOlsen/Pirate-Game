using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantShipEvent : ShipEvent {

	protected override ShipDecision GenerateShipDecision (bool offerBonus) {

		float fleeBreakpoint = -playerShip.trustworthiness * 100f + 100f;
		float parleyBreakpoint = -playerShip.trustworthiness * 100f + 120f;

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

		float engageBreakpoint = -playerShip.lethality * 100f + 80f;
		float giveBreakpoint = -playerShip.lethality * 100f + 100f;

		float point = Random.Range (0f, 100f);

		if (point > giveBreakpoint) {
			return true;
		} else if (point < engageBreakpoint) {
			return false;
		}

		return Random.Range (0, 2) > 0;
	}
}
