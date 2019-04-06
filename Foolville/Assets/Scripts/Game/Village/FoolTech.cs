using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using HoloToolkit;

public class FoolTech : VillageInteractable {


    public enum FoolState : int
    {
        INIT = 0,
        IDLE,
        WALKING,
        BUILDING,
        THINKING,
        DANCING
    }
    [SerializeField, ReadOnly]
    FoolState _state = FoolState.INIT;
    public FoolState CurrState { get { return _state; } }

    FoolTalkTech _talkTech = null;

    CottageTech _currentCottage = null;
    int _buildingLayerMask;

    [SerializeField]
    bool _debugFool = false;

    Vector3 _walkStartOffset = Vector3.zero;
    Vector3 _walkDestinationOffset = Vector3.zero;
    float _moveSpeed = 0.0f;
    float _currMoveTime = 0.0f;
    float _moveProgress = 0.0f;

    [SerializeField] AudioSource _footstepAudioSource = null;

    FoolArtController _artController = null;

    [SerializeField, ReadOnly]bool _isPig = false;
    public bool IsPig { get { return _isPig; } }
    Coroutine _dissolveRoutine = null;

    void Awake ()
    {
        _talkTech = this.GetComponent<FoolTalkTech>();
        _artController = this.GetComponent<FoolArtController>();

        _buildingLayerMask = LayerMask.NameToLayer( "Building" );

        _moveSpeed = Random.Range( VillageConstants.instance.MoveSpeedRange.x, VillageConstants.instance.MoveSpeedRange.y );
	}

    public void Initialize()
    {
        _artController.EnableIdle();
        _talkTech.PlayIdleTalk();
        _isPig = false;
        SetFoolState( FoolState.WALKING );
    }

    // Update is called once per frame
    void Update ()
    {
        switch( _state )
        {
            case FoolState.INIT:
                Initialize();
                break;
            case FoolState.WALKING:
                UpdateWalking();
                break;
            case FoolState.BUILDING:
                UpdateBuilding();
                break;
            default:
                break;
        }

        if( _focusTarget )
        {
            if( SunManager.instance.State == SunManager.SunState.HIGH_FOCUS && _focusTime > VillageConstants.instance.BurnTimer + VillageConstants.instance.BurnStartTimer && _dissolveRoutine == null )
            {
                _dissolveRoutine = StartCoroutine( BurnRoutine() );
            }
            else if( SunManager.instance.State != SunManager.SunState.HIGH_FOCUS && _focusTime > 0.0f )
            {
                _focusTime = 0.0f;
            }

            if ( ( SunManager.instance.State == SunManager.SunState.MEDIUM_FOCUS || SunManager.instance.State == SunManager.SunState.LOW_FOCUS ) && _state != FoolState.DANCING )
            {
                StartDancing();
            }
            else if( SunManager.instance.State == SunManager.SunState.HIGH_FOCUS && _state == FoolState.DANCING )
            {
                StartCoroutine( DelayedIdle() );
            }
        }
	}

    

    void UpdateWalking()
    {
        if ( _moveProgress / _currMoveTime >= 1.0f )
        {            
            CompleteWalking();
        }
        else
        {
            _moveProgress += Time.deltaTime;
            this.transform.position = Vector3.Lerp( VillageManager.instance.FlatEarth.transform.position + _walkStartOffset, VillageManager.instance.FlatEarth.transform.position + _walkDestinationOffset, _moveProgress / _currMoveTime );
        }
    }

    void UpdateBuilding()
    {
        if( _currentCottage == null )
        {
            SetFoolState( FoolState.WALKING );
        }
        else if( _currentCottage.CurrState == CottageTech.CottageState.BUILT )
        {
            CompleteBuilding();
        }
    }

    public void SetFoolState( FoolState newState )
    {
        if (newState != _state)
        {
            switch( _state )
            {
                case FoolState.WALKING:
                    _footstepAudioSource.Stop();
                    break;
                default:
                    break;
            }

            switch ( newState )
            {
                case FoolState.WALKING:
                    _footstepAudioSource.Play();
                    _artController.Dissolve = 0.0f; // TODO: should fade in!
                    _artController.EnableIdle();
                    SetNewDestination();                    
                    break;
                case FoolState.BUILDING:
                    StartBuilding();
                    break;
                case FoolState.THINKING:
                    StartCoroutine( DelayedThinking() );
                    break;
                case FoolState.DANCING:
                    _artController.EnableDancing();
                    break;
                default:
                    break;
            }

            if( _debugFool )
            {
                Debug.Log( "Transitioning from " + _state.ToString() + " to " + newState.ToString() );
            }
            
            _state = newState;
        }
    }

