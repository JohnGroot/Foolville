using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlantDataCollection : ScriptableObjectSingleton<PlantDataCollection>
{
    [SerializeField]
    public PlantData[] PlantDataArray;
}

