using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIUtils
{
    public static async Task<bool> FadeCanvas(CanvasGroup canvasGroup, float targetAlpha, float time, bool interactable)
    {
        float startValue = canvasGroup.alpha;
        float timer = 0f;

        while (timer < time)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startValue, targetAlpha, timer / time);

            await Task.Delay(1);
        }

        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;

        return true;
    }

    public static async Task<Task<Button>> SelectButton(CancellationToken ct, params Button[] buttons)
    {
        var tasks = buttons.Select((btn) => ButtonPressed(ct, btn));
        Task<Button> firstFinished = await Task.WhenAny(tasks);

        return firstFinished;
    }

    private static async Task<Button> ButtonPressed(CancellationToken ct, Button button)
    {
        bool isPressed = false;
        button.onClick.AddListener(() => isPressed = true);

        while (!isPressed && !ct.IsCancellationRequested)
            await Task.Yield();

        return ct.IsCancellationRequested ? null : button;
    }

    public static async Task<Func<Task>> ButtonPressed(Button button, Func<Task> actionOnButton)
    {
        bool isPressed = false;
        button.onClick.AddListener(() => isPressed = true);

        while (!isPressed)
            await Task.Yield();

        return actionOnButton;
    }

    public static Vector3 GetMouseWorldPosition(Camera cam, float zDistance = 20)
    {
        Vector3 mousePos = (Vector3)Mouse.current.position.ReadValue() + new Vector3(0, 0, zDistance);
        return cam.ScreenToWorldPoint(mousePos);
    }
}
