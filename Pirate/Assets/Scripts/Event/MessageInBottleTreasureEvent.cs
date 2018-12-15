using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageInBottleTreasureEvent : EmptyZoneEvent {

	public void ResondToTreasureMission (bool treasure) {

		if (treasure) {
			
			float r = Random.Range (0f, 1f);

			if (r >= .4f) {
				Map.StartEvent ("TreasureEvent");
			} else {
				Map.StartEvent ("IslandTrapEvent");
			}

		} else {
			ProceedToNextZone ();
		}
	}
}
