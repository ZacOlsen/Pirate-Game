using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentManager : MonoBehaviour {

	public GameObject buttonPrefab;

	public Button CreateButton (Sprite sprite, string text) {
		
		Button button = Instantiate (buttonPrefab).GetComponent<Button> ();
		button.transform.Find ("Image").GetComponent<Image> ().sprite = sprite;
		button.GetComponentInChildren<Text> ().text = text;

		AddToPanel (button);

		return button;
	}

	public void AddToPanel (Button button) {
		button.transform.SetParent (transform, false);
		ReadjustPanel ();
	}

	public void ReadjustPanel () {

		float buttonHeight = ((RectTransform) buttonPrefab.transform).sizeDelta.y;
		RectTransform t = (RectTransform)transform;
		t.sizeDelta = new Vector2 (t.sizeDelta.x, buttonHeight * t.childCount);

		for (int i = 0; i < t.childCount; i++) {
			((RectTransform) t.GetChild (i)).anchoredPosition = 
				new Vector3 (0, buttonHeight * t.childCount / 2 - buttonHeight * i - buttonHeight / 2, 0);
		}

		t.anchoredPosition = Vector3.zero;
	}
}
