using UnityEngine;

public class CharacterDialogue : MonoBehaviour
{
    [Header("Character Dialogue Settings")]
    [SerializeField] private Actor actor;
    public int maxVariants = 3;             // Number of monologue variants available per context

    public void Talk()
    {
        if (DialogueManager.Instance.IsDialogueRunning())
        {
            return;
        }

        string nodeName = GetRandomDialogueNode();
        DialogueManager.Instance.StartDialogue(nodeName);
    }

    string GetRandomDialogueNode()
    {
        int index = Random.Range(1, maxVariants + 1);
        return $"dialogue_{actor.actorName.ToLower()}_{index}";

        //return $"dialogue_{actor.actorName}_{location}_{situation}_{index}";
    }
}
