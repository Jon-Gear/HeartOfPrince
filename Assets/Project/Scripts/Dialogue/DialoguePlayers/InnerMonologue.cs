using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class InnerMonologue : MonoBehaviour
{
    [Header("Monologue Settings")]
    public string mood = "neutral";         // e.g. "angry", "tired"
    public string location = "alley";       // e.g. "roof", "storefront"
    public string situation = "idle";       // e.g. "resting", "watching"
    public int maxVariants = 3;             // Number of monologue variants available per context

    [Header("Random Timing")]
    public float minInterval = 20f;
    public float maxInterval = 60f;
    
    void Start()
    {
        StartCoroutine(MonologueLoop());
    }

    IEnumerator MonologueLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            if(DialogueManager.Instance.IsDialogueRunning() || 
                DialogueManager.Instance.IsInnerMonologueRunning())
            {
                continue;
            }

            string nodeName = GetRandomMonologueNode();
            DialogueManager.Instance.StartInnerMonologue(nodeName);
        }
    }

    string GetRandomMonologueNode()
    {
        int index = Random.Range(1, maxVariants + 1);
        return $"monologue_test_{index}";

        //return $"monologue_{mood}_{location}_{situation}_{index}";
    }
}
