using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentColorController : MonoBehaviour {

    [SerializeField]
    Material _islandMat;
    [SerializeField]
    Material[] _buildingMatArray;
    [SerializeField]
    Material[] _plantMatArray;

    [SerializeField]
    Gradient _enviroColorGradient;
	
	void Update ()
    {
        _islandMat.color = Colorx.Slerp( _islandMat.color, _enviroColorGradient.Evaluate( SunManager.instance.SunInterp ), 50.0f * Time.deltaTime );

        for( int i = 0; i < _buildingMatArray.Length; i++ )
        {
            _buildingMatArray[i].color = Colorx.Slerp( _buildingMatArray[i].color, _enviroColorGradient.Evaluate( SunManager.instance.SunInterp ), 50.0f * Time.deltaTime );
        }

        for ( int i = 0; i < _plantMatArray.Length; i++ )
        {
            _plantMatArray[i].color = Colorx.Slerp( _plantMatArray[i].color, _enviroColorGradient.Evaluate( SunManager.instance.SunInterp ), 50.0f * Time.deltaTime );
        }
    }
}
