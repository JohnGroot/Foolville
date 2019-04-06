using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// TODO: implement system that fades all diff audio tracks in and out. might be unnecessary idk


public class AudioManager : SingletonBehaviour<AudioManager> {

    [SerializeField, Space( 5 )]
    AudioMixer _mainMixer;
    [SerializeField]
    AnimationCurve _idleTalkVolumeCurve;
    [SerializeField]
    AnimationCurve _dimFireVolumeCurve;
    [SerializeField]
    AnimationCurve _intenseFireVolumeCurve;
    [SerializeField]
    AnimationCurve _ambienceVolumeCurve;
    [SerializeField]
    AnimationCurve _sunLennonVolumeCurve;
    [SerializeField]
    AnimationCurve _danceMusicVolumeCurve;

    float currSunInterp = 0.0f;

    // Use this for initialization
    void Awake ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        currSunInterp = SunManager.instance.SunInterp;

        // higher interp is Farther Away
        _mainMixer.SetFloat( "idleTalkVol", _idleTalkVolumeCurve.Evaluate( currSunInterp ) );
        _mainMixer.SetFloat( "dimFireVol", _dimFireVolumeCurve.Evaluate( currSunInterp ) );
        _mainMixer.SetFloat( "ambienceVol", _ambienceVolumeCurve.Evaluate( currSunInterp ) );
        _mainMixer.SetFloat( "intenseFireVol", _intenseFireVolumeCurve.Evaluate( currSunInterp ) );
        _mainMixer.SetFloat( "sunLennonVol", _sunLennonVolumeCurve.Evaluate( currSunInterp ) );
        _mainMixer.SetFloat( "danceMusicVol", _danceMusicVolumeCurve.Evaluate( currSunInterp ) );

        // _singingMixingGroup.audioMixer.SetFloat( "Volume", currSunInterp );
        // _scattingMixingGroup.audioMixer.SetFloat( "Volume", currSunInterp );
        // _screamingMixerGroup.audioMixer.SetFloat( "Volume", currSunInterp );
        // _sunLennonMixerGroup.audioMixer.SetFloat( "Volume", currSunInterp );
    }
}
