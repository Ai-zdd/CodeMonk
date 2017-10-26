using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHealth : MonoBehaviour 
{
	public int colorComb = 0;

	public int currentBg = 0;
	public int currentFg = 1;


	public float health = 100f;
	public float orgHealth;

	private SpriteRenderer healthBar;
	private SpriteRenderer healthBarBg;
	private Vector3 healthScale;

	void Start () 
	{
		healthBar = transform.Find("Health0").GetComponent<SpriteRenderer>();
		healthBarBg = transform.Find ("Health1").GetComponent<SpriteRenderer> ();
		healthScale = healthBar.transform.localScale;
	}

	void FixedUpdate () 
	{
		if (health > 0) 
		{
			if (colorComb == 0)
			{
				healthBar.material.color = Color.green;

				healthBarBg.enabled = false;
			}
			else if (colorComb == 1) 
			{
				healthBarBg.enabled = true;
				healthBar.material.color = Color.blue;

				healthBarBg.material.color = Color.green;
			}
			else if (colorComb == 2)
			{
				healthBarBg.enabled = true;
				healthBar.material.color = Color.yellow;

				healthBarBg.material.color = Color.blue;
			}
			else if (colorComb == 3) 
			{
				healthBarBg.enabled = true;
				healthBar.material.color = Color.blue;

				healthBarBg.material.color = Color.yellow;
			}
			else if (colorComb == 4) 
			{
				healthBarBg.enabled = true;
				healthBar.material.color = Color.magenta;

				healthBarBg.material.color = Color.blue;
			}

			healthBar.transform.localScale = new Vector3 (healthScale.x * health * (1f / orgHealth), healthScale.y, 1);
		}
	}
}
