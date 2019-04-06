using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class VillageInteractionManager : SingletonBehaviour<VillageInteractionManager> {

    VillageInteractable _currGazeFocus = null;

    [SerializeField,ReadOnly] float _focusTime = 0.0f;
    public float FocusTime { get { return _focusTime; } }

    List<FoolTech> _focusedFoolList = new List<FoolTech>();
    List<CottageTech> _focusedCottageList = new List<CottageTech>();
    int index = 0;

    void Start ()
    {        
        //GazeManager.Instance.FocusedObjectChanged += HandleFocusObjectChanged;
    }

    private void Update()
    {
        if( _focusedFoolList.Count > 0 )
        {
            for ( index = 0; index < _focusedFoolList.Count; index++ )
            {
                _focusedFoolList[index].FocusTime += Time.deltaTime;
            }
        }

        if ( _focusedCottageList.Count > 0 )
        {
            for ( index = 0; index < _focusedCottageList.Count; index++ )
            {
                _focusedCottageList[index].FocusTime += Time.deltaTime;
            }
        }

    }


    #region Focus Change Methods

    public void AddFoolFocus( FoolTech fool )
    {
        if( !_focusedFoolList.Contains( fool ) )
        {
            _focusedFoolList.Add( fool );
            fool.ReceivedGaze();
        }
    }

    public void RemoveFoolFocus( FoolTech fool )
    {
        if( _focusedFoolList.Contains( fool ) )
        {
            _focusedFoolList.Remove( fool );
            fool.LostGaze();
        }
    }

    public void AddCottageFocus( CottageTech cottage )
    {
        if ( !_focusedCottageList.Contains( cottage ) )
        {
            _focusedCottageList.Add( cottage );
            cottage.ReceivedGaze();
        }
    }

    public void RemoveCottageFocus( CottageTech cottage )
    {
        if( _focusedCottageList.Contains(cottage))
        {
            _focusedCottageList.Remove( cottage );
            cottage.LostGaze();
        }        
    }
    
    public void RemoveAllFocus()
    {
        foreach( CottageTech c in _focusedCottageList )
        {
            c.LostGaze();
        }
        _focusedCottageList.Clear();

        foreach( FoolTech f in _focusedFoolList )
        {
            f.LostGaze();
        }
        _focusedFoolList.Clear();
    }

    #endregion
}
