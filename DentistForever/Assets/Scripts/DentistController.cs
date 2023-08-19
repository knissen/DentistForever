using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DentistController : MonoBehaviour, IOnGameStart, IOnGameEnd
{
    [SerializeField] private LayerMask _collisionLayers;
    [SerializeField] private Transform _toolParent;
    [SerializeField] private float _maxZDistance;
    [SerializeField] private bool _enabled;

    private Vector3 _CameraPosition;
    private Vector3 _ZDistance;
    private RaycastHit[] _hitBuffer = new RaycastHit[10];
    private Ray _rayToMouse = new Ray();
    private IToothTool _selectedTool;

    public void OnGameStart()
    {
        _CameraPosition = Camera.main.transform.position;
        _ZDistance = new Vector3(0, 0, _maxZDistance);

        _rayToMouse.origin = _CameraPosition;

        _selectedTool = _toolParent.GetComponentInChildren<IToothTool>();

        _enabled = true;

        Debug.Log("Dentist Enabled");
    }

    public async UniTask OnGameEnd(CancellationToken cancellationToken)
    {
        _enabled = false;

        await UniTask.Yield();
    }

    private void Update()
    {
        if (!_enabled) return;

        MoveToolToPointer();
    }

    private void MoveToolToPointer()
    {
        _ZDistance = new Vector3(0, 0, _maxZDistance);

        Vector3 mousePos = Input.mousePosition + _ZDistance;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

        _rayToMouse.direction = Vector3.Normalize(mouseWorldPos - _CameraPosition);

        int hits = Physics.RaycastNonAlloc(_rayToMouse, _hitBuffer, _maxZDistance, _collisionLayers);

        if(hits > 0)
        {
            _toolParent.position = _hitBuffer[0].point;

            if(_hitBuffer[0].transform.gameObject.TryGetComponent(out Tooth tooth))
            {
                _selectedTool.UseTool(tooth);
            }
        }
        else
        {
            _toolParent.position = mouseWorldPos;
        }
    }
}