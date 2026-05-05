using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenEffectsManager
{
    [SerializeField] public CanvasGroup BasicFadeCanvasGroup;
    [SerializeField] public PromptBubble PromptBubble;

    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Example usage:
    public Task BasicFadeIn(float duration)
    {
        return ScreenEffectUtils.FadeIn(BasicFadeCanvasGroup, duration);
    }

    public Task BasicFadeOut(float duration)
    {
        return ScreenEffectUtils.FadeOut(BasicFadeCanvasGroup, duration);
    }

    public void ShowPrompt(string message, GameObject target, Vector3 targetOffset)
    {
        PromptBubble.ShowPrompt(message, target, targetOffset);
    }

    public void HidePrompt()
    {
        PromptBubble.HidePrompt();
    }

    
}




