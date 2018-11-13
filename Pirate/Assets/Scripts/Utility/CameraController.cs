using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Camera primary;
	public Camera altCam1;
	public Camera altCam2;

	public Vector3 topLevelPos;
	public Vector3 botLevelPos;

	public GameObject toggleShipViewButton;

	private static CameraMode mode = CameraMode.SPLIT;
	private static CameraShipLevelView view = CameraShipLevelView.TOP;

	private enum CameraMode {
		TOGGLE,
		SPLIT
	}
		
	private enum CameraShipLevelView {
		TOP,
		BOT
	}

	void Start () {
		ActivateCameraMode ();
		ActivateToggledView ();
	}

	public void ChangeCameraMode () {

		//swap to opposite of current
		switch (mode) {
		case CameraMode.TOGGLE:
			mode = CameraMode.SPLIT;
			break;
		case CameraMode.SPLIT:
			mode = CameraMode.TOGGLE;
			break;
		}

		ActivateCameraMode ();
	}

	private void ActivateCameraMode () {
		switch (mode) {
		case CameraMode.TOGGLE:
			SetCameraActives (true);
			break;
		case CameraMode.SPLIT:
			SetCameraActives (false);
			break;
		}
	}

	public void ToggleLevelView () {

		//swap to opposite of current
		switch (view) {
		case CameraShipLevelView.TOP:
			view = CameraShipLevelView.BOT;
			break;
		case CameraShipLevelView.BOT:
			view = CameraShipLevelView.TOP;
			break;
		}

		ActivateToggledView ();
	}

	private void ActivateToggledView () {

		switch (view) {
		case CameraShipLevelView.TOP:
			primary.transform.position = topLevelPos;
			break;
		case CameraShipLevelView.BOT:
			primary.transform.position = botLevelPos;
			break;
		}
	}

	private void SetCameraActives (bool actives) {
		primary.enabled = actives;
		toggleShipViewButton.gameObject.SetActive (actives);
		altCam1.enabled = !actives;
		altCam2.enabled = !actives;
	}
}
