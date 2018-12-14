﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageInBottleRescueEvent : EmptyZoneEvent {

	public void ResondToRescueMission (bool rescue) {

		if (rescue) {
			
			float r = Random.Range (0f, 1f);

			if (r >= .5f) {
				Map.StartEvent ("RescueEvent");
			} else {
				Map.StartEvent ("IslandTrapEvent");
			}

		} else {
			ProceedToNextZone ();
		}
	}
}
