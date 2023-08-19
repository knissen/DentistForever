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

    public void AddSplat(GameObject projectorPrefab, float damageOverTime)
    {
        //Debug.Log("Tooth position " + transform.position);
        //Debug.DrawLine(Camera.main.transform.position, transform.position, Color.red, 3f);

        Ray rayFromCamera = new Ray(Camera.main.transform.position, transform.position - Camera.main.transform.position);

        rayFromCamera.origin += _side == JawSide.Lower ? Vector3.up * 0.15f : Vector3.down * 0.15f;

        if(Physics.Raycast(rayFromCamera, out RaycastHit hit, 5))
        {
            Debug.Log("Hit Something!");
            //Debug.DrawLine(hit.point, hit.normal, Color.red, 3f);

            Vector3 projectorPos = hit.point + hit.normal * _projectorDistance + Random.onUnitSphere * 0.1f;
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
