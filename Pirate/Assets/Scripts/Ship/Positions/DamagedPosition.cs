using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedPosition : ShipPosition {

	public GameObject floodedTilePrefab;
	[SerializeField] private int damagePoints = 20;
	[SerializeField] private float floodTime = 1;
	[SerializeField] private int repairShipHeal = 3;
	private const float Z_VAL = -.05f;

	private MTreeNode<MovementTile> root;

	protected new void Start () {
		base.Start ();
		InvokeRepeating ("FloodTile", floodTime, floodTime);

		Vector3 pos = transform.position;
		pos.z = Z_VAL;
		transform.position = pos;
	}

	public bool Repair (int repair) {

		damagePoints -= repair;
		if (damagePoints <= 0) {
			Ship ship = transform.parent.GetComponent<Ship> ();
			ship.damagedTiles.Remove (this);
			ship.Heal (repairShipHeal);
			Destroy (gameObject);

			return true;
		}

		return false;
	}

	public bool IsAvailable () {
		return !isManned && damagePoints > 0;
	}

	private void FloodTile () {

		if (root == null) {
			root = new MTreeNode<MovementTile> (tile);
			AddChildren (root);
			if (!tile.floodedTile) {
				CreateFloodedTile (tile);
			}
			return;
		
		} else if (!root.data.floodedTile) {
			CreateFloodedTile (root.data);
		}

		//search tree in breadth first order
		Queue<MTreeNode<MovementTile>> queue = new Queue<MTreeNode<MovementTile>> ();
		queue.Enqueue (root);

		while (queue.Count > 0) {
			
			MTreeNode<MovementTile> node = queue.Dequeue ();
			for (int i = 0; i < node.children.Count; i++) {

				if (!node.children [i].data.floodedTile) {
					CreateFloodedTile (node.children [i].data);
					AddChildren (node.children [i]);
					return;
				}

				queue.Enqueue (node.children [i]);
			}
		}
	}

	private BailPosition CreateFloodedTile (MovementTile t) {

		BailPosition bp = Instantiate (floodedTilePrefab, t.transform.position, 
			Quaternion.identity).GetComponent<BailPosition> ();

		bp.transform.parent = transform.parent;
		t.floodedTile = bp;
		bp.tile = t;
		bp.isAssignAbleTo = isAssignAbleTo;

		//TODO: make know which ship
		transform.parent.GetComponent<Ship> ().AddFloodedTile (bp);

		return bp;
	}

	private void AddChildren (MTreeNode<MovementTile> node) {

		MovementTile[] tiles = AStar.GetTileNeighbors (node.data);
		for (int i = 0; i < tiles.Length; i++) {
			if (tiles [i] && tiles [i].walkable && !tiles [i].floodedTile) {
				node.AddChild (new MTreeNode<MovementTile> (tiles [i]));
			}
		}
	}
}
