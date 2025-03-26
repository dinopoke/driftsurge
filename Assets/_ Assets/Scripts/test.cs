using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    public GameObject player;

    private float floaty = 10;

    // Use this for initialization
    void Awake ()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
        //_goHere = player.GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetFloat("_HighlightZ", floaty);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }
}
