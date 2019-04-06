using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoolTalkTech : MonoBehaviour {

    AudioSource _source = null;
    [SerializeField] AudioSource _idleTalkSource = null;

    [SerializeField] AudioClip _helloClip = null;
    [SerializeField] AudioClip _burnClip = null;

    public bool IsTalking { get  { return _source.isPlaying; } }

    AudioClip _idleTalkClip = null;
    Coroutine _idleTalkRoutine = null;

	void Awake ()
    {
        _source = this.GetComponent<AudioSource>();
        _idleTalkClip = AudioClipCollection.instance.FoolTalkClipArray[Random.Range( 0, AudioClipCollection.instance.FoolTalkClipArray.Length - 1 )];
        
        PlayIdleTalk();
	}

    private void OnDisable()
    {
        if( _idleTalkRoutine != null )
        {
            StopCoroutine( _idleTalkRoutine );
            _idleTalkRoutine = null;
        }
        _idleTalkSource.Stop();
    }

    public void PlayIdleTalk()
    {
        _idleTalkRoutine = StartCoroutine( IdleTalkRoutine() );        
    }

    IEnumerator IdleTalkRoutine()
    {
        if( _idleTalkSource.isPlaying )
            yield return new WaitUntil( () => _idleTalkSource.isPlaying );

        yield return new WaitForSeconds( Random.Range( VillageConstants.instance.IdleTalkRange.x, VillageConstants.instance.IdleTalkRange.y ) );

        _idleTalkSource.clip = _idleTalkClip;
        _idleTalkSource.loop = false;
        _idleTalkSource.Play();

        _idleTalkRoutine = StartCoroutine( IdleTalkRoutine() );
    }

    public void SayHello()
    {
        _source.clip = _helloClip;
        _source.loop = false;
        _source.Play();
    }

    public void PlayBurnAudio()
    {
        _source.clip = _burnClip;
        _source.loop = false;
        _source.Play();
    }

}
