using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class Tooth : MonoBehaviour, IOnGameStart, IOnGameEnd, IOnGamePaused
{
    public enum JawSide { Upper, Lower }

    public int RemainingHealth { get { return Mathf.RoundToInt(_currentHealth); } }

    [SerializeField] private JawSide _side = JawSide.Upper;
    [SerializeField] private float _damagePerSecond;
    [SerializeField] private float _startingHealth = 100;
    [SerializeField] private float _currentHealth;

    [Header("Splat Projectors")]
    [SerializeField] private float _projectorDistance = 2f;

    private bool _gameRunning;
    private List<Projector> _splats = new List<Projector>();

    private void Update()
    {
        if (!_gameRunning) return;

        if(_damagePerSecond > 0)
        {
            _currentHealth -= _damagePerSecond * Time.deltaTime;
        }
    }

    public void HitWithFood(GameObject projectorPrefab, float damageOverTime)
    {
        CreateSplat(projectorPrefab);

        _damagePerSecond += damageOverTime;
    }

    private void CreateSplat(GameObject projectorPrefab)
    {
        Ray rayFromCamera = new Ray(Camera.main.transform.position, transform.position - Camera.main.transform.position);

        rayFromCamera.origin += _side == JawSide.Lower ? Vector3.up * 0.1f : Vector3.down * 0.1f;

        if (Physics.Raycast(rayFromCamera, out RaycastHit hit, 5))
        {
            float randomDistnace = Random.Range(_projectorDistance * 0.5f, _projectorDistance * 1.5f);
            Vector3 projectorPos = hit.point + hit.normal * randomDistnace + Random.onUnitSphere * 0.1f;
            Quaternion lookAtToothRot = Quaternion.LookRotation(hit.point - projectorPos);

            GameObject projectorObj = Instantiate(projectorPrefab, projectorPos, lookAtToothRot, transform);

            _splats.Add(projectorObj.GetComponent<Projector>());
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

        for (int i = _splats.Count - 1; i >= 0; i--)
        {
            float currentFov = _splats[i].fieldOfView;
            _splats[i].fieldOfView = Mathf.MoveTowards(currentFov, 0f, amount * 10);

            if (_splats[i].fieldOfView <= 0)
            {
                Destroy(_splats[i].gameObject);

                _splats.RemoveAt(i);
            }
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

    public void SetPausedState(bool isPaused)
    {
        _gameRunning = !isPaused;
    }
}
