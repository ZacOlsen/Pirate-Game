using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour {

	public GameObject damagedTile;
	private MovementTile destination;
	[SerializeField] private float speed = 8f;
	private bool hit;
	private Vector3 targetPos;
	private bool shotByPlayer;
	private Ship targetShip;
	private int damage;

	void FixedUpdate () {

		float distance = Vector2.Distance (transform.position, targetPos);

		if (distance > CrewMember.MOVEMENT_ERROR_RANGE) {
			transform.position = Vector3.Lerp (transform.position, targetPos, speed / distance * Time.fixedDeltaTime);
		} else {

			if (hit) {

				if (destination.damagedTile) {
					targetShip.damagedTiles.Remove (destination.damagedTile);
					Destroy (destination.damagedTile.gameObject);
				}

				DamagedPosition dp = Instantiate (damagedTile, destination.transform.position, 
					Quaternion.identity).GetComponent<DamagedPosition> ();
				destination.damagedTile = dp;

				dp.tile = destination;
				dp.isAssignAbleTo = !shotByPlayer;
				dp.transform.parent = destination.transform.parent.parent;

				targetShip.damagedTiles.Add (dp);
				targetShip.TakeDamage (damage);
			}
		
			Destroy (gameObject);
		}
	}

	public void Init (MovementTile dest, bool hit, bool shotByPlayer, Ship targetShip, int damage) {

		this.hit = hit;
		destination = dest;
		this.shotByPlayer = shotByPlayer;
		this.targetShip = targetShip;
		targetPos = hit ? dest.transform.position : ((dest.transform.position - transform.position) * 3);
		this.damage = damage;
	}
}
