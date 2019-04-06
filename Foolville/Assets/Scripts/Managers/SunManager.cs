using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunManager : SingletonBehaviour<SunManager> {

    public enum SunState : int
    {
        NONE = 0,
        LOW_FOCUS,
        MEDIUM_FOCUS,
        HIGH_FOCUS
    }
    [SerializeField, ReadOnly] SunState _state = SunState.NONE;
    public SunState State { get { return _state; } }
    const float SUNSTATE_MEDIUMFOCUS_THRESHOLD = 0.33f;
    const float SUNSTATE_LOWFOCUS_THRESHOLD = 0.66f;

    [SerializeField]
    GameObject _sunSpot = null;

    [SerializeField, Tooltip("Object the player is judging distance from.")]
    GameObject _worldObject = null;    

    public GameObject SunWorldRef { set { _worldObject = value; } }

    [SerializeField]
    GameObject _playerObject = null;

    [SerializeField, ReadOnlyAttribute]
    float _sunInterp = 0.0f;
    public float SunInterp { get { return _sunInterp; }  }

    [SerializeField, ReadOnlyAttribute]
    float _sunLennonInterp = 0.0f;
    public float SunLennonInterp { get { return _sunLennonInterp; } }

    [SerializeField]
    AnimationCurve _sunDistanceAnimCurve = null;

    [SerializeField, Tooltip("World space range of distance from player to world object. X is min, Y is max.")]
    Vector2 _playerDistanceRange = Vector2.up;

    [SerializeField, Tooltip( "World space range of distance from player to world object. X is min, Y is max." )]
    Vector2 _sunLennonDistanceRange = Vector2.up;

    [SerializeField] Gradient _sunColorGradient;
	public Color SunColor { get { return _sunColorGradient.Evaluate( _sunInterp ); } }

	// Use this for initialization
	void Awake ()
    {

	}

	// Update is called once per frame
	void Update ()
    {
        UpdateSunInterp();   
	}

    void UpdateSunInterp()
    {
        // TODO: should probably be distance to the position that the player is looking at.
        if ( _worldObject != null && _playerObject != null )
        {
            _sunInterp = _sunDistanceAnimCurve.Evaluate( Mathf.InverseLerp( _playerDistanceRange.x, _playerDistanceRange.y, Vector3.Distance( _sunSpot.transform.position, _playerObject.transform.position ) ) );
            //if( VillageManager.instance.FlatEarth != null )
            //{
            //    _sunLennonInterp = _sunDistanceAnimCurve.Evaluate( Mathf.InverseLerp( _sunLennonDistanceRange.x, _sunLennonDistanceRange.y, Vector3.Distance( Camera.main.transform.position, VillageManager.instance.FlatEarth.transform.position ) ) );
            //}
            
            if ( _sunInterp < SUNSTATE_MEDIUMFOCUS_THRESHOLD && _state != SunState.HIGH_FOCUS )
            {
                _state = SunState.HIGH_FOCUS;
            }
            else if( _sunInterp > SUNSTATE_MEDIUMFOCUS_THRESHOLD && _sunInterp < SUNSTATE_LOWFOCUS_THRESHOLD && _state != SunState.MEDIUM_FOCUS )
            {
                _state = SunState.MEDIUM_FOCUS;
            }
            else if( _sunInterp > SUNSTATE_LOWFOCUS_THRESHOLD && _state != SunState.LOW_FOCUS )
            {
                _state = SunState.LOW_FOCUS;
            }
        }
    }
}
