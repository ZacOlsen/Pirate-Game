using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SectorButtonNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	
	public List<SectorButtonNode> children;
	private bool visited;
	public static SectorButtonNode current {
		private set;
		get;
	}

	public Button button {
		private set;
		get;
	}

	public Text text {
		private set;
		get;
	}

	void Awake () {

		button = GetComponent<Button> ();
		text = GetComponentInChildren<Text> ();
		text.enabled = false;

		button.onClick.AddListener (BeginEvent);
	}

	void OnEnable () {
		SceneManager.sceneLoaded += StartScene;
	}

	void OnDisable () {
		SceneManager.sceneLoaded -= StartScene;
	}

	void StartScene (Scene scene, LoadSceneMode mode) {

		ColorBlock cb = button.colors;

		if (current == this) {
			cb.disabledColor = Color.green;
			visited = true;

		} else if (visited) {
			cb.disabledColor = Color.red;
		}

		button.colors = cb;
	}

	public void OnPointerEnter (PointerEventData pointer) {
#if UNITY_EDITOR
	//	text.enabled = true;
#endif
	}

	public void OnPointerExit (PointerEventData pointer) {
#if UNITY_EDITOR
	//	text.enabled = false;
#endif
	}

	private void BeginEvent () {
		BeginEvent (text.text);
	}

	public void BeginEvent (string eventName) {
		transform.parent.gameObject.SetActive (true);
		transform.parent.SetParent (null);
		DontDestroyOnLoad (transform.parent.gameObject);

		SceneManager.LoadScene (eventName);
		EnableNextSectorOptions ();
	}

	public void EnableNextSectorOptions () {

		if (current) {
			for(int i = 0; i < current.children.Count; i++) {
				current.children[i].button.interactable = false;
			}
		}

		current = this;

		button.interactable = false;
	//	ColorBlock cb = button.colors;
	//	cb.disabledColor = Color.green;
	//	button.colors = cb;

		for (int i = 0; i < children.Count; i++) {
			children[i].button.interactable = true;
		}
	}
}
