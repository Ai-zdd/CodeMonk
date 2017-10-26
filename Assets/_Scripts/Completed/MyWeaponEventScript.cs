using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
	public class MyWeaponEventScript : MonoBehaviour 
	{
		public GameObject enemyObject;
		public Rigidbody2D dandao;
		public float dandaoPosX_1 = -4f;
		public float dandaoPosX_2 = 0;
		public float dandaoPosX_3 = 4f;
		public float dandaoPosY = 0;

		private Rigidbody2D dandaoInstance_1;
		private Rigidbody2D dandaoInstance_2;
		private Rigidbody2D dandaoInstance_3;
		private Vector3 lastMousePosition = Vector3.zero;

		private bool isMouseDraged = false;
		private bool isShootting = false;

		private Vector3 originalPos;
		private int channel;

		private Vector3 playerPos;
		private Vector3 enemyPos;

		void Start ()
		{
			// Setting up the references.
			originalPos = transform.position;

			playerPos = GameObject.Find ("Player").transform.position;

			enemyPos = GameObject.Find ("Enemy").transform.position;
		}

		void OnMouseUp()
		{

			if(isShootting)
			{
				return;
			}

			lastMousePosition = Vector3.zero;

			if(isMouseDraged)
			{
				if (transform.position.y >= -9)
				{

					if (transform.position.x <= -2)
					{
						channel = 1;
					}
					else if (transform.position.x > -2 && transform.position.x < 2)
					{
						channel = 2;
					}
					else
					{
						channel = 3;
					}

					GameObject obj1 = Instantiate (gameObject, playerPos, Quaternion.identity);

					obj1.tag = "myWeapon";

					obj1.GetComponent<WeaponThrow> ().enabled = true;

					obj1.GetComponent<WeaponThrow> ().channel = channel;

					GameObject obj2 = Instantiate (enemyObject, enemyPos, Quaternion.identity);

					obj2.tag = "enemyWeapon";

					obj2.GetComponent<WeaponThrow> ().enabled = true;

					obj2.GetComponent<WeaponPropertyScript> ().type = 1;

					obj2.GetComponent<WeaponThrow> ().channel = channel;

					gameObject.transform.position = originalPos;

				}
				else
				{
					gameObject.transform.position = originalPos;

					isShootting = false;
				}

				hideAllDandao();
			}
		}

		void OnMouseDrag()
		{
			if (isShootting)
			{
				return;
			}

			if (lastMousePosition != Vector3.zero)  
			{
				Vector3 offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - lastMousePosition;

				transform.position += offset;

				if (offset.x != 0 || offset.y !=  0)
				{

					isMouseDraged = true;

					if (transform.position.y >= -9)
					{
						showAllDandao();

						if (transform.position.x <= -2)
						{
							highlightDandao (1);
						}
						else if (transform.position.x > -2 && transform.position.x < 2)
						{
							highlightDandao (2);
						}
						else
						{
							highlightDandao (3);
						}
					}
					else
					{
						hideAllDandao();
					}
				}
			}  

			lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		void showAllDandao()
		{
			Vector3 dandapPos;

			if(!dandaoInstance_1)
			{
				dandapPos = new Vector3(dandaoPosX_1, dandaoPosY, transform.position.z);
				dandaoInstance_1 = Instantiate(dandao, dandapPos, Quaternion.identity) as Rigidbody2D;	
			}

			if(!dandaoInstance_2)
			{
				dandapPos = new Vector3(dandaoPosX_2, dandaoPosY, transform.position.z);
				dandaoInstance_2 = Instantiate(dandao, dandapPos, Quaternion.identity) as Rigidbody2D;	
			}

			if(!dandaoInstance_3)
			{
				dandapPos = new Vector3(dandaoPosX_3, dandaoPosY, transform.position.z);
				dandaoInstance_3 = Instantiate(dandao, dandapPos, Quaternion.identity) as Rigidbody2D;	
			}
		}

		void highlightDandao(int index)
		{
			if (index == 1)
			{
				if (dandaoInstance_1)
				{
					dandaoInstance_1.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 1f);	
				}

				if (dandaoInstance_2)
				{
					dandaoInstance_2.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
				}

				if (dandaoInstance_3)
				{
					dandaoInstance_3.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
				}
			}
			else if (index == 2)
			{
				if (dandaoInstance_1)
				{
					dandaoInstance_1.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
				}

				if (dandaoInstance_2)
				{
					dandaoInstance_2.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 1f);	
				}

				if (dandaoInstance_3)
				{
					dandaoInstance_3.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
				}
			}
			else
			{
				if (dandaoInstance_1)
				{
					dandaoInstance_1.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
				}

				if (dandaoInstance_2)
				{
					dandaoInstance_2.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
				}

				if (dandaoInstance_3)
				{
					dandaoInstance_3.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 1f);	
				}
			}
		}

		void hideAllDandao()
		{
			if(dandaoInstance_1)
			{
				Destroy (dandaoInstance_1.gameObject);	
			}

			if(dandaoInstance_2)
			{
				Destroy (dandaoInstance_2.gameObject);	
			}

			if(dandaoInstance_3)
			{
				Destroy (dandaoInstance_3.gameObject);	
			}
		}
	}
}

