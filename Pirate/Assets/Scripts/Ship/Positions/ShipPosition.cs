using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ShipPosition : MonoBehaviour {

	[SerializeField] private CrewMember.Role posRole = CrewMember.Role.CAPTAIN;
	public bool isManned = false;
	private SelectionManager selectManager = null;

	public MovementTile tile;

	public bool isAssignAbleTo = false;

	protected void Start () {
		RelocateSelector ();
	}

	void OnDestroy () {
		SceneManager.sceneLoaded -= StartScene;
	}

	public CrewMember.Role getRole () {
		return posRole;
	}

	private void OnMouseOver () {
		if (Input.GetMouseButtonUp (1) && isAssignAbleTo && !EventSystem.current.IsPointerOverGameObject ()) {
			selectManager.AssignTo (this);
		}
	}

	void OnEnable () {
		SceneManager.sceneLoaded += StartScene;
	}

	void StartScene (Scene scene, LoadSceneMode mode) {
		RelocateSelector ();
	}

	private void RelocateSelector () {
		selectManager = GameObject.Find ("SelectionManager").GetComponent<SelectionManager> ();
	}
}
