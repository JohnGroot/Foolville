using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageConstants : ScriptableObjectSingleton<VillageConstants>
{
    ///
    /// Cottage Variables
    ///
    public int InitVillagerSpawnCount = 5;
    public float CottageBuildTime = 2.0f;
    public AnimationCurve CottageBuildCurve;
    public Vector2 CottageScaleRange = new Vector2( 0.25f, 0.5f );
    public Vector2 CottageYPosRange = new Vector2( 0.0f, 0.5f );
    public Vector2 VillagerSpawnTimeRange = new Vector2( 0.15f, 0.5f );
    public float CottageMinSeparation = 0.35f;
    public Vector2 VillagerBirthRateRange = new Vector2( 1.0f, 15.0f );
    public AnimationCurve BirthRateCurve;
    public int MaxVillagersSpawned = 5;
    public int MaxPlantsSpawned = 5;

    ///
    /// FOOL Variables
    ///
    public Vector2 PathDistanceRange = new Vector2( 1.0f, 5.0f );
    public Vector2 MoveSpeedRange = new Vector2( 1.0f, 3.5f );
    public float RemainingDistanceStop = 0.05f;
    public float WalkingStateOdds = 0.25f;
    public float ThinkingStateOdds = 0.5f;
    public Vector2 ThinkingTimeRange = new Vector2( 1.0f, 5.0f );
    public float BuildPosOffset = 1.0f;
    public float HelloTimer = 0.25f;
    public float BurnTimer = 3.0f;
    public float BurnStartTimer = 1.0f;
    public float BurnDissolveTimer = 1.0f;
    public Vector2 IdleTalkRange = new Vector2( 0.1f, 0.75f );

    ///
    /// PIG Variables
    /// 
    public float PigIdleReturnTime = 0.75f;
}
