using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageEditorCamera : MonoBehaviour {

    [SerializeField, Range(0.0f, 720.0f)] float _rotateSpeed = 20.0f;
    [SerializeField]
    bool enable = true;

	// Use this for initialization
	void Awake ()
    {
        if(enable)
        {
            Camera.main.transform.SetParent( this.transform );
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if(enable)
        {
            this.transform.Rotate( Vector3.up, _rotateSpeed * Time.deltaTime, Space.World );
            Camera.main.transform.LookAt( Vector3.zero );
        }        
	}
}
