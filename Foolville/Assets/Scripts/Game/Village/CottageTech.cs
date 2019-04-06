using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CottageTech : VillageInteractable
{
    public enum CottageState : int
    {
        INIT = 0,
        BUILDING,
        BUILT
    }
    [SerializeField, ReadOnly]
    CottageState _state = CottageState.INIT;
    public CottageState CurrState { get { return _state; } }

    Coroutine _birthRoutine = null;
    Coroutine _plantSpawnRoutine = null;

    int _villagerSpawnCount = 0;
    int _plantSpawnCount = 0;

	void Awake ()
    {
        Initialize();
	}
    private void OnDisable()
    {
        if ( _plantSpawnRoutine != null )
        {
            StopCoroutine( _plantSpawnRoutine );
        }
        if ( _plantSpawnRoutine != null )
        {
            StopCoroutine( _plantSpawnRoutine );
        }
    }
    public void Initialize()
    {
        StartCoroutine( DelayedBuildCottage() );
    }

    void Update()
    {
        if ( _focusTarget )
        {
            if( SunManager.instance.State == SunManager.SunState.HIGH_FOCUS && _focusTime > VillageConstants.instance.BurnTimer )
            {
                //VillageManager.instance.DestroyCottage( this );
            }
        }
    }

    IEnumerator DelayedBuildCottage()
    {
        yield return new WaitUntil( () => VillageManager.instance.FlatEarth != null );

        // BUILD THE COTTAGE
        _state = CottageState.BUILDING;
        float timer = 0.0f;
        while ( timer < VillageConstants.instance.CottageBuildTime )
        {
            timer += Time.deltaTime;

            transform.localScale = JohnTech.UniformVec( Mathf.Lerp( VillageConstants.instance.CottageScaleRange.x, 
                VillageConstants.instance.CottageScaleRange.y, 
                VillageConstants.instance.CottageBuildCurve.Evaluate( timer / VillageConstants.instance.CottageBuildTime ) ) );

            transform.SetPosY( VillageManager.instance.FlatEarth.transform.position.y + Mathf.Lerp( VillageConstants.instance.CottageYPosRange.x, VillageConstants.instance.CottageYPosRange.y, VillageConstants.instance.CottageBuildCurve.Evaluate( timer / VillageConstants.instance.CottageBuildTime ) ) );

            yield return 0;
        }        

        // FINISH BUILDING
        _state = CottageState.BUILT;

        // SPAWN VILLAGERS
        for (int i = 0; i < VillageConstants.instance.InitVillagerSpawnCount; i++ )
        {
            VillageManager.instance.SpawnFool( this.transform.position );

            yield return new WaitForSeconds( Random.Range( VillageConstants.instance.VillagerSpawnTimeRange.x, VillageConstants.instance.VillagerSpawnTimeRange.y ) );
        }

        _birthRoutine = StartCoroutine( GiveBirth() );
        //_plantSpawnRoutine = StartCoroutine( GrowPlant() );
    }

    IEnumerator GiveBirth()
    {
        yield return new WaitForSeconds( Mathf.Lerp( VillageConstants.instance.VillagerBirthRateRange.x, 
            VillageConstants.instance.VillagerBirthRateRange.y, 
            VillageConstants.instance.BirthRateCurve.Evaluate( (float)_villagerSpawnCount / (float)VillageConstants.instance.MaxVillagersSpawned ) ) );

        VillageManager.instance.SpawnFool( this.transform.position );

        _villagerSpawnCount++;

        if( _villagerSpawnCount < VillageConstants.instance.MaxVillagersSpawned )
        {
            _birthRoutine = StartCoroutine( GiveBirth());
        }        
    }

    IEnumerator GrowPlant()
    {
        yield return new WaitForSeconds( UnityEngine.Random.Range( 3.0f, 5.0f ) );

        Vector2 spawnDir = UnityEngine.Random.insideUnitCircle;
        spawnDir *= 0.15f;
        Vector3 spawnPos = this.transform.position + new Vector3( spawnDir.x, 0.0f, spawnDir.y );

        VillageManager.instance.SpawnPlant( spawnPos );

        _plantSpawnCount++;

        if( _plantSpawnCount < VillageConstants.instance.MaxPlantsSpawned )
        {
            _plantSpawnRoutine = StartCoroutine( GrowPlant() );
        }
    }


    #region Interactable Methods

    public override void ReceivedGaze()
    {
        Debug.Log( "Cottage Received Gaze" );

        _focusTarget = true;
        _focusTime = 0.0f;
    }

    public override void LostGaze()
    {
        Debug.Log( "Cottage Lost Gaze" );

        _focusTarget = false;
    }

    #endregion

}


