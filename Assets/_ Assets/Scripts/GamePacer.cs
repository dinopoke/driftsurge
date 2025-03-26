using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePacer : MonoBehaviour {

    TrackStatsScriptableObject trackStats;

	PlayerStatsScriptableObject playerStats;

	public static GamePacer instance;


	// Use this for initialization
	void Start () {

		playerStats = ScriptableObjectManager.instance.playerStats;
		trackStats = ScriptableObjectManager.instance.trackStats;

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }		
	}
	
	// Update is called once per frame
	public float Pace(float currentStat, float maxStat, float startStat) {
		
		if ( currentStat < maxStat - 0.1f){

			currentStat  += (Mathf.Abs(maxStat - startStat) / trackStats.paceRampUpTime) * Time.deltaTime;

		}
		else if ( currentStat > maxStat + 0.1f){

			currentStat  -= (Mathf.Abs(startStat - maxStat) / trackStats.paceRampUpTime) * Time.deltaTime;

		}

		return currentStat;

	}
	
}
