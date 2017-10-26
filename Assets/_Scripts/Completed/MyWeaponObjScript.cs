using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
	public class MyWeaponObjScript : MonoBehaviour {

		public GameObject[] weapons;
		public float posX = -4.28f;
		public float posY = -8.72f;
		public float offset = 2.56f;

		public int count = 3;	// 法宝栏数

		public Rigidbody2D boom;

		private GameObject weaponInstance;

		public Rigidbody2D dandao;
		public float dandaoPosX_1 = -4f;
		public float dandaoPosX_2 = 0;
		public float dandaoPosX_3 = 4f;
		public float dandaoPosY = 0;

		private Rigidbody2D dandaoInstance_1;
		private Rigidbody2D dandaoInstance_2;
		private Rigidbody2D dandaoInstance_3;

		private Vector3 pos;

		void Start ()
		{
			for (int i=0; i<count; i++) {
				pos = new Vector3 (posX + i * offset, posY, 0);
				weaponInstance = Instantiate(weapons[i], pos, Quaternion.identity) as GameObject;
				weaponInstance.AddComponent<MyWeaponEventScript> ();
				int j = i + 1;
				weaponInstance.tag = "w_00" + j;
			}
		}

		public void showAllDandao(){
			Vector3 dandapPos;
			if(!dandaoInstance_1){
				dandapPos = new Vector3(dandaoPosX_1, dandaoPosY, transform.position.z);
				dandaoInstance_1 = Instantiate(dandao, dandapPos, Quaternion.identity) as Rigidbody2D;	
			}

			if(!dandaoInstance_2){
				dandapPos = new Vector3(dandaoPosX_2, dandaoPosY, transform.position.z);
				dandaoInstance_2 = Instantiate(dandao, dandapPos, Quaternion.identity) as Rigidbody2D;	
			}

			if(!dandaoInstance_3){
				dandapPos = new Vector3(dandaoPosX_3, dandaoPosY, transform.position.z);
				dandaoInstance_3 = Instantiate(dandao, dandapPos, Quaternion.identity) as Rigidbody2D;	
			}
		}

		public void highlightDandao(int index){
			if (index == 1) {
				if(dandaoInstance_1){
					dandaoInstance_1.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 1f);	
				}
				if(dandaoInstance_2){
					dandaoInstance_2.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
				}
				if(dandaoInstance_3){
					dandaoInstance_3.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
				}
			} else if (index == 2) {
				if(dandaoInstance_1){
					dandaoInstance_1.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
				}
				if(dandaoInstance_2){
					dandaoInstance_2.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 1f);	
				}
				if(dandaoInstance_3){
					dandaoInstance_3.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
				}
			} else {
				if(dandaoInstance_1){
					dandaoInstance_1.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
				}
				if(dandaoInstance_2){
					dandaoInstance_2.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
				}
				if(dandaoInstance_3){
					dandaoInstance_3.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 1f);	
				}
			}
		}

		public void hideAllDandao(){
			if(dandaoInstance_1){
				Destroy (dandaoInstance_1.gameObject);	
			}
			if(dandaoInstance_2){
				Destroy (dandaoInstance_2.gameObject);	
			}
			if(dandaoInstance_3){
				Destroy (dandaoInstance_3.gameObject);	
			}
		}

		public void fighting(Vector3 position,float deathTime){
			Rigidbody2D boomInstance = Instantiate(boom, position, Quaternion.identity) as Rigidbody2D;
			Destroy (boomInstance.gameObject, deathTime);
		}
	}
}

