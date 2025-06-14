using UnityEngine;
using System;
using Yarn.Unity;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class SingleOptionsBubble : DialogueViewBase
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] internal TextMeshProUGUI lineText = null;
    [SerializeField] GameObject prevOption = null;
    [SerializeField] GameObject nextOption = null;

    [SerializeField] float fadeTime = 0.1f;

    private DialogueOption[] _dialogueOptions;
    // The method we should call when an option has been selected.
    Action<int> OnOptionSelected;

    OptionView optionView;
    private int _currentOption = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        optionView = GetComponent<OptionView>();
        optionView.OnOptionSelected = OptionViewWasSelected;
    }

    public void PrevOption()
    {
        if (_currentOption <= 0)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject, null);
            return;
        }
        _currentOption -= 1;
        SetOption(_dialogueOptions[_currentOption]);
    }

    public void NextOption()
    {
        if (_currentOption >= _dialogueOptions.Length - 1)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject, null);
            return;
        }

        _currentOption += 1;
        SetOption(_dialogueOptions[_currentOption]);
    }

    void SetOption(DialogueOption newOption)
    {
        // low priority to-do: add the typewriter effect from LineView
        optionView.Option = newOption;

        if (_currentOption == 0)
        {
            prevOption.SetActive(false);
            EventSystem.current.SetSelectedGameObject(this.gameObject, null);
        }
        else
        {
            prevOption.SetActive(true);
        }

        if (_currentOption == _dialogueOptions.Length - 1)
        {
            nextOption.SetActive(false);
            EventSystem.current.SetSelectedGameObject(this.gameObject, null);
        }
        else
        {
            nextOption.SetActive(true);
        }
    }



    void OptionViewWasSelected(DialogueOption option)
    {
        StartCoroutine(OptionViewWasSelectedInternal(option));

        IEnumerator OptionViewWasSelectedInternal(DialogueOption selectedOption)
        {
            yield return StartCoroutine(FadeAndDisableOptionViews(canvasGroup, 1, 0, fadeTime));
            OnOptionSelected(selectedOption.DialogueOptionID);
        }
    }

    public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
    {
        _dialogueOptions = dialogueOptions;
        OnOptionSelected = onOptionSelected;

        _currentOption = 0;
        SetOption(_dialogueOptions[_currentOption]);

        EventSystem.current.SetSelectedGameObject(this.gameObject, null);

        StartCoroutine(Effects.FadeAlpha(canvasGroup, 0, 1, fadeTime));
    }

    public override void DialogueComplete()
    {
        // do we still have any options being shown?
        if (canvasGroup.alpha > 0)
        {
            StopAllCoroutines();
            //OnOptionSelected = null;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            _dialogueOptions = null;

            StartCoroutine(FadeAndDisableOptionViews(canvasGroup, canvasGroup.alpha, 0, fadeTime));
        }
    }

    private IEnumerator FadeAndDisableOptionViews(CanvasGroup canvasGroup, float from, float to, float fadeTime)
    {
        yield return Effects.FadeAlpha(canvasGroup, from, to, fadeTime);
    }
}
