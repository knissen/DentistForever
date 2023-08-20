using Cysharp.Threading.Tasks;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Food : MonoBehaviour
{
    public enum FoodType { Sweet, Sticky }
    public enum SpreadType { SingleTooth, Random }

    public FoodType Type { get { return _type; } }
    public float DPS { get { return _damagePerSecond * Random.Range(0.8f, 1f); } }
    public SpreadType Spread { get { return _spreadType; } }
    public int MaxTeethAffected { get { return _maxTeethAffected; } }
    public GameObject SplatPrefab { get { return _projectorPrefab; } }

    [SerializeField] private float _damagePerSecond;
    [SerializeField] private FoodType _type;
    [SerializeField] private SpreadType _spreadType;
    [SerializeField] private int _maxTeethAffected = 5;

    [SerializeField] private ParticleSystem _eatParticleSystem;
    [SerializeField] private int _emissionCount = 20;
    [SerializeField] private GameObject _projectorPrefab;

    [SerializeField] private StudioEventEmitter _eatenEmitter;

    private Mouth _mouth;
    private bool _isBeingEaten;

    public void Init(Mouth mouthObject)
    {
        _mouth = mouthObject;
    }

    private void Update()
    {
        if (ShouldBeEaten())
        {
            _isBeingEaten = true;

            _mouth.EatFood(this);

            DestroyFood();
        }
    }

    private bool ShouldBeEaten()
    {
        if (_isBeingEaten) return false;

        return (transform.position.z < _mouth.EatFoodPosition.z);
    }

    private async void DestroyFood()
    {
        //Debug.Log("Destroying Food " + gameObject.name);

        _eatParticleSystem.Emit(_emissionCount);
        _eatenEmitter.Play();

        await UniTask.Delay((int)(_eatParticleSystem.main.duration * 1000));

        Destroy(gameObject);
    }
}
