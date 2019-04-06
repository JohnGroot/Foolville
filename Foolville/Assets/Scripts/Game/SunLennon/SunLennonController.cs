using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunLennonController : MonoBehaviour {

    AudioSource _source = null;
    //[SerializeField] AnimationCurve _speechVolumeCurve = null;

    [SerializeField] Transform _sunPositionTransform = null;
    [SerializeField] float _yPosition = 2.0f;

    Material _sunMat;
    bool _sunTexState = false;
    float _texTimer = 0.0f;
    [SerializeField, Space(5)] float _animTime = 2.00f;
    [SerializeField] Texture2D texState1 = null;
    [SerializeField] Texture2D texState2 = null;

	// Use this for initialization
	void Awake ()
    {
        _source = this.GetComponent<AudioSource>();
        _sunMat = this.GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //_source.volume = _speechVolumeCurve.Evaluate( SunManager.instance.SunInterp );

        this.transform.position = _sunPositionTransform.position;
        this.transform.AddPosY( _yPosition );


        this.transform.LookAt( Camera.main.transform );
        this.transform.Rotate( 0.0f, 180.0f, 0.0f );

        _texTimer += Time.deltaTime;

        if( _texTimer >= _animTime )
        {
            SwapSunTexture();
        }
    }

    void SwapSunTexture()
    {
        if ( _sunTexState )
        {
            _sunMat.mainTexture = texState1;
        }
        else
        {
            _sunMat.mainTexture = texState2;
        }

        _sunTexState = !_sunTexState;
        _texTimer = 0.0f;
    }
}
