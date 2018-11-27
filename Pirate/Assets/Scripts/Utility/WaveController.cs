using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

	[SerializeField] private float xRange = 2;
	[SerializeField] private float yRange = 1;
	[SerializeField] private float speed = .01f;

	private float iterator = -1f;
	private Vector3 startPos;

	[SerializeField] private WaveDirection direction = WaveDirection.RIGHT;

	private enum WaveDirection {
		RIGHT,
		LEFT
	}

	void Start () {
		startPos = transform.position;
		//TODO: make random direction
	}

	void FixedUpdate () {

		switch (direction) {
			case WaveDirection.RIGHT:
				iterator += speed;

				if (iterator > xRange / 2f) {
					direction = WaveDirection.LEFT;
				}

				break;

			case WaveDirection.LEFT:
				iterator -= speed;

				if (iterator < -xRange / 2f) {
					direction = WaveDirection.RIGHT;
				}

				break;
		}

		Vector3 newPoint = new Vector3 (iterator * 2f / xRange, iterator * iterator * yRange / 2f, 0);
		transform.position = newPoint + startPos;
	}
}
