﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public static AudioManager instance;

	// Use this for initialization
	void Start () {

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
}
