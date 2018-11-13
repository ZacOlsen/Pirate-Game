using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerShip : Ship {

	public float trustworthiness {
		private set;
		get;
	}

	public float lethality {
		private set;
		get;
	}

	/**
	 * Clamped between [-1, 1]. positive is in favor or royal navy, negative is in favor of armada
	 */
	public float factionTrust {
		private set;
		get;
	}

	public int persuadeValue {
		get { return GetCaptain ().intelligence + crewMembers.Count; }
	}

	public PactStatus royalNavyStatus;
	public PactStatus armadaStatus;

	private Text stats;

	public string sectorEndReport {
		private set;
		get;
	}

	private bool firstSectorStarted;

	public enum PactStatus {
		NOT_FORMED,
		FORMED,
		BROKEN
	}

	protected new void Start () {

		base.Start ();

		DontDestroyOnLoad (this);
		trustworthiness = .5f;
		lethality = .5f;
	}

	void OnEnable () {
		SceneManager.sceneLoaded += StartScene;
		SceneManager.activeSceneChanged += ChangeScene;
	}

	void OnDisable () {
		SceneManager.sceneLoaded -= StartScene;
		SceneManager.activeSceneChanged -= ChangeScene;
	}

	public new bool Assign (CrewMember crew, ShipPosition shipPos, bool asNewMemeber = false) {

		bool result = base.Assign (crew, shipPos, asNewMemeber);

		UpdateShipStats ();

		if (result && asNewMemeber) {
			//spagehtti due to unable to reorder this to end of list, needs to be at end so crewmember changescene runs first
			SceneManager.activeSceneChanged -= ChangeScene;
			SceneManager.activeSceneChanged += ChangeScene;
		}

		return result;
	}

	void StartScene (Scene scene, LoadSceneMode mode) {
		firstSectorStarted = true;
		stats = GameObject.Find ("Stats").GetComponent<Text> ();
		UpdateShipStats ();
	}

	private void ChangeScene (Scene prev, Scene next) {

		if (!firstSectorStarted) {
			return;
		}

		//flooded loop
		for (int i = 0; i < floodedTiles.Count; i++) {
			Destroy (floodedTiles[i].gameObject);
		}

		floodedTiles.Clear ();

		string woodReport = "";
		if (carpentersQuaters.isManned) {
			int crewRepairAmount = carpentersQuaters.tile.crewMem.endSectorRepairAmount;
			int needToRepair = (maxHealth - health) >= crewRepairAmount ? crewRepairAmount : maxHealth - health;
			int woodAvailableToUse = CanAfford (0, needToRepair, 0) ? needToRepair : wood;
			if (woodAvailableToUse > 0) {
				Spend (0, woodAvailableToUse, 0, false);
				Heal (woodAvailableToUse);
				woodReport += "Ship was repaired for " + woodAvailableToUse + " by using " + woodAvailableToUse + " wood. ";
			}
		}

		//food and doctor loop
		string doctorReport = "";
		string foodReport = "";
		
		if (doctorsQuaters.isManned) {

			for (int i = 0; i < crewMembers.Count; i++) {

				CrewMember c = crewMembers[i];
				int healed = 0;
				bool starve = false;
				bool died = false;

				if (food > 0) {
					food = Mathf.Clamp (food - 2, 0, int.MaxValue);
					healed = c.Heal (doctorsQuaters.tile.crewMem.healingAbility);

				} else {
					starve = true;

					if (c.health < 4) {
						healed = c.Heal (doctorsQuaters.tile.crewMem.healingAbility);
						died = c.TakeDamage (4);
					} else {
						died = c.TakeDamage (4);
						healed = c.Heal (doctorsQuaters.tile.crewMem.healingAbility);
					}
				}

				if (healed > 0) {
					doctorReport += c.personName + " was healed for " + (healed) + ". ";
				}

				if (died) {
					foodReport += c.personName + " has died from starvation. ";
					i--;
				} else if (starve) {
					foodReport += c.personName + " has taken 4 damage due to starvation. ";
				}
			}
		}

		if (food == 0) {
			foodReport = "We have run out of food! " + foodReport;
		} else {
			foodReport = "We have used " + (crewMembers.Count * 2) + " food. ";
		}

		sectorEndReport = foodReport + doctorReport + woodReport;

		//TODO: training happen before stats updated maybe?
		for (int i = 0; i < crewMembers.Count; i++) {

			switch (crewMembers[i].role) {

				case CrewMember.Role.CAPTAIN:
					crewMembers[i].dexterity += 1;
					crewMembers[i].intelligence += 1;
					break;

				case CrewMember.Role.NAVIGATOR:
				case CrewMember.Role.DOCTOR:
					crewMembers[i].intelligence += 1;
					break;

				case CrewMember.Role.BAILER:
				case CrewMember.Role.CARPENTER:
				case CrewMember.Role.CANNONEER:
					crewMembers[i].dexterity += 1;
					break;
			}
		}
	}

	public void AddInventory (int food, int wood, int gold, bool updateStats = true) {
		base.AddInventory (food, wood, gold);

		if (updateStats) {
			UpdateShipStats ();
		}
	}

	public bool Spend (int food, int wood, int gold, bool updateStats = true) {
		bool result = base.Spend (food, wood, gold);

		if (updateStats) {
			UpdateShipStats ();
		}

		return result;
	}

	public void ForceSpend (int food, int wood, int gold, bool updateStats = true) {
		base.ForceSpend (food, wood, gold);

		if (updateStats) {
			UpdateShipStats ();
		}
	}

	private void UpdateShipStats () {
		stats.text = "<color=magenta>Food: " + food +
			"</color><color=#614133>\nWood: " + wood +
			"</color><color=yellow>\nGold: " + gold +
			"</color><color=red>\nHeath: " + health + "/" + maxHealth +
			"</color>\nSpeed: " + speed +
			"<color=purple>\nVisionRange: " + visionRange + "</color>";
	}

	public void AddTrust (float delta) {
		trustworthiness = Mathf.Clamp01 (trustworthiness + delta);
	}

	public void AddLethality (float delta) {
		lethality = Mathf.Clamp01 (lethality + delta);
	}

	public void AddInFavorOfArmada (float trustDelta) {
		AdjustFactionTrust (-trustDelta);  //flip because armada is negative
	}

	public void AddInFavorOfRoyalNavy (float trustDelta) {
		AdjustFactionTrust (trustDelta);
	}

	private void AdjustFactionTrust (float delta) {
		factionTrust = Mathf.Clamp (factionTrust + delta, -1f, 1f);
	}
}
