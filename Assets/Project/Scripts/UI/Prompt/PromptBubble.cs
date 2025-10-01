using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromptBubble : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform bubbleTransform;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;
    private Transform targetTransform;
    private Vector3 targetOffset;

    private bool isUsed = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isUsed = true;
        HidePrompt();
    }

    // Update is called once per frame
    void Update()
    {
        if(targetTransform == null) return;

        bubbleTransform.anchoredPosition = ScreenEffectUtils.AnchorToWorldPosition(bubbleTransform, targetTransform.position + targetOffset, canvas, Camera.main);
    }
    public async void ShowPrompt(string message, GameObject target, Vector3 offset = default)
    {
        if (isUsed) return;

        isUsed = true;
        text.text = message;
        targetTransform = target.transform;
        targetOffset = offset;
        await ScreenEffectUtils.FadeOut(canvasGroup, 0.2f);
    }

    public async void HidePrompt()
    {
        if(!isUsed) return;

        await ScreenEffectUtils.FadeIn(canvasGroup, 0.2f);
        targetTransform = null;
        text.text = "";
        isUsed = false;
    }
}
