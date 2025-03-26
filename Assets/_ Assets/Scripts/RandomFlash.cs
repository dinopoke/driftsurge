using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFlash : MonoBehaviour {

	MeshRenderer meshRenderer;

	Player playerScript;

	float moveSpeed;

	// Use this for initialization
	void Start () {

		playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.enabled = false;

		StartCoroutine(randomEnable());
		StartCoroutine(Movement());
	}

	IEnumerator randomEnable(){

		while (true){

		yield return new WaitForSeconds(Random.Range(0, 20f));

		meshRenderer.enabled = true;

		yield return new WaitForSeconds(Random.Range(0.5f, 2f));

		meshRenderer.enabled = false;

		yield return new WaitForSeconds(Random.Range(30, 60f));


		yield return null;

		}



	}
	
	IEnumerator Movement(){

		while (true){

			moveSpeed = playerScript.speed;

			transform.localPosition = transform.localPosition +  new Vector3(0, 0, (moveSpeed * Time.deltaTime));

			if (transform.localPosition.z >= 21.5f){

				transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -2f);

			}

			yield return null;

		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
