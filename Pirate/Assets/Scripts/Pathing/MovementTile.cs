using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MovementTile : MonoBehaviour, IComparable<MovementTile> {

	public Sprite temp;
	public Sprite tile;
	public bool walkable = false;

	public ShipPosition shipPos;
	public DamagedPosition damagedTile;
	public BailPosition floodedTile;
	public CrewMember crewMem;
	public bool isEdgeTile;

	/**
	 * will be used for managing areas, 0, 1, 2,...
	 */
	public int layer = 0;
	public bool teleportLayerChange = false;

	//heap stuff
	public int heapIndex;

	//Astar stuff
	public float fitness = float.MaxValue;
	public float goal = float.MaxValue;
	public float weight = 1;
	public bool closed;
	public MovementTile prev;
	public int x;
	public int y;

	void Update () {

		if (prev) {
			Debug.DrawLine (transform.position, prev.transform.position, Color.red);
		}
	}

	void OnValidate () {

		SpriteRenderer sr = GetComponent<SpriteRenderer> ();

		if (walkable) {
			sr.sprite = tile;
			sr.color = Color.white;

		} else {
			sr.sprite = temp;
			sr.color = Color.black;
		}
	}

	public int CompareTo (MovementTile other) {
		return fitness - other.fitness < 0 ? 1 : -1;
	}
}
