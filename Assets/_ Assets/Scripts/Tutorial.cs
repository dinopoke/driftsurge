using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    public static Tutorial instance;

	public bool tutorialRunning = false;

	public enum TutorialPhase {start, boost, drift, barrier, barrierBreak, wait, unstoppableWarning, end} ;

    public TutorialPhase currentTutorialPhase;	

	int phaseCounter;

	bool intiatePhase;
	public bool endPhase;



	void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }		
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartTutorial(){
		tutorialRunning = true;

		phaseCounter = 0;
		intiatePhase =  true;
		endPhase =  false;
		UpdatePhase();		

		StartCoroutine(TutorialLoop());

	}

	IEnumerator TutorialLoop(){
		while (currentTutorialPhase != TutorialPhase.end){
			switch (currentTutorialPhase){
			case TutorialPhase.start:
				if (intiatePhase){
					yield return new WaitForSeconds(3.0f);
					endPhase = true;
					intiatePhase = false;
				}
				break;

			case TutorialPhase.boost:

				if(GameManager.instance.player.currentBoostMeter> 0){
					if (intiatePhase){
							intiatePhase = false;
							ShowBoostPrompt();
							ShowBoostMeter();

					}
				}
				else {
					endPhase = true;
				}

				break;
			case TutorialPhase.wait:
				if (intiatePhase){

					//yield return new WaitForSeconds(3.0f);
					endPhase = true;
					intiatePhase = false;
				}
				break;
			case TutorialPhase.unstoppableWarning:
				if (intiatePhase){
					GameManager.instance.EnableUnstoppableForce(true);
					yield return new WaitForSeconds(2.0f);
					endPhase = true;
					intiatePhase = false;
				}
				break;				
			case TutorialPhase.drift:

				break;
			case TutorialPhase.barrier:

				break;
			case TutorialPhase.end:

				break;				
			default:
				break;}

			CheckPhaseEnd();

			yield return null;
		}


		yield return null;
	}


	// Change tutorial order here
	void UpdatePhase (){

		switch (phaseCounter){
		case 0:
			currentTutorialPhase = TutorialPhase.start;
		break;
		case 1:
			currentTutorialPhase = TutorialPhase.boost;
		break;
		case 2:
			currentTutorialPhase = TutorialPhase.drift;
		break;	
		case 3:
			currentTutorialPhase = TutorialPhase.wait;
		break;
		case 4:
			currentTutorialPhase = TutorialPhase.boost;
		break;
		case 5:
			currentTutorialPhase = TutorialPhase.drift;
		break;		
		case 6:
			currentTutorialPhase = TutorialPhase.unstoppableWarning;	
		break;
		case 7:
			currentTutorialPhase = TutorialPhase.boost;
		break;
		case 8:
			currentTutorialPhase = TutorialPhase.drift;
		break;				
		case 9:
			currentTutorialPhase = TutorialPhase.barrier;
		break;
		case 10:
			currentTutorialPhase = TutorialPhase.barrierBreak;
		break;	
		case 11:
			currentTutorialPhase = TutorialPhase.end;
			EndTutorial();
		break;									
		default:
		break;

		}

	}

	void ChangePhase(){
		phaseCounter ++;
		UpdatePhase();
		endPhase = false;
		intiatePhase = true;

	}

	void CheckPhaseEnd(){
		if (endPhase){
			ChangePhase();
		}
	}


	public void CheckTutorialDrifting(){

		if (currentTutorialPhase == TutorialPhase.drift){
			if (GameManager.instance.driftButtonPrompt.GetComponent<Animator>().GetBool("start")){
				GameManager.instance.driftButtonPrompt.GetComponent<Animator>().SetBool("pressed", true);
				GameManager.instance.driftButtonPrompt.GetComponent<Animator>().SetBool("start", false);

			}
		}
		

	}

	public void CheckTutorialBoosting(){

		if (GameManager.instance.boostButtonPrompt.GetComponent<Animator>().GetBool("start")){
			GameManager.instance.boostButtonPrompt.GetComponent<Animator>().SetBool("pressed", true);
			GameManager.instance.boostButtonPrompt.GetComponent<Animator>().SetBool("start", false);
		}

		if (currentTutorialPhase == TutorialPhase.boost){
		
/* 			if (GameManager.instance.boostButtonPrompt.GetComponent<Animator>().GetBool("pressed")){
				
			} */
			if (GameManager.instance.player.currentBoostMeter <= 0){
					// End phase when all boost is used
					endPhase = true;
			}

		}
		else if(currentTutorialPhase == TutorialPhase.barrierBreak){
			TrackManager.instance.slowEnd = true;
		}




	}


	public void CheckBoostFillMeter(){
		
		// Checking if you have drifted enough to fill the meter
		if (currentTutorialPhase == TutorialPhase.drift && GameManager.instance.player.currentBoostMeter > ScriptableObjectManager.instance.playerStats.MAX_BOOST_METER){			
			endPhase = true;
		}

	}

	public void BarrierBreak(){
		EndSlowdown();
		endPhase = true;	

	}
	public void EndBarrierPhase(){
		endPhase = true;	

	}

	void ShowDriftPrompt(){
		GameManager.instance.driftButtonPrompt.GetComponent<Animator>().SetBool("pressed", false);
		GameManager.instance.driftButtonPrompt.GetComponent<Animator>().SetBool("start", true);

		GameManager.instance.player.driftDisabled = false;		
		
	}

	void ShowBoostPrompt(){
		GameManager.instance.boostButtonPrompt.GetComponent<Animator>().SetBool("pressed", false);
		GameManager.instance.boostButtonPrompt.GetComponent<Animator>().SetBool("start", true);

		GameManager.instance.player.boostDisabled = false;		
		
	}

	void ShowBoostMeter(){

		// Maybe consolidate this somewhere else?
    	GameManager.instance.player.fillMeter.gameObject.SetActive(true);

		
	}

	public void CheckDriftPrompt(){

		if(GameManager.instance.driftButtonPrompt.GetComponent<Animator>().GetBool("start") == false){
			ShowDriftPrompt();
		}

	}

	public void CheckBoostPrompt(){
		if (GameManager.instance.boostButtonPrompt.GetComponent<Animator>().GetBool("start") == false){
			ShowBoostPrompt();
		}
	}

	void EndTutorial(){

		// Tutorial Ends
		tutorialRunning = false;

		// Tutorial won't play again
		PlayerPrefs.SetInt("TutorialPlayed", 1);
		GameManager.instance.StartGameSetup();
	}

	public void EndSlowdown(){
		
		TrackManager.instance.slowEnd = true;

	}
}
