using GameCreator.Runtime.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
public class CharacterViewBackgroundDialogue : DialogueViewBase 
{
    [SerializeField, Range(1, 3)]
    private int backgroundDialogueIndex = 1;
    [Tooltip("for best results, set the rectTransform anchors to middle-center, and make sure the rectTransform's pivot Y is set to 0")]
    public RectTransform dialogueRect;

    private Canvas canvas;

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        switch(backgroundDialogueIndex)
        {
            case 1:
                DialogueManager.Instance.SetBackgroundDialogueSpeaker_1(dialogueLine.CharacterName);
                break;
            case 2:
                DialogueManager.Instance.SetBackgroundDialogueSpeaker_2(dialogueLine.CharacterName);
                break;
            case 3:
                DialogueManager.Instance.SetBackgroundDialogueSpeaker_3(dialogueLine.CharacterName);
                break;
        }

        ActorRegistry.Instance.SetBackgroundDialogueCurrentSpeaker(dialogueLine.CharacterName);
        onDialogueLineFinished();
    }

    private void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    void Update()
    {
        if(!DialogueManager.Instance.IsAnyBackgroundDialogueRunning()) return;

        if (dialogueRect != null && dialogueRect.gameObject.activeInHierarchy)
        {
            dialogueRect.anchoredPosition = ScreenEffectUtils.AnchorToWorldPosition(dialogueRect, ActorRegistry.Instance.backgroundDialogueCurrentSpeaker.positionWithOffset, canvas, Camera.main);
        }
    }
}
