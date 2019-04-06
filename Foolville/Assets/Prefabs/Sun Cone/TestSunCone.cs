using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSunCone : MonoBehaviour 
{
	[SerializeField] Transform sun;

	[SerializeField] Transform originJoint;
	[SerializeField] Transform endJoint;

	[SerializeField] BurnDecalParticleSystem burnSystem;

	void Awake()
	{
		originJoint.position = sun.position;
	}

	void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			Vector3 pos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
			endJoint.position = pos;
		}

		if (Input.GetMouseButton(0))
		{
			endJoint.localScale = Vector3.Lerp(endJoint.localScale, new Vector3(2f, 1f, 2f), 7f * Time.deltaTime);
			burnSystem.AddDecal(endJoint.position, endJoint.localScale.magnitude * 2f);
		}
		else
		{
			endJoint.localScale = Vector3.Lerp(endJoint.localScale, new Vector3(1f, 1f, 1f), 7f * Time.deltaTime);
			//burnSystem.AddDecal(endJoint.position, 2f);
		}

		
	}
}
