using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameClock : MonoBehaviour, IOnGameStart, IOnGamePaused, IOnGameEnd
{
    public float gameTotalTime;

    private float _gameTimer;
    private bool _gameRunning;

    public void OnGameStart()
    {
        _gameRunning = true;
        _gameTimer = 0f;
    }

    public void SetPausedState(bool isPaused)
    {
        _gameRunning = !isPaused;
    }

    public async UniTask OnGameEnd(CancellationToken cancellationToken)
    {
        gameTotalTime = _gameTimer;

        _gameRunning = false;

        await UniTask.Yield();
    }

    private void Update()
    {
        if (_gameRunning)
            _gameTimer += Time.deltaTime;
    }
}
