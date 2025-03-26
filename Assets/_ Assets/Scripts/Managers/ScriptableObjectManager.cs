using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectManager : MonoBehaviour {


    public static ScriptableObjectManager instance;

	public PlayerStatsScriptableObject playerStats;
	public TrackStatsScriptableObject trackStats;
	public JuiceScriptableObject juiceStats;
	public ColourStatsScriptableObject colourStats;


	// Use this for initialization
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
	
	// Update is called once per frame
	void Update () {
		
	}
}
