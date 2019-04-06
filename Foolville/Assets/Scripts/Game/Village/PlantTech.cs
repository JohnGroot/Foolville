using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlantData
{
    [SerializeField] private String _plantName;
    public Texture2D _plantTexture;
    public Vector2 _plantScaleXRange;
    public Vector2 _plantScaleYRange;
    public float _growSpeed;
    public float _plantYPosition;
}

public class PlantTech : VillageInteractable {

    MeshRenderer _meshRenderer = null;

    public enum PlantType
    {
        NONE = -1,
        TREE = 0,
        SMALL_BUSH,
        MEDIUM_BUSH,
        LARGE_BUSH
    }
    PlantType _plantType = PlantType.NONE;
    public PlantType Type { get { return _plantType; } }

    PlantData _currPlantData;
    Vector3 _grownScale = Vector3.zero;

	void Awake ()
    {
        _meshRenderer = this.GetComponent<MeshRenderer>();
	}
	
    public void Initialize()
    {
        _plantType = (PlantType)UnityEngine.Random.Range( 0, PlantDataCollection.instance.PlantDataArray.Length -1 );
        _currPlantData = PlantDataCollection.instance.PlantDataArray[(int)_plantType];

        _meshRenderer.material.mainTexture = _currPlantData._plantTexture;
        float xScale = UnityEngine.Random.Range( _currPlantData._plantScaleXRange.x, _currPlantData._plantScaleXRange.y );
        _grownScale.x = xScale;
        _grownScale.z = xScale;
        _grownScale.y = UnityEngine.Random.Range( _currPlantData._plantScaleYRange.x, _currPlantData._plantScaleYRange.y );

        this.transform.localScale = Vector3.zero;
        this.transform.SetPosY( VillageManager.instance.FlatEarth.transform.position.y + _currPlantData._plantYPosition );

        StartCoroutine( GrowthRoutine() );
    }

    IEnumerator GrowthRoutine()
    {
        float timer = 0.0f;

        while( timer < _currPlantData._growSpeed )
        {
            this.transform.localScale = Vector3.Lerp( Vector3.zero, _grownScale, timer / _currPlantData._growSpeed );
            timer += Time.deltaTime;
            yield return 0;
        }

    }

    public override void ReceivedGaze()
    {
        _focusTarget = true;
        _focusTime = 0.0f;
    }

    public override void LostGaze()
    {
        _focusTarget = false;
    }
}
