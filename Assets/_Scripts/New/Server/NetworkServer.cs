using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

public class NetworkServer : MonoBehaviour {
	private TcpListener myListener;
	private TcpClient newClient;
	private BinaryReader br;
	private BinaryWriter bw;
	private ServerBattleField battleField;

	// Use this for initialization
	void Start () {
		battleField = GameObject.Find ("BattleField").GetComponent<ServerBattleField>();
		startServer ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void startServer()
	{
		Thread myThread = new Thread(ServerA);
		myThread.Start();
	}

	private void ServerA() {
		myListener = new TcpListener (IPAddress.Any, 2020);//创建TcpListener实例
		myListener.Start();//start
		//服务器开启提示
		Debug.Log ("Server Has Init!");

		newClient = myListener.AcceptTcpClient();//等待客户端连接
		Debug.Log ("Accept Client :" + newClient.Client.RemoteEndPoint.ToString ());
		while (true)
		{
			try
			{
				NetworkStream clientStream = newClient.GetStream();//利用TcpClient对象GetStream方法得到网络流
				br = new BinaryReader(clientStream);
				string receive = null;
				receive = br.ReadString();//读取
				ClientCommand receiveObj = JsonUtility.FromJson<ClientCommand>(receive);
				battleField.processCommand(receiveObj);
			}
			catch
			{
				Debug.Log("接收失败！");
				return;
			}
		}
	}

	//发送消息
	public void sendCommand(CommandModel command)
	{
		if(newClient == null){
			return;
		}
		try{
			NetworkStream clientStream = newClient.GetStream();
			bw = new BinaryWriter(clientStream);
			string json = JsonUtility.ToJson(command);
			bw.Write(json);
		}catch {
			Debug.Log ("发送失败！");
		}
	}
}
