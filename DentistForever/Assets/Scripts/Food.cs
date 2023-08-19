using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public enum FoodType { Sweet, Sticky }
    public enum SpreadType { SingleTooth, Random }

    public FoodType Type { get { return _type; } }
    public float DPS { get { return _damagePerSecond; } }
    public SpreadType Spread { get { return _spreadType; } }

    [SerializeField] private float _damagePerSecond;
    [SerializeField] private FoodType _type;
    [SerializeField] private SpreadType _spreadType;

    private Mouth _mouth;

    public void Init(Mouth mouthObject)
    {
        _mouth = mouthObject;
    }

    private void Update()
    {
        if (ShouldBeEaten())
        {
            _mouth.EatFood(this);
            DestroyFood();
        }
    }

    private bool ShouldBeEaten()
    {
        return (transform.position.z < _mouth.EatFoodPosition.z);
    }

    private void DestroyFood()
    {
        //Debug.Log("Destroying Food " + gameObject.name);

        Destroy(gameObject);
    }
}
