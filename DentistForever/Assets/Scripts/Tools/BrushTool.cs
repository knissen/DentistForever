using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BrushTool : MonoBehaviour, IToothTool
{
    [SerializeField] private ParticleSystem _foamParticles;
    [SerializeField] private int _particlesEmitRate;
    [SerializeField] private float _cleaningRate = 1f;
    [SerializeField] private float _minDeltaToActivate = 0.5f;
    [SerializeField] private Transform _brushTransform;

    [Header("Audio Settings")]
    [SerializeField] private StudioEventEmitter _brushEmitter;
    [SerializeField] private float _audioCoolDownRate = 0.5f;

    private float _audioTimer;
    private float _camX;
    private float _camY;

    private void Start()
    {
        _camX = Camera.main.transform.position.x;
        _camY = Camera.main.transform.position.y;
    }

    private void Update()
    {
        AlignTool();
    }

    private void AlignTool()
    {
        bool isLeft = transform.position.x < _camX;
        bool isUp = transform.position.y > _camY;

        Vector3 lookPoint = isLeft ? new Vector3(1, 0, 4) : new Vector3(6, 0, 3);
        Vector3 rotUpDir = isUp ? Vector3.down : Vector3.up;

        Quaternion rotation = Quaternion.LookRotation(lookPoint - transform.position, rotUpDir);

        _brushTransform.rotation = rotation;
    }

    public void UseTool(Tooth tooth)
    {
        if(Mouse.current.delta.magnitude > _minDeltaToActivate)
        {
            _foamParticles.Emit(_particlesEmitRate);

            if (_audioTimer <= 0f && !_brushEmitter.IsPlaying())
            {
                _brushEmitter.Play();
                _audioTimer = 1f;
            }
            if (_brushEmitter.IsPlaying())
                _audioTimer = 1f;

            tooth.RestoreHealth(_cleaningRate * Time.deltaTime);
        }
        else
        {
            DecayAudio();
        }
    }

    public void OffTooth()
    {
        DecayAudio();
    }

    private void DecayAudio()
    {
        _audioTimer = Mathf.MoveTowards(_audioTimer, 0f, _audioCoolDownRate * Time.deltaTime);

        if (_audioTimer <= 0f && _brushEmitter.IsPlaying())
        {
            _brushEmitter.Stop();
            _audioTimer = 1f;
        }
    }
}
