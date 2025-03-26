using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class Player : MonoBehaviour
{

    PlayerStatsScriptableObject playerStats;

    public ArcSegment currentArc;
    Vector3 movement;
    Rigidbody rb;
    Quaternion steering;
    Vector3 velocity = Vector3.zero;
    Vector3 gravity = Vector3.zero;
    float fallSpeed = 0;

    float distance = 0;
    public float groundHeight;
    public bool onGround = true;

    float turnGoal;
    float turnRate = 0;
    public float maxTurnRate;
    public float turnAcceleration;

    public float currentMaxSpeed;
    public float acceleration;  


    public float speed = 0;

    public enum Movestate {normal, boost, drift, noBoost, invulnerable} ;

    public Movestate currentMovestate;

    Transform track;

    float h;
    float pedal;

    public ParticleSystem particleMaxSpeed;
    ParticleSystem particleDriftSparksRight;
    ParticleSystem particleDriftSparksLeft;
    ParticleSystem particleDeath;




    //[HideInInspector]
    public float currentBoostMeter;

    public Image fillMeter; 
    public Image fillMeterBackground; 


    float currentHeat;

    Material playerMaterial;
    public Transform playerModel;


    public bool mobileBoost = false;
    public bool mobileDrift= false;

	bool boostReleaseDelay = false;
	float boostReleaseDelayTimer = 0;


	bool boostLeeway = false;

    float boostLeewayTimer = 0;

	float angleChange;

    bool trackChange;

    Color startColour;

    TrailRenderer playerTrail;

    public float cameraBoostInRate = 1f;

    public float cameraBoostOutRate = 1f;

    public float cameraDriftInRate = 1f;

    public float cameraDriftOutRate = 1f;

    public float normalFov = 60f;
    public float driftFov = 90f;

    public float boostFov = 30f;


    Camera mainCamera;


    SimpleRoughShake shakeScript;

    
    Vector3 originalCameraPosition;

    [HideInInspector]
    public float shakeAmount;


    [Header("Drifting on Normal Track")]
    public float initialShakeAmount = 1;

    public float finalShakeAmount = 0.3f;
    public float shakeChangeSpeed = 1;

    [Header("Drifting on Drift Track")]

    public float driftInitialShakeAmount = 0;
    public float driftFinalShakeAmount = 0.3f;
    public float driftShakeChangeSpeed = 1;

    [Header("Boost Colours")]

    public Color32 defaultMeter = new Color32(45, 240, 0, 255);
    public Color32 boostingMeter = new Color32(131, 206, 114, 255);

    public Color32 sweetspotMeter = new Color32(131, 206, 114, 255);

   [Header("Sounds")]

   public AudioSource driftAudioSource;

   float currentBoostTime;

   float curveLength;

   public AudioSource goodDriftAudioSource;
   public AudioSource boostAudioSource;

   public AudioSource breakAudioSource;
   public AudioClip breakAudioClip;

   public AudioSource boomAudioSource;

   public AudioClip boomAudioClip;

    public bool boostDisabled = true;
    public bool driftDisabled = true;


	public int currentMultiplier = 1;

	private bool multiplierTimerIncrease;

	private bool multiplierTimerDecrease;

	private float multiplierTimer;
	private float multiplierTimerD;


	public Text multiplierUIText;
	public Animator multiplierUIAnimator;

	public Text multiplierShadowUIText;
	public Animator multiplierShadowUIAnimator;


	public UnstoppableForce uForceScript;

    bool droppingMultipler;

    bool wasBoosting;
    bool boostStarted;

	public void Initialize()
    {


        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        shakeScript = GameObject.Find("Main Camera").GetComponent<SimpleRoughShake>();

        track = GameObject.Find("Track").transform;
        currentArc = TrackManager.instance.currentArc;

        currentMovestate = Movestate.normal;

        currentBoostMeter = playerStats.startingBoost;
        currentHeat = 0;

        // Getting child material
        playerMaterial = transform.Find("Capsule").GetComponent<MeshRenderer>().material;
        playerTrail = transform.Find("Capsule").GetComponent<TrailRenderer>();


        startColour = playerMaterial.color;

        playerModel = transform.Find("Capsule");

        particleDriftSparksRight = transform.Find("ps_Sparks_Right").GetComponent<ParticleSystem>();
        particleDriftSparksLeft= transform.Find("ps_Sparks_Left").GetComponent<ParticleSystem>();

        particleDeath= transform.Find("ps_PlayerDeath").GetComponent<ParticleSystem>();




        mobileBoost = false;
        mobileDrift = false;

		angleChange = 0;


        originalCameraPosition = mainCamera.transform.localPosition;

        currentBoostTime = 0;

        curveLength = playerStats.boostUseCurve.keys[playerStats.boostUseCurve.keys.Length - 1].time;

		currentMultiplier = 1;
		multiplierUIText.text = "";

        multiplierShadowUIText.text = "";


    }

    void Start(){

        playerStats = ScriptableObjectManager.instance.playerStats;
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.paused) {
            
 

            if (currentMovestate == Movestate.boost){
                
                pedal = 1; 

                // change boost ammount


                if (currentBoostTime <  curveLength){

                    //currentBoostMeter -= playerStats.boostUseCurve.Evaluate(currentBoostTime);

                    currentBoostTime += Time.deltaTime;

                }

                else {

                    //currentBoostMeter -= playerStats.boostUseCurve.Evaluate(curveLength);

                }

                

                currentBoostMeter -= playerStats.boostUseSpeed * Time.deltaTime;  


                currentMaxSpeed = playerStats.boostMaxSpeed;
                acceleration = playerStats.boostAcceleration; 
            }

            else if (currentMovestate == Movestate.noBoost){


                if (playerStats.accelerateWhenNoBoost){

                    currentMaxSpeed = playerStats.noBoostMaxSpeed;
                    acceleration = playerStats.noBoostAcceleration;
                    pedal = 1;
                }
                else {
					currentMaxSpeed = playerStats.normalMaxSpeed;
					acceleration = playerStats.normalAcceleration ;  
                    pedal = 1;
                }      

 

            }
            else if (currentMovestate == Movestate.drift){

                pedal = -1; 

                // You are drifting on a drift track



                if (currentArc.driftArc){
                    currentMaxSpeed = playerStats.normalMaxSpeed;
                    acceleration = playerStats.driftDeccelerationOnDriftTrack; 

                    if (!goodDriftAudioSource.isPlaying){

                        goodDriftAudioSource.volume = 1;

                        goodDriftAudioSource.Play();

                    }


                    DriftTrackLightUp();
                    DriftTrackJuice();                     

                }  
                else {
                    currentMaxSpeed = playerStats.normalMaxSpeed;
                    acceleration = playerStats.driftDecceleration;  

                    CheckDropMultiplier();

                    StartCoroutine(SoundDecay(goodDriftAudioSource, 2f));

                    NoDriftTrackJuice();                     


                }   
                if (currentArc.driftArc && currentBoostMeter < playerStats.MAX_BOOST_METER){
                    currentBoostMeter += playerStats.boostFillSpeed * Time.deltaTime;  

                    if (Tutorial.instance.tutorialRunning){
                        Tutorial.instance.CheckBoostFillMeter();
                    }

                }	

                else{

                }				       
					
            }
            else {
                currentMaxSpeed = playerStats.normalMaxSpeed;
                acceleration = playerStats.normalAcceleration ;  

                if (playerStats.accelerateNormally){

                    if (speed > 0){
                        pedal = 1;    
                    }
                    else {

                        pedal = 1;
                    }
        

                }
                else{

                }        

                         
     
            }


            if (Tutorial.instance.tutorialRunning){
                // Tutorial drift prompt when player touches a drift arc
                if (Tutorial.instance.currentTutorialPhase == Tutorial.TutorialPhase.drift &&  currentArc.driftArc){
                    Tutorial.instance.CheckDriftPrompt();

                }
            }

            Move(h, pedal);



            // Turn off movement

            if (playerStats.UseOverheatMechanic){

                CheckOverheat();
            }
				

            CheckBounds();
            UpdateDistance();

            fillMeter.fillAmount = currentBoostMeter / playerStats.MAX_BOOST_METER;


            // Run Boost Release Delay
			if (boostReleaseDelay) {
				boostReleaseDelayTimer -= Time.deltaTime;
				if (boostReleaseDelayTimer <= 0 || currentBoostMeter <= -playerStats.boostMeterOverflowAmount ) {
					boostReleaseDelay = false;
                    BoostLeeWayStart();
				}
			
			}

            // Run Boost Leeway
			if (boostLeeway) {

				boostLeewayTimer -= Time.deltaTime;

                InvulnJuice();

				if (boostLeewayTimer <= 0) {
                    //Debug.Log("leeway end");                   				
					boostLeeway = false;

				}
                else{

                }
			
			}


            JuiceLoop();


            if (Tutorial.instance.tutorialRunning == false){

                // Scoring
                CheckIncreaseMultiplier();
                CheckBreakMultiplier();
            }
        }
    }

    void Update(){

        // Do input in Update Only?

         if (!GameManager.instance.paused) {       
            #if UNITY_STANDALONE || UNITY_WEBPLAYER
            h = Input.GetAxisRaw("Horizontal");
            #endif

            CheckInput();

         }  

    }

    void CheckInput(){

		if (Input.GetButtonUp("Boost")){
			BoostUpPress();

		}

		if (Input.GetButtonDown("Boost")){
			BoostDownPress();


		}    
        if (Input.GetButtonDown("Drift")){

            DriftDownPress();


		}   
        if (Input.GetButtonUp("Drift")){
            DriftUpPress();
		}       


        if (Input.GetButton("Boost") || mobileBoost){

            if (!boostDisabled && boostStarted){

                // BOOST
                if (currentBoostMeter > - playerStats.boostMeterOverflowAmount){
                    wasBoosting = true;
                    currentMovestate = Movestate.boost;   
                    BoostJuice();
                }
                else{

                    BoostUpPress();
                    
                    NoJuice();

                    if (boostLeeway){
                        currentMovestate = Movestate.invulnerable;
                    }
                    else{
                        //Debug.Log(wasBoosting);
                        if (wasBoosting){
                            BoostLeeWayStart();

                        }
                        currentMovestate = Movestate.noBoost;   
                    }

                }
    
                if(Tutorial.instance.tutorialRunning){
                    Tutorial.instance.CheckTutorialBoosting();
                }
            }

        }
        // DRIFT
        else if (Input.GetButton("Drift") || mobileDrift){

            if (!driftDisabled){        
                currentMovestate = Movestate.drift;
                DriftJuice();

                if(Tutorial.instance.tutorialRunning){
                    Tutorial.instance.CheckTutorialDrifting();
                }                
            }

        }
        else {
        // NORMAL
			if (boostReleaseDelay == false) {
				currentMovestate = Movestate.normal; 
                currentBoostTime = 0;
                NormalJuice();

            } 
            if (boostLeeway == true){
				currentMovestate = Movestate.invulnerable; 
            }
        }



    }

    void BoostDownPress(){

        if(!boostDisabled){
            BoostDownJuice();
            boostStarted = true;
        }

        if (GameManager.instance.gameOver){
            GameManager.instance.GameOverInput();
        }

    }
     void BoostUpPress(){

        if(!boostDisabled){
            
            boostStarted = false;

            BoostReleaseDelayStart();

        }
    } 

    void DriftDownPress(){

        if(!driftDisabled){
            DriftDownJuice();
            }
        if (GameManager.instance.gameOver){
            GameManager.instance.GameOverInput();
        }
    }

    void DriftUpPress(){

        droppingMultipler = false;

        DriftUpJuice();
    }

    void Move(float h, float v)
    {
        turnGoal = h * maxTurnRate;
        if(Mathf.Abs(turnGoal) < 0.2f)
        {
            turnGoal = 0;
        }

        if (onGround)
        {
            if (turnRate < turnGoal)
            {
                turnRate += turnAcceleration;
            }
            else if (turnRate > turnGoal)
            {
                turnRate -= turnAcceleration;
            }
            if (turnGoal == 0)
            {
                turnRate /= 1.5f;
            }

            turnRate = Mathf.Clamp(turnRate, -maxTurnRate, maxTurnRate);

            steering = Quaternion.AngleAxis(turnRate, Vector3.up);
        }

        if(v > 0)
        {
            if (speed <= currentMaxSpeed)
            {
                speed += v * acceleration;
            }
            else {

                speed = currentMaxSpeed;

            }
        } 

        else if (v == 0){
            if (speed > playerStats.driftMinSpeed)
            {
                speed -= playerStats.normalDecceleration;
                if(Mathf.Abs(speed) < playerStats.driftMinSpeed + 0.05f)
                {
                    speed = playerStats.driftMinSpeed;
                }
            }

        }

        else {
            if (speed > playerStats.driftMinSpeed)
            {
                speed += v * acceleration;
                if(Mathf.Abs(speed) < playerStats.driftMinSpeed + 0.05f)
                {
                    speed = playerStats.driftMinSpeed;
                }
            }
            else {
                    speed = playerStats.driftMinSpeed;
                
            }
        }
        
        velocity = (transform.rotation * Vector3.forward).normalized * speed;

        if (!onGround)
        {
            fallSpeed -= 0.04f;
            gravity.Set(0, fallSpeed, 0);
            velocity = velocity + gravity;
            transform.rotation = transform.rotation * Quaternion.AngleAxis(0.6f, Vector3.right);
        }

		// Sets player rotation

        transform.rotation = Quaternion.AngleAxis(currentArc.flipped ? currentArc.startAngle + angleChange : -currentArc.startAngle - angleChange, Vector3.up);

		if (currentMovestate == Movestate.drift){

            //playerModel.rotation = Quaternion.AngleAxis(currentArc.flipped ? 45 : -45, Vector3.forward);
            playerModel.localRotation = Quaternion.Euler(90, 0, currentArc.flipped ? - 45 : 45);

            


        }
        else{
            playerModel.localRotation = Quaternion.Euler(90, 0, 0);
 

        }

        // Movement code is now in the check bounds method

		//transform.position = transform.position + velocity * Time.deltaTime;
		//transform.rotation = transform.rotation * steering;


    }

    void CheckOverheat(){
        if (speed > playerStats.heatUpSpeedThreshold) {

            if (currentHeat < playerStats.MAX_HEAT){
                currentHeat += playerStats.heatRate * Time.deltaTime;
            }
            else {

            if (playerStats.deathOnOverHeat){
                GameManager.instance.GameOver();

            }
            // Gameover



            }
        }
        else{

            if (currentHeat > 0){
                currentHeat -= playerStats.coolRate * Time.deltaTime;

            }
        }


        playerMaterial.color = Color.Lerp(startColour, Color.white, currentHeat / playerStats.MAX_HEAT);
    }


    void CheckBounds()
    {



        if (currentArc != null) {


            float angularVelocity = (speed / currentArc.radius) * 100 * Time.deltaTime;



            angleChange += angularVelocity;
        }

        if (currentArc.ContainsPoint(transform.position, 0.05f))
        {
            //onGround = true;
        }
        else
        {



            if (currentArc.nextArc != null && currentArc.nextArc.ContainsPoint(transform.position, 0.01f))
            {

                float oldAngleChange;
                oldAngleChange = (angleChange - currentArc.angle); 

                float newSpeed;
                newSpeed = oldAngleChange * currentArc.radius;

                TrackManager.instance.IncrementArc();
                currentArc = TrackManager.instance.currentArc;

                angleChange = (newSpeed / currentArc.radius);


            }
            else
            {
                //onGround = false;
            }
        }        


        if (currentArc != null) {
            transform.position = currentArc.ArcToWorld (new ArcPoint (currentArc.startAngle + angleChange, currentArc.radius)).getFlat ();
        }


        
    }

	void BoostReleaseDelayStart(){
		if (currentBoostMeter > - playerStats.boostMeterOverflowAmount) {
			boostReleaseDelayTimer = playerStats.boostReleaseDelayTime;
            boostReleaseDelay = true;
		}
	}

    void BoostLeeWayStart(){

        BoostUpJuice();
		boostLeewayTimer = playerStats.boostLeewayTime;
		boostLeeway = true;
        wasBoosting = false;

        //Debug.Log("start");

    }


    void UpdateDistance()
    {
        if (!onGround)
        {
            return;
        }
        distance = currentArc.GetDistance(currentArc.WorldToArc(transform.position).t);
        groundHeight = distance * ScriptableObjectManager.instance.trackStats.slope;
        TrackManager.instance.OffsetHeight(groundHeight);

		// Multiplier is calculated here!

        if(!Tutorial.instance.tutorialRunning){
            GameManager.instance.UpdateScore(distance);
        }
    }

    // Distance getter
    public float GetDistance(){

        return distance;
    }


    public bool CheckBarrierCollision() {
        if (currentMovestate == Movestate.boost || currentMovestate == Movestate.invulnerable){

            return true;

        }
        else{

            // GameOver
            PlayerCollsionDeath();


			GameManager.instance.GameOver();           
            return false;
        }


    }

    public void MobileBoostButtonDown(bool isDown){
        // Input is also used for starting the game
        mobileBoost = isDown;
        if(isDown){

            BoostDownPress();

        }
        else {
            BoostUpPress();
        }


    }

    public void  MobileDriftButtonDown(bool isDown){
        mobileDrift = isDown;

        if (isDown){
            DriftDownPress();
        }
        else {
            DriftUpPress();
        }
    }


    void BoostDownJuice(){

        if (currentBoostMeter > 0){
            particleMaxSpeed.Play(); 

            playerTrail.startColor = Color.white;

            fillMeter.color = boostingMeter;

            PlaySound(boostAudioSource);

            GameManager.instance.GetComponent<ColourChanger>().InvertColours(true);

        }



    }

    void BoostUpJuice(){

        if (particleMaxSpeed.isPlaying) {

            particleMaxSpeed.Stop();  

        }
        fillMeter.color = defaultMeter;

        StartCoroutine(SoundDecay(boostAudioSource, 6f));

        GameManager.instance.GetComponent<ColourChanger>().InvertColours(false);



    }     
    void BoostJuice(){


        CheckSparksPlaying();

            if (mainCamera.fieldOfView < boostFov){
                mainCamera.fieldOfView += cameraBoostInRate * Time.deltaTime;

            }

        mainCamera.transform.localPosition = originalCameraPosition;
        shakeAmount = 0f;

        playerMaterial.SetColor("_OutlineColor", Color.white);
        playerMaterial.SetFloat("_Outline", 0.0026f);

        fillMeterBackground.enabled = true;

        if (currentBoostMeter <= 0){

            StartCoroutine(SoundDecay(boostAudioSource, 2f));
        }             


    }

    void DriftDownJuice(){


        // Mechanics done here - will need to refactor
        CheckIfDriftOnSweetspot();

            
        PlaySound(driftAudioSource);


        if (currentArc.driftArc){
            shakeAmount = driftInitialShakeAmount;

            // This tops up the boost meter back to 0 if the boostMeterLeeway has be used
            if (currentBoostMeter < 0){

                currentBoostMeter = 0;
            }

            if (currentBoostMeter < playerStats.MAX_BOOST_METER){

                // Only change if we haven't hit a sweet spot

                if (fillMeter.color == defaultMeter){

                    fillMeter.color = boostingMeter;
                }


                
            }

            PlaySound(goodDriftAudioSource);

        }
        else {
            shakeAmount = initialShakeAmount;

            StartCoroutine(SoundDecay(goodDriftAudioSource));


        }


    }

    void DriftUpJuice(){


        StartCoroutine(SoundDecay(driftAudioSource));

        StartCoroutine(SoundDecay(goodDriftAudioSource, 2f));

        fillMeter.color = defaultMeter;


    }

    void DriftJuice(){

        playerTrail.startColor = Color.black;

        if (particleMaxSpeed.isPlaying) {
            particleMaxSpeed.Stop();  

        }


        if (currentArc.flipped ){
            particleDriftSparksRight.Stop();
            
            particleDriftSparksLeft.Play();

        }
        else {
            particleDriftSparksLeft.Stop();

            particleDriftSparksRight.Play();
        }

        if (!currentArc.driftArc){
            if (GameManager.instance.vibrateIsOn){
                Handheld.Vibrate();
                }
        }




    } 

    void NoJuice(){

        if (particleMaxSpeed.isPlaying) {
            particleMaxSpeed.Stop();  

        }

        if (mainCamera.fieldOfView < normalFov - 1f){
            mainCamera.fieldOfView += cameraDriftOutRate * Time.deltaTime;

        }
        else if (mainCamera.fieldOfView >  normalFov + 1f){

            mainCamera.fieldOfView -= cameraBoostOutRate * Time.deltaTime;

        }  

        playerTrail.startColor = Color.grey;      


    }

    void NormalJuice(){

        CheckSparksPlaying();

        if (particleMaxSpeed.isPlaying) {
            particleMaxSpeed.Stop();  

        }
        playerTrail.startColor = Color.grey;      

        if (mainCamera.fieldOfView < normalFov - 1f){
            mainCamera.fieldOfView += cameraDriftOutRate * Time.deltaTime;

        }
        else if (mainCamera.fieldOfView >  normalFov + 1f){

            mainCamera.fieldOfView -= cameraBoostOutRate * Time.deltaTime;

        }   

        else{

            mainCamera.fieldOfView = normalFov;

        }      

        mainCamera.transform.localPosition = originalCameraPosition;
        shakeAmount = 0f;

        playerMaterial.SetColor("_OutlineColor", Color.white);
        playerMaterial.SetFloat("_Outline", 0.0026f);

        fillMeterBackground.enabled = false;


    }           

    void JuiceLoop(){


    }

    void DriftTrackJuice(){

        if (mainCamera.fieldOfView > driftFov){
            mainCamera.fieldOfView -= cameraDriftInRate * Time.deltaTime;

        }



        if (driftFinalShakeAmount > driftInitialShakeAmount){
            if (shakeAmount < driftFinalShakeAmount){
                    shakeAmount += driftShakeChangeSpeed * Time.deltaTime;

            }
        }
        else {

            if (shakeAmount > driftFinalShakeAmount){
                    shakeAmount -= driftShakeChangeSpeed * Time.deltaTime;

            }
        }


        mainCamera.transform.localPosition = (Vector3)(Random.insideUnitCircle * shakeAmount) + new Vector3(originalCameraPosition.x,originalCameraPosition.y,originalCameraPosition.z);

        playerMaterial.SetColor("_OutlineColor", Color.green);
        playerMaterial.SetFloat("_Outline", 0.02f);

        if (!GameManager.instance.beatenScore){
            currentArc.GetComponent<MeshRenderer>().material.color = ScriptableObjectManager.instance.colourStats.defaultDriftGlowTrack;

        }
        else{
            currentArc.GetComponent<MeshRenderer>().material.color = ScriptableObjectManager.instance.colourStats.recordDriftGlowTrack;

        }
        fillMeterBackground.enabled = true;




    }

    void NoDriftTrackJuice(){
                
        if (mainCamera.fieldOfView < normalFov - 1f){
            mainCamera.fieldOfView += cameraDriftOutRate * Time.deltaTime;

        }
        else if (mainCamera.fieldOfView >  normalFov + 1f){

            mainCamera.fieldOfView -= cameraBoostOutRate * Time.deltaTime;

        }   

        else{

            mainCamera.fieldOfView = normalFov;

        }  

        if (finalShakeAmount > initialShakeAmount){
            if (shakeAmount < finalShakeAmount){
                    shakeAmount += shakeChangeSpeed * Time.deltaTime;

            }
        }
        else {

            if (shakeAmount > finalShakeAmount){
                    shakeAmount -= shakeChangeSpeed * Time.deltaTime;

                

            }
        }

        mainCamera.transform.localPosition = (Vector3)(Random.insideUnitCircle * shakeAmount) + new Vector3(originalCameraPosition.x,originalCameraPosition.y,originalCameraPosition.z);


        
        playerMaterial.SetColor("_OutlineColor", Color.white);
        playerMaterial.SetFloat("_Outline", 0.0026f);
        fillMeterBackground.enabled = false;


    }
    void CheckSparksPlaying(){
            if (particleDriftSparksRight.isPlaying){
                particleDriftSparksRight.Stop();
            }            

            if (particleDriftSparksLeft.isPlaying){
                particleDriftSparksLeft.Stop();
            }  
    }    


    public void PlayerCollsionDeath(){

        if (Tutorial.instance.tutorialRunning){
            Tutorial.instance.EndSlowdown();
        }        
        
        if (particleMaxSpeed.isPlaying) {
            particleMaxSpeed.Stop();  

        }
        CheckSparksPlaying();

        boomAudioSource.PlayOneShot(boomAudioClip);

		multiplierUIText.text = "";

        playerModel.gameObject.SetActive(false);
        fillMeter.enabled = false;
        fillMeterBackground.enabled = false;

        particleDeath.Play();
        StartCoroutine(ScreenShake(0.6f, 1.3f));
        StartCoroutine(RevertCamera());
        StartCoroutine(SustainedVibration(0.5f));

    }      

    IEnumerator ScreenShake(float shakeAmount, float shakeMagnitude){
        float shake = shakeMagnitude;
        float decreaseFactor = 1.0f;
        while (shake > 0){
        
            mainCamera.transform.localPosition = (Vector3)(Random.insideUnitCircle * shakeAmount) + new Vector3(originalCameraPosition.x,originalCameraPosition.y,originalCameraPosition.z);
            shake -= Time.deltaTime * decreaseFactor;
            yield return null;
        }
        yield return null;
    }

    IEnumerator RevertCamera(){

        while (mainCamera.fieldOfView != normalFov){

            if (mainCamera.fieldOfView < normalFov - 1f){
                mainCamera.fieldOfView += cameraDriftOutRate * Time.deltaTime;

            }
            else if (mainCamera.fieldOfView >  normalFov + 1f){

                mainCamera.fieldOfView -= cameraBoostOutRate * Time.deltaTime;

            }   

            else{

                mainCamera.fieldOfView = normalFov;

            }
            yield return null;
        }     
        yield return null;
    }

    IEnumerator SustainedVibration(float length){

        if (GameManager.instance.vibrateIsOn) {

        for(float  t = 0 ; t < length ; t += Time.deltaTime ) // Change the end condition (t < 1) if you want
        {
            Handheld.Vibrate();
            yield return new WaitForSeconds(Time.deltaTime);
        }

        }

        yield return null;
    }

    void PlaySound(AudioSource source){

        source.Stop();
        source.volume = 1;
        source.Play();
    }

    IEnumerator SoundDecay(AudioSource source, float decay = 4f){

        while (source.isPlaying){

            source.volume -= decay * Time.deltaTime;



            if (source.volume <= 0){
               source.Stop();
            }

            yield return null;

        }


        yield return null;
    }

    void DriftTrackLightUp(){

  
        // Arc Length = s = r * radians

        float arcLength = currentArc.radius * currentArc.angle * Mathf.Deg2Rad;

        // Lerp from 0 to 10

        float lightValue = Mathf.InverseLerp(0, arcLength,distance)  *  64f;

        currentArc.GetComponent<ArcSegment>().UpdateTransitionEffect(lightValue);


    }

    // NOT WORKING YET
    void CheckIfDriftOnSweetspot(){
                
//        Debug.Log("distance " + distance);
//       Debug.Log("totalDistance " + currentArc.totalDistance);

        if (distance <= playerStats.driftSweetSpotSize){

            if (currentArc.driftArc){
                Debug.Log("SWEEEE");

                StartCoroutine(SweetSpotCoroutine());

            }



        }
        else if ((Mathf.Abs(distance - currentArc.totalDistance) > playerStats.driftSweetSpotSize)){

            //StartCoroutine(SweetSpotCoroutine());
        }

    }

    IEnumerator SweetSpotCoroutine(){

        Color lerpedColor;
        float i = 0;



        lerpedColor = Color.Lerp(sweetspotMeter, boostingMeter, i);

        fillMeter.color = lerpedColor;

        yield return null;
    }

	void CheckIncreaseMultiplier(){
		// speed should be slower than boostSpeed vars
		if (speed >= playerStats.speedToStartMultiplier) {
			
            
			if (multiplierTimerIncrease == false) {
				multiplierTimerIncrease = true;

				multiplierTimer = 0;
			} else {
                multiplierUIAnimator.SetInteger("multiplierState", 0); 
				if (multiplierTimer < playerStats.timeUntilMutliplierIncrease) {	
					multiplierTimer += Time.deltaTime;
				} else {

					IncreaseMultiplier ();
					multiplierTimerIncrease = false;
					multiplierTimer = 0;



				}
			}
		} else {
		
			multiplierTimerIncrease = false;
		
		}

	}
	void CheckBreakMultiplier(){
		
		if (uForceScript.GetDistanceAway() < playerStats.distanceToEndMultiplier) {

			if (multiplierTimerDecrease == false) {

				multiplierTimerDecrease = true;
				multiplierTimerD = 0;
			} else {
				if (multiplierTimerD < playerStats.timeUntilMutliplierBreak) {
                    multiplierUIAnimator.SetInteger("multiplierState", 2); 
					multiplierTimerD += Time.deltaTime;
				} else {

					BreakMultiplier ();

				}
			}
		} 
        else {
            multiplierUIAnimator.SetInteger("multiplierState", 0); 
            CancelBreak ();
        }

        if (multiplierTimerIncrease) {	
            
            CancelBreak ();
        }
	}

	void IncreaseMultiplier(){

		currentMultiplier += 1;
		CancelBreak ();

		// start animation
        StartCoroutine("multiplierStartAnim");
        

	}

	// Hacky
	IEnumerator multiplierStartAnim(){
        multiplierUIAnimator.SetInteger("multiplierState",1);
		yield return new WaitForSeconds (0.1f);
		multiplierUIText.text = currentMultiplier + " x";
		yield return new WaitForSeconds (0.3f);
        multiplierUIAnimator.SetInteger("multiplierState",0);
		yield return null;

	}

	void CancelBreak(){
		multiplierTimerDecrease = false;
		multiplierTimerD = 0;
	}

	void BreakMultiplier(){
		multiplierTimerDecrease = false;
		multiplierTimerIncrease = false;

		multiplierTimerD = 0;

        if (currentMultiplier > 1){
            multiplierShadowUIText.text = currentMultiplier + " x";
            multiplierShadowUIAnimator.SetTrigger("Break");
        }



		currentMultiplier = 1;
		multiplierUIText.text = "";
        multiplierUIAnimator.SetInteger("multiplierState",0);




	}

    void CheckDropMultiplier(){
        if (!droppingMultipler){
            if (currentMultiplier > 2){  // Let's not make it too punishing

                DropMultiplier();
                droppingMultipler = true;
            }
        }
    }

    void DropMultiplier(){
        if (currentMultiplier > 1 ){

            multiplierShadowUIText.text = currentMultiplier + " x";
            multiplierShadowUIAnimator.SetTrigger("Drop");

            currentMultiplier -= 1;
            if (currentMultiplier > 1 ){
                multiplierUIText.text = currentMultiplier + " x";
            }
            else{

                BreakMultiplier();
            }



        } 

    }

    void InvulnJuice(){
        if (GameManager.instance.debugColours){
            playerMaterial.color = Color.blue;
        }
    }
}
