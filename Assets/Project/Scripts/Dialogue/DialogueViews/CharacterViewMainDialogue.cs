using GameCreator.Runtime.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.SceneManagement;
using System.Threading;

[RequireComponent(typeof(Canvas))]
public class CharacterViewMainDialogue
{
    /**
    [Tooltip("for best results, set the rectTransform anchors to middle-center, and make sure the rectTransform's pivot Y is set to 0")]
    public RectTransform[] dialogueRects;

    private Canvas canvas;

    public override YarnTask OnDialogueCompleteAsync()
    {
        throw new NotImplementedException();
    }

    public override YarnTask OnDialogueStartedAsync()
    {
        throw new NotImplementedException();
    }


    public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
        var dialogueManager = GameManager.Instance.GetSystem<DialogueManager>();
        dialogueManager.main.SetSpeaker(line.CharacterName);
        dialogueManager.main.RunLine();

        onDialogueLineFinished();
    }

    public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
    {
    }

    public override YarnTask<DialogueOption> RunOptionsAsync(DialogueOption[] dialogueOptions, CancellationToken cancellationToken)
    {
        base.RunOptions(dialogueOptions, onOptionSelected);
        var CharacterManager = GameManager.Instance.GetSystem<CharacterManager>();


        GameManager.Instance.GetSystem<DialogueManager>().main.SetSpeaker(CharacterManager.playerActor.actorName);

        return YarnTask<DialogueOption>.FromResult(null);

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
    /**/
}
