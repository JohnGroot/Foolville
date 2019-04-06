using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct FoolTextureSet
{
    [SerializeField]
    string _foolName;
    public Texture2D IdleTexture;
    public Texture2D WalkTexture;
    public Texture2D FirstCryingTexture;
    public Texture2D SecondCryingTexture;
    public Texture2D FirstDanceTexture;
    public Texture2D SecondDanceTexture;
}


public class FoolArtController : MonoBehaviour {

    FoolTech _foolParent = null;

    Material _foolMat = null;

	[SerializeField, ReadOnly] FoolTextureSet _currTextureSet;

	[SerializeField] float _walkAnimDuration = 0.05f;
    [SerializeField] float _danceAnimDuration = 0.25f;
    float _walkAnimTimer = 0.0f;
    [SerializeField, ReadOnly]int _texAnimIndex = 0;

    public enum FoolArtState : int
    {
        NONE = 0,
        AFLAME,
        DANCING,
        CHARRED
    }
    [SerializeField, ReadOnly]
    FoolArtState _artState = FoolArtState.NONE;
    public FoolArtState ArtState { get { return _artState; } }

    [SerializeField]
    Gradient _burnGradient;

    public float Dissolve { get { return _foolMat.GetFloat( _dissolveID); } set { _foolMat.SetFloat( _dissolveID, value ); } }
    int _dissolveID = 0;

    // Use this for initialization
    void Awake () 
	{
		_foolMat = this.GetComponent<MeshRenderer>().material;
		_foolParent = this.GetComponent<FoolTech>();

        _dissolveID = Shader.PropertyToID( "_Threshold" );

		InitializeTextures();	
	}

	void InitializeTextures()
	{
		_currTextureSet = FoolTextureCollection.instance.FoolTextureArray[UnityEngine.Random.Range( 0, FoolTextureCollection.instance.FoolTextureArray.Length - 1 )];

		_foolMat.mainTexture = _currTextureSet.IdleTexture;

        _artState = FoolArtState.NONE;
	}

	void Update()
	{
		if( _foolParent.CurrState == FoolTech.FoolState.WALKING || _foolParent.CurrState == FoolTech.FoolState.DANCING )
		{
			_walkAnimTimer += Time.deltaTime;

			if ( ( _foolParent.CurrState != FoolTech.FoolState.DANCING  && _walkAnimTimer > _walkAnimDuration )
                || ( _foolParent.CurrState == FoolTech.FoolState.DANCING && _walkAnimTimer > _danceAnimDuration ) )
			{
				SwapWalkTexture();
			}
            
		}

        HandleSunFocus();
	}

    void HandleSunFocus()
    {
        if ( _foolParent.FocusTarget  )
        {
            if( SunManager.instance.State == SunManager.SunState.HIGH_FOCUS && _artState != FoolArtState.AFLAME && _foolParent.FocusTime > VillageConstants.instance.BurnStartTimer )
            {
                EnableFire();
            }
            else if( SunManager.instance.State != SunManager.SunState.HIGH_FOCUS && _artState != FoolArtState.AFLAME && _artState != FoolArtState.DANCING )
            {
                EnableDancing();
            }            
        }
        else
        {
            if(_artState == FoolArtState.AFLAME)
            {
                EnableIdle();
            }
        }

        if( _artState == FoolArtState.AFLAME )
        {
           // _foolMat.color = Colorx.Slerp( _foolMat.color, Color.white, 50.0f * Time.deltaTime );
        }
        else
        {
            //_foolMat.color = Colorx.Slerp( _foolMat.color, _burnGradient.Evaluate( SunManager.instance.SunInterp ), 50.0f * Time.deltaTime );
        }
        
    }

	void SwapWalkTexture()
	{
        if( !_foolParent.IsPig )
        {
            if ( _texAnimIndex >= 1 )
            {
                _texAnimIndex = 0;
            }
            else if ( _texAnimIndex == 0 )
            {
                _texAnimIndex = 1;
            }

            if (_artState == FoolArtState.AFLAME)
            {
                _foolMat.mainTexture = FoolTextureCollection.instance.FoolBurnTextures[_texAnimIndex];
            }
            else if( _artState == FoolArtState.DANCING )
            {
                if ( _texAnimIndex == 0 )
                {
                    _foolMat.mainTexture = _currTextureSet.FirstDanceTexture;
                }
                else
                {
                    _foolMat.mainTexture = _currTextureSet.SecondDanceTexture;
                }
            }
            else
            {
                if ( _texAnimIndex == 0 )
                {
                    _foolMat.mainTexture = _currTextureSet.IdleTexture;
                }
                else
                {
                    _foolMat.mainTexture = _currTextureSet.WalkTexture;
                }
            }         
        }
        else
        {
            HandlePigWalk();
        }

        _walkAnimTimer = 0.0f;
    }

    void HandlePigWalk()
    {
        if( _artState == FoolArtState.AFLAME )
        {
            if ( _texAnimIndex >= FoolTextureCollection.instance.PigBurnTextures.Length - 1 )
            {
                _texAnimIndex = 0;
            }

            _foolMat.mainTexture = FoolTextureCollection.instance.PigBurnTextures[_texAnimIndex];
                        
            _texAnimIndex++;            
        }
        else
        {
            _foolMat.mainTexture = FoolTextureCollection.instance.PigWalkTextures[_texAnimIndex];

            if ( _texAnimIndex == FoolTextureCollection.instance.PigWalkTextures.Length - 1 )
            {
                _texAnimIndex = 0;
            }
            else
            {
                _texAnimIndex++;
            }            
        }
    }

    public void EnableFire()
    {
        Debug.Log( "Enabling Fire" );
        _artState = FoolArtState.AFLAME;
        if ( !_foolParent.IsPig )
        {
            _foolMat.mainTexture = FoolTextureCollection.instance.FoolBurnTextures[0];
        }
        else
        {
            _foolMat.mainTexture = FoolTextureCollection.instance.PigBurnTextures[0];
        }
    }

    public void EnableCharred()
    {
        _artState = FoolArtState.CHARRED;

        _foolMat.mainTexture = FoolTextureCollection.instance.FoolCharred;
    }

    public void EnableIdle()
    {        
        _artState = FoolArtState.NONE;

        if ( !_foolParent.IsPig )
        {
            _foolMat.mainTexture = _currTextureSet.IdleTexture;
        }
        else
        {
            _foolMat.mainTexture = FoolTextureCollection.instance.PigWalkTextures[0];
        }
    }

    public void EnableDancing()
    {
        _artState = FoolArtState.DANCING;
        if( !_foolParent.IsPig )
        {
            _foolMat.mainTexture = _currTextureSet.FirstDanceTexture;
        }
    }

}
