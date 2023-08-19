using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouth : MonoBehaviour
{
    public static Mouth Instance { get; private set; }

    public Vector3 EatFoodPosition { get { return _eatFoodPosition.position; } }

    [SerializeField] private Transform _eatFoodPosition;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one mouth in scene, bad!!!!!");
            Destroy(gameObject);
        }
    }

    public void EatFood(Food food)
    {
        // Trigger Eat animation


        // Splatter food on teeth
    }
}
