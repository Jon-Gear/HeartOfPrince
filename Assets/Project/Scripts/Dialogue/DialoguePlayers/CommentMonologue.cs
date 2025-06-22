using UnityEngine;

public class CommentMonologue : MonoBehaviour
{
    [Header("Comment Monologue Settings")]
    public string subject = "default";
    public int maxVariants = 3;             // Number of monologue variants available per context

    public void Comment()
    {
        if (DialogueManager.Instance.IsDialogueRunning() ||
            DialogueManager.Instance.IsInnerMonologueRunning())
        {
            return;
        }

        string nodeName = GetRandomMonologueNode();
        DialogueManager.Instance.StartInnerMonologue(nodeName);
    }

    string GetRandomMonologueNode()
    {
        int index = Random.Range(1, maxVariants + 1);
        return $"comment_{subject}_{index}";
    }
}
