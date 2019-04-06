using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTech : VillageInteractable
{
    Material _mat = null;

    Texture _mainTexture = null;
    bool _mainTexActive = true;

    [SerializeField] Texture2D[] _fireTexArray = new Texture2D[2];
    float _animTimer = 0.0f;
    const float FIRE_ANIMSPEED = 0.5f;
    int _texAnimIndex = 0;
    
    [SerializeField, Space( 5 )]
    bool hasDance = false;
    [SerializeField]
    Texture[] _danceTexArray = new Texture[2]; 


    void Awake ()
    {
        _mat = this.GetComponent<MeshRenderer>().material;
        _mainTexture = _mat.mainTexture;
	}
	
	
	void Update ()
    {
        if( _focusTarget )
        {
            if ( ( SunManager.instance.State == SunManager.SunState.HIGH_FOCUS || ( SunManager.instance.State == SunManager.SunState.MEDIUM_FOCUS && !hasDance ) ) )
            {
                _animTimer += Time.deltaTime;
                if ( _animTimer > FIRE_ANIMSPEED )
                {
                    SwapFireTextures();
                }
            }

            if ( _mainTexActive && ( SunManager.instance.State == SunManager.SunState.HIGH_FOCUS || SunManager.instance.State == SunManager.SunState.MEDIUM_FOCUS ) )
            {
                _mainTexActive = false;
            }
            else if (SunManager.instance.State == SunManager.SunState.LOW_FOCUS || ( SunManager.instance.State == SunManager.SunState.MEDIUM_FOCUS && hasDance ) )
            {
                if ( !_mainTexActive && !hasDance )
                {
                    SetMainTexture();
                }
                else if( hasDance )
                {
                    _animTimer += Time.deltaTime;
                    if ( _animTimer > FIRE_ANIMSPEED )
                    {
                        SwapDanceTextures();
                    }
                }
                
            }
        } 
        else if( !_mainTexActive && !_focusTarget )
        {
            SetMainTexture();
        }

	}

    void SwapFireTextures()
    {
        ToggleAnimIndex();

        _mat.mainTexture = _fireTexArray[_texAnimIndex];        
    }

    void SwapDanceTextures()
    {
        ToggleAnimIndex();

        _mat.mainTexture = _danceTexArray[_texAnimIndex];
    }

    void ToggleAnimIndex()
    {
        if ( _texAnimIndex >= 1 )
        {
            _texAnimIndex = 0;
        }
        else
        {
            _texAnimIndex = 1;
        }

        _animTimer = 0.0f;
    }

    void SetMainTexture()
    {
        _mainTexActive = true;
        _mat.mainTexture = _mainTexture;
    }

    public override void ReceivedGaze()
    {
        _focusTarget = true;
        
        SwapFireTextures();
    }

    public override void LostGaze()
    {
        _focusTarget = false;
        SetMainTexture();
    }
}
