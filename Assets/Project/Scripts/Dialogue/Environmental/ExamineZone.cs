using UnityEngine;

[RequireComponent(typeof(TriggerCollider))]
public class ExamineZone : MonoBehaviour
{
    [SerializeField] private string promptText = "Examine";
    [SerializeField] private TopicPlayerMonologue topic;
    [SerializeField] private Vector3 promptOffset = new Vector3(0, 2, 0);
    private TriggerCollider triggerArea;

    void Start()
    {
        triggerArea = GetComponent<TriggerCollider>();
        triggerArea.TriggerEntered += OnZoneEntered;
        triggerArea.TriggerStayed += OnZoneStayed;
        triggerArea.TriggerExited += OnZoneExited;
    }

    public void Examine()
    {
        if (DialogueManager.Instance.IsDialogueRunning())
        {
            return;
        }
        if(topic == null)
        {
            Debug.LogWarning($"ExamineZone on {gameObject.name} has no Topic assigned!");
            return;
        }
        DialogueManager.Instance.StartInnerMonologue(topic.GetTopicNodeName());
    }

    private void OnZoneEntered(Collider other)
    {
        if(DialogueManager.Instance.IsDialogueRunning())
        {
            return;
        }
        ScreenEffectsManager.Instance.ShowPrompt(promptText, this.gameObject, promptOffset);
    }

    private void OnZoneStayed(Collider other)
    {
        if (!DialogueManager.Instance.IsDialogueRunning())
        {
            ScreenEffectsManager.Instance.ShowPrompt(promptText, this.gameObject, promptOffset);
        }
        else
        {
            ScreenEffectsManager.Instance.HidePrompt();
        }
    }
    private void OnZoneExited(Collider other)
    {
        ScreenEffectsManager.Instance.HidePrompt();
    }
}
