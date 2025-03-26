using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("Debug")]

    public bool tutorialAlways;
    public bool disableUnstoppableForce;
    public bool disablePacer;
    public bool debugColours;
    public bool debugText;





    float deathTimer = 0;

    public static GameManager instance;
    ArcSegment currentArc;
    
    [Space(10)]
    [Header("General")]    
    public Player player;
    public Animator screenFader;
    public Text scoreCounterText;
    public Animator distanceTextAnimator;


    public int scaledDistance = 0;

    int rawScore;

    int score;


    float playerHeight = 0;

    public bool paused = false;

    bool mobileRestart;

	CameraRig cameraScript;

	public bool gameOver = false;

    public GameObject namething;

    public Text scoreTitle;

    public Text scoreEndText;

    public Text highscoreTitle;

    public Text highscoreText;

    bool instantScoreShow;

    [HideInInspector]
    UnstoppableForce unstoppableForce;


    [HideInInspector]
    public GameObject driftButtonPrompt;
    [HideInInspector]
    public GameObject boostButtonPrompt;

    int tutorialDistance;

    float highestScore;

    public GameObject audioManager;


    // The tick rate of the score


    public GameObject mobileBoostButton;

    public GameObject mobileDriftButton;

    public GameObject settingsScreen;
    public GameObject homeScreen;


    [HideInInspector]
    public bool audioIsOn = true;
    [HideInInspector]

    public bool vibrateIsOn = true; 


    public Image audioButtonImage;

    public Image vibrateButtonImage;

    public Image adsButtonImage;


    public Sprite audioButtonOn;
    public Sprite audioButtonOff;


    public Sprite vibrateButtonOn;
    public Sprite vibrateButtonOff;

    public Sprite adsButtonOn;
    public Sprite adsButtonOff;

    public GameObject playerAudio;

    public GameObject adButton;
    public GameObject iapButton;



    public int adState;

    public Text errorText;


    public Material highDriftMaterial;
    public Material highNormalMaterial; 

    [HideInInspector]
    public bool beatenScore;

	ColourStatsScriptableObject colourStats;

    float distanceScore;

    public Text playerSpeed;
	public Text UFSpeed;
	public Text distancetext;


	void Awake()
    {
        //PlayerPrefs.SetInt("Premium", 0);

        Application.targetFrameRate = 60;

        Screen.SetResolution(1480, 720, true);

        
        Time.timeScale = 1f;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;


        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        mobileRestart = false;

        cameraScript = GameObject.Find("CameraPivot").GetComponent<CameraRig>();

        instantScoreShow = false;

        unstoppableForce = GameObject.Find("Unstoppable Force").GetComponent<UnstoppableForce>();

        driftButtonPrompt = GameObject.Find("Drift Button Prompt");

        boostButtonPrompt = GameObject.Find("Boost Button Prompt");

        settingsScreen.SetActive(false);
  
        DisableUI();

        CheckAudio();
        CheckVibrate();
        CheckAds();
        CheckPremium();
    }
    
    void DisableUI(){
        scoreCounterText.enabled = false;
        scoreTitle.enabled = false;
        scoreEndText.enabled = false;
        highscoreTitle.enabled = false;
        highscoreText.enabled = false;

    }
    void Start()
    {

        AssignScriptableObjects();

		cameraScript.smoothing = cameraScript.startCameraSpeed;

        TrackManager.instance.InitializeTrack();
		TrackManager.instance.IncrementArc();
        currentArc = TrackManager.instance.currentArc;
		player.Initialize();

		player.transform.position = currentArc.ArcToWorld(new ArcPoint(currentArc.startAngle, currentArc.radius)).getFlat() + new Vector3(0, 0.3f, 0);


		// Sets player rotation
		player.transform.rotation = Quaternion.AngleAxis(currentArc.flipped ? currentArc.startAngle : -currentArc.startAngle , Vector3.up);

        paused = true;
		gameOver = false;

        EnableUnstoppableForce(false);

        highestScore = PlayerPrefs.GetInt("highestScore");

        highDriftMaterial.color = colourStats.defaultDriftTrack;
        highNormalMaterial.color = colourStats.defaultNormalTrack;
        beatenScore = false;

        player.fillMeter.gameObject.SetActive(false);

        StartDebugCalculations();

    }


    void AssignScriptableObjects(){

        // Assign Scriptable Objects
        colourStats = ScriptableObjectManager.instance.colourStats;

    }

    void DebugCalculations(){

        if (debugText) {
            playerSpeed.text = "Player Speed " + player.speed;	

            if (unstoppableForce){
                UFSpeed.text = "UF Speed " + unstoppableForce.GetSpeed();	
                distancetext.text = "Distance " + unstoppableForce.GetDistanceAway();	
            }

        }


    }

    void StartDebugCalculations(){

        if (debugText){

        }
        else {
            playerSpeed.text = "";	
            UFSpeed.text = "";	
            distancetext.text = "";	

        }
    }


    void FixedUpdate()
    {

		if (paused && !gameOver) {

			if (Input.GetButton ("Boost") || Input.GetButton ("Drift") || player.mobileBoost || player.mobileDrift) {

                if ((PlayerPrefs.GetInt("TutorialPlayed") == 1)){
                    StartGame(false);
                }
                else{
                    StartGame(true);

                }
				



			}
		}

        else{

			
        }


        if (Input.GetButton ("Restart") || mobileRestart) {

            SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
        }

		else if(gameOver){

            if (Input.GetButtonDown ("Boost") || Input.GetButtonDown ("Drift") ) {
                GameOverInput();
            }

		}

        DebugCalculations();
        
    }

    void StartGame(bool tutorial = false){

        player.boostDisabled = true;
		player.driftDisabled = true;


        if (tutorial || tutorialAlways){

            Tutorial.instance.StartTutorial();


        }
        else {


            StartCoroutine(StartGameSkipTutorialCoroutine());


        }

        paused = false;

        foreach (Transform child in namething.transform)
        {
            child.GetComponent<Animator>().SetBool("startgame", true);
        }

        settingsScreen.SetActive(false);
        homeScreen.SetActive(false);



        namething.transform.SetParent(null, true);

        cameraScript.smoothing = cameraScript.positionSmoothing;

        highestScore = PlayerPrefs.GetInt("highestScore");


    }

    IEnumerator StartGameSkipTutorialCoroutine(){

        yield return new WaitForSeconds(2f);

		player.boostDisabled = false;
		player.driftDisabled = false;

    	GameManager.instance.player.fillMeter.gameObject.SetActive(true);

        yield return new WaitForSeconds(.5f);

        instance.EnableUnstoppableForce(true);
        StartGameSetup();

        yield return null;
    }

    public void StartGameSetup(){

        if(!disablePacer){
            StartGamePacing();
        }


        StartScoring();
    }

    void StartGamePacing(){
        unstoppableForce.StartPacing();
        TrackManager.instance.StartPacing();
    }
    
    public void GameOverInput(){


        if (scoreTitle.enabled){

            if (!instantScoreShow){
                StopCoroutine("ShowScores");
                ShowScoresInstant();        
                instantScoreShow = true;
            }
            else{
                SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
                
            }
        }

    }
    public void UpdateScore(float playerDistance)
    {        

        // Modify the player distance into a better scaled distance
        scaledDistance = Mathf.RoundToInt(playerDistance * ScriptableObjectManager.instance.playerStats.SCORE_RATE) - tutorialDistance;

        if ((distanceScore) < scaledDistance){

            rawScore = rawScore + player.currentMultiplier;
            distanceScore = scaledDistance;
        }	

        // Scale the score and convert into and int

        score = (int)(rawScore * ScriptableObjectManager.instance.playerStats.SCORE_SCALE);

        scoreCounterText.text = score.ToString();

        if (highestScore != 0 && score >= highestScore && !beatenScore){

            HighScoreColours();
            beatenScore = true;

        }
        
    }

    void HighScoreColours(){

        // Highscore colouring

        distanceTextAnimator.enabled = true;
        scoreTitle.color = new Color(251, 255, 0, 255) ;
        highDriftMaterial.color = colourStats.recordDriftTrack;
        //highNormalMaterial.color = new Color32( 241, 135, 0, 108 );
        highNormalMaterial.color = colourStats.recordNormalTrack;

    }

    public void MobileRestartButtonUp(){
        if (paused){

            mobileRestart = true;
        }

    }
    IEnumerator ShowScores(){

        scoreEndText.text = scoreCounterText.text;

        Debug.Log(scoreEndText);

        scoreCounterText.enabled = false;

        while(true){

            yield return null;    
            
            if (Input.GetButton ("Boost") || Input.GetButton ("Drift") || player.mobileBoost || player.mobileDrift){


                // Check if can play ads
                if (AdsScript.instance != null){
                    if (!AdsScript.instance.adPlaying){

                        break;

                    }

                }
                else {
                    break;
                }


            }

        }


        yield return new WaitForSeconds(0.5f);

        scoreTitle.enabled = true;

        if (!scoreEndText.enabled){

            yield return new WaitForSeconds(0.5f);
            scoreEndText.enabled = true;

        }

        yield return new WaitForSeconds(0.5f);

        highscoreTitle.enabled = true;


        yield return new WaitForSeconds(0.5f);

        highscoreText.text = PlayerPrefs.GetInt("highestScore").ToString();
        highscoreText.enabled = true;

        if (PlayerPrefs.GetInt("highestScore") < score){

            PlayerPrefs.SetInt("highestScore", (int) score);

            yield return new WaitForSeconds(0.5f);

            highscoreText.text = PlayerPrefs.GetInt("highestScore").ToString();
        }


        instantScoreShow = true;
        yield return null;
    }

    void ShowScoresInstant(){

        scoreTitle.enabled = true;
        scoreEndText.enabled = true;
        highscoreTitle.enabled = true;

        if (PlayerPrefs.GetInt("highestScore") < score){
            PlayerPrefs.SetInt("highestScore", score);            
        }
        highscoreText.text = PlayerPrefs.GetInt("highestScore").ToString();       
        highscoreText.enabled = true;
    }

	public void GameOver(){

        Time.timeScale = 1f;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;

        StartCoroutine(ShowScores());  

		paused = true;
		gameOver = true;

	}
    public void EnableUnstoppableForce(bool flag){

        if (!disableUnstoppableForce){
            unstoppableForce.SetRunningState(flag);
        }
    }

    public void StartScoring(){

        tutorialDistance = scaledDistance;



        //Debug.Log("distanceTute" + tutorialDistance);
        distanceScore = 0;
        scoreCounterText.enabled = true;
        rawScore = 0;
        //distanceTextAnimator.enabled = true;



    }

    public void OnSettingsPress(){

        CheckPremium();

        cameraScript.SettingsView();

        mobileBoostButton.SetActive(false);
        mobileDriftButton.SetActive(false);


 

        settingsScreen.SetActive(true);

        namething.SetActive(false);
        homeScreen.SetActive(false);


    }

    public void OnHomePress(){


        cameraScript.GameView();

        mobileBoostButton.SetActive(true);
        mobileDriftButton.SetActive(true);

        settingsScreen.SetActive(false);

        homeScreen.SetActive(true);
        namething.SetActive(true);

        errorText.text = "";


    }    

    public void OnAudioButtonPress(){
        audioIsOn = !audioIsOn;

        if (audioIsOn){
            PlayerPrefs.SetInt("AudioIsOff", 0);

        }
        else {
            PlayerPrefs.SetInt("AudioIsOff", 1);

        }


        CheckAudio();


    }

    public void CheckAudio(){


        audioManager = GameObject.Find("AudioManager");

        if (PlayerPrefs.GetInt("AudioIsOff") == 0){
            audioManager.GetComponent<AudioSource>().volume =  1;
            player.GetComponent<AudioSource>().enabled = true;
            audioButtonImage.sprite = audioButtonOn;
            playerAudio.SetActive(true);
            audioIsOn = true;
        }
        else {

            audioManager.GetComponent<AudioSource>().volume = 0;
            player.GetComponent<AudioSource>().enabled = false;

            audioButtonImage.sprite = audioButtonOff;
            playerAudio.SetActive(false);

            audioIsOn = false;

        }

    }

    public void OnVibrateButtonPress(){

        vibrateIsOn = !vibrateIsOn;

        if (vibrateIsOn){
            PlayerPrefs.SetInt("VibrationIsOff", 0);
            Handheld.Vibrate();

        }
        else {
            PlayerPrefs.SetInt("VibrationIsOff", 1);

        }

        CheckVibrate();


    }
    public void CheckVibrate(){

        if (PlayerPrefs.GetInt("VibrationIsOff") == 1){

            vibrateButtonImage.sprite = vibrateButtonOff;
            vibrateIsOn = false;
        }
        else {
            vibrateButtonImage.sprite = vibrateButtonOn;
            vibrateIsOn = true; 




        }

    }  

 


    public void OnAdButtonPress(){

        adState = (adState + 1) % 3;

        if (adState == 0){
            PlayerPrefs.SetInt("AdIsOff", 0);

        }
        else if (adState == 1) {
            PlayerPrefs.SetInt("AdIsOff", 1);

        }
        else if (adState == 2) {
            PlayerPrefs.SetInt("AdIsOff", 2);

        }

        CheckAds();



        
    }    
    public void CheckAds(){


        if (PlayerPrefs.GetInt("AdIsOff") == 0){

            adsButtonImage.sprite = adsButtonOn;
            adsButtonImage.color = new Color(221,221,221,225);

            adState = 0;
        }
        else if (PlayerPrefs.GetInt("AdIsOff") == 1) {
            adsButtonImage.sprite = adsButtonOn;
            adsButtonImage.color = Color.red;
            adState = 1;


        }
        else if (PlayerPrefs.GetInt("AdIsOff") == 2) {
            adsButtonImage.sprite = adsButtonOff;
            adsButtonImage.color = new Color(221,221,221,225);
            adState = 2;


        }


    }     
    public void OnResetTutorialButtonPress(){

        OnHomePress();

        //maybe remove for release?
        PlayerPrefs.SetInt("TutorialPlayed", 0);
        StartGame(true);

        
    }   

    

    public void PremiumOn(){


        PlayerPrefs.SetInt("Premium", 1);
        StartCoroutine(DelayedDeactivate(iapButton, 0.1f));
        adButton.SetActive(true);
        adsButtonImage.sprite = adsButtonOn;
        adsButtonImage.color = Color.red;
        adState = 1;
        PlayerPrefs.SetInt("AdIsOff", 1);



        



    }

    IEnumerator DelayedDeactivate(GameObject thing, float seconds = 0){

        yield return new WaitForSeconds(seconds);


        iapButton.SetActive(false);

        yield return null;


    }

    public void CheckPremium(){

        if (PlayerPrefs.GetInt("Premium") == 1){

            iapButton.SetActive(false);

            adButton.SetActive(true);
        }
        else {

            adState = 0;
            PlayerPrefs.SetInt("AdIsOff", 0);
            adButton.SetActive(false);
            iapButton.SetActive(true);


        }

    }

    public void ResetScore(){

        PlayerPrefs.SetInt("highestScore", 0);
    }

    public void ResetPremium(){

        PlayerPrefs.SetInt("Premium", 0);
        CheckPremium();
    }

    public void ShowPurchaseError(string reason){




    }
    


}
