using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour, IOnGameStart, IOnGameEnd, IOnGamePaused
{
    [SerializeField] private GameObject[] _foods;
    [SerializeField] private float _startDelay;
    [SerializeField] private float _timePerSpawn = 2f;
    [SerializeField] private Transform _foodParent;
    [SerializeField] private float _foodMoveSpeed;

    private CancellationTokenSource _cancellationTokenSource;
    private bool _gamePaused;

    public async void OnGameStart()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        await UniTask.Delay((int)(_startDelay * 1000), false, PlayerLoopTiming.Update, _cancellationTokenSource.Token);

        await UniTask.WhenAny(SpawnFood(_cancellationTokenSource.Token), MoveFood(_cancellationTokenSource.Token));
    }

    public void SetPausedState(bool isPaused)
    {
        _gamePaused = isPaused;
    }

    public async UniTask OnGameEnd(CancellationToken cancellationToken)
    {
        if(_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }

        await UniTask.Yield();

        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        _gamePaused = false;
    }

    private void OnDestroy()
    {
        if (_cancellationTokenSource != null)
            _cancellationTokenSource.Cancel();
    }

    private async UniTask SpawnFood(CancellationToken cancellationToken)
    {
        float spawnTimer = _timePerSpawn - 1f;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (!_gamePaused)
            {
                spawnTimer += Time.deltaTime;

                if (spawnTimer >= _timePerSpawn)
                {
                    SpawnFood();

                    spawnTimer = 0f;
                } 
            }

            await UniTask.Yield();
        }
    }

    private async UniTask MoveFood(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!_gamePaused)
            {
                for (int i = 0; i < _foodParent.childCount; i++)
                {
                    Transform child = _foodParent.GetChild(i);

                    child.position += _foodParent.forward * _foodMoveSpeed * Time.deltaTime;
                } 
            }

            await UniTask.Yield();
        }
    }

    private void SpawnFood()
    {
        GameObject foodToSpawn = _foods[Random.Range(0, _foods.Length - 1)];

        GameObject foodObject = Instantiate(foodToSpawn, _foodParent.position, Quaternion.identity, _foodParent);

        if(foodObject.TryGetComponent(out Food fd))
        {
            fd.Init(Mouth.Instance);
        }
    }
}
