using GameCreator.Runtime.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
public class CharacterViewBackgroundDialogue 
{
    /*
    [SerializeField, Range(1, 3)]
    private int backgroundDialogueIndex = 1;
    [Tooltip("for best results, set the rectTransform anchors to middle-center, and make sure the rectTransform's pivot Y is set to 0")]
    public RectTransform dialogueRect;
    private Dialogue dialogue;

    private Canvas canvas;

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        dialogue.SetSpeaker(dialogueLine.CharacterName);
        onDialogueLineFinished();
    }

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        switch (backgroundDialogueIndex)
        {
            case 1:
                dialogue = GameManager.Instance.GetSystem<DialogueManager>().background_1;
                break;
            case 2:
                dialogue = GameManager.Instance.GetSystem<DialogueManager>().background_2;
                break;
            case 3:
                dialogue = GameManager.Instance.GetSystem<DialogueManager>().background_3;
                break;
        }
    }

    void Update()
    {
        if (!dialogue.IsRunning()) return;
        if (dialogue.GetSpeaker() == null) return;


        if (dialogueRect != null && dialogueRect.gameObject.activeInHierarchy)
        {
            dialogueRect.anchoredPosition = ScreenEffectUtils.AnchorToWorldPosition(dialogueRect, dialogue.GetSpeaker().positionWithOffset, canvas, Camera.main);
        }
    }
    */
}
