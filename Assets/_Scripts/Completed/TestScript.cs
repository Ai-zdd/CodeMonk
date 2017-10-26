using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
	public class TestScript : MonoBehaviour {

		public GameObject firstMyWolf = null;
		public GameObject firstEnemyWolf = null;

		public float firstMyWolfATK = 0;
		public float firstEnemyWolfATK = 0;

		public float firstMyWolfHP = 0;
		public float firstEnemyWolfHP = 0;

		public GameObject myWeapon = null;
		public GameObject enemyWeapon = null;

		public float myWolfATK = 0;
		public float enemyWolfATK = 0;

		public float myWolfHP = 0;
		public float enemyWolfHP = 0;

		public bool trigger = false;

		private SpriteRenderer healthBar0;
		private Transform healthBar1;
		private SpriteRenderer healthBar2;
		private SpriteRenderer healthBar3;
		private SpriteRenderer healthBar4;
		private Vector3 healthScale0;
		private Vector3 healthScale1;
		private Vector3 healthScale2;
		private Vector3 healthScale3;
		private Vector3 healthScale4;
		public float health0 = 100f;
		public float health1 = 100f;
		public float health2 = 100f;
		public float health3 = 100f;
		public float health4 = 100f;

		void Awake ()
		{

		}

		void Start()
		{

		}

		void FixedUpdate()
		{
			
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.CompareTag ("myWeapon")) {
				if (other.gameObject.name != firstMyWolf.name) 
				{
					myWeapon = other.gameObject;
					myWolfATK = other.gameObject.GetComponent<WeaponPropertyScript> ().atk;
					myWolfHP = other.gameObject.GetComponent<WeaponPropertyScript> ().hp;
					firstMyWolfATK += other.gameObject.GetComponent<WeaponPropertyScript> ().atk;

					if (GameObject.Find ("MyWeaponHealth").transform.Find ("healthBar1").gameObject.activeSelf == false) {
						GameObject.Find ("MyWeaponHealth").transform.Find ("healthBar1").gameObject.SetActive (true);
						healthBar1 = GameObject.Find ("MyWeaponHealth").transform.Find ("healthBar1"); 
						healthScale1 = healthBar1.localScale;
					}
				}
			}

			if (other.CompareTag ("enemyWeapon")) {
				if (other.gameObject.name != firstEnemyWolf.name) 
				{
					enemyWeapon = other.gameObject;
					enemyWolfATK = other.gameObject.GetComponent<WeaponPropertyScript> ().atk;
					enemyWolfHP = other.gameObject.GetComponent<WeaponPropertyScript> ().hp;
					firstEnemyWolfATK += other.gameObject.GetComponent<WeaponPropertyScript> ().atk;
				}
			}
		}

	}
}

