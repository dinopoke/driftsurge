using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Stats/PlayerStats", order = 1)]
public class PlayerStatsScriptableObject : ScriptableObject {


	
	// Use this for initialization
	[Header("Normal Stats")]

    public bool accelerateNormally;
    public float normalMaxSpeed = 100f;
    public float normalAcceleration = 1f;

    public float normalDecceleration = 0.5f;


	[Header("Boost Stats")]

    public float boostMaxSpeed = 220f;
    public float boostAcceleration = 2f;


	public float MAX_BOOST_METER = 100f;

    [Header("Drift Stats")]
    public float driftMinSpeed = 0;
    public float driftDecceleration = -2f;

    public float driftDeccelerationOnDriftTrack = 0f;

    public float driftSweetSpotSize = 1;




	[Header("Boost Meter Stats")]

    public float startingBoost = 0f;	
    public float boostUseSpeed = 20f;

    
    public AnimationCurve boostUseCurve;
    public float boostFillSpeed = 50f;

	public bool accelerateWhenNoBoost = true;

	public float noBoostMaxSpeed = 220f;

    public float noBoostAcceleration = 1f;

	[Header("Boost Leeway Stats")]


    [Tooltip("This is how much extra time (in secs) you are still invulnerable after releasing the boost - 0.4 looks ok")]
	public float boostLeewayTime = 0.4f;

    [Tooltip("This is how much extra time (in secs) it takes for the boost to actually stop after you let go of the boost button")]
	public float boostReleaseDelayTime = 0.2f;

    [Tooltip("This is how much leeway boost meter you have when you are techincally on 0")]

	public float boostMeterOverflowAmount = 5f;

	public float MAX_HEAT = 100f;

    [Header("Overheat Stats")]

    public bool UseOverheatMechanic = true;
    public float heatUpSpeedThreshold = 15f;
    public float heatRate = 200f;
    public float coolRate = 200f;

    public bool deathOnOverHeat = false;

	[Header("Multiplier Stats")]

	// Needs to be below boost speed - 61 looks okay?
	public float speedToStartMultiplier;

	public float timeUntilMutliplierIncrease;

	public float distanceToEndMultiplier;

	public float timeUntilMutliplierBreak;


	[Header("Score Stats")]

    [Tooltip("The tick rate of the score - ie how often it increases")]

    public float SCORE_RATE = 0.05f;   

    [Tooltip("How much it increases by each tick. Increments of 10 by default")]

    public float SCORE_SCALE = 10;




}
