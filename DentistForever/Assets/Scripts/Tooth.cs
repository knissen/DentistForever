using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Tooth : MonoBehaviour, IOnGameStart, IOnGameEnd
{
    public int RemainingHealth { get { return Mathf.RoundToInt(_currentHealth); } }

    [SerializeField] private float _damagePerSecond;
    [SerializeField] private float _startingHealth = 100;
    [SerializeField] private float _currentHealth;

    private bool _gameRunning;

    private void Update()
    {
        if (!_gameRunning) return;

        if(_damagePerSecond > 0)
        {
            _currentHealth -= _damagePerSecond * Time.deltaTime;
        }
    }

    public void RestoreHealth(float amount)
    {
        if(_damagePerSecond > 0)
        {
            _damagePerSecond = Mathf.Clamp(_damagePerSecond - amount, 0, _startingHealth);
        }
        else
        {
            _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _startingHealth);
        }
    }

    public void OnGameStart()
    {
        _currentHealth = _startingHealth;

        _gameRunning = true;
    }

    public async UniTask OnGameEnd(CancellationToken cancellationToken)
    {
        _gameRunning = false;

        await UniTask.Yield();
    }
}
