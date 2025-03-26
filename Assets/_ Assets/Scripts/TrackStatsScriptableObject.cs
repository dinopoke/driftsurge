using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTrackStats", menuName = "Stats/TrackStats", order = 1)]

public class TrackStatsScriptableObject : ScriptableObject {


	[Header("Normal Track")]

	[Range(0, 1000)]

	public float minimumNormalLength = 4;

	[Range(0, 1000)]

	public float maximumNormalLength = 4;

	[Header("Drift Track")]

	public float chanceForDriftTrack = 20f;

	[Range(0, 500f)]
	public float minimumDriftRadius = 20;

	[Range(0, 500f)]

	public float maximumDriftRadius= 100;

	[Range(0, 360)]
	public float minimumDriftAngle = 30f;

	[Range(0, 360f)]

	public float maximumDriftAngle = 160f;

	public int maxNumberOfNormalTracksUntilDrift = 5;


	[Header("Barrier Track")]
	public float chanceForBarrier = 10f;

	public int maxNumberOfBarriersUntilDrift = 3;


	[Header("Unstoppable Force")]



	[Tooltip("If the player is above or at at this speed, the block will slow down. The values between speed above and speed below are where the boi stays stationary")]
	public float UFPlayerSpeedAboveThreshold = 59f;
	[Tooltip("If the player is below this speed, the block thing will accelerate")]
	public float UFPlayerSpeedBelowThreshold = 58f;

	public float UFSpeedForward = 10f;

	public float UFAcceleration = 25f;

	[Tooltip("The rate which the block slows down while the player is above the Speed Threshold")]
	public float UFSpeedBackward = 10f;

	public float UFDeceleration = 1f;


	[Tooltip("This is how the block thing can be away from the player. The distance is capped to keep pressure so that the player can't go really far away from it")]
	public float UFMaxDistanceAway = 35f;



	[Tooltip("This is how close the boi can be to the player before instant death")]
	public float UFMinDistanceAway = 5f;


	[Header("Ads")]

	public float chanceForAdBarrier = 30f;

	[Header("Misc")]

	public float trackWidth = 6f;
	public float slope = -0.08f;

	[Header("Balance Tweaks For Generation")]

	public float minimumAngleBeforeBarrier = 60;
	public float minimumRadiusBeforeBarrier = 30;

	public float minimumDistanceBetweenBarriers = 100;


	[Header("Pacing Time")]

	public float paceRampUpTime = 20;

	[Header("Pacing Stats")]

	public float startChanceForDriftTrack = 20f;

	[Range(0, 1000)]
	public float startMinimumNormalLength = 4;

	[Range(0, 1000)]

	public float startMaximumNormalLength = 4;

	[Range(0, 500f)]
	public float startMinimumDriftRadius = 20;

	[Range(0, 500f)]

	public float startMaximumDriftRadius= 100;

	[Range(0, 360)]
	public float startMinimumDriftAngle = 30f;

	[Range(0, 360f)]

	public float startMaximumDriftAngle = 160f;

	public int startMaxNumberOfNormalTracksUntilDrift = 5;


	public float startChanceForBarrier = 10f;

	public int startMaxNumberOfBarriersUntilDrift = 3;

	public int startUFSpeedForward= 2;



}
