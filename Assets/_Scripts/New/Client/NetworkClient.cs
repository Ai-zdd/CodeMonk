using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

public class NetworkClient : MonoBehaviour {
	private TcpClient client;
	public BinaryReader br;
	public BinaryWriter bw;
	private ClientBattleField battleField;

	// Use this for initialization
	void Start () {
		battleField = GameObject.Find ("BattleField").GetComponent<ClientBattleField>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void startConnection()
	{
		Thread myThread = new Thread(ClientA);
		myThread.Start();
	}

	private void ClientA() {
		//通过服务器的ip和端口号，创建TcpClient实例
		client = new TcpClient(AddressFamily.InterNetwork);
		IPEndPoint serverIPEndPoint = new IPEndPoint (IPAddress.Parse ("192.168.0.183"), 2020);
		client.Connect (serverIPEndPoint);
		Debug.Log ("与服务器连接成功,我是"+client.Client.LocalEndPoint.ToString());

		while (true)
		{
			try
			{
				NetworkStream clientStream = client.GetStream();
				br = new BinaryReader(clientStream);
				string receive = null;

				receive = br.ReadString();
				ServerCommand receiveObj = JsonUtility.FromJson<ServerCommand>(receive);
				battleField.processCommand(receiveObj);
				Debug.Log(""+receive);
			}
			catch
			{
				Debug.Log ("接收失败！" );
				return;
			}
		}
	}

	//发送消息
	public void sendCommand(ClientCommand command)
	{
		if(client == null){
			return;
		}
		try{
			NetworkStream clientStream = client.GetStream();
			bw = new BinaryWriter(clientStream);
			string json = JsonUtility.ToJson(command);
			bw.Write(json);
		}catch {
			Debug.Log ("发送失败！" );
		}
	}
}
