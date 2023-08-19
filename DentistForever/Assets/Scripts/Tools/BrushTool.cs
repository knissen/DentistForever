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

    public void UseTool(Tooth tooth)
    {
        if(Mouse.current.delta.magnitude > _minDeltaToActivate)
        {
            _foamParticles.Emit(_particlesEmitRate);

            tooth.RestoreHealth(_cleaningRate * Time.deltaTime);
        }
    }
}
