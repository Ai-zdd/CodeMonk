using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
	public class BHealth : MonoBehaviour {

		public float health = 100f;		
		public float damageAmount = 50f;

		private SpriteRenderer healthBar;			
		private Vector3 healthScale;			

		void Awake ()
		{
			healthBar = GameObject.Find("EnemyHealthBar").GetComponent<SpriteRenderer>();

			healthScale = healthBar.transform.localScale;
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if(other.gameObject.tag == "myWeapon")
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

