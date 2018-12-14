using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour {

	private static string[] allSceneNames = { "EmptyZoneEvent", "MessageInBottleRescueEvent",
		"MessageInBottleTreasureEvent", "FishingShipEvent", "ArmadaShipEvent", "MerchantShipEvent",
		"RoyalNavyShipEvent"
	};

	private static string[] earlySceneNames = { "EmptyZoneEvent", "MessageInBottleRescueEvent",
		"MessageInBottleTreasureEvent"
	};

	private static string[] lateSceneNames = { "FishingShipEvent", "ArmadaShipEvent", "MerchantShipEvent",
		"RoyalNavyShipEvent"
	};

	private const string L_SYSTEM = "EEEEAAAALLL";

	private PlayerShip ship;
	private Text info;
	private GameObject camModePackage;

	void Awake () {

#if UNITY_EDITOR
		//Random.InitState (47);
		//TODO: maybe remove?
#endif

		GenerateSceneNames (transform.GetChild (0).GetComponent<SectorButtonNode> (), 0);
		ship = GameObject.Find ("ship").GetComponent<PlayerShip> ();
	}

	void OnEnable () {
		SceneManager.sceneLoaded += StartScene;
	}

	void OnDisable () {
		SceneManager.sceneLoaded -= StartScene;
	}

	void StartScene (Scene scene, LoadSceneMode mode) {

		gameObject.SetActive (false);
		transform.SetParent (GameObject.Find ("Canvas").transform, false);
		transform.SetSiblingIndex (1); //TODO: hard coded for now

		((RectTransform)transform).anchoredPosition = Vector3.zero;
		((RectTransform)transform).sizeDelta = Vector2.zero;
		transform.localScale = Vector3.one;

		info = GameObject.Find ("Info").GetComponent<Text> ();

		GameObject.Find ("OpenMap").GetComponent<Button> ().onClick.AddListener (ToggleMap);
		camModePackage = GameObject.Find ("Cam Mode Package");
	}

	private void GenerateSceneNames (SectorButtonNode sbNode, int layer) {

		if (layer == 0) {
			sbNode.text.text = "InitialTest";

		} else if (sbNode.children.Count == 0) {
			sbNode.text.text = "VictoryEvent";

		} else {
			//TODO: used for hardcoded levels
			if (sbNode.text.text == "Button") {

				switch (L_SYSTEM[layer]) {
					case 'E':
						sbNode.text.text = earlySceneNames[Random.Range (0, earlySceneNames.Length)];
						break;
					case 'A':
						sbNode.text.text = allSceneNames[Random.Range (0, allSceneNames.Length)];
						break;
					case 'L':
						sbNode.text.text = lateSceneNames[Random.Range (0, lateSceneNames.Length)];
						break;
				}
			}
		}

		sbNode.button.interactable = false;
		for(int i = 0; i < sbNode.children.Count; i++) {
			SectorButtonNode child = sbNode.children[i];
			GenerateSceneNames (child, layer + 1);
		}

		if (layer == 0) {
			sbNode.EnableNextSectorOptions ();
		}
	}

	private void ToggleMap () {

		CancelInvoke ();

		if (!ship.wheel.isManned) {
			info.text = "Map cannot be opened without captain assigned.";
			Invoke ("ClearInfoText", 3);
			return;
		}

		if (ship.damagedTiles.Count > 0) {
			info.text = "cannot leave sector while ship is damaged.";
			Invoke ("ClearInfoText", 3);
			return;
		}

		gameObject.SetActive (!gameObject.activeSelf);
		camModePackage.SetActive (!gameObject.activeSelf);
	}

	private void ClearInfoText () {
		info.text = "";
	}

	public static void StartEvent (string sceneName) {
		int rand = Random.Range (0, SectorButtonNode.current.children.Count);
		SectorButtonNode.current.children[rand].BeginEvent (sceneName);
	}
}
