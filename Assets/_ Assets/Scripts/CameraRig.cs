using UnityEngine;
using System.Collections;

public class CameraRig : MonoBehaviour {

    Transform player;

    Player playerScript;
    Transform capsule;


    Vector3 offset;

    public float startCameraSpeed = 2f;

    [HideInInspector]
    public float smoothing = 2;

    public float positionSmoothing = 13f;

    public bool useRotationSmoothing;

    public float rotationSmoothing = 5f;

    public bool useRotationSmoothingOnlyOnDrift;

    public float nonDynamicRotationSmoothing = 10f;

    public enum CameraState {Game, Settings, Ad}

    public CameraState currentCameraState;

    public Transform settingsViewTransform;

    Transform lookTarget;




    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<Player>();
        capsule = GameObject.Find("Capsule").transform;

        currentCameraState = CameraState.Game;

    }

    void FixedUpdate()
    {


        switch (currentCameraState){
            
            case CameraState.Game:
                transform.position = Vector3.Slerp(transform.position, player.position, smoothing * Time.deltaTime);
                //Debug.Log(capsule.rotation.);

                if (useRotationSmoothing){

                    if (useRotationSmoothingOnlyOnDrift){
                        
                        if (playerScript.currentArc.driftArc){
                            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,  capsule.eulerAngles.y ,capsule.eulerAngles.z), rotationSmoothing * Time.deltaTime);                
                        }
                        else {
                            transform.rotation = Quaternion.Lerp(transform.rotation, player.rotation , nonDynamicRotationSmoothing * Time.deltaTime);

                        }

                    }
                    else {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,  capsule.eulerAngles.y ,capsule.eulerAngles.z), rotationSmoothing * Time.deltaTime);                
                    }

                }
                else {

                    transform.rotation = player.rotation;

                }
                break;

            case CameraState.Settings:
                transform.position = Vector3.Slerp(transform.position, settingsViewTransform.position, smoothing * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, settingsViewTransform.rotation , smoothing * Time.deltaTime);

            break;
            case CameraState.Ad:

                transform.position = Vector3.Slerp(transform.position, lookTarget.position, 3f * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookTarget.rotation , 3f * Time.deltaTime);

            break;

        }





    }

    public void SettingsView(){

        currentCameraState = CameraState.Settings;

    }

    public void GameView(){

        currentCameraState = CameraState.Game;
        
    }

    public void AdView(Transform ad){

        lookTarget = ad;
        currentCameraState = CameraState.Ad;



    }
}
