using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourChanger : MonoBehaviour {

	ColourStatsScriptableObject colourStats;
	Color oldTopColour;
	Color oldHorizonColour;
	Color oldBottomColour;



	Color newTopColour;
	Color newHorizonColour;

	Color newBottomColour;




	bool inverting;

	bool fadingOut;

	// Use this for initialization
	void Start () {

		colourStats = ScriptableObjectManager.instance.colourStats;

		inverting = false;
		fadingOut = false;
		

		StartCoroutine(ColourChanging());

	}
	
	IEnumerator ColourChanging(){

		while (true){

			yield return new WaitForSeconds(Random.Range(colourStats.minChangeTime, colourStats.maxChangeTime));


			int i = Random.Range(0, colourStats.skyboxColours.Count);

			newTopColour = colourStats.skyboxColours[i].topColour;
			newHorizonColour = colourStats.skyboxColours[i].horizonColour;
			newBottomColour = colourStats.skyboxColours[i].bottomColour;


			float t = 0;
			oldTopColour = RenderSettings.skybox.GetColor("_Color1");
			oldHorizonColour = RenderSettings.skybox.GetColor("_Color2");
			oldBottomColour = RenderSettings.skybox.GetColor("_Color3");



			while (t <= 1){

				if (!inverting && !fadingOut){

					RenderSettings.skybox.SetColor("_Color1", Color.Lerp(oldTopColour, newTopColour, t));
					RenderSettings.skybox.SetColor("_Color2", Color.Lerp(oldHorizonColour, newHorizonColour, t));
					RenderSettings.skybox.SetColor("_Color3", Color.Lerp(oldBottomColour, newBottomColour, t));

					t += Time.deltaTime * colourStats.colourChangeSpeed;

				}

				yield return null;

			}

		yield return null;

		}


	}

	public void InvertColours(bool flag){

		if (flag){
			inverting = true;
			oldHorizonColour = RenderSettings.skybox.GetColor("_Color2");
			RenderSettings.skybox.SetColor("_Color2", colourStats.boostHorizonColour);	
		}

		else {

			if (!fadingOut && inverting){
				StartCoroutine(ColourLerp());
			}

		}


	}

	IEnumerator ColourLerp(){

		inverting = false;
		fadingOut = true;

		float t = 0;

		while (t <= 1){

			RenderSettings.skybox.SetColor("_Color2", Color.Lerp(colourStats.boostHorizonColour, oldHorizonColour, t));

			t += Time.deltaTime * colourStats.boostColourChangeSpeed;

			if (inverting){
				RenderSettings.skybox.SetColor("_Color2", oldHorizonColour);	

				break;
			}

			yield return null;

			
		}



		fadingOut = false;
		yield return null;

	}

}
