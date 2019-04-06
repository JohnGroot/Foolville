using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class KeywordManagerPlatform : MonoBehaviour {

	void Awake()
	{
		#if UNITY_EDITOR_OSX
		this.GetComponent<KeywordManager>().enabled = false;
		#endif
	}
}
