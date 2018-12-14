using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandTrapEvent : BattleEvent {

	public Beach beach;
	[SerializeField] private Button button;	

	private int food;
	private int wood;
	private int gold;

	protected new void Start () {

		base.Start ();

		int islandPeopleNum = Random.Range (minNumOfPeople, maxNumOfPeople + 1);

		food = Random.Range (foodMin, foodMax + 1);
		wood = Random.Range (woodMin, woodMax + 1);
		gold = Random.Range (goldMin, goldMax + 1);

		for (int i = 0; i < islandPeopleNum; i++) {
			CrewMember c = CreateCrewMember (beach.GetRandomTile ());
			otherPeople.Add (c);
		}

		battleManager.Init (playerShip.GetBoardingTile (), beach.boardingTile);
	}

	protected new void OnDestroy () {

		base.OnDestroy ();
		for (int i = 0; i < otherPeople.Count; i++) {
			if (otherPeople[i]) {
				Destroy (otherPeople[i].gameObject);
			}
		}
	}

	public override void GainEntireInventory () {
		base.GainEntireInventory ();
		playerShip.AddInventory (food, wood, gold);
	}

	public void OkButton () {
		gameObject.SetActive (false);
		battleManager.BeginBoardingAssault (playerShip, otherPeople);

		playerShip.InitializeBattle (battleManager);

		for (int i = 0; i < otherPeople.Count; i++) {
			otherPeople [i].battleManager = battleManager;
		}

		canChangeCrewPositions = true;
	}

	public override void BattleOver (string endBattleText, bool gameOver) {

		if (inventoryGained) {
			endBattleText += " We gained " + food + " food, " + wood + " wood, " + gold + " gold.";
		}

		base.BattleOver (endBattleText, gameOver);

		for (int i = 0; i < otherPeople.Count; i++) {
			if (otherPeople[i]) {
				otherPeople[i].battleManager = null;
			}
		}

		button.onClick.SetPersistentListenerState (0, UnityEngine.Events.UnityEventCallState.Off);

		if (gameOver) {
			button.onClick.AddListener (ReturnToMainMenu);
			button.GetComponentInChildren<Text> ().text = "Main Menu";
			canChangeCrewPositions = false;

		} else {
			//TODO: add message about inventory gains
			button.onClick.AddListener (ProceedToNextZone);
		}
	}
}
