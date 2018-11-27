using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryEvent : EmptyZoneEvent {

	protected new void Start () {

		base.Start ();

		if (playerShip.crewMembers.Count == 0) {
			message.text = "Your entire crew has perished. Return to the Main Menu.";
		}
	}

	public void MainMenu () {
		Destroy (playerShip.gameObject);
		SceneManager.LoadScene ("Start Menu");
	}
}
