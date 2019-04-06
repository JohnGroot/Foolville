using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class SpawnBallClick : MonoBehaviour {

    GestureRecognizer _gestureRecognizer = null;

    [SerializeField]
    GameObject _ball = null;
    [SerializeField]
    Vector2 _ballSpawnMinMax = new Vector2( 0.25f, 2.0f );
    [SerializeField]
    float _ballSpawnForce = 1.0f;

	// Use this for initialization
	void Awake ()
    {
        _gestureRecognizer = new GestureRecognizer();

        _gestureRecognizer.TappedEvent += HandleTapEvent;

        _gestureRecognizer.StartCapturingGestures();
	}

    private void OnDestroy()
    {
        _gestureRecognizer.TappedEvent -= HandleTapEvent;

        _gestureRecognizer.Dispose();
    }

    // Update is called once per frame
    void Update ()
    {
        if( Input.GetKey(KeyCode.Space))
        {
            Debug.Log( _gestureRecognizer.IsCapturingGestures() );
        }        
	}

    void HandleTapEvent( InteractionSourceKind source, int tapCount, Ray headRay  )
    {
        Debug.Log( "Handled Tap" );

        Vector3 playerEyePos = Camera.main.transform.position;
        GameObject newBall = null;

        for (int i = 0; i < tapCount; i++ )
        {
            newBall = Instantiate( _ball ) as GameObject;
            newBall.transform.position = playerEyePos + ( headRay.direction * Random.Range( _ballSpawnMinMax.x, _ballSpawnMinMax.y ) );

            newBall.GetComponent<Rigidbody>().AddForce( headRay.direction * _ballSpawnForce );
        }
    }
}
