using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientBattleField : MonoBehaviour {

	private NetworkClient networkClient;

	public Rigidbody2D boom;

	public GameObject[] orWeapons;
	public Vector3[] orWeaponsPositions;

	private Vector3 shootStartBottom;
	private Vector3 shootStartTop;

	private List<GameObject> weaponObjList;
	private int weaponLocalId = 0;//0到无限大递增


	private List<GameObject> CoObjList;


	public GameObject grappleGround;
	private List<GrappleGround> grappleGrounds;//拼杀场list

	private ServerCommand lastCommand = null;

	public int battleState = 0;//0:战斗中,1:我方胜利,2:敌方胜利,3:战场失效
	public GameObject winBattleImg;
	public GameObject loseBattleImg;

	void Awake() {
		networkClient = GameObject.Find ("Network").GetComponent<NetworkClient> ();	

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
			foreach (WeaponState serverWeapon in lastCommand.wL) {
				bool find = false;
				GameObject needRmoveObj = null;
				foreach (GameObject localWeaponObj in weaponObjList) {
					WeaponEvent we = localWeaponObj.GetComponent<WeaponEvent> ();
					if(we.state.sId == serverWeapon.sId) {
						//找到了
						find = true;
						//同步状态
						if (serverWeapon.state == 0) {
							//不会出现这种情况
						} else if (serverWeapon.state == 1) {
							//移动中
						} else if (serverWeapon.state == 2) {
							//碰撞中
						}  else {
							//销毁
							needRmoveObj = localWeaponObj;
						}
						we.state.state = serverWeapon.state;
						if (serverWeapon.lId == -1) {
							//产生自服务端,法宝向下运行
							we.direction = 1;
							we.state.pI = TrackManager.Instance.pointAmount-1-serverWeapon.pI;
						} else {
							//产生自客户端，法宝向上运行
							we.direction = 0;
							we.state.pI = TrackManager.Instance.pointAmount-1-serverWeapon.pI;
						}
					}
				}
				if(needRmoveObj != null){
					removeWeaponFromList (needRmoveObj);
					Destroy (needRmoveObj);
				}

				//没找见，表明服务器新增了一个法宝
				if(!find && serverWeapon.state != 3) {
					Rigidbody2D w;
					bool isUp;
					if(serverWeapon.lId == -1){
						//产生自服务端
						w = Instantiate(orWeapons[serverWeapon.pN].GetComponent<Rigidbody2D>(), shootStartTop, Quaternion.identity) as Rigidbody2D;
						w.gameObject.tag = "enemyWeapon";
						isUp = false;
					}else{
						//产生自客户端
						w = Instantiate(orWeapons[serverWeapon.pN].GetComponent<Rigidbody2D>(), shootStartBottom, Quaternion.identity) as Rigidbody2D;
						w.gameObject.tag = "myWeapon";
						isUp = true;
					}

					//设置轨道并移动
					WeaponEvent we = w.gameObject.GetComponent<WeaponEvent> ();
					we.setTrackPoints (TrackManager.Instance.getTrackWithTrajectory(serverWeapon.tr));
					we.state.tr = serverWeapon.tr;
					we.state.sId = serverWeapon.sId;
					we.state.lId = serverWeapon.lId;
					if (isUp) {
						we.direction = 0;
					} else {
						we.direction = 1;
					}
					we.state.pI = TrackManager.Instance.pointAmount-1-serverWeapon.pI;
					we.state.state = 1;

					//添加进武器列表
					weaponObjList.Add (w.gameObject);
				}
			}


			foreach (CollisionState serverCo in lastCommand.cL) {
				bool find = false;
				GameObject needRmoveObj = null;
				foreach (GameObject localCoObj in CoObjList) {
					Boom b = localCoObj.GetComponent<Boom> ();
					if(b.state.sId == serverCo.sId){
						//找到了
						find = true;
						//同步状态
						if (serverCo.state == 0) {
							//不会出现这种情况
						}else {
							//销毁
							needRmoveObj = localCoObj;
						}
						b.state.state = serverCo.state;
					}
				}
				if(needRmoveObj != null){
					CoObjList.Remove (needRmoveObj);
					Destroy (needRmoveObj);
				}

				//没找见，表明服务器新增了一个法宝
				if(!find) {
					Vector3 v3 = new Vector3 (serverCo.x,serverCo.y,transform.position.z);
					 Rigidbody2D boomInstance = Instantiate(boom, v3, Quaternion.identity) as Rigidbody2D;
					Boom b = boomInstance.gameObject.GetComponent<Boom> ();
					b.state = serverCo;

					//添加进碰撞列表列表
					CoObjList.Add (boomInstance.gameObject);
				}
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
			we.isServer = false;
			WeaponPropertyScript wp = weaponInstance.GetComponent<WeaponPropertyScript> ();
			we.state.pN = wp.prefabNumber;
		}
	}

	//发射法宝
	public void shootWeapon(int trajectory,GameObject weaponObj){
		ClientCommand command = new ClientCommand ();
		WeaponEvent we = weaponObj.GetComponent<WeaponEvent> ();
		WeaponState weaponS = new WeaponState ();
		weaponS.pN = we.state.pN;
		weaponS.lId = weaponLocalId;
		weaponS.tr = trajectory;
		weaponLocalId++;
		command.w = weaponS;

		//向服务端发送消息
		sendCommandToServer(command);
	}

	public void removeWeaponFromList (GameObject weaponObj){
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
					ggScript.isServer = false;

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

	//向服务端发送消息
	private void sendCommandToServer (ClientCommand command){
		networkClient.sendCommand (command);
	}

	//处理接收的消息
	public void processCommand (ServerCommand command){
		lastCommand = command;
	}
}
