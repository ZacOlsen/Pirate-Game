using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonPosition : ShipPosition {

	public GameObject cannonBall;
	[SerializeField] private Transform spawnLoc = null;

	public void Fire (BattleManager battleManager, bool hit) {

		MovementTile t = battleManager.GetRandomTile (!isAssignAbleTo);

		CannonBall cb = Instantiate (cannonBall, spawnLoc.position, Quaternion.identity).GetComponent<CannonBall> ();
		
		//TODO: maybe fix this reroll system
		while (!t.walkable) {
			t = battleManager.GetRandomTile (!isAssignAbleTo);
		}

		Ship target = isAssignAbleTo ? battleManager.otherShip : battleManager.playerShip;
		int damage = isAssignAbleTo ? battleManager.playerShip.cannonDamage : battleManager.otherShip.cannonDamage;
		cb.Init (t, hit, isAssignAbleTo, target, damage);
	}
}
