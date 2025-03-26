using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Barrier : MonoBehaviour {

	Player player;

	public float distanceOffset;

	public Animation barrierExplodeAnim;
	public ParticleSystem barrierExplodeParticle;
	public GameObject barrierExplodeImpact;


	public Material whiteBase;
	public Material whiteDetail1;
	public Material whiteDetail2;
	public Material whiteDetail3;
	public Material whiteDetail4;

	public Material whiteDetail5;


	
	public Material redBase;
	public Material redDetail;

	public Material adTexture;

	Renderer plane1renderer;

	Renderer plane2renderer;

	Material[] redMat;
	Material[] whiteMat;

	[HideInInspector]
	public bool adBarrier = true;

	public Transform adViewTransform;

	TrackStatsScriptableObject trackStats;

	// Use this for initialization
	void Start () {
		
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

		plane1renderer = transform.Find("Plane 1").GetComponent<MeshRenderer>();
		plane2renderer = transform.Find("Plane 2").GetComponent<MeshRenderer>();


		redMat = new Material[2];
		redMat[0] = redBase;
		redMat[1] = redDetail;	

		ChooseRandomMaterials();	

		trackStats = ScriptableObjectManager.instance.trackStats;


		if (GameManager.instance.adState == 0 && Random.Range(0, 100) <= trackStats.chanceForAdBarrier){

			adBarrier = true;
		}
		else if(GameManager.instance.adState == 1){

			adBarrier = true;

		}
		else {

			adBarrier = false;

		}


		if (adBarrier){

			whiteMat[1] = adTexture;

			plane1renderer.materials = whiteMat;
			plane2renderer.materials = whiteMat;
		}

	}

	void ChooseRandomMaterials(){



		int i = Random.Range(1,10);

		switch (i){
			case 1:

				whiteMat = new Material[2];
				whiteMat[0] = whiteBase;
				whiteMat[1] = whiteDetail1;

			break;
			case 2:
				whiteMat = new Material[2];
				whiteMat[0] = whiteBase;
				whiteMat[1] = whiteDetail2;			
			break;
			case 3:
				whiteMat = new Material[2];
				whiteMat[0] = whiteBase;
				whiteMat[1] = whiteDetail3;

			break;
			case 4:
				whiteMat = new Material[3];
				whiteMat[0] = whiteBase;
				whiteMat[1] = whiteDetail1;
				whiteMat[2] = whiteDetail2;


			break;
			case 5:
				whiteMat = new Material[3];
				whiteMat[0] = whiteBase;
				whiteMat[1] = whiteDetail1;
				whiteMat[2] = whiteDetail3;				

			break;
			case 6:
				whiteMat = new Material[3];
				whiteMat[0] = whiteBase;
				whiteMat[1] = whiteDetail2;
				whiteMat[2] = whiteDetail3;		

			break;
			case 7:
				whiteMat = new Material[4];
				whiteMat[0] = whiteBase;
				whiteMat[1] = whiteDetail1;
				whiteMat[2] = whiteDetail2;		
				whiteMat[3] = whiteDetail3;		

			break;	
			case 8:

				whiteMat = new Material[2];
				whiteMat[0] = whiteBase;
				whiteMat[1] = whiteDetail4;

			break;
			case 9:
				whiteMat = new Material[2];
				whiteMat[0] = whiteBase;
				whiteMat[1] = whiteDetail5;			
			break;					
			default:
			break;



		}

		plane1renderer.materials = whiteMat;
		plane2renderer.materials = whiteMat;

		

	}

	public void ResetBarrier()
	{
		distanceOffset = 0;
        transform.position = Vector3.zero;
		GetComponent<Collider>().enabled = true;
		barrierExplodeImpact.SetActive(false);	

		ChooseRandomMaterials();



	}


	void OnTriggerEnter(Collider other) {
		if (other.transform.tag == "PlayerCapsule"){

			bool boosted;

			boosted = player.CheckBarrierCollision();

			if (boosted){

				GetComponent<Collider>().enabled = false;

				StartCoroutine(BarrierDestroy());

				if (Tutorial.instance.tutorialRunning){
					Tutorial.instance.BarrierBreak();
				}
			}

			else {

				if (adBarrier){

				AdsScript.instance.ShowAd(1.5f, adViewTransform);		
				}
							
			}



		}

	}

	IEnumerator BarrierDestroy(){

		player.breakAudioSource.PlayOneShot(player.breakAudioClip, 1);


		plane1renderer.materials = redMat;
		plane2renderer.materials = redMat;


		barrierExplodeAnim.Play();
		barrierExplodeParticle.Play();
		Time.timeScale = 0.01f;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;

		barrierExplodeImpact.SetActive(true);

		yield return new WaitForSeconds(0.002f);
		
		Time.timeScale = 1f;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;


		yield return new WaitForSeconds(barrierExplodeAnim.clip.length * 2);

		gameObject.SetActive(false);

		yield return null;
	}

	
}
