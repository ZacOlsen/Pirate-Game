using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmptyZoneEvent : MonoBehaviour {

	[SerializeField] protected Text message;
	[SerializeField] protected Button mapButton;
	[SerializeField] private GameObject report = null;
	protected PlayerShip playerShip;

	public bool canChangeCrewPositions {
		protected set;
		get;
	}

	public bool canChangeCrewSelection {
		protected set;
		get;
	}

	protected void Start () {

		mapButton.interactable = false;
		playerShip = GameObject.Find ("ship").GetComponent<PlayerShip> ();
		report.GetComponentInChildren<Text> ().text = playerShip.sectorEndReport;
		gameObject.SetActive (false);
		canChangeCrewSelection = true;
	}

	public void CloseReport () {
		report.SetActive (false);
		gameObject.SetActive (true);
	}

	public void ProceedToNextZone () {
		mapButton.interactable = true;
		gameObject.SetActive (false);
		canChangeCrewPositions = true;
	}
}
