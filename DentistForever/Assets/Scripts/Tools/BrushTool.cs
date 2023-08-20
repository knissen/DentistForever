using FMODUnity;
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

    [Header("Audio Settings")]
    [SerializeField] private StudioEventEmitter _brushEmitter;
    [SerializeField] private float _audioCoolDownRate = 0.5f;

    private float _audioTimer;

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
