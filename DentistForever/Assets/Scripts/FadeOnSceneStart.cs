using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FadeOnSceneStart : MonoBehaviour, IOnGameInit
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeTime;

    private void Awake()
    {
        _canvasGroup.alpha = 1f;
    }

    public async UniTask OnGameInit(CancellationToken cancellationToken)
    {
        _canvasGroup.alpha = 1;

        float timer = 0f;

        while(timer < _fadeTime && !cancellationToken.IsCancellationRequested)
        {
            _canvasGroup.alpha = Mathf.Lerp(1, 0, timer / _fadeTime);

            timer += Time.deltaTime;

            await UniTask.Yield(cancellationToken);
        }
    }
}
