using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Yapping : MonoBehaviour
{
    [Tooltip("List of Yarn node names to choose from.")]
    public List<string> backgroundNodes = new List<string>();

    [Tooltip("Minimum time between yaps (seconds).")]
    public float minInterval = 1.0f;

    [Tooltip("Maximum time between yaps (seconds).")]
    public float maxInterval = 2.0f;

    void Start()
    {
        StartCoroutine(YapLoop());
    }

    private IEnumerator YapLoop()
    {
        while (true)
        {
            // Wait a random amount of time between yaps
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // Skip if a dialogue is already running
            if (DialogueManager.Instance.IsDialogueRunning() || DialogueManager.Instance.IsBackgroundDialogueRunning())
                continue;

            // Choose a random node and start it
            if (backgroundNodes.Count > 0)
            {
                string randomNode = backgroundNodes[Random.Range(0, backgroundNodes.Count)];
                DialogueManager.Instance.StartBackgroundDialogue(randomNode);
            }
        }
    }
}
