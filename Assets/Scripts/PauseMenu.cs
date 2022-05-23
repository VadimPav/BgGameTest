using System;
using System.Collections;
using UnityEngine;
public class PauseMenu : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private Coroutine _fadeAnimation;


    public void SetPauseState(bool value)
    {
        gameObject.SetActive(value);

        if (value)
        {
            _fadeAnimation = StartCoroutine(AlphaCoroutine(0, 1f, _canvasGroup));
        }
        else
        {
            _canvasGroup.alpha = 0;
        }
    }
    private void Start()
    {
        SetPauseState(false);
    }
    private void OnDisable()
    {
        if (_fadeAnimation != null) StopCoroutine(_fadeAnimation);
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public IEnumerator AlphaCoroutine(float from, float to, CanvasGroup screen, float cooldown = 0f, Action callback = null)
    {
        yield return new WaitForSeconds(cooldown);

        float time = 0;

        while (time < 1)
        {
            yield return new WaitForSeconds(0.001f);

            time += 0.05f;
            if (screen) screen.alpha = Mathf.Lerp(from, to, time);
        }

        callback?.Invoke();
    }
}
