using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour {

	[SerializeField] private EmptyZoneEvent zoneEvent;
	private CrewMember selected;
	private Text selectedDisplay;
	private PlayerShip ship;

	void Start () {
		selectedDisplay = GetComponent<Text> ();
		ship = GameObject.Find ("ship").GetComponent<PlayerShip> ();
	}

	public void Select (CrewMember crew, bool fromEvent = false) {

		if (zoneEvent.canChangeCrewSelection || fromEvent) {
			selected = crew;
			DisplayStats ();
		}
	}

	public void UpdateStats (CrewMember crew) {

		if (crew == selected) {

			if(crew.health <= 0) {
				selected = null;
			}

			DisplayStats ();
		}
	}

	private void DisplayStats () {

		if (selected) {
			selectedDisplay.text = selected.personName + "\n" +
				selected.role +
				"\nHealth: " + selected.health + "/" + selected.maxHealth +
				"\nIntelligence: " + selected.intelligence +
				"\nDexterity: " + selected.dexterity;

		} else {
			selectedDisplay.text = "";
		}
	}

	public CrewMember GetSelected () {
		return selected;
	}

	public void AssignTo (ShipPosition pos) {

		if (!selected || !selected.isPlayerCrew || selected.currentAction == CrewMember.Action.ATTACKING
			|| !zoneEvent.canChangeCrewPositions) {

			return;
		}

		//TODO: clean this shit up
		if (pos.isManned) {

			CrewMember memToSwap = null;
			for (int i = 0; i < ship.crewMembers.Count; i++) {

				if (ship.crewMembers[i].shipPos == pos) {
					memToSwap = ship.crewMembers[i];
					break;
				}
			}

			ship.Unassign (memToSwap);
			ShipPosition temp = selected.shipPos;
			ship.Assign (selected, pos);
			ship.Assign (memToSwap, temp, true);

		} else {
			ship.Assign (selected, pos);
		}

		selected = null;
		DisplayStats ();
	}
}
