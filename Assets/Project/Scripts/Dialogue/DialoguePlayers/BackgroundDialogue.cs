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

    [Header("Node Template Options")]
    public List<string> nodeTemplates = new List<string>
    {
        "background_dialogue_{actor}_{index}",
        //"background_dialogue_{actor}_{location}_{index}",
        "background_dialogue_{actor}_{time}_{index}"
        //"background_dialogue_{actor}_{location}_{time}_{index}",
        //"background_dialogue_{actor}_{mood}_{index}"
    };

    [Tooltip("Minimum time between dialogues (seconds).")]
    public float minInterval = 1.0f;

    [Tooltip("Maximum time between dialogues (seconds).")]
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
        string template = nodeTemplates[Random.Range(0, nodeTemplates.Count)];

        string nodeName = template
            .Replace("{actor}", actor.actorName.ToLower())
            //.Replace("{location}", location.ToLower())
            .Replace("{time}", TimeManager.Instance.ToString(TimeManager.Instance.GetDayTime()).ToLower())
            //.Replace("{mood}", mood.ToLower())
            .Replace("{index}", index.ToString());

        return nodeName;
    }
}
