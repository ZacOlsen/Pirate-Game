using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CrewMember : Damageable {

	public Role role;
	public ShipPosition shipPos;
	public Ship ship;

	//TODO: probs make these private
	public string personName;
	public int intelligence;
	public int dexterity;
	public Action currentAction;

	private float timeOfLastAction;

	public bool printe;

	private SelectionManager selectManager = null;
	public MovementTile current;
	[SerializeField] private float speed = 1;
	private bool onTile;
	private MyLinkedList<MovementTile> path = new MyLinkedList<MovementTile> ();

	public const float MOVEMENT_ERROR_RANGE = .01f;
	private const float Z_VAL = -.1f;

//	public BattleManager battleManager;
	[SerializeField] private CrewMember target;
	public bool isPlayerCrew = false;

	public bool couldFindPath = true;

	[SerializeField] private bool hasWater;

	[SerializeField] private Animator anim;
	[SerializeField] private SpriteRenderer sr;

	private const float MAX_REPAIR_SPEED = .1f;
	private const float AVERAGE_REPAIR_SPEED = .5f;

	private float repairSpeed {
		get { return 1f / (AVERAGE_REPAIR_SPEED * (float)dexterity) + MAX_REPAIR_SPEED; }
	}

	private int repairAmount {
		get { return (dexterity - EncounterPeopleEvent.AVERAGE_STARTING_DEXTERITY + 8) / 4 + 1; }
	}

	public int endSectorRepairAmount {
		get { return (dexterity - EncounterPeopleEvent.AVERAGE_STARTING_DEXTERITY + 10); }
	}

	public int healingAbility {
		get { return (intelligence - EncounterPeopleEvent.AVERAGE_STARTING_INTELLIGENCE + 8) / 4; }
	}

	private float attackSpeed {
		get { return 1.4f / Mathf.Sqrt(dexterity); }
	}

	private int damage {
		get {
			return dexterity * EncounterPeopleEvent.AVERAGE_STARTING_HEALTH /
			  (EncounterPeopleEvent.AVERAGE_STARTING_DEXTERITY * 4);
		}
	} 

	private float cannonFireSpeed {
		get { return -.95f * Mathf.Log (dexterity) + 4f; }
	}

	private float cannonHitChance {
		get {
			return -95f / (1f + Mathf.Exp (.4f * (dexterity - 
				EncounterPeopleEvent.AVERAGE_STARTING_DEXTERITY))) + 95f;
		}
	}

	public int worth {
		get { return dexterity + intelligence + health; }
	}

	public enum Role {
		CAPTAIN,
		NAVIGATOR,
		BAILER,
		CARPENTER,
		DOCTOR,
		CANNONEER
	}

	public enum Action {
		IDLE,
		WALKING,
		ATTACKING
	}

	void Awake () {
		DontDestroyOnLoad (this);
	}

	protected new void Start () {

		base.Start ();

		Vector3 pos = transform.position;
		pos.z = Z_VAL;
		transform.position = pos;

		RelocateSelector ();
	}

	void FixedUpdate () {

		if (printe) {
			PrintMethod ();
			printe = false;
			Debug.Break ();
		}

		Vector3 tilePos = current.transform.position;
		float distance = Vector2.Distance (transform.position, tilePos);

		bool onTilePrev = onTile;
		if (distance > MOVEMENT_ERROR_RANGE) {
			Vector3 pos = Vector3.Lerp (transform.position, tilePos, speed / distance * Time.fixedDeltaTime);
			pos.z = Z_VAL;
			transform.position = pos;
			onTile = false;
		} else {
			onTile = true;
		}

		if (target && path.Count > 0 && target.current != path.last.data) {
			BeginPath (AStar.FindPath (current, target.current, true));
			couldFindPath = false;
		}

		if (path.Count > 0 && onTile) {
			path.RemoveFirst ();

			if (path.Count > 0) {

				if (path.first.data.crewMem) {

					if (path.first.data.crewMem == target) {
						return;
					}

					//Debug.Log (personName + " blocked");
					bool cfpTemp;
					MyLinkedList<MovementTile> p = AStar.FindPath (current, target ? target.current : path.last.data, 
						false, out cfpTemp);

					if (!target && !cfpTemp) {
						//... there's nothing here
					} else {
						BeginPath (p);
						couldFindPath = cfpTemp;
						if (path.Count > 1 && !path.first.next.data.crewMem) {
							path.RemoveFirst ();
						}
					}
				}

				if (path.Count > 0) {

					if (path.first.data.teleportLayerChange && current.teleportLayerChange &&
						path.first.data.layer != current.layer) {
						Vector3 pos = path.first.data.transform.position;
						pos.z = Z_VAL;
						transform.position = pos;
					}

					ChangeCurrentTile (path.first.data);
				}
			}
		}

		if (path.Count == 0 && !target && ((shipPos && current == shipPos.tile) ||
			(role == Role.BAILER && couldFindPath))) {

			currentAction = Action.IDLE;
			if (onTile && !onTilePrev) {
				timeOfLastAction = Time.time;
			}
		}

		if (target && path.Count == 0 && couldFindPath) {
			currentAction = Action.ATTACKING;
		}

		if (target && onTile && !couldFindPath && currentAction != Action.ATTACKING) {
			BeginPath (AStar.FindPath (current, target.current, false, out couldFindPath));
		}

		if (shipPos && current != shipPos.tile && path.last.data != shipPos.tile &&
			(!target || (role != Role.CANNONEER && target))) {

			BeginPath (AStar.FindPath (current, shipPos.tile, true));

			if (target) {
				target = null;
			}
		}

		switch (currentAction) {

			case Action.IDLE:

				if (role == Role.BAILER) {

					//TODO: make dump face correct direction when picked up next to edge
					anim.SetInteger ("Idle Action", hasWater ? 3 : 2);
					
					if (Time.time - timeOfLastAction > .5f) {

						if (!hasWater) {
							BeginPath (ship.GetShortestPathToEdgeTile (current));
							current.floodedTile.Bail ();
						} else {
							ShipPosition bailPos = ship.GetClosestFloodedTile (current);
							ship.Assign (this, bailPos ? bailPos : ship.GetVacantPositionByPriority ());
						}

						timeOfLastAction = Time.time;
						hasWater = !hasWater;
						anim.SetBool ("Has Water", hasWater);
					}

				} else if (role == Role.CARPENTER) {

					if (current.damagedTile) {

						anim.SetInteger ("Idle Action", 1);

						if (Time.time - timeOfLastAction > repairSpeed) {
							timeOfLastAction = Time.time;

							if (current.damagedTile.Repair (repairAmount)) {

								DamagedPosition dp = ship.GetClosestDamagedTile (current);
								if (dp) {
									ship.Assign (this, dp);

								} else if (ship.carpentersQuaters && !ship.carpentersQuaters.isManned) {
									ship.Assign (this, ship.carpentersQuaters);

								} else {
									ship.Assign (this, ship.GetVacantPositionByPriority ());
								}
							}
						}

					} else {

						DamagedPosition dp = ship.GetClosestDamagedTile (current);
						if (dp) {
							ship.Assign (this, dp);

						} else {
							anim.SetInteger ("Idle Action", 0);
						}
					}

				} else if (current.damagedTile) {
					anim.SetInteger ("Idle Action", 1);

					//TODO: if repairing current position and job is changed or attack started for
					//cannoneer, reset damaged tile ismanned
					if (Time.time - timeOfLastAction > repairSpeed) {
						timeOfLastAction = Time.time;

						current.damagedTile.Repair (repairAmount);
					}

					current.damagedTile.isManned = true;

				} else {

					anim.SetInteger ("Idle Action", 0);

				}

				break;

			case Action.WALKING:

				break;

			case Action.ATTACKING:

				//hopefully this only triggers on isalnd dwellers during an attack
				if (!target && !shipPos) {
					currentAction = Action.IDLE;
				}

				anim.SetInteger ("FacingDirection", GetDirectionOfTile (target.current));

				if (!target.target) {
					target.target = this;
				}

				if (Time.time - timeOfLastAction > attackSpeed) {
					timeOfLastAction = Time.time;
					target.TakeDamage (damage);
				}

				break;
		}

		anim.SetInteger ("Action", (int)currentAction);

		Debug.DrawLine (transform.position, current.transform.position, Color.red);
	}

	void OnEnable () {
		SceneManager.sceneLoaded += StartScene;
		SceneManager.activeSceneChanged += ChangeScene;
	}

	void OnDisable () {
		SceneManager.sceneLoaded -= StartScene;
		SceneManager.activeSceneChanged -= ChangeScene;
	}

	void StartScene (Scene scene, LoadSceneMode mode) {
		RelocateSelector ();
	}

	void ChangeScene (Scene prev, Scene next) {

		if (shipPos && (shipPos.tile != current || !onTile)) {
			ChangeCurrentTile (shipPos.tile);
			transform.position = shipPos.tile.transform.position + new Vector3 (0, 0, Z_VAL);
			path.Empty ();
		}
	}

	void OnMouseUp () {
		if (!EventSystem.current.IsPointerOverGameObject ()) {
			selectManager.Select (this);
		}
	}

	public void SetBaseStats (string name, int intel, int dex, int hp) {
		personName = name;
		intelligence = intel;
		dexterity = dex;
		health = hp;
	}

	private void ChangeCurrentTile (MovementTile newCurrent) {
		current.crewMem = null;
		newCurrent.crewMem = this;
		current = newCurrent;

		anim.SetInteger ("FacingDirection", GetDirectionOfTile (current));
	}

	private int GetDirectionOfTile (MovementTile tile) {

		float angle = Vector2.SignedAngle (Vector3.right, tile.transform.position - transform.position);
		int num = 0;
		if (angle < 45 && angle > -45) {
			num = 0;
		} else if (angle > 45 && angle < 135) {
			num = 1;
		} else if (angle > 135 || angle < -135) {
			num = 2;
		} else if (angle < -45 && angle > -135) {
			num = 3;
		}

		return num;
	}

	private void RelocateSelector () {
		selectManager = GameObject.Find ("SelectionManager").GetComponent<SelectionManager> ();
	}

	public void BeginPath (MyLinkedList<MovementTile> path) {
		currentAction = Action.WALKING;
		this.path = path;
	}

	/**
	 * return true if dies
	 */
	public new bool TakeDamage (int damage) {

		//initial check to avoid bad attack error
		if (health <= 0) {
			return true;
		}

		bool result = base.TakeDamage (damage);

		if (result) {
			if (ship) {
				ship.Unassign (this);
			}

			Destroy (gameObject);
		}

		if (selectManager) {
			selectManager.UpdateStats (this);
		}

		return result;
	}

	public void PrintMethod () {

		MyLinkedList<MovementTile>.MyLinkedListNode node = path.first;
		while (node != null) {
			Debug.Log (node.data);
			node = node.next;
		}

		Debug.Log ("end");
		if (target) {
			bool cdf = false;
			MyLinkedList<MovementTile> p = AStar.FindPath (current, target.current, false, out cdf);
			Debug.Log (cdf);
			node = p.first;
			while (node != null) {
				Debug.Log (node.data);
				node = node.next;
			}
		}
	}
}
