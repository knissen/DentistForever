using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour, IOnGameStart, IOnGameEnd
{
    [SerializeField] private GameObject[] _foods;
    [SerializeField] private float _startDelay;
    [SerializeField] private float _timePerSpawn = 2f;
    [SerializeField] private Transform _foodParent;
    [SerializeField] private float _foodMoveSpeed;

    private CancellationTokenSource _cancellationTokenSource;

    public async void OnGameStart()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        await UniTask.Delay((int)(_startDelay * 1000), false, PlayerLoopTiming.Update, _cancellationTokenSource.Token);

        await UniTask.WhenAny(SpawnFood(_cancellationTokenSource.Token), MoveFood(_cancellationTokenSource.Token));
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
        }
    }

    private async UniTask SpawnFood(CancellationToken cancellationToken)
    {
        float spawnTimer = 0f;

        while (!cancellationToken.IsCancellationRequested)
        {
            await UniTask.Yield();

            spawnTimer += Time.deltaTime;

            if (spawnTimer >= _timePerSpawn)
            {
                SpawnFood();

                spawnTimer = 0f;
            }
        }
    }

    private async UniTask MoveFood(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await UniTask.Yield();

            for (int i = 0; i < _foodParent.childCount; i++)
            {
                Transform child = _foodParent.GetChild(i);

                child.position += _foodParent.forward * _foodMoveSpeed * Time.deltaTime;
            }
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
