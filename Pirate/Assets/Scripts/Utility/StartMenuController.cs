using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour {

	[SerializeField] private Button playButton = null;
	[SerializeField] private InputField seedInput = null;

	void Start () {
		Random.InitState (System.DateTime.Now.Millisecond);
	}

	public void Play () {
		int seed = int.Parse (seedInput.text);
		Random.InitState (seed);
		SceneManager.LoadScene ("InitialTest");
	}

	public void GenerateRandomSeed () {
		seedInput.text = Random.Range (int.MinValue, int.MaxValue).ToString ();
	}

	public void SeedIsValid (string value) {
		int seed;
		playButton.interactable = int.TryParse (value, out seed);
	}
}
