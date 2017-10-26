using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : MonoBehaviour {

	public bool isEnemy;
	public float health = 1000f;					// The player's health.
	private float damageAmount;					// The amount of damage to take when enemies touch the player

	private SpriteRenderer healthBar;			// Reference to the sprite renderer of the health bar.
	private Vector3 healthScale;

	public bool isServer;
	private ServerBattleField sbfCs;
	private ClientBattleField cbfCs;

	void Awake ()
	{
		if (isEnemy) {
			healthBar = GameObject.Find("hp-bar_2").GetComponent<SpriteRenderer>();
		} else {
			healthBar = GameObject.Find ("hp-bar_1").GetComponent<SpriteRenderer> ();
		}

		healthScale = healthBar.transform.localScale;

		sbfCs = GameObject.Find ("BattleField").GetComponent<ServerBattleField>();
		cbfCs = GameObject.Find ("BattleField").GetComponent<ClientBattleField>();
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		WeaponEvent we = other.GetComponent<WeaponEvent> ();
		if (isEnemy) {
			if(other.gameObject.tag == "myWeapon")
			{
				if(health > 0f)
				{
					TakeDamage(other.transform);
				}

				if (isServer) {
					we.state.state = 3;
					sbfCs.removeWeaponFromList (other.gameObject, false);
				} else {
					cbfCs.removeWeaponFromList (other.gameObject);
					Destroy (other.gameObject);
				}

				StartCoroutine (playerBackwards ());
			}
		} else {
			if(other.gameObject.tag == "enemyWeapon")
			{
				if(health > 0f)
				{
					TakeDamage(other.transform);
				}
				if (isServer) {
					we.state.state = 3;
					sbfCs.removeWeaponFromList (other.gameObject, false);
				} else {
					cbfCs.removeWeaponFromList (other.gameObject);
					Destroy (other.gameObject);
				}
				StartCoroutine (playerBackwards ());
			}
		}
	}

	void TakeDamage (Transform enemy)
	{
		damageAmount = enemy.GetComponent<WeaponPropertyScript> ().atk * 50f;

		health -= damageAmount;

		if (health <= 0)
		{
			if (isServer) {
				if (isEnemy) {
					sbfCs.battleState = 1;
				} else {
					sbfCs.battleState = 2;
				}
			} else {
				if (isEnemy) {
					cbfCs.battleState = 1;
				} else {
					cbfCs.battleState = 2;
				}
			}

			health = 0;
		}
		UpdateHealthBar();
	}

	public void UpdateHealthBar ()
	{
		healthBar.transform.localScale = new Vector3(healthScale.x * health * 0.001f, 1, 1);
	}

	private IEnumerator playerBackwards (){
		if (isEnemy) {
			transform.localPosition = new Vector3 (transform.localPosition.x,transform.localPosition.y+0.5f,transform.localPosition.z);
			yield return new WaitForSeconds(0.3f); 
			transform.localPosition = new Vector3 (transform.localPosition.x,transform.localPosition.y-0.5f,transform.localPosition.z);
		} else {
			transform.localPosition = new Vector3 (transform.localPosition.x,transform.localPosition.y-0.5f,transform.localPosition.z);
			yield return new WaitForSeconds(0.3f); 
			transform.localPosition = new Vector3 (transform.localPosition.x,transform.localPosition.y+0.5f,transform.localPosition.z);
		}
	}
}
