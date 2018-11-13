using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {

	[SerializeField] private Transform healthBar = null;

	//TODO: make these protected
	public int health;
	public int maxHealth;

	protected void Start () {
		UpdateHealthBar ();
	}

	public int Heal (int health) {

		int prevHp = this.health;
		this.health = Mathf.Clamp (this.health + health, this.health, maxHealth);

		UpdateHealthBar ();
		return this.health - prevHp;
	}

	/**
	 * return true if dies
	 */
	public bool TakeDamage (int damage) {
		health -= damage;
		UpdateHealthBar ();
		return health <= 0;
	}

	protected void UpdateHealthBar () {
		healthBar.localScale = new Vector3 (Mathf.Clamp01 ((float)health / (float)maxHealth), 1, 1);
	}
}
