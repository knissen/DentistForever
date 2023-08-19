using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _quitButton;

    public async void Start()
    {
        CancellationTokenSource source = new CancellationTokenSource();

        try
        {
            var quitTask = UIUtils.ButtonPressed(_quitButton, QuitPressed);
            var gameStartTask = UIUtils.ButtonPressed(_startGameButton, StartGame);

            var firstSelected = await Task.WhenAny(quitTask, gameStartTask);

            // Await in case an exception was thrown to propagate it
            await firstSelected;
            source.Cancel();

            var thingToDo = firstSelected.Result;
            await thingToDo();
        }
        finally
        {
            source.Dispose();
        }
    }

    public async Task StartGame()
    {
        await SceneManager.LoadSceneAsync("Main");
    }

    private async Task QuitPressed()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
        await UniTask.Yield();
    }
}
