using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BattleEvent : EncounterPeopleEvent {

	public BattleManager battleManager;
	protected bool inventoryGained;

	protected void OnDestroy () {
		//TODO: change from hard coded
		AStar.RemoveLayerFromLayout (3);
	}

	public virtual void GainEntireInventory () {
		inventoryGained = true;
	}

	public virtual void BattleOver (string endBattleText, bool gameOver) {

		message.text = endBattleText;
		gameObject.SetActive (true);

		if (battleManager) {
			Destroy (battleManager);
		}
	}

	protected void ReturnToMainMenu () {
		Destroy (playerShip.gameObject);
		SceneManager.LoadScene ("Start Menu");
	}
}
