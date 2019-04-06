using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnDecalParticleSystem : MonoBehaviour 
{
	private int dataIndex;
	private ParticleSystem decalSystem;
	private ParticleSystem.EmitParams[] decalParticles;

    [SerializeField]
    private Vector2 emitIntervalRange = new Vector2( 0.05f, 2.0f );
    private float emitInterval = 0.5f;
    private float emitTimer = 0f;

    [SerializeField]
    private float _decalYPos = 0.01f;


    void Awake()
	{
		decalSystem = GetComponent(typeof(ParticleSystem)) as ParticleSystem;
		decalParticles = new ParticleSystem.EmitParams[decalSystem.main.maxParticles];

		dataIndex = 0;
	}

    void Update()
    {
        emitTimer += Time.deltaTime;

        emitInterval = Mathf.Lerp( emitIntervalRange.x, emitIntervalRange.y, SunManager.instance.SunInterp );
    }

	public void AddDecal(Vector3 pos, float size)
	{
        if (emitTimer < emitInterval)
        {
            return;
        }

        if( decalSystem != null )
        {
            if ( dataIndex >= decalSystem.main.maxParticles )
            {
                dataIndex = 0;
            }

            decalParticles[dataIndex].position = pos + ( Vector3.up * _decalYPos );
            //decalParticles[dataIndex].startSize = Random.Range( size - 0.1f, size + 0.1f );
            decalParticles[dataIndex].startSize = size;

            Vector3 rot = Quaternion.LookRotation( Vector3.down, Vector3.up ).eulerAngles;
            rot.z = Random.Range( 0f, 360f );
            decalParticles[dataIndex].rotation3D = rot;

            //decalParticles[dataIndex].startColor = gradient.Evaluate(Random.Range(0f, 1f));

            decalParticles[dataIndex].startLifetime = emitInterval;

            decalSystem.Emit( decalParticles[dataIndex], 1 );

            dataIndex++;
        }

        emitTimer = 0f;
        //emitInterval = Random.Range(0.3f, 0.6f);
	}
}
