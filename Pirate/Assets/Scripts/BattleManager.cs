using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {

	public GameObject plankPrefab;
	public Text timer;
	public Button fleeButton;

	private bool hasFleed;
	private bool createdPlank;

	[SerializeField] private BattlePhase phase;
	[SerializeField] private int bombardmentTime = 60;

	public PlayerShip playerShip {
		private set;
		get;
	}

	public Ship otherShip {
		private set;
	 	get;
	}

	private List<CrewMember> playerPriorities = new List<CrewMember> ();
	private List<CrewMember> otherPriorities = new List<CrewMember> ();

	private MovementTile playerBoarding;
	private MovementTile otherBoarding;

	public BattleEvent battleEvent;

	private const float BATTLE_LETHALITY_DELTA = .1f;

	private enum BattlePhase {
		BOMBARDMENT,
		BOARDING,
		OVER
	}

	void FixedUpdate () {

		if (phase == BattlePhase.BOARDING) {
			ReEvaluatePriorities ();
		}
	}

	void OnDestroy () {

		if (fleeButton) {
			Destroy (fleeButton.gameObject);
		}
	}

	public void Init (MovementTile playerBoarding, MovementTile otherBoarding) {
		this.playerBoarding = playerBoarding;
		this.otherBoarding = otherBoarding;
	}

	private void TimerCountDown () {

		int min = bombardmentTime / 60;
		int sec = bombardmentTime % 60;

		timer.text = "Bombardment Time Remaining: " + min + ":" + sec.ToString ("00");
		bombardmentTime--;

		if (bombardmentTime < 0) {
			CancelInvoke ();
			BeginBoardingAssault (playerShip.crewMembers, otherShip.crewMembers);
		}
	}

	public void BeginBombardment (PlayerShip player, Ship other) {
		
		InvokeRepeating ("TimerCountDown", 0, 1);
		phase = BattlePhase.BOMBARDMENT;

		playerShip = player;
		otherShip = other;

		fleeButton.interactable = false;
		fleeButton.gameObject.SetActive (true);
	}

	public void BeginBoardingAssault (PlayerShip player, Ship other) {
		playerShip = player;
		otherShip = other;
		BeginBoardingAssault (playerShip.crewMembers, other.crewMembers);
		otherShip.InitializeBattle (this);
	}

	public void BeginBoardingAssault (PlayerShip player, List<CrewMember> other) {
		playerShip = player;
		BeginBoardingAssault (playerShip.crewMembers, other);
	}

	private void BeginBoardingAssault (List<CrewMember> playerCrew, List<CrewMember> otherCrew) {

		CreateBoardingPlank ();

		fleeButton.interactable = true;
		fleeButton.gameObject.SetActive (true);

		phase = BattlePhase.BOARDING;
		CancelInvoke ();
		timer.text = "Boarding Phase";

		PopulatePriorityList (playerCrew, playerPriorities);
		PopulatePriorityList (otherCrew, otherPriorities);

		ReEvaluatePriorities ();

		playerShip.InitializeBattle (this);
	}

	public void Flee () {
		
		hasFleed = true;
		timer.text = "Repair Phase";
		fleeButton.interactable = false;
		battleEvent.BattleOver ("You have successfully fled.", false);
		playerShip.AddLethality (-BATTLE_LETHALITY_DELTA);
	}

	public void ShipDies (Ship ship) {

		if (phase != BattlePhase.OVER) {
			
			phase = BattlePhase.OVER;

			if (ship == playerShip) {
				LostBattle ();

			} else {
				WonBattle ();
			}
		}
	}

	private void LostBattle () {
		Text info = GameObject.Find ("Info").GetComponent<Text> ();
		info.text = "player has died";
		battleEvent.BattleOver ("Your journey has ended. Return to Main Menu.", true);
		GameObject.Find ("OpenMap").GetComponent<Button> ().interactable = false;
	}

	private void WonBattle () {

		Text info = GameObject.Find ("Info").GetComponent<Text> ();
		info.text = "enemy has died";

		if (phase == BattlePhase.BOARDING) {
			battleEvent.GainEntireInventory ();
		}

		//battle over called after because gain inventory needs to happen first for message to
		//display properly
		battleEvent.BattleOver ("You have successfully killed the enemy!", false);
		playerShip.AddLethality (BATTLE_LETHALITY_DELTA);
	}

	private void PopulatePriorityList (List<CrewMember> crew, List<CrewMember> priorityList) {

		for (int i = 0; i < crew.Count; i++) {
			priorityList.Add (crew [i]);
		}
	}

	public void ReEvaluatePriorities () {

		if (playerPriorities.Count == 0) {
			LostBattle ();
			return;
		}

		if (otherPriorities.Count == 0) {
			WonBattle ();
			return;
		}

		RePrioritize (playerPriorities, otherBoarding);
		RePrioritize (otherPriorities, playerBoarding);
	}

	private void RePrioritize (List<CrewMember> priorities, MovementTile tile) {

		for (int i = 0; i < priorities.Count; i++) {
			if (!priorities [i]) {
				priorities.RemoveAt (i);
				i--;
			}
		}

		for (int i = priorities.Count - 1; i > 0; i--) {
			for (int j = i; j > 0; j--) {

				if (CompareCrewMember (priorities [j - 1], priorities [j], tile) > 0) {
					CrewMember c = priorities [j - 1];
					priorities [j - 1] = priorities [j];
					priorities [j] = c;
				} else {
					break;
				}
			}
		}
	}

	private int CompareCrewMember (CrewMember crew1, CrewMember crew2, MovementTile tile) {

		if (crew1.role == CrewMember.Role.CANNONEER) {

			if (crew2.role == CrewMember.Role.CANNONEER) {
				return IsCloserToThan (crew1, crew2, tile);
			}

			return -1;

		} else if (crew2.role == CrewMember.Role.CANNONEER) {
			return 1;

		} else {
			return IsCloserToThan (crew1, crew2, tile);			
		}
	}

	private int IsCloserToThan (CrewMember crew1, CrewMember crew2, MovementTile tile) {
		
		float disToPortal1 = Vector2.SqrMagnitude (crew1.transform.position - tile.transform.position);
		float disToPortal2 = Vector2.SqrMagnitude (crew2.transform.position - tile.transform.position);

		if (disToPortal1 == disToPortal2) {
			return 0;
		}

		return disToPortal1 < disToPortal2 ? -1 : 1;
	}

	public CrewMember GetNextPlayerPriority () {
		return (!hasFleed && playerPriorities.Count > 0) ? playerPriorities [0] : null;
	}

	public CrewMember GetNextOtherPriority () {
		return (!hasFleed && otherPriorities.Count > 0) ? otherPriorities [0] : null;
	}

	public MovementTile GetRandomTile (bool playerTile) {

		int layer =  playerTile ? playerShip.layers [Random.Range (0, playerShip.layers.Length)] 
			: otherShip.layers [Random.Range (0, otherShip.layers.Length)];

		return AStar.GetRandomTileOnLayer (layer);
	}

	public void CreateBoardingPlank () {

		if (createdPlank) {
			return;
		}

		createdPlank = true;

		Transform plank = Instantiate (plankPrefab, new Vector2 (2, .25f), Quaternion.identity).transform;
		MovementTile[,] plankTiles = new MovementTile[2, 1];

		for (int x = 0; x < plankTiles.GetLength (0); x++) {
			for (int y = 0; y < plankTiles.GetLength (1); y++) {
				plankTiles [x, y] = plank.GetChild (x * 1 + y).GetComponent<MovementTile> ();
				plankTiles [x, y].x = x;
				plankTiles [x, y].y = y;
			}
		}

		AStar.AddAreaToLayout (plankTiles);

		LevelLayout.Portal portalPlayerPlank = new LevelLayout.Portal (playerBoarding, plankTiles [0, 0]);
		AStar.AddPortalToLayout (portalPlayerPlank);

		LevelLayout.Portal portalPlankShip = new LevelLayout.Portal (plankTiles [1, 0], otherBoarding);
		AStar.AddPortalToLayout (portalPlankShip);
	}
}
