using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour {
	public Rigidbody2D trajectory;
	private Rigidbody2D trajectoryInstance_1;
	private Rigidbody2D trajectoryInstance_2;
	private Rigidbody2D trajectoryInstance_3;
	public float trajectoryPosX_1;
	public float trajectoryPosX_2;	
	public float trajectoryPosX_3;
	public float trajectoryPosY;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void showAllDandao(){
		Vector3 pos;
		if(!trajectoryInstance_1){
			pos = new Vector3(trajectoryPosX_1, trajectoryPosY, transform.position.z);
			trajectoryInstance_1 = Instantiate(trajectory, pos, Quaternion.identity) as Rigidbody2D;	
		}

		if(!trajectoryInstance_2){
			pos = new Vector3(trajectoryPosX_2, trajectoryPosY, transform.position.z);
			trajectoryInstance_2 = Instantiate(trajectory, pos, Quaternion.identity) as Rigidbody2D;	
		}

		if(!trajectoryInstance_3){
			pos = new Vector3(trajectoryPosX_3, trajectoryPosY, transform.position.z);
			trajectoryInstance_3 = Instantiate(trajectory, pos, Quaternion.identity) as Rigidbody2D;	
		}
	}

	public void highlightDandao(int index){
		if (index == 0) {
			if(trajectoryInstance_1){
				trajectoryInstance_1.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 1f);	
			}
			if(trajectoryInstance_2){
				trajectoryInstance_2.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
			}
			if(trajectoryInstance_3){
				trajectoryInstance_3.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
			}
		} else if (index == 1) {
			if(trajectoryInstance_1){
				trajectoryInstance_1.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
			}
			if(trajectoryInstance_2){
				trajectoryInstance_2.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 1f);	
			}
			if(trajectoryInstance_3){
				trajectoryInstance_3.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
			}
		} else {
			if(trajectoryInstance_1){
				trajectoryInstance_1.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
			}
			if(trajectoryInstance_2){
				trajectoryInstance_2.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 0.4f);	
			}
			if(trajectoryInstance_3){
				trajectoryInstance_3.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 1f);	
			}
		}
	}

	public void hideAllDandao(){
		if(trajectoryInstance_1){
			Destroy (trajectoryInstance_1.gameObject);	
		}
		if(trajectoryInstance_2){
			Destroy (trajectoryInstance_2.gameObject);	
		}
		if(trajectoryInstance_3){
			Destroy (trajectoryInstance_3.gameObject);	
		}
	}
}
