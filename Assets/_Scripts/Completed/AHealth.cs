using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
	public class AHealth : MonoBehaviour {

		public float health = 100f;					// The player's health.
		public float damageAmount = 50f;			// The amount of damage to take when enemies touch the player

		private SpriteRenderer healthBar;			// Reference to the sprite renderer of the health bar.
		private Vector3 healthScale;

		void Awake ()
		{
			healthBar = GameObject.Find("MyHealthBar").GetComponent<SpriteRenderer>();

			healthScale = healthBar.transform.localScale;
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if(other.gameObject.tag == "enemyWeapon")
			{
				if(health > 0f)
				{
					TakeDamage(other.transform);
				}

				Destroy (other.gameObject);
			}
		}

		void TakeDamage (Transform enemy)
		{
			damageAmount = enemy.GetComponent<WeaponPropertyScript> ().atk * 10f;

			health -= damageAmount;

			UpdateHealthBar();

			if (health <= 0)
			{
				Destroy(this.gameObject);
			}
		}
			
		public void UpdateHealthBar ()
		{
			healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - health * 0.01f);

			healthBar.transform.localScale = new Vector3(healthScale.x * health * 0.01f, 1, 1);
		}
	}
}

