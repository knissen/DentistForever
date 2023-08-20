using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToothSettings", menuName = "DentistForever/Tooth Settings")]
public class ToothSettings : ScriptableObject
{
    public float startingHealth = 100;
    public float maxDPS = 10f;
    public float shakingThreshold = 20;
    public float ejectForce = 20;
    public float ejectUpBias = 1.5f;
}
