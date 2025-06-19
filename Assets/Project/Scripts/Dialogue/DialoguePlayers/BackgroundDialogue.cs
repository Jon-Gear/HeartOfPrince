using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class BackgroundDialogue: MonoBehaviour
{
    [Header("Background Dialogue Settings")]
    [SerializeField] private Actor actor;
    [SerializeField] private int maxVariants = 3;             // Number of monologue variants available per context

    [Tooltip("Minimum time between yaps (seconds).")]
    public float minInterval = 1.0f;

    [Tooltip("Maximum time between yaps (seconds).")]
    public float maxInterval = 2.0f;

    void Start()
    {
        StartCoroutine(BackgroundDialogueLoop());
    }

    private IEnumerator BackgroundDialogueLoop()
    {
        while (true)
        {
            // Wait a random amount of time between yaps
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // Skip if a dialogue is already running
            if (DialogueManager.Instance.IsDialogueRunning() || 
                DialogueManager.Instance.IsInnerMonologueRunning() || 
                DialogueManager.Instance.IsBackgroundDialogueRunning())
                continue;

            string nodeName = GetRandomBackgroundDialogueNode();
            DialogueManager.Instance.StartBackgroundDialogue(nodeName);
        }
    }

    string GetRandomBackgroundDialogueNode()
    {
        int index = Random.Range(1, maxVariants + 1);
        return $"background_dialogue_{actor.actorName}_{index}";

        //return $"dialogue_{actor.actorName}_{location}_{situation}_{index}";
    }
}
