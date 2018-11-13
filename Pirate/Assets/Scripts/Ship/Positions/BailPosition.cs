using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BailPosition : ShipPosition {

	[SerializeField] private Sprite none = null, one = null, twoAdjacent = null, twoAcross = null, 
		twoAdjacentDiagonal = null, three = null, threeDiagonalRight = null, threeDiagonalLeft = null, 
		threeTwoDiagonal = null, four = null, fourOneDiagonal = null, fourTwoDiagonalAdjacent = null,
		fourTwoDiagonalAcross = null, fourThreeDiagonal = null, fourFourDiagonal = null;

    public SpriteRenderer sr;
	private const float Z_VAL = -.025f;

	public bool printe;

	protected new void Start () {
		base.Start ();

		Vector3 pos = transform.position;
		pos.z = Z_VAL;
		transform.position = pos;
		UpdateTileImage();
	}

	public void Bail () {

		tile.floodedTile = null;
		Destroy (gameObject);

		List<MovementTile> adjs = FindAdjacents ();
		for (int i = 0; i < adjs.Count; i++) {
			adjs[i].floodedTile.UpdateTileImage ();
		}
	}

	void Update () {
		if (printe) {
			printe = false;
			PrintAdjacents ();
		}
	}

    public void UpdateTileImage () {

		List<MovementTile> adjacents = FindAdjacents ();
		List<MovementTile> diagonals = FindDiagonals (adjacents);

		Sprite current = sr.sprite;

		switch (adjacents.Count) {
			case 0:
				sr.sprite = none;
				break;

			case 1:
				sr.sprite = one;
				RotateTowardsAdjacent (adjacents[0]);
				break;

			case 2:

				MovementTile t1 = adjacents[0];
				MovementTile t2 = adjacents[1];
				Vector2Int vec = new Vector2Int (t1.x - t2.x, t1.y - t2.y);

				if (vec.x * vec.x + vec.y * vec.y == 4) {
					RotateTowardsAdjacent (adjacents[0]);
					sr.sprite = twoAcross;
				} else {
					RotateTowardsDiagonal (AStar.GetTile(t1.x - tile.x + t2.x, 
						t1.y - tile.y + t2.y, tile.layer));
					sr.sprite = diagonals.Count == 1 ? twoAdjacentDiagonal : twoAdjacent;
				}

				break;

			case 3:

				//find where adjacent isnt
				MovementTile second = null;
				MovementTile[] neighbors = AStar.GetTileNeighbors (tile);
				for (int i = 0; i < neighbors.Length; i++) {
					if (!neighbors[i] || !neighbors[i].floodedTile) {
						second = neighbors[(i + 2) % 4];
					}
				}

				RotateTowardsAdjacent (second);
				switch (diagonals.Count) {

					case 0:
						sr.sprite = three;
						break;

					case 1:

						float angle = Vector2.SignedAngle (second.transform.position - transform.position,
							diagonals[0].transform.position - transform.position);
	
						sr.sprite = angle > 0 ? threeDiagonalLeft : threeDiagonalRight;
						break;

					case 2:
						sr.sprite = threeTwoDiagonal;
						break;
				}

				break;

			case 4:

				switch (diagonals.Count) {

					case 0:
						sr.sprite = four;
						break;

					case 1:

						RotateTowardsDiagonal (diagonals[0]);
						sr.sprite = fourOneDiagonal;
						break;

					case 2:

						MovementTile d1 = diagonals[0];
						MovementTile d2 = diagonals[1];
						if ((d1.x - d2.x) * (d1.x - d2.x) + (d1.y - d2.y) * (d1.y - d2.y) == 4) {

							int x = (d1.x + d2.x) / 2;
							int y = (d1.y + d2.y) / 2;
							MovementTile middle = AStar.GetTile (x, y, tile.layer);

							RotateTowardsAdjacent (middle);

							sr.sprite = fourTwoDiagonalAdjacent;

						} else {

							RotateTowardsDiagonal (diagonals[0]);
							sr.sprite = fourTwoDiagonalAcross;
						}

						break;

					case 3:

						int tx = -(diagonals[0].x + diagonals[1].x + diagonals[2].x - 3 * tile.x) + tile.x;
						int ty = -(diagonals[0].y + diagonals[1].y + diagonals[2].y - 3 * tile.y) + tile.y;
						MovementTile t = AStar.GetTile (tx, ty, tile.layer);

						RotateTowardsDiagonal (t);
						sr.sprite = fourThreeDiagonal;
						break;

					case 4:
						sr.sprite = fourFourDiagonal;
						break;
				}

				break;
		}

		if (current != sr.sprite) {
			for (int i = 0; i < adjacents.Count; i++) {
				if (adjacents[i] && adjacents[i].floodedTile) {
					adjacents[i].floodedTile.UpdateTileImage ();
				}
			}
		}
	}

	private void RotateTowardsDiagonal (MovementTile t) {
		Vector2 targetDir = t.transform.position - transform.position;
		float angle = Mathf.Atan2 (targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90 - 45;
		transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
	}

	private void RotateTowardsAdjacent (MovementTile t) {
		transform.LookAt (transform.position + Vector3.forward,
			t.transform.position - transform.position);
	}

	private void PrintAdjacents () {

		List<MovementTile> adjacents = FindAdjacents ();
		Debug.Log ("adjacents");
		for (int i = 0; i < adjacents.Count; i++) {
			Debug.Log (adjacents[i].x + " " + adjacents[i].y);
		}

		List<MovementTile> diagonals = FindDiagonals (adjacents);
		Debug.Log ("Diagonals");
		for (int i = 0; i < diagonals.Count; i++) {
			Debug.Log (diagonals[i].x + " " + diagonals[i].y);
		}

		Debug.Break ();
	}

	private List<MovementTile> FindAdjacents () {

		List<MovementTile> adjacents = new List<MovementTile> ();

		MovementTile temp = AStar.GetTile (tile.x, tile.y + 1, tile.layer);
		if (temp && temp.floodedTile) {
			adjacents.Add (temp);
		}

		temp = AStar.GetTile (tile.x + 1, tile.y, tile.layer);
		if (temp && temp.floodedTile) {
			adjacents.Add (temp);
		}

		temp = AStar.GetTile (tile.x, tile.y - 1, tile.layer);
		if (temp && temp.floodedTile) {
			adjacents.Add (temp);
		}

		temp = AStar.GetTile (tile.x - 1, tile.y, tile.layer);
		if (temp && temp.floodedTile) {
			adjacents.Add (temp);
		}

		return adjacents;
	}

	private List<MovementTile> FindDiagonals (List<MovementTile> adjacents) {

		List<MovementTile> diagonals = new List<MovementTile> ();
		for (int i = 0; i < adjacents.Count; i++) {

			MovementTile t1 = i == 0 ? adjacents[adjacents.Count - 1] : adjacents[i - 1];
			MovementTile t2 = adjacents[i];

			if ((t1.x - t2.x) * (t1.x - t2.x) + (t1.y - t2.y) * (t1.y - t2.y) == 2) {
				int x = tile.x != t1.x ? t1.x : t2.x;
				int y = tile.y != t1.y ? t1.y : t2.y;
				MovementTile diagonal = AStar.GetTile (x, y, tile.layer);

				if (diagonal.floodedTile && !diagonals.Contains(diagonal)) {
					diagonals.Add (diagonal);
				}
			}
		}
		
		return diagonals;
	}
}
