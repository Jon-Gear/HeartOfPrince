using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenEffectsManager : Singleton<ScreenEffectsManager>
{
    [SerializeField] public CanvasGroup BasicFadeCanvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Fades the CanvasGroup to a target alpha over time.
    /// </summary>
    public async Task FadeCanvasGroup(CanvasGroup cg, float targetAlpha, float duration)
    {
        float startAlpha = cg.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            await Task.Yield();
        }

        cg.alpha = targetAlpha;
    }

    // Example usage:
    public Task BasicFadeIn(float duration)
    {
        BasicFadeCanvasGroup.alpha = 1.0f;
        return FadeCanvasGroup(BasicFadeCanvasGroup, 0.0f, duration);
    }

    public Task BasicFadeOut(float duration)
    {
        BasicFadeCanvasGroup.alpha = 0.0f;
        return FadeCanvasGroup(BasicFadeCanvasGroup, 1.0f, duration);
    }
}


