using UnityEngine;

public class BillboardMe : MonoBehaviour 
{
	void Update()
	{
		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
			Camera.main.transform.rotation * Vector3.up);
        transform.rotation = Quaternion.Euler( 0.0f, transform.rotation.eulerAngles.y, 0.0f );
	}
}
