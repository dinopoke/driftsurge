using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnstoppableForce : MonoBehaviour {


    TrackStatsScriptableObject trackStats;

    public Player player;	


	[SerializeField]
	float speed = 0;

    Vector3 velocity = Vector3.zero;

	[SerializeField]
	float distanceAway = 30f;


	[SerializeField]
	bool running;


	bool move;

	float currentMaxSpeed;

	float minDistance;

	float speedBelow;
	float speedAbove;



	// Use this for initialization
	void Start(){
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		running = false;

		trackStats = ScriptableObjectManager.instance.trackStats;

		currentMaxSpeed = trackStats.UFSpeedForward;
		minDistance = trackStats.UFMinDistanceAway;		
		speedBelow = trackStats.UFPlayerSpeedBelowThreshold;
		speedAbove = trackStats.UFPlayerSpeedAboveThreshold;

	}
	
	// Dunno if late update is okay heh
	void LateUpdate () {

		if (!GameManager.instance.paused && running) {
			Move();	
		}

		distanceAway = Vector3.Distance(transform.position, player.playerModel.transform.position);
	}

	public void StartPacing(){

        StartCoroutine(StartPacingCoroutine());

	}

	IEnumerator StartPacingCoroutine(){

		float timer = 0;
		currentMaxSpeed = trackStats.startUFSpeedForward;


		while (timer <= trackStats.paceRampUpTime){
            currentMaxSpeed = GamePacer.instance.Pace(currentMaxSpeed,trackStats.UFSpeedForward, trackStats.startUFSpeedForward);
            timer += Time.deltaTime;
			yield return null;
		}
		currentMaxSpeed = trackStats.UFSpeedForward;

		yield return null;	
	}

	void Move(){
		
		CheckTutorialValues();

		if(player.speed < speedBelow)
		{
			// Move boy forward
			if (speed < currentMaxSpeed)
			{
				// Speed up
				speed += trackStats.UFAcceleration;
			}
			if (speed > currentMaxSpeed){

				speed = currentMaxSpeed;
			}
		}
		
		else if (player.speed >= speedAbove){
	
			// Move boy backward
			if (speed > -trackStats.UFSpeedBackward){
				// Speed up
				speed -= trackStats.UFDeceleration;
			}
			if (speed < -trackStats.UFSpeedBackward){
				speed = -trackStats.UFSpeedBackward;
			}		
		}

		if (distanceAway < minDistance){		
			if(Tutorial.instance.tutorialRunning){

			}
			// Don't move any closer
			return;
		}		

		if (distanceAway >= trackStats.UFMaxDistanceAway && speed <= 0){			
		// Don't move away any further

			return;
		}	

		velocity = transform.forward * speed;
		transform.position = transform.position + velocity * Time.deltaTime;	

	}

	void OnTriggerEnter(Collider other) {
		if (other.transform.tag == "PlayerCapsule"){
			player.PlayerCollsionDeath();
			GameManager.instance.GameOver();

		}

	}

	public void SetRunningState(bool flag){
		running = flag;
	}

	public float GetSpeed(){
		if (!GameManager.instance.disableUnstoppableForce){
			return speed;
		}
		return 0f;
	}

	public float GetDistanceAway(){
		if (!GameManager.instance.disableUnstoppableForce){

			return distanceAway;
		}
		return 0f;
	}

	void CheckTutorialValues(){
		if(Tutorial.instance.tutorialRunning && (Tutorial.instance.currentTutorialPhase == Tutorial.TutorialPhase.unstoppableWarning || Tutorial.instance.currentTutorialPhase == Tutorial.TutorialPhase.boost)){
			// Max speed during tutorial - may need to clean up
			minDistance = 19f;
			currentMaxSpeed = 10;

			speedBelow = 65;
			speedAbove = 100;
		}
		else{
			speedBelow = trackStats.UFPlayerSpeedBelowThreshold;
			speedAbove = trackStats.UFPlayerSpeedAboveThreshold;

		}
	}

}
