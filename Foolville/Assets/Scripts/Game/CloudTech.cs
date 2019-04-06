using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudTech : MonoBehaviour {

	[SerializeField] float _parentRotateSpeed = 5.0f;

	int _cloudPivotIndex = 0;
	[SerializeField, Space(5)] List<Transform> _cloudPivotList = new List<Transform>();
	[SerializeField] Vector2 _pivotRotateSpeedRange = new Vector2( 5.0f, 7.5f );
	List<float> _pivotSpeedList = new List<float>();

	// Use this for initialization
	void Awake () 
	{
		for( _cloudPivotIndex = 0; _cloudPivotIndex < _cloudPivotList.Count; _cloudPivotIndex++ )
		{
			_pivotSpeedList.Add( Random.Range( _pivotRotateSpeedRange.x, _pivotRotateSpeedRange.y ) );
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// THIS BLOWS
		for( _cloudPivotIndex = 0; _cloudPivotIndex < _cloudPivotList.Count; _cloudPivotIndex++ )
		{
			_cloudPivotList[_cloudPivotIndex].Rotate( 0.0f, 0.0f,_pivotSpeedList[_cloudPivotIndex] * Time.deltaTime, Space.Self );
		}

		this.transform.Rotate( 0.0f, 5.0f * Time.deltaTime, 0.0f );	
	}
}
