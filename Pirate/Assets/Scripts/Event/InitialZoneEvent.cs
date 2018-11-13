using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialZoneEvent : EncounterPeopleEvent {

	protected new void Start () {

		playerShip = GameObject.Find ("ship").GetComponent<PlayerShip> ();

		int crewNum = Random.Range (minNumOfPeople, maxNumOfPeople + 1);

		for (int i = 0; i < crewNum; i++) {
			ShipPosition sp = playerShip.GetVacantPositionByPriority ();
			CrewMember c = CreateCrewMember (sp.tile);
			c.isPlayerCrew = true;
			playerShip.Assign (c, sp, true);
		}

		canChangeCrewPositions = true;
		canChangeCrewSelection = true;
	}
}
