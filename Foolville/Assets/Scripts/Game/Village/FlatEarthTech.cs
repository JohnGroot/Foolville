using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlatEarthTech : MonoBehaviour {


    const float FLATEARTH_CAMFORWARD = 1.15f;
    const float FLATEARTH_CAMDOWN = 0.2f;
    public const float FLATEARTH_WIDTH = 1f;

	// Use this for initialization
	void Awake ()
    {

	}

    private void Start()
    {
        SunManager.instance.SunWorldRef = this.gameObject;
    }

	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Initialize()
    {
        // Position in front of Player 
        transform.position = Camera.main.transform.position + (Vector3.down * FLATEARTH_CAMDOWN ) + ( Camera.main.transform.forward * FLATEARTH_CAMFORWARD );
        transform.localScale = new Vector3( FLATEARTH_WIDTH, FLATEARTH_WIDTH, FLATEARTH_WIDTH );
    }
}
