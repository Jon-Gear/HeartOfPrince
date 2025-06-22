using GameCreator.Runtime.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class InnerMonologue : MonoBehaviour
{
    [Header("Inner Monologue Settings")]
    [SerializeField] private Actor actor;

    [Tooltip("Minimum time between monologues (seconds).")]
    public float minInterval = 10.0f;

    [Tooltip("Maximum time between monologues (seconds).")]
    public float maxInterval = 20.0f;

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

            string nodeName = actor.GetRandomThought();
            DialogueManager.Instance.StartInnerMonologue(nodeName);
        }
    }
}
