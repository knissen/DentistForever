using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour, IOnGameEnd
{
    [SerializeField] private InputAction _openMenuAction;

    [SerializeField] private TextMeshProUGUI _gameOverMessage;
    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _quitButton;

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeTime = 0.25f;
    [SerializeField] private GameClock _gameClock;

    private CancellationTokenSource _cancellationTokenSource;
    private bool _menuOpen = false;

    private void Start()
    {
        _canvasGroup.alpha = 0f;
        _gameOverMessage.enabled = false;

        _openMenuAction.performed += OnOpenActoinPressed;
        _openMenuAction.Enable();
    }

    private void OnDestroy()
    {
        _openMenuAction.Disable();

        if (_cancellationTokenSource != null)
            _cancellationTokenSource.Cancel();
    }

    private async void OnOpenActoinPressed(InputAction.CallbackContext obj)
    {
        if (!_menuOpen)
        {
            OpenMenu(); 
        }
        else
        {
            _cancellationTokenSource.Cancel();

            await CloseMenu();
        }
    }

    private async void OpenMenu()
    {
        _menuOpen = true;

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            GameStateManager.Instance.PauseGame();

            await UIUtils.FadeCanvas(_canvasGroup, 1, _fadeTime, true);

            var closeMenuTask = UIUtils.ButtonPressed(_continueGameButton, CloseMenu);
            var restartTask = UIUtils.ButtonPressed(_restartButton, RestartGame);
            var quitTask = UIUtils.ButtonPressed(_quitButton, QuitPressed);

            var firstSelected = await Task.WhenAny(quitTask, closeMenuTask, restartTask);

            // Await in case an exception was thrown to propagate it
            await firstSelected;
            _cancellationTokenSource.Cancel();

            var thingToDo = firstSelected.Result;
            await thingToDo();
        }
        finally
        {
            _cancellationTokenSource.Dispose();
        }
    }

    private async Task CloseMenu()
    {
        _menuOpen = false;

        await UIUtils.FadeCanvas(_canvasGroup, 0, _fadeTime, false);

        GameStateManager.Instance.UnPauseGame();
    }

    private async Task RestartGame()
    {
        await UIUtils.FadeCanvas(_canvasGroup, 0, _fadeTime, false);

        GameStateManager.Instance.RestartGame();
    }

    private async Task QuitPressed()
    {
        await SceneManager.LoadSceneAsync("Title");
    }

    public async UniTask OnGameEnd(CancellationToken cancellationToken)
    {
        await UniTask.Delay(2);

        _gameOverMessage.enabled = true;
        _gameOverMessage.text = string.Format("Total Time: {0}s", Mathf.RoundToInt(_gameClock.gameTotalTime).ToString());

        OpenMenu();
    }
}
