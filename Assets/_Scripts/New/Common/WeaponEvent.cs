using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEvent : MonoBehaviour {
	public GameObject particleObj;
	public bool isServer; 
	private Trajectory trajectoryCs;
	private ServerBattleField sbfCs;
	private ClientBattleField cbfCs;

	private Vector3 lastMousePosition = Vector3.zero; 

	public bool isInBattlefield = false;
	private bool isMouseDraged = false;
	private Vector3 originalPos;

	public TrackPoint[] trackPoints;
	public WeaponState state;
	public int direction;//0:想上，1:向下

	public int collisionPointIndex = -1;

	private int number1 = 0;
	private int number2 = 0;
	void FixedUpdate()
	{
		if(state.state == 0){
			isInBattlefield = false;
		}else if(state.state == 1) {
			//运行中
			if (isServer) {
				if (sbfCs.battleState > 0) {
					state.state = 3;
					sbfCs.removeWeaponFromList (this.gameObject,false);
				}
			} else {
				if (cbfCs.battleState > 0) {
					cbfCs.removeWeaponFromList (this.gameObject);
					Destroy (this.gameObject);
				}
			}

			number1 = 0;
			number2 = 0;
			isInBattlefield = true;
			transform.Rotate (new Vector3 (0, 0, 45) * 0.2f);
			if (state.pI < 0) {
				state.pI = 0;
			} else if (state.pI > trackPoints.Length-1){
				state.pI = trackPoints.Length - 1;
			}
			this.transform.localPosition = new Vector3 (trackPoints[state.pI].x,trackPoints[state.pI].y,transform.position.z);

			if (particleObj != null) {
				particleObj.transform.localPosition = transform.localPosition;	
			}

			if (direction == 0) {
				state.pI++;
			} else {
				state.pI--;
			}

//			if(isServer) {
//				if (direction == 0) {
//					if(state.pI == trackPoints.Length-1) {
//						state.state = 3;
//						sbfCs.removeWeaponFromList (this.gameObject,true);
//					}
//				} else {
//					if(state.pI == 0) {
//						state.state = 3;
//						sbfCs.removeWeaponFromList (this.gameObject,true);
//					}
//				}
//			}else {
//				if (direction == 0) {
//					if(state.pI == trackPoints.Length-1){
//						cbfCs.removeWeaponFromList (this.gameObject);
//						Destroy (this.gameObject);
//					}
//				} else {
//					if(state.pI == 0){
//						cbfCs.removeWeaponFromList (this.gameObject);
//						Destroy (this.gameObject);
//					}
//				}
//			}
		}else if (state.state == 2) {
			//碰撞中
			isInBattlefield = true;
			transform.Rotate (new Vector3 (0, 0, 0));

			if (isServer) {
				if (sbfCs.battleState > 0) {
					state.state = 3;
					sbfCs.removeWeaponFromList (this.gameObject,false);
				}
			} else {
				if (cbfCs.battleState > 0) {
					cbfCs.removeWeaponFromList (this.gameObject);
					Destroy (this.gameObject);
				}
			}

			if (this.transform.localPosition.y == trackPoints [state.pI].y) {
				if(number1 <= 5){
					number1++;
				}else {
					if (direction == 0) {
						this.transform.localPosition = new Vector3 (trackPoints[state.pI].x,trackPoints[state.pI].y-0.5f,transform.position.z);
					} else {
						this.transform.localPosition = new Vector3 (trackPoints[state.pI].x,trackPoints[state.pI].y+0.5f,transform.position.z);
					}
					number1 = 0;
				}
			} else {
				if (number2 <= 5) {
					number2++;
				} else {
					this.transform.localPosition = new Vector3 (trackPoints [state.pI].x, trackPoints [state.pI].y, transform.position.z);
					number2 = 0;
				}
			}
			if (particleObj != null) {
				particleObj.transform.localPosition = transform.localPosition;	
			}
		}else {
			//销毁

			isInBattlefield = true;
			if(isServer && this.gameObject != null){
				if (particleObj != null) {
					Destroy (particleObj);	
				}

				Destroy (this.gameObject);
			}
		
		}
	}
		

	void Awake ()
	{
		// Setting up the references.
		trajectoryCs = GameObject.Find ("Trajectory").GetComponent<Trajectory>();
		originalPos = this.transform.position;
		state = new WeaponState ();
		sbfCs = GameObject.Find ("BattleField").GetComponent<ServerBattleField>();
		cbfCs = GameObject.Find ("BattleField").GetComponent<ClientBattleField>();
	}

	public void setTrackPoints (TrackPoint[] tps){
		this.trackPoints = tps;
	}

	void OnMouseUp() {
		if(this.isInBattlefield){
			return;
		}

		if (isServer) {
			if (sbfCs.battleState > 0) {
				gameObject.transform.position = originalPos;
				trajectoryCs.hideAllDandao();
				return;
			}
		} else {
			if (cbfCs.battleState > 0) {
				gameObject.transform.position = originalPos;
				trajectoryCs.hideAllDandao();
				return;
			}
		}

		lastMousePosition = Vector3.zero;

		if(isMouseDraged){
			if (this.transform.position.y >= -5.5) {
				int trajectory;
				if (this.transform.position.x <= -2) {
					trajectory = 0;
				} else if (this.transform.position.x > -2 && this.transform.position.x < 2) {
					trajectory = 1;
				} else {
					trajectory = 2;
				}
				state.state = 0;
				if(isServer){
					sbfCs.shootWeapon (trajectory,gameObject);
				}else{
					cbfCs.shootWeapon (trajectory,gameObject);
				}
			}
			gameObject.transform.position = originalPos;
			trajectoryCs.hideAllDandao();
		}
	}

	void OnMouseDrag() {
		if(this.isInBattlefield){
			return;
		}

		if (isServer) {
			if (sbfCs.battleState > 0) {
				gameObject.transform.position = originalPos;
				trajectoryCs.hideAllDandao();
				return;
			}
		} else {
			if (cbfCs.battleState > 0) {
				gameObject.transform.position = originalPos;
				trajectoryCs.hideAllDandao();
				return;
			}
		}

		if (lastMousePosition != Vector3.zero)  
		{  
			Vector3 offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - lastMousePosition;  

			this.transform.position += offset;  

			if(offset.x != 0 || offset.y !=  0){

				isMouseDraged = true;

				if (this.transform.position.y >= -5.5) {

					trajectoryCs.showAllDandao();

					if (this.transform.position.x <= -2) {
						trajectoryCs.highlightDandao (0);
					} else if (this.transform.position.x > -2 && this.transform.position.x < 2) {
						trajectoryCs.highlightDandao (1);
					} else {
						trajectoryCs.highlightDandao (2);
					}
				} else {
					trajectoryCs.hideAllDandao();

				}
			}
		}  

		lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if(this.state.state == 0){
			return;
		}
		if ((!gameObject.CompareTag("enemyWeapon") && other.CompareTag("enemyWeapon")) 
			|| other.CompareTag("GrappleGround")) {
			print ("chou!!!");
			if (isServer) {
				sbfCs.CheckObjects (gameObject, other.gameObject);
			} else {
				cbfCs.CheckObjects (gameObject, other.gameObject);
			}
		}
	}
}
