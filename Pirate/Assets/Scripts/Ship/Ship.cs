using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Damageable {

	[SerializeField] private LevelLayout.WalkableArea[] areas = null;
	[SerializeField] private LevelLayout.Portal[] portals = null;
	public int[] layers {
		private set;
		get;
	}

	public List<CrewMember> crewMembers;

	public ShipPosition wheel;
	public ShipPosition crowsNest;
	public ShipPosition doctorsQuaters;
	public ShipPosition carpentersQuaters;
	public List<CannonPosition> cannons;
	public List<DamagedPosition> damagedTiles;

	public List<BailPosition> floodedTiles;
	private int maxFloodTiles;

	private List<MovementTile> edgeTiles;

	[SerializeField] private MovementTile boardingTile = null;

	[SerializeField] protected int food = 20;
	[SerializeField] protected int wood;
	[SerializeField] protected int gold;

	public int speed {
		get {
			int s = 0;
			s += wheel.isManned ? GetCaptain ().intelligence : 0;
			s += crowsNest.isManned ? (GetNavgiator ().intelligence / 2) : 0;
			return s;
		}
	}

	public int visionRange {
		get { return crowsNest.isManned ? GetNavgiator ().intelligence : 0; }
	}

	//TODO: encapsulate better
	public int cannonDamage = 5;

	void Awake () {

		layers = new int[areas.Length];
		edgeTiles = new List<MovementTile> ();
		floodedTiles = new List<BailPosition> ();

		List<MovementTile[,]> areaTiles = new List<MovementTile[,]> ();
		for (int i = 0; i < areas.Length; i++) {

			Transform area = areas[i].area;
			Vector2Int dims = areas [i].dimensions;
			MovementTile[,] tiles = new MovementTile [dims.x, dims.y];

			for (int x = 0; x < dims.x; x++) {
				for (int y = 0; y < dims.y; y++) {
					tiles [x, y] = area.GetChild (x * 8 + y).GetComponent<MovementTile> ();
					tiles [x, y].x = x;
					tiles [x, y].y = y;

					if (tiles [x, y].isEdgeTile) {
						edgeTiles.Add (tiles [x, y]);
					}

					if (tiles [x, y].walkable) {
						maxFloodTiles++;
						//TODO: testing this (also in plank prefab)
						tiles[x, y].GetComponent<SpriteRenderer> ().enabled = false;
					}
				}
			}

			layers [i] = tiles [0, 0].layer;
			areaTiles.Add (tiles);

			AStar.AddAreaToLayout (tiles);
		}

		List<LevelLayout.Portal> portalTiles = new List<LevelLayout.Portal> ();
		for (int i = 0; i < portals.Length; i++) {
			portalTiles.Add (portals [i]);
			AStar.AddPortalToLayout (portals [i]);
		}
	}

	void OnDestroy () {

		for (int i = 0; i < crewMembers.Count; i++) {
			if (crewMembers [i]) {
				Destroy (crewMembers [i].gameObject);
			}
		}

		for (int i = 0; i < layers.Length; i++) {
			AStar.RemoveLayerFromLayout (layers [i]);
		}
	}

	public bool Assign (CrewMember crew, ShipPosition shipPos, bool asNewMemeber = false) {

		if (!shipPos.isManned) {

			if (crew.shipPos) {
				crew.shipPos.isManned = false;
			}

			crew.shipPos = shipPos;
			crew.role = shipPos.getRole ();

			shipPos.isManned = true;

			if (asNewMemeber) {
				crewMembers.Add (crew);
				crew.ship = this;
			}

			crew.BeginPath (AStar.FindPath (crew.current, shipPos.tile, true));
			return true;
		}

		return false;
	}

	public void Unassign (CrewMember crew) {
		crew.shipPos.isManned = false;
		crew.shipPos = null;
		crewMembers.Remove (crew);
		crew.ship = null;
	}

	public ShipPosition GetVacantPositionByPriority () {

		if (!wheel.isManned) {
			return wheel;
		}

		if (!crowsNest.isManned) {
			return crowsNest;
		}

		if (doctorsQuaters && !doctorsQuaters.isManned) {
			return doctorsQuaters;
		}

		if (carpentersQuaters && !carpentersQuaters.isManned) {
			return carpentersQuaters;
		}

		for (int i = 0; i < cannons.Count; i++) {
			if (!cannons [i].isManned) {
				return cannons [i];
			}
		}

		return null;
	}

	public DamagedPosition GetClosestDamagedTile (MovementTile tile) {

		DamagedPosition dp = null;
		int shortestPathCount = int.MaxValue;

		for (int i = 0; i < damagedTiles.Count; i++) {

			if (damagedTiles[i].IsAvailable ()) {
				int pathCount = AStar.FindPath (tile, damagedTiles[i].tile, true).Count;

				if (shortestPathCount > pathCount) {
					shortestPathCount = pathCount;
					dp = damagedTiles[i];
				}
			}
		}

		return dp;
	}

	public void AddFloodedTile (BailPosition flooded) {

		floodedTiles.Add (flooded);
		if (floodedTiles.Count >= maxFloodTiles) {
		}
	}

	public BailPosition GetClosestFloodedTile (MovementTile tile) {

		BailPosition bp = null;
		int shortestPathCount = int.MaxValue;

		for (int i = 0; i < floodedTiles.Count; i++) {

			if (!floodedTiles[i]) {
				floodedTiles.RemoveAt (i);
				i--;
			}

			if (!floodedTiles[i].isManned) {
				int pathCount = AStar.FindPath (tile, floodedTiles[i].tile, true).Count;

				if (shortestPathCount > pathCount) {
					shortestPathCount = pathCount;
					bp = floodedTiles[i];
				}
			}
		}

		return bp;
	}

	public CrewMember GetCaptain () {

		for (int i = 0; i < crewMembers.Count; i++) {
			if (crewMembers [i].role == CrewMember.Role.CAPTAIN) {
				return crewMembers [i];
			}
		}

		return null;
	}

	public CrewMember GetNavgiator () {

		for (int i = 0; i < crewMembers.Count; i++) {
			if (crewMembers [i].role == CrewMember.Role.NAVIGATOR) {
				return crewMembers [i];
			}
		}

		return null;
	}

	public int GetMaxCrewMembers () {

		int total = cannons.Count;
		total += wheel ? 1 : 0;
		total += crowsNest ? 1 : 0;
		total += doctorsQuaters ? 1 : 0;
		total += carpentersQuaters ? 1 : 0;

		return total;
	}
		
	public MovementTile GetBoardingTile () {
		return boardingTile;
	}

	public bool CanAfford (int food, int wood, int gold) {
		return this.food >= food && this.wood >= wood && this.gold >= gold;
	}

	public void AddInventory (int food, int wood, int gold) {
		this.food += food;
		this.wood += wood;
		this.gold += gold;
	}

	public bool Spend (int food, int wood, int gold) {

		if (!CanAfford (food, wood, gold)) {
			Debug.LogError ("Attempt to Buy but cant afford");
			return false;
		}

		this.food -= food;
		this.gold -= gold;
		this.wood -= wood;
		return true;
	}

	public void ForceSpend (int food, int wood, int gold) {
		this.food = (this.food - food < 0) ? 0 : (this.food - food); 
		this.wood = (this.wood - wood < 0) ? 0 : (this.wood - wood);
		this.gold = (this.gold - gold < 0) ? 0 : (this.gold - gold);
	}
		
	public int GetFood () {
		return food;
	}

	public int GetWood () {
		return wood;
	}

	public int GetGold () {
		return gold;
	}

	public MyLinkedList<MovementTile> GetShortestPathToEdgeTile (MovementTile tile) {

		MyLinkedList<MovementTile> shortestPath = null;
		for (int i = 0; i < edgeTiles.Count; i++) {
			MyLinkedList<MovementTile> path = AStar.FindPath (tile, edgeTiles [i], true);

			if (shortestPath == null || shortestPath.Count > path.Count) {
				shortestPath = path;
			}
		}

		return shortestPath;
	}
	
	public new bool TakeDamage (int damage) {

		bool result = base.TakeDamage (damage);

		return result;
	}
}