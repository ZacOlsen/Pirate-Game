using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ShipEvent : BattleEvent {

	public Sprite foodPic;
	public Sprite woodPic;
	public Ship otherShip;

	public GameObject goodsMenu;
	public GameObject crewMenu;
	public GameObject mainMenu;
	public GameObject bribeMenu;

	public GameObject continueMessageButton;
	public GameObject otherParleyOptions;
	public GameObject engageOptions;
	public GameObject shipSeesFirstOptions;
	public GameObject playerSeesFirstOptions;
	public GameObject playerParleyOptions;
	public GameObject finishedButton;

	public GameObject buyManager;
	public ContentManager goodsContent;
	public ContentManager otherPeopleContent;
	public Button threatenButton;
	public Button bribe;
	public Button persuade;
	public InputField moneyToBribe;
	public Text info;

	protected Button foodButton;
	protected Button woodButton;

	protected Button current;

	protected bool actionSuccess = false;

	protected const float TRUST_ACTION_DELTA = .1f;

	protected enum ShipDecision {
		ENGAGE,
		FLEE,
		PARLEY
	}

	protected new void Start () {
		
		base.Start ();

		int otherPeople = Random.Range (minNumOfPeople, maxNumOfPeople + 1);
		for (int i = 0; i < otherPeople; i++) {

			ShipPosition sp = otherShip.GetVacantPositionByPriority ();
			CrewMember c = CreateCrewMember (sp.tile);
			c.ship = otherShip;
			otherShip.Assign (c, sp, true);
		}

		int food = Random.Range (foodMin, foodMax + 1);
		int wood = Random.Range (woodMin, woodMax + 1);
		int gold = Random.Range (goldMin, goldMax + 1);

		otherShip.AddInventory (food, wood, gold);

		if (PlayerHasRangeAdvantage ()) {

			message.text += "The ship has not yet seen us.\nWhat do we wish to do?";
			shipSeesFirstOptions.SetActive (false);
			playerSeesFirstOptions.SetActive (true);

		} else {

			ShipDecision decision = GenerateShipDecision (false);

			switch (decision) {
				case ShipDecision.FLEE:
					otherParleyOptions.SetActive (false);

					message.text += "The ship is attempting to flee us.\nWe are ";

					if (!PlayerHasFasterShip ()) {
						engageOptions.SetActive (false);
						message.text += "not ";
					}

					message.text += "faster than them.";
					break;

				case ShipDecision.PARLEY:
					message.text += "The ship wishes to parley with us.";
					break;

				case ShipDecision.ENGAGE:
					message.text += "The ship is engaging us.";
					continueMessageButton.SetActive (true);
					shipSeesFirstOptions.SetActive (false);
					break;
			}
		}

		battleManager.Init (playerShip.GetBoardingTile (), otherShip.GetBoardingTile ());
	}

	protected abstract ShipDecision GenerateShipDecision (bool offerBonus);

	protected abstract bool IsThreatenSuccessful ();

	public virtual void ContinueMessage () {}

	protected bool PlayerHasRangeAdvantage () {
		return playerShip.visionRange > otherShip.visionRange;
	}

	protected bool PlayerHasFasterShip () {
		return playerShip.speed > otherShip.speed;
	}
		
	private void SetCurrent (Button button) {

		if (current) {
			current.interactable = true;
		}

		current = button;
		button.interactable = false;
	}

	protected void PopulateBuyMenu () {

		//food
		foodButton = goodsContent.CreateButton (foodPic, "Food: Buy 2 for 2 Gold\nTotal: " + otherShip.GetFood ());
		foodButton.name = "Food";

		ColorBlock cb = foodButton.colors;
		cb.disabledColor = Color.gray;
		foodButton.colors = cb;

		foodButton.onClick.AddListener (delegate {
			BuyFood ();
		});

		//wood
		woodButton = goodsContent.CreateButton (woodPic, "Wood: Buy 2 for 2 Gold\nTotal: " + otherShip.GetWood ());
		woodButton.name = "Wood";
		woodButton.colors = cb;

		woodButton.onClick.AddListener (delegate {
			BuyWood ();
		});
	}

	protected void PopulateCrewMenu () {

		for (int i = 0; i < otherShip.crewMembers.Count; i++) {

			CrewMember c = otherShip.crewMembers [i];
			Button button = otherPeopleContent.CreateButton (c.GetComponent<SpriteRenderer> ().sprite,
				c.personName);

			button.onClick.AddListener (delegate {
				SetCurrent (button);
				selectionManager.Select (c, true);
				ToggleCrewActions (true);
			});
		}
	}

	public void Engage () {

		gameObject.SetActive (false);

		battleManager.BeginBombardment (playerShip, otherShip);
		playerShip.InitializeBattle (battleManager);
		otherShip.InitializeBattle (battleManager);

		playerShip.AddTrust (-TRUST_ACTION_DELTA);
		canChangeCrewPositions = true;
	}

	public void AttemptToEngage () {

		if (PlayerHasFasterShip () || GenerateShipDecision (false) == ShipDecision.ENGAGE) {
			Engage ();
			actionSuccess = true;
			return;
		}

		message.text = "They are faster than us and we cannot engage. They have fled.";
		finishedButton.SetActive (true);
		playerSeesFirstOptions.SetActive (false);
		playerShip.AddTrust (-TRUST_ACTION_DELTA);
		actionSuccess = false;
	}

	public override void BattleOver (string endBattleText, bool gameOver) {

		if (inventoryGained) {
			endBattleText += " We gained " + otherShip.GetFood () + " food, " + 
				otherShip.GetWood () + " wood, " + otherShip.GetGold () + " gold.";
		}

		base.BattleOver (endBattleText, gameOver);
		playerSeesFirstOptions.SetActive (false);
		shipSeesFirstOptions.SetActive (false);
		finishedButton.SetActive (true);

		if (gameOver) {
			Button b = finishedButton.GetComponent<Button> ();
			b.onClick.SetPersistentListenerState (0, UnityEngine.Events.UnityEventCallState.Off);
			b.onClick.AddListener (ReturnToMainMenu);
			finishedButton.GetComponentInChildren<Text> ().text = "Main Menu";
			canChangeCrewPositions = false;
		}
	}

	public void AcceptParley () {

		battleManager.CreateBoardingPlank ();
		PopulateBuyMenu ();
		PopulateCrewMenu ();
		gameObject.SetActive (false);
		buyManager.SetActive (true);
	}

	public void AttemptToParley () {

		ShipDecision decision = GenerateShipDecision (true);

		if (decision == ShipDecision.PARLEY) {
			AcceptParley ();
			return;

		} else if (decision == ShipDecision.ENGAGE) {
			message.text = "The ship is engaging us.";
			continueMessageButton.SetActive (true);
			playerSeesFirstOptions.SetActive (false);
			return;
		}

		message.text = "They are not accepting our parley.";
		playerParleyOptions.SetActive (false);
	}

	public void AttemptSurpriseAttack () {

		ShipDecision decision = GenerateShipDecision (true);

		if (decision == ShipDecision.PARLEY) {
			SurpriseAttack ();
			return;

		} else if (decision == ShipDecision.ENGAGE) {
			message.text = "The ship is engaging us.";
			continueMessageButton.SetActive (true);
			playerSeesFirstOptions.SetActive (false);
			return;
		}

		message.text = "They are not accepting our parley.";
		playerParleyOptions.SetActive (false);
	}

	public void SurpriseAttack () {
		playerShip.AddTrust (-TRUST_ACTION_DELTA);
		BeginBoardingAssult ();
		canChangeCrewPositions = true;
	}

	public void Ignore () {

		if (PlayerHasRangeAdvantage () && GenerateShipDecision (false) == ShipDecision.ENGAGE && 
			!PlayerHasFasterShip ()) {

			message.text = "The ship is engaging us. We cannot escape them.";
			continueMessageButton.SetActive (true);
			playerSeesFirstOptions.SetActive (false);
			actionSuccess = false;
		}

		playerShip.AddTrust (TRUST_ACTION_DELTA);
		actionSuccess = true;
		Finished ();
	}

	protected void BuyFood () {

		InitiateClearInfo ();

		//TODO: make this not hard coded

		int foodToBuy = 2;
		if (!otherShip.CanAfford (foodToBuy, 0, 0)) {
			foodToBuy = otherShip.GetFood ();
		}

		if (playerShip.CanAfford (0, 0, foodToBuy)) {
			playerShip.Spend (0, 0, foodToBuy);
			playerShip.AddInventory (foodToBuy, 0, 0);
			otherShip.Spend (foodToBuy, 0, 0);
			otherShip.AddInventory (0, 0, foodToBuy);

			if (otherShip.GetFood () <= 0) {
				//	Destroy (foodButton.transform.gameObject);
				//	goodsContent.ReadjustPanel ();
				foodButton.interactable = false;
			}

			info.text = "Successfully bought " + foodToBuy + " food for " + foodToBuy + " gold!";

			foodToBuy = otherShip.GetFood () < 2 ? otherShip.GetFood () : 2;
			foodButton.GetComponentInChildren<Text> ().text = "Food: Buy " + foodToBuy + " for " +
				foodToBuy + " Gold\nTotal: " + otherShip.GetFood ();

		} else {
			info.text = "Cannot Afford!";
		}
	}

	protected void BuyWood () {

		InitiateClearInfo ();

		//TODO: make this not hard coded

		int woodToBuy = 2;

		if (!otherShip.CanAfford (0, woodToBuy, 0)) {
			woodToBuy = otherShip.GetWood ();
		}

		if (playerShip.CanAfford (0, 0, woodToBuy)) {
			playerShip.Spend (0, 0, woodToBuy);
			playerShip.AddInventory (0, woodToBuy, 0);
			otherShip.Spend (0, woodToBuy, 0);
			otherShip.AddInventory (0, 0, woodToBuy);

			if (otherShip.GetWood () <= 0) {
				//	Destroy (woodButton.transform.gameObject);
				//	goodsContent.ReadjustPanel ();
				woodButton.interactable = false;
			}

			info.text = "Successfully bought " + woodToBuy + " wood for " + woodToBuy + " gold!";

			woodToBuy = otherShip.GetWood () < 2 ? otherShip.GetWood () : 2;
			woodButton.GetComponentInChildren<Text> ().text = "Wood: Buy " + woodToBuy + " for " +
				woodToBuy + " Gold\nTotal: " + otherShip.GetWood ();

		} else {
			info.text = "Cannot Afford!";
		}
	}

	protected void ToggleCrewActions (bool action) {
		bribe.interactable = action;
		persuade.interactable = action;
	}

	public void Persuade () {

		if (CanFitMoreCrew () && playerShip.persuadeValue > selectionManager.GetSelected ().worth) {
			info.text = "Persaude success!";
			MoveToShip ();
		} else {
			info.text = "Failed to persuade!";
		}

		InitiateClearInfo ();
		RemoveFromCrewList ();
		ToggleCrewActions (false);
	}

	public void ShowBribeMenu () {
		bribeMenu.SetActive (true);
	}

	public void HideBribeMenu () {
		bribeMenu.SetActive (false);
	}

	public void Bribe () {

		int gold = 0;
		InitiateClearInfo ();

		if (!int.TryParse (moneyToBribe.text, out gold)) {
			info.text = "Enter a number!";
			return;
		}

		if (playerShip.CanAfford (0, 0, gold)) {
			if (CanFitMoreCrew () && gold >= selectionManager.GetSelected ().worth) {
				playerShip.Spend (0, 0, gold);
				info.text = "Bribe success!";
				MoveToShip ();
			} else {
				info.text = "Failed to bribe!";
			}

		} else {
			info.text = "Cannot Afford!";
			return;
		}

		RemoveFromCrewList ();
		moneyToBribe.text = "";
		HideBribeMenu ();
		ToggleCrewActions (false);
	}

	protected bool CanFitMoreCrew () {
		return selectionManager.GetSelected () && playerShip.crewMembers.Count < playerShip.GetMaxCrewMembers ();
	}

	protected void MoveToShip () {

		threatenButton.interactable = false;
		CrewMember selected = selectionManager.GetSelected ();
		otherShip.crewMembers.Remove (selected);
		selected.isPlayerCrew = true;
		playerShip.Assign (selected, playerShip.GetVacantPositionByPriority (), true);

		if (otherShip.crewMembers.Count == 0) {
			GainEntireInventory ();
			Finished ();
			info.text += "\nWe have gained the ship's entire crew and their inventory.";
		}
	}

	protected void RemoveFromCrewList () {

		Destroy (current.gameObject);
		current.transform.SetParent (null);
		otherPeopleContent.ReadjustPanel ();
		selectionManager.Select (null, true);
	}

	public void ThreatenEngagement () {

		InitiateClearInfo ();
		playerShip.AddTrust (-TRUST_ACTION_DELTA);

		if (IsThreatenSuccessful ()) {
			GainEntireInventory ();
			Destroy (foodButton.transform.gameObject);
			info.text = "Successfully stolen ships inventory.";
			Finished ();
			return;
		} 

		info.text = "Threat was not successful, prepare for assult.";
		buyManager.SetActive (false);
		BeginBoardingAssult ();
	}

	public override void GainEntireInventory () {

		base.GainEntireInventory ();

		int f = otherShip.GetFood ();
		int w = otherShip.GetWood ();
		int g = otherShip.GetGold ();
		playerShip.AddInventory (f, w, g);
	//	otherShip.Spend (f, w, g);
	}

	protected void InitiateClearInfo () {
		CancelInvoke ("ClearInfoText");
		Invoke ("ClearInfoText", 3);
	}

	protected void ClearInfoText () {
		info.text = "";
	}

	public void ShowGoodsMenu () {
		mainMenu.SetActive (false);
		goodsMenu.SetActive (true);
	}

	public void ShowCrewMenu () {
		mainMenu.SetActive (false);
		crewMenu.SetActive (true);
		ToggleCrewActions (false);
		canChangeCrewSelection = false;
	}

	public void ShowMainMenu () {
		mainMenu.SetActive (true);
		crewMenu.SetActive (false);
		goodsMenu.SetActive (false);
		selectionManager.Select (null, true);

		if (current) {
			current.interactable = true;
		}

		canChangeCrewSelection = true;
	}

	public void BeginBoardingAssult () {

		gameObject.SetActive (false);

		battleManager.BeginBoardingAssault (playerShip, otherShip);

		playerShip.InitializeBattle (battleManager);
		otherShip.InitializeBattle (battleManager);
	}
		
	public void Finished () {
		buyManager.SetActive (false);
		ProceedToNextZone ();
	}
}
