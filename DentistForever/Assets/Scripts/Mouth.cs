using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Mouth : MonoBehaviour, IOnGameStart
{
    public static Mouth Instance { get; private set; }

    public Vector3 EatFoodPosition { get { return _eatFoodPosition.position; } }

    [SerializeField] private Transform _eatFoodPosition;
    [SerializeField] private Animator _mouthAnimator;

    [SerializeField] private StudioEventEmitter _biteEmitter;

    private bool _gameRunning;
    private List<Tooth> _teeth;

    private void Awake()
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

    private void Update()
    {
        if (!_gameRunning) return;

        if (AllTeethDead())
        {
            _gameRunning = false;

            GameStateManager.Instance.EndGame();
        }
    }

    public void OnGameStart()
    {
        _teeth = GetComponentsInChildren<Tooth>().ToList();

        _gameRunning = true;
    }

    public void EatFood(Food food)
    {
        // Trigger Eat animation
        _mouthAnimator.SetTrigger("Bite");
        _biteEmitter.Play();        

        // Splatter food on teeth
        switch (food.Spread)
        {
            case Food.SpreadType.SingleTooth:
                break;
            case Food.SpreadType.Random:
                for (int i = 0; i < food.MaxTeethAffected; i++)
                {
                    if (AllTeethDead())
                        return;

                    int index = UnityEngine.Random.Range(0, _teeth.Count);
                    _teeth[index].HitWithFood(food.SplatPrefab, food.DPS);
                }
                break;
            default:
                break;
        }
    }

    private bool AllTeethDead()
    {
        for (int i = _teeth.Count - 1;  i >= 0; i--)
        {
            if (_teeth[i].RemainingHealth <= 0)
                _teeth.RemoveAt(i);
        }

        return _teeth.Count == 0;
    }
}
