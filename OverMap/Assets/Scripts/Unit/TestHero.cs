using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexUnit), true)]
public class TestHero : HexUnit
{
    void Start()
    {
        Debug.Log(isFly + isCanWater.ToString());
        speed = 36;
    }
}
