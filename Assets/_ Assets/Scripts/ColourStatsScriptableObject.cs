using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewColourStats", menuName = "Stats/ColourStats", order = 1)]

public class ColourStatsScriptableObject : ScriptableObject {

	public float minChangeTime;
	public float maxChangeTime;

	public float colourChangeSpeed;

	[System.Serializable]
	public struct SkyboxGradient{
		public Color topColour;

		public Color horizonColour;
		public Color bottomColour;

	}

	[SerializeField]
	public List<SkyboxGradient> skyboxColours;



	public float boostColourChangeSpeed;

	public Color boostHorizonColour;

	[Header("Track Colours")]

	public Color defaultNormalTrack = new Color32( 0, 90, 195, 108 );
	public Color defaultDriftTrack = new Color32( 0, 255, 255, 141 );
	public Color defaultDriftGlowTrack = Color.cyan;

	public Color recordNormalTrack = Color.red;
	public Color recordDriftTrack = new Color32( 251, 255, 0, 141 );
	public Color recordDriftGlowTrack = Color.yellow ;




}
