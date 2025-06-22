using GameCreator.Runtime.Behavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class BackgroundDialogue: MonoBehaviour
{
    [Header("Background Dialogue Settings")]
    [SerializeField] private Actor actor;

    [SerializeField] private List<ThoughtData> defaultThoughts = new List<ThoughtData>() 
    {
        new ThoughtData("background_dialogue_{actor}", 3), 
        new ThoughtData("background_dialogue_{actor}_{location}", 3), 
        new ThoughtData("background_dialogue_{actor}_{time}", 3), 
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

            string nodeName = ChooseDialogueNode();
            
            DialogueManager.Instance.StartBackgroundDialogue(nodeName);
        }
    }

    private string ChooseDialogueNode()
    {
        string randomThought = actor.GetRandomThought();
        if (!string.IsNullOrEmpty(randomThought))
        {
            return randomThought;
        }


        ThoughtData thought = defaultThoughts[Random.Range(0, defaultThoughts.Count)];

        string thoughtNodeName = thought.GetThoughtNodeName();


        string nodeName = thoughtNodeName
            .Replace("{actor}", actor.actorName.ToLower())
            //.Replace("{location}", location.ToLower())
            .Replace("{time}", TimeManager.Instance.ToString(TimeManager.Instance.GetDayTime()).ToLower());
        //.Replace("{mood}", mood.ToLower());
        return nodeName;

    }
}
