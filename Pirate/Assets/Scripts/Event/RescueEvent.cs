using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RescueEvent : EncounterPeopleEvent {

	public GameObject plankPrefab;
	public ContentManager islandContent;
	public ContentManager crewContent;

	public GameObject exchangeManager;

	private int food;

	private Button current;

	public Beach beach;

	protected new void Start () {

		base.Start ();

		int islandPeopleNum = Random.Range (minNumOfPeople, maxNumOfPeople + 1);

		food = Random.Range (5, 15);

		if (islandPeopleNum == 0) {
			message.text = "No one was found alive on this island.\nWe have found " + food +
				" food on this island.\nProceed to next sector.";
			
		} else {
			
			for (int i = 0; i < islandPeopleNum; i++) {
				CrewMember c = CreateCrewMember (beach.GetRandomTile ());
				otherPeople.Add (c);
				CreateButton (c, islandContent);
			}
				
			for (int i = 0; i < playerShip.crewMembers.Count; i++) {
				CrewMember c = playerShip.crewMembers [i];
				CreateButton (c, crewContent);
			}

			message.text = "We have found " + islandPeopleNum + " people alive on this island.\n" +
				"We have found " + food + " food on this island.";
		}

		exchangeManager.SetActive (false);
		gameObject.SetActive (false);
		playerShip.AddInventory (food, 0, 0);

		CreateBoardingPlank ();
	}

	protected void OnDestroy () {
		//TODO: change from hard coded
		AStar.RemoveLayerFromLayout (3);
	}

	public void OkButton () {

		gameObject.SetActive (false);

		if (otherPeople.Count == 0) {
			ProceedToNextZone ();
			return;
		}
			
		exchangeManager.SetActive (true);
		canChangeCrewSelection = false;
	}
		
	public void MoveToCrew () {

		if (!current || playerShip.crewMembers.Contains (selectionManager.GetSelected ())) {
			return;
		}

		if (playerShip.crewMembers.Count < playerShip.GetMaxCrewMembers ()) {

			crewContent.AddToPanel (current);
			islandContent.ReadjustPanel ();

			CrewMember selected = selectionManager.GetSelected ();
			otherPeople.Remove (selected);
			selected.isPlayerCrew = true;
			playerShip.Assign (selected, playerShip.GetVacantPositionByPriority (), true);
		}
	}

	public void MoveToIsland () {

		if (!current || otherPeople.Contains (selectionManager.GetSelected ())) {
			return;
		}

		islandContent.AddToPanel (current);
		crewContent.ReadjustPanel ();

		CrewMember selected = selectionManager.GetSelected ();
		playerShip.Unassign (selected);
		selected.isPlayerCrew = false;
		otherPeople.Add (selected);
		selected.BeginPath (AStar.FindPath (selected.current, beach.GetRandomTile (), true));
	}
		
	public void Finished () {

		for (int i = 0; i < otherPeople.Count; i++) {
			Destroy (otherPeople [i].gameObject);
		}

		exchangeManager.SetActive (false);

		canChangeCrewPositions = true;
		canChangeCrewSelection = true;

		ProceedToNextZone ();
	}

	private void SetCurrent (Button button) {

		if (current) {
			current.interactable = true;
		}

		current = button;
		button.interactable = false;
	}

	private void CreateButton (CrewMember c, ContentManager cm) {

		Button button = cm.CreateButton (c.GetComponent<SpriteRenderer> ().sprite, c.personName);

		button.onClick.AddListener (delegate {
			selectionManager.Select (c, true);
			SetCurrent (button);
		});
	}

	private void CreateBoardingPlank () {

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

		LevelLayout.Portal portalPlayerPlank = new LevelLayout.Portal (playerShip.GetBoardingTile (), plankTiles [0, 0]);
		AStar.AddPortalToLayout (portalPlayerPlank);

		LevelLayout.Portal portalPlankShip = new LevelLayout.Portal (plankTiles [1, 0], beach.boardingTile);
		AStar.AddPortalToLayout (portalPlankShip);
	}

}
