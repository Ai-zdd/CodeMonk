using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
	public class WeaponPropertyScript : MonoBehaviour {

		public int id = 0;
		public float atk = 1f;// 攻击力
		public float hp = 100;// 生命值
		public int type = 0;	// 0:我方武器 1:敌方武器
		public int status = 0;	// 0:飞行态 1:拼杀态
		public bool triggerFlag = false;

		void Start()
		{
			
		}

		void Update()
		{
			
		}
	}
}

