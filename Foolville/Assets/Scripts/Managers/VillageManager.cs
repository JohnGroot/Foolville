using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageManager : SingletonBehaviour<VillageManager> {

    List<CottageTech> _cottageList = new List<CottageTech>();
    List<FoolTech> _foolList = new List<FoolTech>();
    List<PlantTech> _plantList = new List<PlantTech>();

    [SerializeField]
    GameObject _foolPrefab = null;
    const int FOOL_INITSPAWNCOUNT = 50;
    const float FOOL_SPAWNY = 0.065f;

    [SerializeField]
    GameObject _cottagePrefab = null;
    const int COTTAGE_INITSPAWNCOUNT = 20;
    const float COTTAGE_SPAWNY = 0.3f;

    [SerializeField]
    GameObject _plantPrefab = null;
    const int PLANT_INITSPAWNCOUNT = 50;
    const float PLANT_SPAWNY = 0.075f;

    [SerializeField, Space( 5 )]
    GameObject _flatEarthPrefab = null;
    [SerializeField]FlatEarthTech _flatEarth = null;   
    public FlatEarthTech FlatEarth { get { return _flatEarth; } }

	// Use this for initialization
	void Awake ()
    {
        StartCoroutine( DelayedInitialize() );
	}

    IEnumerator DelayedInitialize()
    {
        yield return new WaitUntil( () => HololensManager.instance.IsInitialized );

        Initialize();
    }

    public override void Initialize()
    {
        GameObject tempObj = null;
        for ( int i = 0; i < FOOL_INITSPAWNCOUNT; i++ )
        {
            tempObj = Instantiate<GameObject>( _foolPrefab, _flatEarth.transform );
            tempObj.SetActive( false );

            _foolList.Add( tempObj.GetComponent<FoolTech>() );
        }

        //for ( int i = 0; i < COTTAGE_INITSPAWNCOUNT; i++ )
        //{
        //    tempObj = Instantiate<GameObject>( _cottagePrefab );
        //    tempObj.SetActive( false );

        //    _foolList.Add( tempObj.GetComponent<FoolTech>() );
        //}

        //for ( int i = 0; i < PLANT_INITSPAWNCOUNT; i++ )
        //{
        //    tempObj = Instantiate<GameObject>( _plantPrefab );
        //    tempObj.SetActive( false );

        //    _foolList.Add( tempObj.GetComponent<FoolTech>() );
        //}

        if ( _flatEarth == null )
        {
            _flatEarth = Instantiate<GameObject>( _flatEarthPrefab ).GetComponent<FlatEarthTech>();
        }

        _flatEarth.Initialize();

        //BuildCottage( _flatEarth.transform.position + ( Vector3.up * COTTAGE_SPAWNY ) );

        isInitialized = true;
    }
    
    public CottageTech BuildCottage( Vector3 buildPos )
    {
        CottageTech newCottage = _cottageList.Find( x => !x.isActiveAndEnabled );

        if ( newCottage == null)
        {
            newCottage = Instantiate<GameObject>( _cottagePrefab ).GetComponent<CottageTech>();

            _cottageList.Add( newCottage );
        }

        newCottage.gameObject.SetActive( true );

        buildPos.y = _flatEarth.transform.position.y + FOOL_SPAWNY;     // Asserts that the cottage will be built at a Certain Y Position
        newCottage.transform.position = buildPos;        

        newCottage.Initialize();

        return newCottage;
    }

    public void SpawnFool( Vector3 cottagePos )
    {
        FoolTech newFool = _foolList.Find( x => x != null && !x.isActiveAndEnabled );

        if ( newFool == null )
        {
            newFool = Instantiate<GameObject>( _foolPrefab ).GetComponent<FoolTech>();

            _foolList.Add( newFool );
        }

        newFool.gameObject.SetActive( true );

        cottagePos.y = _flatEarth.transform.position.y + FOOL_SPAWNY;    // Asserts that the cottage will be built at a Certain Y Position
        newFool.transform.position = cottagePos;

        newFool.SetFoolState( FoolTech.FoolState.INIT );
    }

    public void SpawnPlant( Vector3 spawnPos )
    {
        PlantTech newPlant = _plantList.Find( x => x != null && !x.isActiveAndEnabled );

        if( newPlant == null )
        {
            newPlant = Instantiate<GameObject>( _plantPrefab ).GetComponent<PlantTech>();

            _plantList.Add( newPlant );
        }

        newPlant.gameObject.SetActive( true );

        //spawnPos.y = _flatEarth.transform.position.y + PLANT_SPAWNY;
        newPlant.transform.position = spawnPos;

        newPlant.Initialize();

    }

    // Some methods to "destroy" things but we r rly just settin them to inactive.
    public void DestroyFool( FoolTech doomedFool )
    {
        if( doomedFool.FocusTarget )
        {
            VillageInteractionManager.instance.RemoveFoolFocus( doomedFool );
        }

        doomedFool.gameObject.SetActive( false );
    }

    public void DestroyCottage( CottageTech doomedCottage )
    {
        if ( doomedCottage.FocusTarget )
        {
            VillageInteractionManager.instance.RemoveCottageFocus( doomedCottage );
        }

        doomedCottage.gameObject.SetActive( false );
    }

    public void DestroyPlant( PlantTech doomedPlant )
    {
        doomedPlant.gameObject.SetActive( false );
    }
}
