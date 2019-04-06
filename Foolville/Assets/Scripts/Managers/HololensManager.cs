using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA;

public class HololensManager : SingletonBehaviour<HololensManager> {

    
    public override void Initialize()
    {        
        isInitialized = true;
    }

    // Use this for initialization
    void Awake ()
    {
        Initialize();
	}

}
