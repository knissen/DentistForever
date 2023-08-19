using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouth : MonoBehaviour, IOnGameStart
{
    public static Mouth Instance { get; private set; }

    public Vector3 EatFoodPosition { get { return _eatFoodPosition.position; } }

    [SerializeField] private Transform _eatFoodPosition;

    private bool _gameRunning;
    private Tooth[] _teeth;

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
        _teeth = GetComponentsInChildren<Tooth>();

        _gameRunning = true;
    }

    public void EatFood(Food food)
    {
        food.transform.parent = null;

        // Trigger Eat animation


        // Splatter food on teeth
    }

    private bool AllTeethDead()
    {
        for (int i = 0; i < _teeth.Length; i++)
        {
            if (_teeth[i].RemainingHealth > 0)
                return false;
        }

        return true;
    }
}
