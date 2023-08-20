using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using DG.Tweening;
using FMODUnity;

public class Tooth : MonoBehaviour, IOnGameStart, IOnGameEnd, IOnGamePaused, IOnGameInit
{
    public enum JawSide { Upper, Lower }
    private enum ToothState { Healthy, Shaking, Dead }

    public int RemainingHealth { get { return Mathf.RoundToInt(_currentHealth); } }

    [SerializeField] private ToothSettings _settings;

    [SerializeField] private JawSide _side = JawSide.Upper;
    [SerializeField] private float _damagePerSecond;
    [SerializeField] private float _currentHealth;

    [Header("Splat Projectors")]
    [SerializeField] private float _projectorDistance = 2f;

    [Header("Audio")]
    [SerializeField] private StudioEventEmitter _looseAudio;
    [SerializeField] private StudioEventEmitter _deadAudio;

    private ToothState _currentState;
    private bool _gameRunning;
    private List<Projector> _splats = new List<Projector>();
    private int _currentTexIndex;               // Current index into the texture array based on remaining health
    private MeshRenderer _meshRenerer;

    private void Awake()
    {
        _meshRenerer = GetComponent<MeshRenderer>();        
    }

    public async UniTask OnGameInit(CancellationToken cancellationToken)
    {
        _currentHealth = _settings.startingHealth;
        _currentTexIndex = -1;

        UpdateTextures();

        await UniTask.Yield();
    }

    public void OnGameStart()
    {
        _gameRunning = true;
    }

    public async UniTask OnGameEnd(CancellationToken cancellationToken)
    {
        _gameRunning = false;

        await UniTask.Yield();
    }

    private void Update()
    {
        if (!_gameRunning) return;

        if(_damagePerSecond > 0)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - (_damagePerSecond * Time.deltaTime), 0, _settings.maxHealth);
        }

        UpdateToothState();
    }

    private void UpdateToothState()
    {
        switch (_currentState)
        {
            case ToothState.Healthy:
                if(_currentHealth < _settings.shakingThreshold)
                    TransitionToShaking();
                UpdateTextures();
                break;
            case ToothState.Shaking:
                if (_currentHealth <= 0f)
                    TransitionToDead();
                else if (_currentHealth > _settings.shakingThreshold)
                    TransitionToHealthy();
                UpdateTextures();
                break;
            case ToothState.Dead:
                break;
            default:
                break;
        }
    }

    private void UpdateTextures()
    {
        // Get the correct index into the texture array based on current health, if health == 0 we potentially get an index too high because of float math
        int nextTexIndex = _currentHealth == 0 ? 5 : Mathf.FloorToInt((_settings.maxHealth - _currentHealth) / ((_settings.maxHealth) / _settings.albedoTextures.Length));

        if(nextTexIndex != _currentTexIndex)
        {
            if (nextTexIndex < 0 || nextTexIndex >= _settings.albedoTextures.Length)
                Debug.LogError(string.Format("Wrong Index! {0}, CurrentHealth:{1}", nextTexIndex, _currentHealth));

            _meshRenerer.material.SetTexture("_MainTex", _settings.albedoTextures[nextTexIndex]);
            _meshRenerer.material.SetTexture("_MetallicGlossMap", _settings.metallicTextures[nextTexIndex]);

            _currentTexIndex = nextTexIndex;
        }
    }

    private void TransitionToHealthy()
    {
        _currentState = ToothState.Healthy;
        _looseAudio.Stop();

        DOTween.Clear();
    }

    private void TransitionToDead()
    {
        _currentState = ToothState.Dead;

        DOTween.Clear();

        Rigidbody body = gameObject.AddComponent<Rigidbody>();

        if (_side == JawSide.Lower)
        {
            Vector3 forceDirection = ((Camera.main.transform.position - transform.position).normalized + Vector3.up * _settings.ejectUpBias).normalized * _settings.ejectForce;

            //Debug.DrawLine(transform.position, forceDirection, Color.red, 3f);

            body.AddForce(forceDirection, ForceMode.Impulse); 
        }

        _looseAudio.Stop();
        _deadAudio.Play();

        if (Random.value < _settings.nerveSpawnChance)
        {
            GameObject nerve = Instantiate(_settings.nerverPrefab, transform.position, Quaternion.identity, transform.parent);

            if (_side == JawSide.Upper)
                nerve.transform.Rotate(Vector3.forward, 180); 
        }
    }

    private void TransitionToShaking()
    {
        _currentState = ToothState.Shaking;

        transform.DOShakePosition(5f, 0.03f);
        _looseAudio.Play();
    }

    public void HitWithFood(GameObject projectorPrefab, float damageOverTime)
    {
        CreateSplat(projectorPrefab);

        _damagePerSecond = Mathf.Clamp(_damagePerSecond + damageOverTime, 0f, _settings.maxDPS);
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

            var proj = projectorObj.GetComponent<Projector>();

            _splats.Add(proj);
        }
    }

    public void RestoreHealth(float amount)
    {
        if(_damagePerSecond > 0)
        {
            _damagePerSecond = Mathf.Clamp(_damagePerSecond - amount, 0, _settings.maxHealth);
        }
        else
        {
            _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _settings.maxHealth);
        }

        for (int i = _splats.Count - 1; i >= 0; i--)
        {
            float currentFov = _splats[i].fieldOfView;
            float nextFov = Mathf.Lerp(0f, _settings.maxDPS, _damagePerSecond / _settings.maxDPS);

            if (nextFov < currentFov)
            {
                _splats[i].fieldOfView = nextFov;
            }

            if (_splats[i].fieldOfView <= 0)
            {
                Destroy(_splats[i].gameObject);

                _splats.RemoveAt(i);
            }
        }
    }

    public void SetPausedState(bool isPaused)
    {
        _gameRunning = !isPaused;
    }
}
