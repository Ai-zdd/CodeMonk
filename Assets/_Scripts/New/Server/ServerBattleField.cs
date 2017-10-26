using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerBattleField : MonoBehaviour {
	private NetworkServer networkServer;

	public Rigidbody2D boom;

	public GameObject[] orWeapons;

	public Vector3[] orWeaponsPositions;



	private Vector3 shootStartBottom;
	private Vector3 shootStartTop;

	private List<GameObject> weaponObjList;
	private int weaponServerId = 0;//0到无限大递增

	private List<GameObject> CoObjList;
	private int coServerId = 0;//0到无限大递增


	public GameObject grappleGround;
	private List<GrappleGround> grappleGrounds;//拼杀场list
	private int grServerId = 0;//0到无限大递增

	private ClientCommand lastCommand = null;

	public int battleState = 0;//0:战斗中,1:我方胜利,2:敌方胜利,3:战场失效
	public GameObject winBattleImg;
	public GameObject loseBattleImg;

	void Awake() {
		networkServer = GameObject.Find ("Network").GetComponent<NetworkServer> ();	

		weaponObjList = new List<GameObject>();
		CoObjList = new List<GameObject>();
		grappleGrounds = new List<GrappleGround> ();

		shootStartBottom = transform.GetChild (0).transform.position;
		shootStartTop = transform.GetChild (1).transform.position;

		initOriginalWeapon ();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(lastCommand != null) {
			//客户端发射了法宝
			if(lastCommand.type == 0) {
				Rigidbody2D w = Instantiate(orWeapons[lastCommand.w.pN].GetComponent<Rigidbody2D>(), shootStartTop, Quaternion.identity) as Rigidbody2D;
				w.gameObject.tag = "enemyWeapon";
				WeaponEvent we = w.gameObject.GetComponent<WeaponEvent> ();
				we.isServer = true;

				//设置轨道并移动
				we.state.pN = lastCommand.w.pN;
				we.setTrackPoints (TrackManager.Instance.getTrackWithTrajectory(lastCommand.w.tr));
				we.state.tr = lastCommand.w.tr;
				we.state.sId = weaponServerId;
				weaponServerId++;
				we.state.lId = lastCommand.w.lId;
				we.direction = 1;
				we.state.pI = TrackManager.Instance.pointAmount-1-lastCommand.w.pI;
				we.state.state = 1;

				//添加进武器列表
				weaponObjList.Add (w.gameObject);

				//反馈消息给客户端
				sendCommandToClient();
			}
			lastCommand = null;
		}

		if(battleState > 0){
			if(battleState == 1){
				GameObject win = Instantiate (winBattleImg,new Vector3(0,0,0),Quaternion.identity);
			}else if (battleState == 2){
				GameObject win = Instantiate (loseBattleImg,new Vector3(0,0,0),Quaternion.identity);
			}	
			battleState = 3;
		}
	}

	void initOriginalWeapon () {
		for (int i=0; i<orWeapons.Length; i++) {
			GameObject tileChoice = orWeapons[i];
			Rigidbody2D weaponInstance = Instantiate(tileChoice.GetComponent<Rigidbody2D>(), orWeaponsPositions[i], Quaternion.identity);
			WeaponEvent we = weaponInstance.gameObject.GetComponent<WeaponEvent> ();
			we.isServer = true;
		}
	}

	//发射法宝
	public void shootWeapon(int trajectory,GameObject weaponObj){
		//新增武器
		Rigidbody2D w = Instantiate(weaponObj.GetComponent<Rigidbody2D>(), shootStartBottom, Quaternion.identity) as Rigidbody2D;
		w.gameObject.tag = "myWeapon";
		WeaponEvent we = w.gameObject.GetComponent<WeaponEvent> ();
		we.particleObj = Instantiate (GameObject.Find("Flames"), w.transform.localPosition, Quaternion.identity) as GameObject;
		we.isServer = true;
		WeaponPropertyScript wp = w.gameObject.GetComponent<WeaponPropertyScript> ();
		we.state.pN = wp.prefabNumber;

		//设置轨道并移动
		we.setTrackPoints (TrackManager.Instance.getTrackWithTrajectory(trajectory));
		we.state.tr = trajectory;
		we.state.sId = weaponServerId;
		weaponServerId++;
		we.direction = 0;
		we.state.pI = 0;
		we.state.state = 1;
	
		//添加进武器列表
		weaponObjList.Add (w.gameObject);

		//向客户端发送消息
		sendCommandToClient();
	}

	//销毁法宝
	public void removeWeaponFromList (GameObject weaponObj,bool needSynClient){
		//向客户端发送消息
		if(needSynClient){
			sendCommandToClient();	
		}
	
		if (weaponObjList.Contains(weaponObj)){
			weaponObjList.Remove (weaponObj);
		}
	}

	//检查碰撞双方是否已经存在于某个拼杀场
	public void CheckObjects(GameObject obj1, GameObject obj2) {
		if (obj1 != null && obj2 != null) {
			if (obj1.CompareTag ("GrappleGround") || obj2.CompareTag ("GrappleGround")) {
				if (obj1.CompareTag ("GrappleGround")) {
					obj1.GetComponent<GrappleGround> ().AddWeaponToList(obj2);
					obj2.GetComponent<WeaponEvent>().state.state = 2;
					obj2.GetComponent<BoxCollider2D> ().enabled = false;
				}

				if (obj2.CompareTag ("GrappleGround")) {
					obj2.GetComponent<GrappleGround> ().AddWeaponToList(obj1);
					obj1.GetComponent<WeaponEvent>().state.state = 2;
					obj1.GetComponent<BoxCollider2D> ().enabled = false;
				}
			} else {
				if (!CheckObjInGrappleGround (obj1) && !CheckObjInGrappleGround (obj2)) {
					obj1.GetComponent<WeaponEvent> ().state.state = 2;
					obj2.GetComponent<WeaponEvent> ().state.state = 2;

					// 创建拼杀场
					Vector3 gPosition = (obj1.transform.position + obj2.transform.position) / 2;
					GameObject grappleGroundObj = Instantiate(grappleGround, gPosition, Quaternion.identity);
					GrappleGround ggScript = grappleGroundObj.GetComponent<GrappleGround> ();
					ggScript.isServer = true;
					ggScript.serverId = grServerId;
					grServerId++;

					// 添加拼杀场列表
					grappleGrounds.Add (ggScript);

					// 扩充猪圈
					ggScript.AddWeaponToList (obj1);
					ggScript.AddWeaponToList (obj2);

					obj1.GetComponent<BoxCollider2D> ().enabled = false;
					obj2.GetComponent<BoxCollider2D> ().enabled = false;
				}
			}
		}
	}

	public bool CheckObjInGrappleGround(GameObject obj) {
		for (int i = 0; i < grappleGrounds.Count; i++) {
			if (grappleGrounds [i].FindInWeapons (obj.GetComponent<WeaponEvent>().state.sId))
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

	private void sendCommandToClient () {
		ServerCommand command = new ServerCommand ();
		List<WeaponState> weaponStateList = new List<WeaponState> ();
		foreach (GameObject weaponObj in weaponObjList) {
			WeaponEvent we = weaponObj.GetComponent<WeaponEvent> ();
			WeaponState weaponS = new WeaponState();
			weaponS.tr = we.state.tr;
			weaponS.sId = we.state.sId;
			weaponS.lId = we.state.lId;
			weaponS.pI = we.state.pI;
			weaponS.pN = we.state.pN;
			if(we.collisionPointIndex != -1){
				weaponS.pI = we.collisionPointIndex;	
				we.collisionPointIndex = -1;
			}
			weaponS.state = we.state.state;
			weaponStateList.Add (weaponS);
		}
		command.wL = weaponStateList;


		List<GrappleGroundState> ggList = new List<GrappleGroundState> ();
		foreach (GrappleGround gg in grappleGrounds) {
			GrappleGroundState s = new GrappleGroundState ();
			s.x = gg.gameObject.transform.position.x;
			s.y = gg.gameObject.transform.position.y;
			s.sId = gg.serverId;
			List<int> swl = new List<int>();
			foreach (GameObject swObj in gg.ourWeapons) {
				swl.Add (swObj.GetComponent<WeaponEvent>().state.sId);
			}
			s.sw = swl;
			List<int> lwl = new List<int>();
			foreach (GameObject lwObj in gg.enemyWeapons) {
				lwl.Add (lwObj.GetComponent<WeaponEvent>().state.sId);
			}
			s.lw = lwl;

			ggList.Add (s);
		}
		command.gl = ggList;
		networkServer.sendCommand (command);
	}

	//处理接收的消息
	public void processCommand (ClientCommand command){
		lastCommand = command;
	}
}
