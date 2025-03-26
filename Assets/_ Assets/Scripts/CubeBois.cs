using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBois : MonoBehaviour {
	
	// Use this for initialization

	public float minStartTime = 0;
	public float maxStartTime = 5f;



	public float minSpawnTime = 1f;
	public float maxSpawnTime = 10f;	

	void Start () {

		gameObject.GetComponent<MeshRenderer>().enabled = false;
		gameObject.GetComponent<Animator>().enabled = false;
		gameObject.GetComponent<TrailRenderer>().enabled = false;
		

		StartCoroutine(CubeSpawning());
	}



	IEnumerator CubeSpawning(){

		while (GameManager.instance.paused){

			yield return null;
		}

		yield return new WaitForSeconds(Random.Range(minStartTime, maxStartTime));

		while (true){
			gameObject.GetComponent<Animator>().enabled = true;
			gameObject.GetComponent<MeshRenderer>().enabled = true;
			gameObject.GetComponent<TrailRenderer>().enabled = true;
			
			yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length );
			gameObject.GetComponent<Animator>().enabled = false;
			gameObject.GetComponent<MeshRenderer>().enabled = false;
			gameObject.GetComponent<TrailRenderer>().enabled = false;

			yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

		}
	} 
	


}
