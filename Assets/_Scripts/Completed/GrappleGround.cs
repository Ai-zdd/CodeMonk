using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
	public class GrappleGround : MonoBehaviour 
	{
		// 猪圈
//		public List<GameObject> weapons;

		public List<GameObject> ourWeapons;

		public List<GameObject> enemyWeapons;

		// 血条
		public GameObject weaponHealth; 

		public GameObject ourWeaponHealth = null;

		public GameObject enemyWeaponHealth = null;

		// 我方头狼
		private GameObject firstWolf = null;

		// 敌方头狼
		private GameObject enemyFirstWolf = null;

		void Awake ()
		{
//			weapons = new List<GameObject> ();

			ourWeapons = new List<GameObject> ();

			enemyWeapons = new List<GameObject> ();
		}

		void FixedUpdate ()
		{
			// 设置头狼
			FindFirstWolf ();

			// 控制双方头狼加攻击力/掉血/死亡
			ChangeProperty ();

			// 战斗双方全部阵亡
			if (ourWeapons.Count == 0 && enemyWeapons.Count == 0)
				GameManager.instance.DestroyGrappleGround (this);

			// 一方胜利传递胜利消息给胜方
			Victory();
		}

		void Victory()
		{
//			List<GameObject> ourWeapons = new List<GameObject> ();
//			List<GameObject> enemyWeapons = new List<GameObject> ();

//			print ("count = "+weapons.Count);
//			for (int i = 0; i < weapons.Count; i++)
//			{
//				if (weapons [i].GetComponent<WeaponPropertyScript> ().type == 0)
//					
//					ourWeapons.Add (weapons[i]);
//
//				if (weapons [i].GetComponent<WeaponPropertyScript> ().type == 1)
//					
//					enemyWeapons.Add (weapons[i]);
//			}

			if (ourWeapons.Count == 0)
			{
				// 敌方胜利
				for(int i=0; i<enemyWeapons.Count; i++)
				{
					enemyWeapons [i].GetComponent<WeaponPropertyScript> ().triggerFlag = false;
					enemyWeapons [i].GetComponent<BoxCollider2D> ().enabled = true;
				}

				GameManager.instance.DestroyGrappleGround (this);
			}

			if (enemyWeapons.Count == 0)
			{
				// 我方胜利
				for(int i=0; i<ourWeapons.Count; i++)
				{
					ourWeapons [i].GetComponent<WeaponPropertyScript> ().triggerFlag = false;
					ourWeapons [i].GetComponent<BoxCollider2D> ().enabled = true;
				}

				GameManager.instance.DestroyGrappleGround (this);
			}
		}

		void ChangeProperty ()
		{
			if (firstWolf != null && enemyFirstWolf != null)
			{
				if (ourWeaponHealth == null)
				{
					Vector3 ourPos = new Vector3 (firstWolf.transform.position.x, firstWolf.transform.position.y + 2f);

					ourWeaponHealth = Instantiate (weaponHealth, ourPos, Quaternion.identity);

					ourWeaponHealth.GetComponent<WeaponHealth> ().orgHealth = firstWolf.GetComponent<WeaponPropertyScript> ().hp;
				}

				if (enemyWeaponHealth == null)
				{
					Vector3 enemyPos = new Vector3 (enemyFirstWolf.transform.position.x, enemyFirstWolf.transform.position.y - 2f);

					enemyWeaponHealth = Instantiate(weaponHealth, enemyPos, Quaternion.identity);

					enemyWeaponHealth.GetComponent<WeaponHealth> ().orgHealth = enemyFirstWolf.GetComponent<WeaponPropertyScript> ().hp;
				}

				firstWolf.GetComponent<WeaponPropertyScript> ().hp -= EnemyTotalAtk();

				ourWeaponHealth.GetComponent<WeaponHealth> ().health = firstWolf.GetComponent<WeaponPropertyScript> ().hp;

				enemyFirstWolf.GetComponent<WeaponPropertyScript> ().hp -= OurTotalAtk();

				enemyWeaponHealth.GetComponent<WeaponHealth> ().health = enemyFirstWolf.GetComponent<WeaponPropertyScript> ().hp;

				if (firstWolf.GetComponent<WeaponPropertyScript> ().hp <= 0)
				{
					PullWeaponForList (firstWolf);
				}

				if (enemyFirstWolf.GetComponent<WeaponPropertyScript> ().hp <= 0)
				{
					PullWeaponForList (enemyFirstWolf);
				}
			}
		}

		float OurTotalAtk()
		{
			float ourTotalAtk = 0;

			for (int i=0; i<ourWeapons.Count; i++)
			{
				
					ourTotalAtk += ourWeapons [i].GetComponent<WeaponPropertyScript> ().atk;
			}

			return ourTotalAtk;
		}

		float EnemyTotalAtk()
		{
			float enemyTotalAtk = 0;

			for (int i=0; i<enemyWeapons.Count; i++)
			{
				enemyTotalAtk += enemyWeapons [i].GetComponent<WeaponPropertyScript> ().atk;
			}

			return enemyTotalAtk;
		}

		// 寻找头狼
		public void FindFirstWolf ()
		{
			for (int i = 0; i < ourWeapons.Count; i++)
			{
				if (firstWolf == null)
				{
					firstWolf = ourWeapons [i];
					break;
				}
			}

			for (int i = 0; i < enemyWeapons.Count; i++)
			{
				if (enemyFirstWolf == null)
				{
					enemyFirstWolf = enemyWeapons [i];
					break;
				}
			}
		}

		public bool FindInWeapons(int id)
		{
			for (int i = 0; i < ourWeapons.Count; i++)
			{
				if (id == ourWeapons [i].GetComponent<WeaponPropertyScript>().id)
					return true;
			}

			for (int i = 0; i < enemyWeapons.Count; i++)
			{
				if (id == enemyWeapons [i].GetComponent<WeaponPropertyScript>().id)
					return true;
			}

			return false;
		}

		public void AddWeaponToList(GameObject obj)
		{
			if (obj.GetComponent<WeaponPropertyScript> ().type == 0)
			{
				ourWeapons.Add (obj);
				OurReset ();
			}
				

			if (obj.GetComponent<WeaponPropertyScript> ().type == 1)
			{
				enemyWeapons.Add (obj);
				EnemyReset ();
			}
		}

		void PullWeaponForList(GameObject obj)
		{
			if (obj.GetComponent<WeaponPropertyScript> ().type == 0)
			{
				ourWeapons.Remove (obj);
				Destroy (obj);
				OurReset ();
			}

			if (obj.GetComponent<WeaponPropertyScript> ().type == 1)
			{
				enemyWeapons.Remove (obj);
				Destroy (obj);
				EnemyReset ();
			}
		}

		void OurReset()
		{
			if (ourWeaponHealth != null)
			{
				int i = ourWeapons.Count;

				if (i == 1)
				{
					ourWeaponHealth.GetComponent<WeaponHealth> ().colorComb = 0;
				}
				else if (i == 2)
				{
					ourWeaponHealth.GetComponent<WeaponHealth> ().colorComb = 1;
				}
				else if (i == 3)
				{
					ourWeaponHealth.GetComponent<WeaponHealth> ().colorComb = 2;
				} 
				else if (i == 4)
				{
					ourWeaponHealth.GetComponent<WeaponHealth> ().colorComb = 3;
				} 
				else if (i >= 5)
				{
					ourWeaponHealth.GetComponent<WeaponHealth> ().colorComb = 4;
				}
			}
		}

		void EnemyReset()
		{
			if (enemyWeaponHealth != null)
			{
				int i = enemyWeapons.Count;

				if (i == 1)
				{
					enemyWeaponHealth.GetComponent<WeaponHealth> ().colorComb = 0;
				}
				else if (i == 2)
				{
					enemyWeaponHealth.GetComponent<WeaponHealth> ().colorComb = 1;
				}
				else if (i == 3)
				{
					enemyWeaponHealth.GetComponent<WeaponHealth> ().colorComb = 2;
				} 
				else if (i == 4)
				{
					enemyWeaponHealth.GetComponent<WeaponHealth> ().colorComb = 3;
				} 
				else if (i >= 5)
				{
					enemyWeaponHealth.GetComponent<WeaponHealth> ().colorComb = 4;
				}
			}
		}
	}
}

