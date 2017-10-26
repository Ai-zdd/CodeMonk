using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleGround : MonoBehaviour 
{
	public bool isServer;

	public int serverId;

	private ServerBattleField sbfCs;
	private ClientBattleField cbfCs;

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
		ourWeapons = new List<GameObject> ();

		enemyWeapons = new List<GameObject> ();

		sbfCs = GameObject.Find ("BattleField").GetComponent<ServerBattleField>();
		cbfCs = GameObject.Find ("BattleField").GetComponent<ClientBattleField>();
	}

	void FixedUpdate ()
	{
		// 设置头狼
		FindFirstWolf ();

		// 控制双方头狼加攻击力/掉血/死亡
		ChangeProperty ();

		// 战斗双方全部阵亡
		if (ourWeapons.Count == 0 && enemyWeapons.Count == 0) {
			if (isServer) {
				sbfCs.DestroyGrappleGround (this);
			} else {
				cbfCs.DestroyGrappleGround (this);
			}
		}

		// 一方胜利传递胜利消息给胜方
		Victory();
	}

	void Victory() {
		if (ourWeapons.Count == 0) {
			// 敌方胜利
			for(int i=0; i<enemyWeapons.Count; i++) {
				if(enemyWeapons [i].gameObject != null){
					enemyWeapons [i].GetComponent<WeaponEvent> ().state.state = 1;
					enemyWeapons [i].GetComponent<BoxCollider2D> ().enabled = true;	
				}
			}
			if (isServer) {
				sbfCs.DestroyGrappleGround (this);
			} else {
				cbfCs.DestroyGrappleGround (this);
			}
		}

		if (enemyWeapons.Count == 0) {
			// 我方胜利
			for(int i=0; i<ourWeapons.Count; i++) {
				if(ourWeapons [i].gameObject != null){
					ourWeapons [i].GetComponent<WeaponEvent> ().state.state = 1;
					ourWeapons [i].GetComponent<BoxCollider2D> ().enabled = true;	
				}
			}
			if (isServer) {
				sbfCs.DestroyGrappleGround (this);
			} else {
				cbfCs.DestroyGrappleGround (this);
			}
		}
	}

	void ChangeProperty ()
	{
		if (firstWolf != null && enemyFirstWolf != null)
		{
			if (ourWeaponHealth == null)
			{
				Vector3 ourPos = new Vector3 (firstWolf.transform.position.x, firstWolf.transform.position.y - 2f);

				ourWeaponHealth = Instantiate (weaponHealth, ourPos, Quaternion.identity);

				ourWeaponHealth.GetComponent<WeaponHealth> ().orgHealth = firstWolf.GetComponent<WeaponPropertyScript> ().hp;
			}

			if (enemyWeaponHealth == null)
			{
				Vector3 enemyPos = new Vector3 (enemyFirstWolf.transform.position.x, enemyFirstWolf.transform.position.y + 2f);

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
			if (id == ourWeapons [i].GetComponent<WeaponEvent>().state.sId)
				return true;
		}

		for (int i = 0; i < enemyWeapons.Count; i++)
		{
			if (id == enemyWeapons [i].GetComponent<WeaponEvent>().state.sId)
				return true;
		}

		return false;
	}

	public void AddWeaponToList(GameObject obj)
	{
		WeaponEvent we = obj.GetComponent<WeaponEvent> ();
		if (we.isServer) {
			if(we.state.lId == -1){
				//自己
				ourWeapons.Add (obj);
				OurReset ();
			}else {
				//敌人
				enemyWeapons.Add (obj);
				EnemyReset ();
			}
		} else {
			if(we.state.lId == -1){
				//敌人
				enemyWeapons.Add (obj);
				EnemyReset ();
			}else {
				//自己
				ourWeapons.Add (obj);
				OurReset ();
			}
		}
	}

	void PullWeaponForList(GameObject obj)
	{
		WeaponEvent we = obj.GetComponent<WeaponEvent> ();
		if (we.isServer) {
			if(we.state.lId == -1){
				//自己
				ourWeapons.Remove (obj);
				we.state.state = 3;
				sbfCs.removeWeaponFromList (obj,false);
				OurReset ();
			}else {
				//敌人
				enemyWeapons.Remove (obj);
				we.state.state = 3;
				sbfCs.removeWeaponFromList (obj,false);
				EnemyReset ();
			}
		} else {
			if(we.state.lId == -1){
				//敌人
				enemyWeapons.Remove (obj);
				cbfCs.removeWeaponFromList (obj);
				Destroy (obj);
				EnemyReset ();
			}else {
				//自己
				ourWeapons.Remove (obj);
				cbfCs.removeWeaponFromList (obj);
				Destroy (obj);
				OurReset ();
			}
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

