using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager {

	//单例
	public static readonly TrackManager Instance = new TrackManager ();

	//左弹道轨迹点集合
	private TrackPoint[] leftTrackPoints;
	//中弹道轨迹点集合
	private TrackPoint[] middelTrackPoints;
	//右弹道轨迹点集合
	private TrackPoint[] rightTrackPoints;

	private Vector3 top;
	private Vector3 left;
	private Vector3 bottom;
	private Vector3 right;
	private Vector3 middle;

	public int pointAmount = 201;
	private Bezier bezier;


	private TrackManager () {
		top = new Vector3 (0.0f,6.5f,0.0f);
		left = new Vector3 (-8.0f,0.0f,0.0f);
		bottom = new Vector3 (0.0f,-5.5f,0.0f);
		right = new Vector3 (8.0f,0.0f,0.0f);
		middle = new Vector3 (0.0f,0.0f,0.0f);
	}

	public TrackPoint[] getTrackWithTrajectory (int trajectory){
		if(trajectory == 0){
			return this.getLeftTrackPoints();
		}else if (trajectory == 1){
			return this.getMiddleTrackPoints();
		}else {
			return this.getRightTrackPoints();
		}
	}

	private TrackPoint[] getLeftTrackPoints () {
		if(leftTrackPoints == null || leftTrackPoints.Length == 0){
			leftTrackPoints = getTrack (0);
		}
		return leftTrackPoints;
	}

	private TrackPoint[] getMiddleTrackPoints () {
		if(middelTrackPoints == null || middelTrackPoints.Length == 0){
			middelTrackPoints = getTrack (1);
		}
		return middelTrackPoints;
	}

	private TrackPoint[] getRightTrackPoints () {
		if(rightTrackPoints == null || rightTrackPoints.Length == 0){
			rightTrackPoints = getTrack (2);
		}
		return rightTrackPoints;
	}

	private TrackPoint[] getTrack(int trajectory) {
		TrackPoint[] points = new TrackPoint[pointAmount];
		if (trajectory == 0) {
			bezier = new Bezier (bottom, left, top);
		} else if (trajectory == 1) {
			bezier = new Bezier (bottom, middle, top);
		} else {
			bezier = new Bezier (bottom, right, top);
		}
		float t = 1 / ((float)pointAmount-1);
		int count = 0;
		while (count < pointAmount)
		{
			TrackPoint point = new TrackPoint ();
			point.x = bezier.GetPointAtTime ((float)(count * t)).x;
			//point.y = bezier.GetPointAtTime ((float)(count * t)).y;
			point.y = bottom.y+count*t*(top.y - bottom.y);

			points [count] = point; 
			count++;
		}
		return points;
	}
}

public class TrackPoint {
	public float x;
	public float y;
}
