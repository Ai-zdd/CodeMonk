using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
	public class WeaponThrow : MonoBehaviour 
	{
		public float leftTrajectory_x = -8f;
		public float rightTrajectory_x = 8f;

		public int pointAmount = 200;//值越大曲线越平滑
		public float pointTime = 0.01f;//每前进一个点的时间

		public int channel; //弹道

		private Bezier bezier;
		private Vector3 startPoint;
		private Vector3 endPoint;

		private Vector3 leftTrajectory;
		private Vector3 middelTrajectory;
		private Vector3 rightTrajectory;

		private WeaponPropertyScript weaponProperty;

		private Vector3 originalPos;
		private Vector3 offset;

		void Awake ()
		{
			GameObject player = GameObject.Find ("Player");
			startPoint = player.transform.localPosition;

			GameObject enemy = GameObject.Find ("Enemy");
			endPoint = enemy.transform.localPosition;

			middelTrajectory = Vector3.zero;
			rightTrajectory = new Vector3 (rightTrajectory_x, 0, transform.position.z);
			leftTrajectory = new Vector3 (leftTrajectory_x, 0, transform.position.z);

			weaponProperty = gameObject.GetComponent<WeaponPropertyScript> ();

			offset = new Vector3 (0, 0.5f, 0);
		}

		void Start () 
		{
			StartCoroutine (Move (channel));
		}

		IEnumerator Move(int track)
		{
			if (gameObject)
			{
				if (track == 1)
				{
					if (weaponProperty.type == 0)
						bezier = new Bezier (startPoint, leftTrajectory, endPoint);
					else
						bezier = new Bezier (endPoint, leftTrajectory, startPoint);
				}
				else if (track == 2)
				{
					if (weaponProperty.type == 0)
						bezier = new Bezier (startPoint, middelTrajectory, endPoint);
					else
						bezier = new Bezier (endPoint, middelTrajectory, startPoint);
				}
				else
				{
					if (weaponProperty.type == 0)
						bezier = new Bezier (startPoint, rightTrajectory, endPoint);
					else
						bezier = new Bezier (endPoint, rightTrajectory, startPoint);
				}

				float t = 1 / (float)pointAmount;
				int count = 0;
				float timer = 0;

				while (count <= pointAmount && gameObject != null)
				{
					if (timer < pointTime)
					{
						timer += Time.deltaTime;
						yield return new WaitForEndOfFrame();
					}
					else
					{
						timer = 0;

						if (!weaponProperty.triggerFlag)
						{
							gameObject.transform.localPosition = bezier.GetPointAtTime((float)(count * t));
							count++;
						}
					}
				}
			}
		}

		void FixedUpdate ()
		{
			transform.Rotate (new Vector3 (0, 0, 45) * 0.2f);
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (GetComponent<WeaponPropertyScript> ().type == 0)
			{
				if (other.CompareTag("enemyWeapon") || other.CompareTag("GrappleGround"))
				{
					GameManager.instance.CheckObjects (gameObject, other.gameObject);
				}
			}

			if (GetComponent<WeaponPropertyScript> ().type == 1)
			{
				if (other.CompareTag("myWeapon") || other.CompareTag("GrappleGround"))
				{
					GameManager.instance.CheckObjects (gameObject, other.gameObject);
				}
			}
		}
	}
}