    #region Walking

    void CompleteWalking()
    {        
        float odds = Random.value;

        if( odds <= VillageConstants.instance.WalkingStateOdds )
        {
            SetFoolState( FoolState.WALKING );
        }
        else if ( odds > VillageConstants.instance.WalkingStateOdds && odds <= VillageConstants.instance.ThinkingStateOdds )
        {
            SetFoolState( FoolState.THINKING );
        }
        else
        {
            SetFoolState( FoolState.BUILDING );
        }

    }

    public void SetNewDestination()
    {
        _moveProgress = 0.0f;

        Vector2 randomMoveDirection = Random.insideUnitCircle.normalized;

        _walkStartOffset = this.transform.position - VillageManager.instance.FlatEarth.transform.position;
        
        _walkDestinationOffset = ( new Vector3( randomMoveDirection.x, 0.0f, randomMoveDirection.y ) * Random.Range( VillageConstants.instance.PathDistanceRange.x, FlatEarthTech.FLATEARTH_WIDTH * 0.5f ) );
        _walkDestinationOffset.y = this.transform.position.y - VillageManager.instance.FlatEarth.transform.position.y;

        _currMoveTime = Vector3.Distance( this.transform.position, _walkDestinationOffset + VillageManager.instance.FlatEarth.transform.position ) / _moveSpeed;
    }

    #endregion

    #region Building
    
    void StartBuilding()
    {        
        if( CheckBuildConstraints() )
        {
            _currentCottage = VillageManager.instance.BuildCottage( this.transform.position + ( this.transform.forward * VillageConstants.instance.BuildPosOffset ) );            
        }
    }
    
    void CompleteBuilding()
    {
        _currentCottage = null;
    }

    /// <summary>
    /// Checks surroundings for other buildings
    /// </summary>
    /// <returns> True if this fool Can Build </returns>
    bool CheckBuildConstraints()
    {
        Collider[] cottageArray = Physics.OverlapSphere( this.transform.position, VillageConstants.instance.CottageMinSeparation, 1 << _buildingLayerMask );

        return cottageArray.Length == 0;
    }

    #endregion

    #region Thinking

    IEnumerator DelayedThinking()
    {
        float time = Random.Range( VillageConstants.instance.ThinkingTimeRange.x, VillageConstants.instance.ThinkingTimeRange.y );

        yield return new WaitForSeconds( Random.Range( VillageConstants.instance.ThinkingTimeRange.x, VillageConstants.instance.ThinkingTimeRange.y ) );

        CompleteThinking();
    }

    void CompleteThinking()
    {
        SetFoolState( FoolState.WALKING );
    }

    #endregion

    #region Gaze Methods

    IEnumerator HelloRoutine()
    {
        SetFoolState( FoolState.IDLE );
        _talkTech.SayHello();

        yield return new WaitWhile( () => _talkTech.IsTalking );
        
    }

    IEnumerator BurnRoutine()
    {
        Debug.Log( "Started Burn Fool" );

        _talkTech.PlayBurnAudio();
        _artController.EnableCharred();

        float timer = 0.0f;

        while (timer < VillageConstants.instance.BurnDissolveTimer)
        {
            timer += Time.deltaTime;

            _artController.Dissolve = timer / VillageConstants.instance.BurnDissolveTimer;

            yield return 0;
        }        

        TogglePig();
        //VillageManager.instance.DestroyFool( this );
    }

    void TogglePig()
    {        
        _isPig = !_isPig;

        _artController.EnableIdle();        

        if( _isPig )
        {
            StartCoroutine( DelayedIdle() );

            LostGaze();
        }

        _dissolveRoutine = null;
    }

    IEnumerator DelayedIdle()
    {
        yield return new WaitForSeconds( VillageConstants.instance.PigIdleReturnTime );

        SetFoolState( FoolState.WALKING );
    }

    #endregion

    #region Interactable Methods

    public override void ReceivedGaze()
    {
        _focusTarget = true;
      
        _focusTime = 0.0f;

        //Debug.Log( "Fool Received Gaze" );        
    }

    public override void LostGaze()
    {
        _focusTarget = false;

        SetFoolState( FoolState.WALKING );

        //Debug.Log( "Fool Lost Gaze" );

        _talkTech.PlayIdleTalk();
    }

    private void StartDancing()
    {
        SetFoolState( FoolState.DANCING );
    }

    #endregion
}
