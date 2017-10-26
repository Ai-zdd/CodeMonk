using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
	public class Bezier : System.Object {

		public Vector3 p0;
		public Vector3 p1;
		public Vector3 p2;

		public float ti = 0f;

		private Vector3 b0 = Vector3.zero;
		private Vector3 b1 = Vector3.zero;
		private Vector3 b2 = Vector3.zero;

		// Init function 
		public Bezier( Vector3 v0, Vector3 v1, Vector3 v2 )
		{
			this.p0 = v0;
			this.p1 = v1;
			this.p2 = v2;
		}

		// 0.0 <= t <= 1.0
		public Vector3 GetPointAtTime( float t )
		{
			this.CheckConstant();
			float t1 = 1 - t;
			float t2 = t * t;
			float t3 = t1 * t1;
			float x = t3 * p0.x + 2 * t1 * t * p1.x + t2 * p2.x;
			float y = t3 * p0.y + 2 * t1 * t * p1.y + t2 * p2.y;
			float z = t3 * p0.z + 2 * t1 * t * p1.z + t2 * p2.z;
			return new Vector3( x, y, z );
		}

		// Check if p0, p1, p2 or p3 have changed
		private void CheckConstant()
		{
			if( this.p0 != this.b0 || this.p1 != this.b1 || this.p2 != this.b2)
			{
				this.b0 = this.p0;
				this.b1 = this.p1;
				this.b2 = this.p2;
			}
		}
	}
}

