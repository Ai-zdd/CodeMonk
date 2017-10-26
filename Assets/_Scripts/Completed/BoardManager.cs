using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
	public class BoardManager : MonoBehaviour 
	{
		public int objectCount = 3;
		public float posX = -4.28f;
		public float posY = -8.72f;
		public float offset = 2.56f;
		public GameObject[] weapons;

		public void SetupScene ()
		{
			for (int i=0; i<objectCount; i++)
			{
				Vector3 pos = new Vector3 (posX + i * offset, posY, 0);

				GameObject tileChoice = weapons[Random.Range (0, weapons.Length)];

				GameObject obj = Instantiate(tileChoice, pos, Quaternion.identity);
			}
		}

		public void showAllDandao()
		{
			
		}

		public void highlightDandao(int index)
		{
			
		}

		public void hideAllDandao()
		{
			
		}

		void OnMouseUp()
		{
			
		}

		void OnMouseDrag()
		{
			
		}


	}
}

