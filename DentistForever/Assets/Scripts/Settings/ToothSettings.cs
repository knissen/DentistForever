using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToothSettings", menuName = "DentistForever/Tooth Settings")]
public class ToothSettings : ScriptableObject
{
    public float maxHealth = 100;
    public float startingHealth = 80;
    public float maxDPS = 10f;
    public float shakingThreshold = 20;
    public float ejectForce = 20;
    public float ejectUpBias = 1.5f;

    public Texture2D[] albedoTextures;
    public Texture2D[] metallicTextures;

    public GameObject nerverPrefab;
    public float nerveSpawnChance = 0.5f;
}
