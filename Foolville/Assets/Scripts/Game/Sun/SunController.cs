using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class SunController : MonoBehaviour, IInputClickHandler
{

    [SerializeField, ReadOnly]
    bool _sunEnabled = true;

    Rigidbody _rigidbody = null;
    SphereCollider _collider = null;

    Vector3 _targetPosition = Vector3.zero;
    public Vector3 SunTargetPos { get { return _targetPosition; } set { _targetPosition = value; } }
    Vector3 _movePosition = Vector3.zero;

    Plane _sunPlacementPlane;
    Ray _sunPlacementRay;
    float _sunPlacementPlaneDistance = 0.0f;

    const float SUN_YPOS = 0.05f;

    const float SUN_MOVESPEED = 20.0f;

    [SerializeField] Vector2 _sunScaleRange = new Vector2( 0.01f , 0.1f);

    [SerializeField] Transform _originJoint;
    const float SUN_OFFSET = 4.0f;
    const float SUN_MAXDOTPRODUCT = -0.15f;

    [SerializeField] Transform _endJoint;

    [SerializeField]
    Material _groundDiscMat = null;
	[SerializeField]
	Material _coneMat = null;

    [SerializeField] BurnDecalParticleSystem _burnSystem;

    [SerializeField]
    float _groundDiscYOffset = 0.025f;
    [SerializeField]
    float _groundDiscScaleOffset = 0.015f;
    [SerializeField]
    float _burnParticleScalar = 0.1f;
	[SerializeField, Range(-0.1f, 0.1f)]
	float _centerOffset = 0.005f;

    [SerializeField]
    ParticleSystem _burnParticles = null;
    
    // Use this for initialization
	void Awake ()
    {
        Vector3 _hitPlaneOrigin = Vector3.zero;
        _hitPlaneOrigin.y = SUN_YPOS;
        _sunPlacementPlane = new Plane( Vector3.up, _hitPlaneOrigin );

        _sunPlacementRay = new Ray( Vector3.zero, Vector3.zero );

        _rigidbody = this.GetComponent<Rigidbody>();
        //_meshRenderer = this.GetComponent<MeshRenderer>();
        _collider = this.GetComponent<SphereCollider>();

        //_sunBeamRenderer.useWorldSpace = true;
        //_sunBeamRenderer.startWidth = 0.0f;  

        SetBeamWidth(5f);

        //StartCoroutine( DelayedPositionGroundDisc() );
    }

    public void Initialize()
    {

    }

	// Update is called once per frame
	void Update ()
    {
        if( _sunEnabled )
        {
            // Sun Distance Interp in the SunManager
            UpdateSunSpotPosition();
            UpdateSunScale();
            UpdateSunColor();
            UpdateSunBeam();

            if( SunManager.instance.State == SunManager.SunState.HIGH_FOCUS && !_burnParticles.isEmitting )
            {
                _burnParticles.Play();
            }
            else if( SunManager.instance.State != SunManager.SunState.HIGH_FOCUS && _burnParticles.isEmitting )
            {
                _burnParticles.Stop();
            }
        }        
    }

    //private void FixedUpdate()
    //{
    //    // Using to Move Position to get as many trigger hits as possible
    //    _rigidbody.MovePosition( _movePosition );
    //}

    void UpdateSunSpotPosition()
    {
        if( VillageManager.instance.FlatEarth != null )
        {
            _sunPlacementPlane.SetNormalAndPosition( Vector3.up, new Vector3( 0.0f, VillageManager.instance.FlatEarth.transform.position.y + SUN_YPOS, 0.0f ) );

            _sunPlacementRay.origin = GazeManager.Instance.GazeOrigin;
            _sunPlacementRay.direction = GazeManager.Instance.GazeNormal;

            if( _sunPlacementPlane.Raycast( _sunPlacementRay, out _sunPlacementPlaneDistance ) )
            {
                _targetPosition = _sunPlacementRay.GetPoint( _sunPlacementPlaneDistance );
            }
           
        }

        _movePosition = Vector3.Lerp( _movePosition, _targetPosition, SUN_MOVESPEED * Time.deltaTime );
    }

    void UpdateSunBeam()
    {
        if ( VillageManager.instance.FlatEarth != null )
        {            
            float camLookAngle = Vector3.Dot( Camera.main.transform.forward, _sunPlacementPlane.normal );
            
            // Make sure that Sunbeam doesn't move if you are looking up (reminder: - dot product is OBTUSE, normal points up)
            if( camLookAngle < SUN_MAXDOTPRODUCT )
            {
                if ( !_collider.enabled )
                {
                    _collider.enabled = true;
                }

                // TODO: make sure sun beam moves based on direction away from Camera
                Vector3 camDir = VillageManager.instance.FlatEarth.transform.position - Camera.main.transform.position;
                camDir.Normalize();
                _originJoint.position = VillageManager.instance.FlatEarth.transform.position + ( Vector3.Reflect( camDir, _sunPlacementPlane.normal ) * SUN_OFFSET );
                _originJoint.SetPosY( 1.5f );

                SetBeamWidth( Mathf.Lerp( _sunScaleRange.x, _sunScaleRange.y, SunManager.instance.SunInterp ) );

                //_movePosition.y += _groundDiscYOffset;
                
				_endJoint.transform.position = _movePosition;

				camDir.y = 0.0f;
				camDir.Normalize();

                Vector3 childCenterPos = _endJoint.transform.position + Vector3.Cross( camDir, Vector3.up ) * _centerOffset;

                _groundDiscMat.SetVector("_Center", childCenterPos );
                _burnSystem.AddDecal( childCenterPos, Mathf.Lerp( _sunScaleRange.x, _sunScaleRange.y, SunManager.instance.SunInterp ) * _burnParticleScalar );
            
            }
            else
            {
                if( _collider.enabled )
                {
                    _collider.enabled = false;
                    VillageInteractionManager.instance.RemoveAllFocus();
                }
            }
        }            
    }

    void UpdateSunScale()
    {        
        // TODO: make this scale the sun spot thing and not the whole object
        //this.transform.localScale = Vector3.one * Mathf.Lerp( _sunScaleRange.x, _sunScaleRange.y, SunManager.instance.SunInterp );
    }

    void UpdateSunColor()
    {
		_groundDiscMat.color = SunManager.instance.SunColor;
		_coneMat.color = SunManager.instance.SunColor;
    }

    void SetBeamWidth( float width )
    {
        _originJoint.localScale = new Vector3(1f, 0.01f, 0.01f);
        _endJoint.localScale = new Vector3( width, 1f, width );
        _groundDiscMat.SetFloat("_ScaleFactor", (width / 100f) - _groundDiscScaleOffset );
    }

    IEnumerator DelayedPositionGroundDisc()
    {
        yield return new WaitUntil( () => VillageManager.instance != null && VillageManager.instance.FlatEarth != null );
        PositionGroundDisc();
    }

    // TODO: Make Sure This Actually Works
    void PositionGroundDisc()
    {
        //Vector3 groundDiscPos = VillageManager.instance.FlatEarth.transform.position;
        //groundDiscPos.y += 0.04f;
        //_groundDiscMat.SetVector( "_Center", groundDiscPos );
    }

    private void OnTriggerEnter( Collider other )
    {        
        if ( other.GetComponent<FoolTech>() != null )
        {
            VillageInteractionManager.instance.AddFoolFocus( other.GetComponent<FoolTech>() );
        }
//         else if( other.GetComponent<CottageTech>() != null )
//         {
//             VillageInteractionManager.instance.AddCottageFocus( other.GetComponent<CottageTech>() );
//         }
        else if( other.GetComponent<FireTech>() != null )
        {
            other.GetComponent<FireTech>().ReceivedGaze();
        }
    }

    private void OnTriggerExit( Collider other )
    {
        if ( other.GetComponent<FoolTech>() != null )
        {
            VillageInteractionManager.instance.RemoveFoolFocus( other.GetComponent<FoolTech>() );
        }
//         else if ( other.GetComponent<CottageTech>() != null )
//         {
//             VillageInteractionManager.instance.RemoveCottageFocus( other.GetComponent<CottageTech>() );
//         }
        else if ( other.GetComponent<FireTech>() != null )
        {
            other.GetComponent<FireTech>().LostGaze();
        }
    }

    public void OnInputClicked( InputClickedEventData eventData )
    {
        _sunEnabled = !_sunEnabled;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere( _targetPosition, 0.05f );

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere( _movePosition, 0.05f );
    }
}
