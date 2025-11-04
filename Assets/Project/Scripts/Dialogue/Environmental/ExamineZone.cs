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
        //if (GameManager.Instance.GetSystem<DialogueManager>().main.IsRunning())
        //{
        //    return;
        //}
        //if(topic == null)
        //{
        //    Debug.LogWarning($"ExamineZone on {gameObject.name} has no Topic assigned!");
        //    return;
        //}
        //GameManager.Instance.GetSystem<DialogueManager>().StartDialogue(topic.GetTopicNodeName());
    }

    private void OnZoneEntered(Collider other)
    {
        //if (GameManager.Instance.GetSystem<DialogueManager>().main.IsRunning())
        //{
        //    return;
        //}
        //GameManager.Instance.GetSystem<ScreenEffectsManager>().ShowPrompt(promptText, this.gameObject, promptOffset);
    }

    private void OnZoneStayed(Collider other)
    {
        //if (!GameManager.Instance.GetSystem<DialogueManager>().main.IsRunning())
        //{
        //    GameManager.Instance.GetSystem<ScreenEffectsManager>().ShowPrompt(promptText, this.gameObject, promptOffset);
        //}
        //else
        //{
        //    GameManager.Instance.GetSystem<ScreenEffectsManager>().HidePrompt();
        //}
    }
    private void OnZoneExited(Collider other)
    {
    //    GameManager.Instance.GetSystem<ScreenEffectsManager>().HidePrompt();
    }
}
