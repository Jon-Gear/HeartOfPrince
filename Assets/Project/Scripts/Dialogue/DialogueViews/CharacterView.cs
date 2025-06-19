using GameCreator.Runtime.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;


public class CharacterView : DialogueViewBase 
{
    Camera worldCamera; // this script assumes you are using a full-screen Unity UI canvas along with a full-screen game camera

    public Canvas canvas;
    public CanvasScaler canvasScaler;

    [Tooltip("for best results, set the rectTransform anchors to middle-center, and make sure the rectTransform's pivot Y is set to 0")]
    public RectTransform dialogueBubbleRect, optionsBubbleRect, backgroundDialogueBubbleRect, innerMonologueBubbleRect;

    [Tooltip("margin is 0-1.0 (0.1 means 10% of screen space)... -1 lets dialogue bubbles appear offscreen or get cutoff")]
    public float bubbleMargin = 0.1f;


    void Awake()
    {
        worldCamera = Camera.main;
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        ActorRegistry.Instance.SetCurrentSpeaker(dialogueLine.CharacterName);
        onDialogueLineFinished();
    }

    

    /// <summary>Calculates where to put dialogue bubble based on worldPosition and any desired screen margins. 
    /// Ensure "constrainToViewportMargin" is between 0.0f-1.0f (% of screen) to constrain to screen, or value of -1 lets bubble go off-screen.</summary>
    Vector2 WorldToAnchoredPosition(RectTransform bubble, Vector3 worldPos, float constrainToViewportMargin = -1f)
    {
        Camera canvasCamera = worldCamera;
        // Canvas "Overlay" mode is special case for ScreenPointToLocalPointInRectangle (see the Unity docs)
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            canvasCamera = null;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bubble.parent.GetComponent<RectTransform>(), // calculate local point inside parent... NOT inside the dialogue bubble itself
            worldCamera.WorldToScreenPoint(worldPos),
            canvasCamera,
            out Vector2 screenPos
        );

        // to force the dialogue bubble to be fully on screen, clamp the bubble rectangle within the screen bounds
        if (constrainToViewportMargin >= 0f)
        {
            // because ScreenPointToLocalPointInRectangle is relative to a Unity UI RectTransform,
            // it may not necessarily match the full screen resolution (i.e. CanvasScaler)

            // it's not really in world space or screen space, it's in a RectTransform "UI space"
            // so we must manually convert our desired screen bounds to this UI space

            bool useCanvasResolution = canvasScaler != null && canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize;
            Vector2 screenSize = Vector2.zero;
            screenSize.x = useCanvasResolution ? canvasScaler.referenceResolution.x : Screen.width;
            screenSize.y = useCanvasResolution ? canvasScaler.referenceResolution.y : Screen.height;

            // calculate "half" values because we are measuring margins based on the center, like a radius
            var halfBubbleWidth = bubble.rect.width / 2;
            var halfBubbleHeight = bubble.rect.height / 2;

            // to calculate margin in UI-space pixels, use a % of the smaller screen dimension
            var margin = screenSize.x < screenSize.y ? screenSize.x * constrainToViewportMargin : screenSize.y * constrainToViewportMargin;

            // finally, clamp the screenPos fully within the screen bounds, while accounting for the bubble's rectTransform anchors
            screenPos.x = Mathf.Clamp(
                screenPos.x,
                margin + halfBubbleWidth - bubble.anchorMin.x * screenSize.x,
                -(margin + halfBubbleWidth) - bubble.anchorMax.x * screenSize.x + screenSize.x
            );

            screenPos.y = Mathf.Clamp(
                screenPos.y,
                margin + halfBubbleHeight - bubble.anchorMin.y * screenSize.y,
                -(margin + halfBubbleHeight) - bubble.anchorMax.y * screenSize.y + screenSize.y
            );
        }

        return screenPos;
    }

    void Update()
    {
        if (!DialogueManager.Instance.IsDialogueRunning() && !DialogueManager.Instance.IsBackgroundDialogueRunning() && !DialogueManager.Instance.IsInnerMonologueRunning()) return;

        if (dialogueBubbleRect.gameObject.activeInHierarchy && DialogueManager.Instance.IsDialogueRunning())
        {
            dialogueBubbleRect.anchoredPosition = WorldToAnchoredPosition(dialogueBubbleRect, ActorRegistry.Instance.currentSpeaker.positionWithOffset, bubbleMargin);
        }

        if (optionsBubbleRect.gameObject.activeInHierarchy && DialogueManager.Instance.IsDialogueRunning())
        {
            optionsBubbleRect.anchoredPosition = WorldToAnchoredPosition(optionsBubbleRect, ActorRegistry.Instance.playerActor.positionWithOffset, bubbleMargin);
        }

        if (innerMonologueBubbleRect.gameObject.activeInHierarchy && DialogueManager.Instance.IsInnerMonologueRunning())
        {
            innerMonologueBubbleRect.anchoredPosition = WorldToAnchoredPosition(innerMonologueBubbleRect, ActorRegistry.Instance.playerActor.positionWithOffset, bubbleMargin);
        }

        if (backgroundDialogueBubbleRect.gameObject.activeInHierarchy && DialogueManager.Instance.IsBackgroundDialogueRunning())
        {
            backgroundDialogueBubbleRect.anchoredPosition = WorldToAnchoredPosition(backgroundDialogueBubbleRect, ActorRegistry.Instance.currentSpeaker.positionWithOffset, bubbleMargin);
        }

    }
}
