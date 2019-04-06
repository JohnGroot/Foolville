using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FoolTextureCollection : ScriptableObjectSingleton<FoolTextureCollection>
{
    public FoolTextureSet[] FoolTextureArray;

    public Texture2D[] FoolBurnTextures = new Texture2D[2];

    public Texture2D FoolCharred;

    [Space(5)]public Texture2D[] PigWalkTextures = new Texture2D[4];
    public Texture2D[] PigBurnTextures = new Texture2D[3];
}
