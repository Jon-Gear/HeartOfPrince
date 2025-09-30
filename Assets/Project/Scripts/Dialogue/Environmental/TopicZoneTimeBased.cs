using UnityEngine;
using UnityEngine.SceneManagement;

public class TopicZoneTimeBased : MonoBehaviour
{
    [SerializeField] private TopicCharacterMonologue morningTopic;
    [SerializeField] private TopicCharacterMonologue noonTopic;
    [SerializeField] private TopicCharacterMonologue afternoonTopic;
    [SerializeField] private TopicCharacterMonologue eveningTopic;
    [SerializeField] private TopicCharacterMonologue nightTopic;

    private TriggerCollider triggerArea;
    void Start()
    {
        triggerArea = GetComponent<TriggerCollider>();
        triggerArea.TriggerEntered += OnZoneEntered;
        triggerArea.TriggerExited += OnZoneExited;
    }

    private void OnZoneEntered(Collider other)
    {
        Actor otherActor = other.gameObject.GetComponent<Actor>();
        if (otherActor == null)
        {
            return;
        }
        CharacterDialogueBrain dialogueBrain = CharacterManager.Instance.GetCharacter(otherActor.actorName);

        DAYTIME currentTime = TimeManager.Instance.GetDayTime();
        switch (currentTime)
        {
            case DAYTIME.Morning:
                dialogueBrain.AddCharacterMonologueTopic(morningTopic);
                break;
            case DAYTIME.Noon:
                dialogueBrain.AddCharacterMonologueTopic(noonTopic);
                break;
            case DAYTIME.Afternoon:
                dialogueBrain.AddCharacterMonologueTopic(afternoonTopic);
                break;
            case DAYTIME.Evening:
                dialogueBrain.AddCharacterMonologueTopic(eveningTopic);
                break;
            case DAYTIME.Night:
                dialogueBrain.AddCharacterMonologueTopic(nightTopic);
                break;
        }
    }
    private void OnZoneExited(Collider other)
    {
        Actor otherActor = other.gameObject.GetComponent<Actor>();
        if (otherActor == null)
        {
            return;
        }
        CharacterDialogueBrain dialogueBrain = CharacterManager.Instance.GetCharacter(otherActor.actorName);

        DAYTIME currentTime = TimeManager.Instance.GetDayTime();
        switch (currentTime)
        {
            case DAYTIME.Morning:
                dialogueBrain.RemoveCharacterMonologueTopic(morningTopic);
                break;
            case DAYTIME.Noon:
                dialogueBrain.RemoveCharacterMonologueTopic(noonTopic);
                break;
            case DAYTIME.Afternoon:
                dialogueBrain.RemoveCharacterMonologueTopic(afternoonTopic);
                break;
            case DAYTIME.Evening:
                dialogueBrain.RemoveCharacterMonologueTopic(eveningTopic);
                break;
            case DAYTIME.Night:
                dialogueBrain.RemoveCharacterMonologueTopic(nightTopic);
                break;
        }
    }
}
