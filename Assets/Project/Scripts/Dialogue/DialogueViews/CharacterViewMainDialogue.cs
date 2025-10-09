using GameCreator.Runtime.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
public class CharacterViewMainDialogue : DialogueViewBase
{
    [Tooltip("for best results, set the rectTransform anchors to middle-center, and make sure the rectTransform's pivot Y is set to 0")]
    public RectTransform[] dialogueRects;

    private Canvas canvas;

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        GameManager.Instance.GetSystem<DialogueManager>().main.SetSpeaker(dialogueLine.CharacterName);
        onDialogueLineFinished();
    }

    public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
    {
        base.RunOptions(dialogueOptions, onOptionSelected);


        GameManager.Instance.GetSystem<DialogueManager>().main.SetSpeaker(ActorRegistry.Instance.playerActor.actorName);
    }

    private void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    void Update()
    {
        if (!GameManager.Instance.GetSystem<DialogueManager>().main.IsRunning()) return;
        if(GameManager.Instance.GetSystem<DialogueManager>().main.GetSpeaker() == null) return;

        for (int i = 0; i < dialogueRects.Length; i++)
        {
            if (dialogueRects[i] != null && dialogueRects[i].gameObject.activeInHierarchy)
            {
                dialogueRects[i].anchoredPosition = ScreenEffectUtils.AnchorToWorldPosition(dialogueRects[i], GameManager.Instance.GetSystem<DialogueManager>().main.GetSpeaker().positionWithOffset, canvas, Camera.main);
            }
        }
    }
}
