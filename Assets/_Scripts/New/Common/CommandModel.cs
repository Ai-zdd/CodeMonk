using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CommandModel {
	public int weaponryID;

	public int dandao;

	public int weaponryType;

	public float speed;

	//public int type;//1:发射法宝 2:法宝碰撞 3:法宝消失
	public int weaponryOwner;//1:server 2:client
	public float collidePositionX;
	public float collidePositionY;

	public int isFeedback;//1是 2不是

	public int processIndex;//用于标记处理的次序
}

//客户端指令(客户端发往服务器)
[Serializable]
public class ClientCommand : CommandModel {
	public int type;//客户端操作类型（0:发射法宝，其他待定）
	public WeaponState w;//发射的法宝（针对type==0）
}

//服务器指令(服务器发往客户端)
[Serializable]
public class ServerCommand : CommandModel {
	public List<WeaponState> wL;//法宝状态集合
	public List<CollisionState> cL;//碰撞产生物状态集合
	public List<GrappleGroundState> gl;//拼杀场状态集合
}

//法宝状态
[Serializable]
public class WeaponState {
	public int lId = -1;//本地id（客户端本地网络节点+1到无限大递增数）
	public int sId = -1;//服务器id(服务器生成，1到无限大递增数)
	public int pN;//预制资源编号
	public int tr;//弹道
	public int pI;//武器所在弹道点集合的下标，用来表明武器的当前位置
	public int state;//0:将要发射，1:运行中，2:碰撞中，3:将要销毁
}

//碰撞产生物状态
[Serializable]
public class CollisionState {
	public float x;//x位置
	public float y;//y位置
	public int sId;//服务器id(服务器生成，1到无限大递增数)
	public int pN;//预制资源编号
	public int state;//0:显示，1:销毁
}

//拼杀场状态
[Serializable]
public class GrappleGroundState {
	public float x;//x位置
	public float y;//y位置
	public int sId;//服务器id(服务器生成，1到无限大递增数)
	public int state;//0:正在拼杀，1:拼杀结束，销毁
	public List<int> sw;//服务器方武器集合，元素为武器的sId
	public List<int> lw;//客户端方的武器集合，元素为武器的sId
}