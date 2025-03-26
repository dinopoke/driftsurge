using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TrackManager : MonoBehaviour {

    public static TrackManager instance;
    TrackStatsScriptableObject trackStats;


    public ArcSegment arcPrefab;
    public Barrier barrierPrefab;
    public Transform player;

    Vector3 nextArcPos = Vector3.zero;
    Vector3 nextArcHeightOffset = Vector3.zero;
    float nextArcAngle = 0;
    float nextArcUVOffset = 0;
    float textureScale = 12;

    public ArcSegment currentArc;
    private ArcSegment furthestArc;
    public float furthestDistance = 0;
    List<ArcSegment> track;
    Transform trackHolder;
    Vector3 trackYOffset = Vector3.zero;

    Transform barrierHolder;
    List<Barrier> barriers;

    int currentDriftCounter;
    int currentBarrierCounter;

    bool oldFlippedValue = false;




    //float minRadius = 10000f;        //7
    //float maxRadius = 10000f;       //22
	float minAngle = 10f;
	float maxAngle = 10f;

    float meshQuality = 1f;

    bool realigning;

    float currentAngle;

    public bool slowEnd = false;

    float currentRadius;

    float currentBarrierDistance;

    //public Queue<Transform> trackPath;


    // da stats
    float chanceForDriftTrack;
    float minimumNormalLength;
	float maximumNormalLength;
	float minimumDriftRadius;
	float maximumDriftRadius;
	float minimumDriftAngle;
	float maximumDriftAngle;
    int maxNumberOfNormalTracksUntilDrift;
	float chanceForBarrier;
	int maxNumberOfBarriersUntilDrift;

    float spawnDistance;
        


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;


    }

    public void InitializeTrack()
    {

        trackStats = ScriptableObjectManager.instance.trackStats;
        InitializeStats();

        trackHolder = new GameObject("Track").transform;
        barrierHolder = new GameObject("Barriers").transform;

        //trackPath = new Queue<Transform>();

        //Transform startTransform =  new GameObject("startPath").transform;

        //trackPath.Enqueue(startTransform);

        track = new List<ArcSegment>();
        barriers = new List<Barrier>();

        while((nextArcPos.getFlat() - player.position.getFlat()).magnitude < 200)
        {
            AddArc();
        }

        currentArc = track[0];
        //startTransform.position = currentArc.ArcToWorld(new ArcPoint(0, currentArc.radius)).getFlat(); 




    }

    void InitializeStats() {

        chanceForDriftTrack = trackStats.chanceForDriftTrack;
        minimumNormalLength = trackStats.minimumNormalLength;
        maximumNormalLength = trackStats.maximumNormalLength;
        minimumDriftRadius = trackStats.minimumDriftRadius;
        maximumDriftRadius = trackStats.maximumDriftRadius;
        minimumDriftAngle = trackStats.minimumDriftAngle;
        maximumDriftAngle = trackStats.maximumDriftAngle;
        maxNumberOfNormalTracksUntilDrift = trackStats.maxNumberOfNormalTracksUntilDrift ;
        chanceForBarrier = trackStats.chanceForBarrier;
        maxNumberOfBarriersUntilDrift = trackStats.maxNumberOfBarriersUntilDrift;

        currentBarrierCounter = maxNumberOfBarriersUntilDrift;
        
    }

    void InitializePaceStats() {

        chanceForDriftTrack = trackStats.startChanceForDriftTrack;
        minimumNormalLength = trackStats.startMinimumNormalLength;
        maximumNormalLength = trackStats.startMaximumNormalLength;
        minimumDriftRadius = trackStats.startMinimumDriftRadius;
        maximumDriftRadius = trackStats.startMaximumDriftRadius;
        minimumDriftAngle = trackStats.startMinimumDriftAngle;
        maximumDriftAngle = trackStats.startMaximumDriftAngle;
        maxNumberOfNormalTracksUntilDrift = trackStats.startMaxNumberOfNormalTracksUntilDrift ;
        chanceForBarrier = trackStats.startChanceForBarrier;
        maxNumberOfBarriersUntilDrift = trackStats.startMaxNumberOfBarriersUntilDrift;
        
    }

    void FixedUpdate()
    {
        for(int i = 0; i < track.Count; i++)
        {
            if(track[i].distanceOffset + track[i].totalDistance < GameManager.instance.player.GetDistance() - 20)
            {

                track[i].transform.SetParent(null);
                track[i].ResetArc();
                track[i].gameObject.SetActive(false);
                //trackPath.Dequeue();
            }
        }

        for(int i = 0; i < barriers.Count; i++)
        {
            if(barriers[i].distanceOffset + track[i].totalDistance <GameManager.instance.player.GetDistance() - 20)
            {

                barriers[i].transform.SetParent(null);
                barriers[i].ResetBarrier();
                barriers[i].gameObject.SetActive(false);

            }
        }        

        if(Tutorial.instance.tutorialRunning){
            spawnDistance = 140; // How far to spawn new tracks
        }
        else{
            spawnDistance = 500;
        }

        if((nextArcPos.getFlat() - player.position).magnitude < spawnDistance)
        {
            AddArc();
        }
    }


    void AddArc()
    {
        ArcSegment newSegment = GetPooledArc();


        if (Tutorial.instance.tutorialRunning){


            // REWRITE THIS LATER
            switch (Tutorial.instance.currentTutorialPhase) {

            // Tutorial Starts Here
            case Tutorial.TutorialPhase.start:
                // making a long boi
                newSegment.driftArc = false;

            break;
            case Tutorial.TutorialPhase.boost:              
                // making a long boi
            newSegment.driftArc = false;

            break;           
            case Tutorial.TutorialPhase.drift:              
                // drift boi time
                newSegment.driftArc = true;

            break;
            case Tutorial.TutorialPhase.barrier:
                newSegment.driftArc = false;
                SpawnTutoralBarrier();
                Tutorial.instance.EndBarrierPhase();

            break;
            case Tutorial.TutorialPhase.end:
                // not sure if needed?                
            break;

            default:
            break;

            }

        }
        else if(GameManager.instance.player.boostDisabled){ // Don't spawn in any when you can't boost

            newSegment.driftArc = false;

        }
        else{



            if (GameManager.instance.paused){

                newSegment.driftArc = false;
            }
            // NORMAL TRACK CODE

            else if (Random.value > chanceForDriftTrack / 100f && currentDriftCounter < maxNumberOfNormalTracksUntilDrift){

                newSegment.driftArc = false;

                if (Random.value < chanceForBarrier / 100f && currentBarrierCounter < maxNumberOfBarriersUntilDrift && furthestDistance  - currentBarrierDistance > trackStats.minimumDistanceBetweenBarriers && (currentAngle >  trackStats.minimumAngleBeforeBarrier || currentRadius > trackStats.minimumRadiusBeforeBarrier)){

                    SpawnBarrier();

                }
                else {

                    currentDriftCounter ++;

                }

            }
            else {
                // Set next track to be a drift
                newSegment.driftArc = true;

                currentBarrierCounter = 0;
                currentDriftCounter = 0;




            }

        }

		// Hardcoding straight and curved tracks

		if (newSegment.driftArc == true) {
			
			newSegment.isStraight = false;

		
		} else {
			
			newSegment.isStraight = false;

		}



        newSegment.transform.position = nextArcPos;
        newSegment.startAngle = nextArcAngle;
        newSegment.width = trackStats.trackWidth;
        newSegment.slope = trackStats.slope;

        float randomRadius;
        float randomAngle;

        if(newSegment.driftArc == false){
            randomRadius = Random.Range(minimumNormalLength, maximumNormalLength);
            randomAngle = Random.Range(minAngle, maxAngle);
        }
        else {
			randomRadius = Random.Range(minimumDriftRadius, maximumDriftRadius);
			randomAngle = Random.Range(minimumDriftAngle, maximumDriftAngle);

        }

        if (Tutorial.instance.tutorialRunning){
            if (Tutorial.instance.currentTutorialPhase == Tutorial.TutorialPhase.start){
                randomRadius  = maximumNormalLength;
                randomAngle = maxAngle;
            }
            else if (Tutorial.instance.currentTutorialPhase == Tutorial.TutorialPhase.drift){
                randomRadius  = maximumDriftRadius;
                randomAngle = trackStats.maximumDriftAngle;

            }

        }

        newSegment.radius = randomRadius;
        newSegment.angle = randomAngle;



        if(Random.value > 0.5)
        {
            newSegment.flipped = true;
        }
        else
        {
            newSegment.flipped = false;
        }

        // Trying to make go straight here

        if (newSegment.flipped){
            currentAngle = newSegment.startAngle;
        }
        else {
            currentAngle = - newSegment.startAngle;
        }

            //Debug.Log(currentAngle);

        if (Mathf.Abs(currentAngle) > 70 && Mathf.Abs(currentAngle) < 250  && !realigning) {
            //Debug.Log("realining started");            

             realigning = true;
            
        }
        else if (Mathf.Abs(currentAngle) <= 70 || Mathf.Abs(currentAngle) >= 250){
            //Debug.Log("realining done");            

            realigning = false;
        }

        if (!realigning){
            oldFlippedValue = newSegment.flipped;
            //Debug.Log("not realigbn");            
        }
        else{
            newSegment.flipped = !oldFlippedValue;
            //Debug.Log("realinging");
        }


        currentRadius = newSegment.radius;

        // End trying

        newSegment.Initialize(); // calculates the total length of the arc and identifies it's end position and center of rotation

        newSegment.distanceOffset = furthestDistance;
        newSegment.uvOffset = nextArcUVOffset;
        newSegment.textureScale = textureScale;

        furthestDistance += newSegment.totalDistance;
        nextArcHeightOffset.Set(0, furthestDistance * trackStats.slope, 0);
        nextArcAngle = newSegment.endAngle;
        nextArcPos = newSegment.endPos;
        nextArcUVOffset += newSegment.totalDistance / textureScale - Mathf.Floor(newSegment.totalDistance / textureScale);
        if(nextArcUVOffset >= 1)
        {
            nextArcUVOffset = nextArcUVOffset - 1;
        }

        if (furthestArc != null)
        {
            furthestArc.nextArc = newSegment; // each arc contains a reference to the next arc in the track
        }
        furthestArc = newSegment;

        newSegment.arcDivisions = Mathf.RoundToInt(newSegment.totalDistance / meshQuality); // adjust mesh divisions based on the length of the arc
        newSegment.GenerateMesh();

        track.Add(newSegment);
        trackHolder.position = Vector3.zero;
        newSegment.transform.SetParent(trackHolder);
        trackHolder.position = trackYOffset;

        newSegment.gameObject.SetActive(true);

        Transform newSegmentPath =  new GameObject("Segment Path").transform;
        newSegmentPath.position = newSegment.endPos;
        newSegmentPath.SetParent(newSegment.transform);
        //trackPath.Enqueue(newSegmentPath);


    }


    ArcSegment GetPooledArc()
    {
        for (int i = 0; i < track.Count; i++)
        {
            if (!track[i].gameObject.activeInHierarchy)
            {
                return track[i];
            }
        }

        // if none is found create a new object only if the pool has not exceeded its hard cap

        ArcSegment newSegment = Instantiate(arcPrefab) as ArcSegment;
        return newSegment;
    }

    Barrier GetPooledBarrier(){

        for (int i = 0; i < barriers.Count; i++)
        {
            if (!barriers[i].gameObject.activeInHierarchy)
            {
                return barriers[i];
            }
        }
        Barrier newBarrier = Instantiate(barrierPrefab) as Barrier;
        return newBarrier;
    }

    public void OffsetHeight(float offset) // the entire track actually moves up in the Y axis, instead of the player descending.  Keeps player from straying ever further from origin.
    {
        trackYOffset.Set(0, -offset, 0);
        trackHolder.position = trackYOffset;
    }

    public void IncrementArc()
    {
        ArcSegment nextArc = currentArc.nextArc;
        currentArc = nextArc;
    }

    IEnumerator BarrierSlowdownCoroutine(float barrierDistance){

        // Time slowdown for the tutorial
        bool slowdown = false;


        // Might need to make this a bit more elegant later
        while (Tutorial.instance.currentTutorialPhase == Tutorial.TutorialPhase.barrier || Tutorial.instance.currentTutorialPhase == Tutorial.TutorialPhase.barrierBreak){

            if (GameManager.instance.player.GetDistance() > barrierDistance - 100 && !slowdown){
                if (player.GetComponent<Player>().currentBoostMeter > 0){   
                    Tutorial.instance.CheckBoostPrompt();

                    yield return new WaitForSeconds(0.2f);
                    slowdown = true;

                    GameManager.instance.boostButtonPrompt.GetComponent<Animator>().speed = 1;
                }
                
            }

            if (slowdown){

                Time.timeScale = Mathf.InverseLerp(0, 300, barrierDistance - GameManager.instance.player.GetDistance());

                Time.fixedDeltaTime = 0.02f * Time.timeScale;

                    if (Time.timeScale < 0.01f){
                        slowdown = false;
                    }


            }

            if (slowEnd) {
                Time.timeScale = 1f;
                //Time.fixedDeltaTime = 0.02f * Time.timeScale;       
                break;                
            }



            yield return null;


        }


        yield return null;

    }

    public void StartPacing(){
        StartCoroutine(StartPacingCoroutine());
    }

    IEnumerator StartPacingCoroutine(){

        InitializePaceStats(); 
        
        float timer = 0;

        while (timer <= trackStats.paceRampUpTime){
            chanceForDriftTrack = GamePacer.instance.Pace(chanceForDriftTrack,trackStats.chanceForDriftTrack, trackStats.startChanceForDriftTrack);
            minimumNormalLength = GamePacer.instance.Pace(minimumNormalLength , trackStats.minimumNormalLength, trackStats.startMinimumNormalLength);
            maximumNormalLength = GamePacer.instance.Pace(maximumNormalLength, trackStats.maximumNormalLength, trackStats.startMaximumNormalLength);
            minimumDriftRadius = GamePacer.instance.Pace(minimumDriftRadius,  trackStats.minimumDriftRadius,  trackStats.startMinimumDriftRadius);
            maximumDriftRadius = GamePacer.instance.Pace(maximumDriftRadius, trackStats.maximumDriftRadius, trackStats.startMaximumDriftRadius);
            minimumDriftAngle = GamePacer.instance.Pace(minimumDriftAngle, trackStats.minimumDriftAngle, trackStats.startMinimumDriftAngle);
            maximumDriftAngle = GamePacer.instance.Pace(maximumDriftAngle, trackStats.maximumDriftAngle, trackStats.startMaximumDriftAngle);
            maxNumberOfNormalTracksUntilDrift = (int) GamePacer.instance.Pace((float)maxNumberOfNormalTracksUntilDrift,(float) trackStats.maxNumberOfNormalTracksUntilDrift, (float) trackStats.startMaxNumberOfNormalTracksUntilDrift);
            chanceForBarrier = GamePacer.instance.Pace(chanceForBarrier, trackStats.chanceForBarrier,  trackStats.startChanceForBarrier);
            maxNumberOfBarriersUntilDrift =  (int) GamePacer.instance.Pace( (float) maxNumberOfBarriersUntilDrift,(float)trackStats.maxNumberOfBarriersUntilDrift,(float)trackStats.startMaxNumberOfBarriersUntilDrift);

            timer += Time.deltaTime;

            yield return null;
        }


        chanceForDriftTrack = trackStats.chanceForDriftTrack;
        minimumNormalLength = trackStats.minimumNormalLength;
        maximumNormalLength = trackStats.maximumNormalLength;
        minimumDriftRadius = trackStats.minimumDriftRadius;
        maximumDriftRadius = trackStats.maximumDriftRadius;
        minimumDriftAngle = trackStats.minimumDriftAngle;
        maximumDriftAngle = trackStats.maximumDriftAngle;
        maxNumberOfNormalTracksUntilDrift = trackStats.maxNumberOfNormalTracksUntilDrift ;
        chanceForBarrier = trackStats.chanceForBarrier;
        maxNumberOfBarriersUntilDrift = trackStats.maxNumberOfBarriersUntilDrift;



        yield return null;
    }

    void SpawnBarrier(){
        
        // Spawn a barrier
        Barrier newBarrier = GetPooledBarrier();
        newBarrier.transform.position = nextArcPos;
        // makey rotaty
        newBarrier.transform.rotation = Quaternion.AngleAxis(-nextArcAngle, Vector3.up);
        
        newBarrier.distanceOffset = furthestDistance;
        newBarrier.transform.SetParent(barrierHolder);

        currentBarrierDistance = furthestDistance;
        currentBarrierCounter ++;

    }

    void SpawnTutoralBarrier(){



        Barrier newBarrier = GetPooledBarrier();
        newBarrier.transform.position = nextArcPos;
        // makey rotaty
        newBarrier.transform.rotation = Quaternion.AngleAxis(-nextArcAngle, Vector3.up);
        
        newBarrier.distanceOffset = furthestDistance;
        newBarrier.transform.SetParent(barrierHolder);

        StartCoroutine(BarrierSlowdownCoroutine(newBarrier.distanceOffset));





    }



}
