using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AdsScript : MonoBehaviour {


    public bool testMode = false;

    public static AdsScript instance;

    public bool adPlaying;

    void Start () {

        adPlaying = false;

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }




    }
	// Update is called once per frame
	void Update () {
		
	}



    public void ShowAd (float waitTime = 0f, Transform adTransform = null) {
    }


}
