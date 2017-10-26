using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
	using System.Collections.Generic;		//Allows us to use Lists. 

	public class GameManager : MonoBehaviour
	{
		public static GameManager instance = null;

		public int objectCount = 3;
		public float posX = -4.28f;
		public float posY = -8.72f;
		public float offset = 2.56f;
		public GameObject[] weapons;
		public GameObject grappleGround;

		private int id = 0;
		private BoardManager boardScript;

		// 拼杀场 List TODO
		private List<GrappleGround> grappleGrounds;

		void Awake()
		{
			if (instance == null)

				instance = this;

			else if (instance != this)

				Destroy(gameObject);

			grappleGrounds = new List<GrappleGround> ();

			boardScript = GetComponent<BoardManager>();

			for (int i=0; i<objectCount; i++)
			{
				Vector3 pos = new Vector3 (posX + i * offset, posY, 0);

				GameObject tileChoice = weapons[Random.Range (0, weapons.Length)];

				Instantiate(tileChoice, pos, Quaternion.identity);
			}
		}

		// Check 碰撞双方是否已经存在于某个拼杀场
		public void CheckObjects(GameObject obj1, GameObject obj2)
		{
			if (obj1 != null && obj2 != null)
			{
				if (obj1.CompareTag ("GrappleGround") || obj2.CompareTag ("GrappleGround")) 
				{
					if (obj1.CompareTag ("GrappleGround"))
					{
						obj1.GetComponent<GrappleGround> ().AddWeaponToList(obj2);

						obj2.GetComponent<WeaponPropertyScript> ().id = ++id;

						obj2.GetComponent<WeaponPropertyScript>().status = 1;

						obj2.GetComponent<WeaponPropertyScript> ().triggerFlag = true;

						obj2.GetComponent<BoxCollider2D> ().enabled = false;
					}
						

					if (obj2.CompareTag ("GrappleGround"))
					{
						obj2.GetComponent<GrappleGround> ().AddWeaponToList(obj1);

						obj1.GetComponent<WeaponPropertyScript> ().id = ++id;

						obj1.GetComponent<WeaponPropertyScript>().status = 1;

						obj1.GetComponent<WeaponPropertyScript> ().triggerFlag = true;

						obj1.GetComponent<BoxCollider2D> ().enabled = false;
					}
				}
				else
				{
					obj1.GetComponent<WeaponPropertyScript> ().id = ++id;

					obj2.GetComponent<WeaponPropertyScript> ().id = ++id;

					if (!CheckObjInGrappleGround (obj1) && !CheckObjInGrappleGround (obj2))
					{
						obj1.GetComponent<WeaponPropertyScript> ().triggerFlag = true;
						obj2.GetComponent<WeaponPropertyScript> ().triggerFlag = true;

						// 创建拼杀场
						Vector3 gPosition = (obj1.transform.position + obj2.transform.position) / 2;

						GameObject grappleGroundObj = Instantiate(grappleGround, gPosition, Quaternion.identity);

						GrappleGround ggScript = grappleGroundObj.GetComponent<GrappleGround> ();

						// 添加拼杀场列表
						grappleGrounds.Add (ggScript);

						// 扩充猪圈
						ggScript.AddWeaponToList (obj1);
						ggScript.AddWeaponToList (obj2);

						// 修改武器状态为拼杀态
						WeaponPropertyScript obj1Property = obj1.GetComponent<WeaponPropertyScript>();

						obj1Property.status = 1;

						WeaponPropertyScript obj2Property = obj2.GetComponent<WeaponPropertyScript>();

						obj2Property.status = 1;

						obj1.GetComponent<BoxCollider2D> ().enabled = false;
						obj2.GetComponent<BoxCollider2D> ().enabled = false;
					}
				}
			}
		}

		public bool CheckObjInGrappleGround(GameObject obj)
		{
			for (int i = 0; i < grappleGrounds.Count; i++)
			{
				if (grappleGrounds [i].FindInWeapons (obj.GetComponent<WeaponPropertyScript>().id))
					
					return true;
			}

			return false;
		}

		public void DestroyGrappleGround(GrappleGround script)
		{
			grappleGrounds.Remove (script);
			Destroy (script.ourWeaponHealth);
			Destroy (script.enemyWeaponHealth);
			Destroy (script.gameObject);
		}
	}
}
