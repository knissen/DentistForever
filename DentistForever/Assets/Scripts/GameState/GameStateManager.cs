using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private CancellationTokenSource cancellationTokenSource;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple GameStateManagers in scene, this is bad!");
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();

        List<IOnGameInit> initComponents = new List<IOnGameInit>();

        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        for (int i = 0; i < rootObjects.Length; i++)
        {
            initComponents.AddRange(rootObjects[i].GetComponentsInChildren<IOnGameInit>());
        }

        List<UniTask> tasks = new List<UniTask>();

        for (int i = 0; i < initComponents.Count; i++)
        {
            tasks.Add(initComponents[i].OnGameInit(cancellationTokenSource.Token));
        }

        await UniTask.WhenAll(tasks);

        Debug.Log("All tasks initialized");

        List<IOnGameStart> startComponents = new List<IOnGameStart>();

        for (int i = 0; i < rootObjects.Length; i++)
        {
            startComponents.AddRange(rootObjects[i].GetComponentsInChildren<IOnGameStart>());
        }

        for (int i = 0; i < startComponents.Count; i++)
        {
            startComponents[i].OnGameStart();
        }

        Debug.Log("Gameplay started in Initializer");

        cancellationTokenSource.Dispose();
    }

    public async void EndGame()
    {
        cancellationTokenSource = new CancellationTokenSource();

        List<IOnGameEnd> endComponents = new List<IOnGameEnd>();

        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        for (int i = 0; i < rootObjects.Length; i++)
        {
            endComponents.AddRange(rootObjects[i].GetComponentsInChildren<IOnGameEnd>());
        }

        List<UniTask> tasks = new List<UniTask>();

        for (int i = 0; i < endComponents.Count; i++)
        {
            tasks.Add(endComponents[i].OnGameEnd(cancellationTokenSource.Token));
        }

        await UniTask.WhenAll(tasks);

        Debug.Log("All tasks initialized");
    }
}
