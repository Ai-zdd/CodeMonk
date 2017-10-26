using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartConnectButton : MonoBehaviour {

	private NetworkClient networkClient;	

	// Use this for initialization
	void Start () {
		networkClient = GameObject.Find ("Network").GetComponent<NetworkClient> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseUp() {
		networkClient.startConnection ();
		Destroy (this.gameObject);
	}
}
