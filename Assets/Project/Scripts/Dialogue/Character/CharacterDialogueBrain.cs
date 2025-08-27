using GameCreator.Runtime.Behavior;
using GameCreator.Runtime.Dialogue;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/DialogueTopic")]
public class DialogueTopic : ScriptableObject
{
    public string TopicName;
    public int maxVariants = 3; // Number of dialogue variants available for this topic
    
    public string GetTopicNodeName()
    {
        string nodeName = "";

        nodeName += "{actor}_dialogue_";

        nodeName += TopicName.ToLower();

        if (maxVariants > 0)
        {
            int index = Random.Range(1, maxVariants + 1);
            nodeName += $"_{index}";
        }

        return nodeName;
    }
}

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/BackgroundDialogueTopic")]
public class BackgroundDialogueTopic : ScriptableObject
{
    public string TopicName;
    public int maxVariants = 3; // Number of dialogue variants available for this topic

    [Space]
    [Header("Contextual Factors")]
    public bool isTimeBased = false; // Whether this topic is time-based (e.g., morning, afternoon, night)
    public bool isWeatherBased = false; // Whether this topic is weather-based
    public bool isLocationBased = false; // Whether this topic is location-based
    public bool isDependentOnWhoIsNearby = false; // Whether this topic depends on who is nearby
    public List<String> specificCharactersNearby = new List<string>(); // Specific characters that trigger this topic

    public string GetTopicNodeName()
    {
        string nodeName = "";

        nodeName += "{actor}_background_dialogue_";

        nodeName += TopicName.ToLower();

        if(isTimeBased)
        {
            nodeName += "_{time}";
        }

        if(isWeatherBased)
        {
            nodeName += "_{weather}";
        }

        if(isLocationBased)
        {
            nodeName += "_{location}";
        }

        if(isDependentOnWhoIsNearby && specificCharactersNearby.Count > 0)
        {
            // Randomly choose one of the specific characters to include in the node name
            string character = specificCharactersNearby[Random.Range(0, specificCharactersNearby.Count)];
            nodeName += $"_with_{character.ToLower()}";
        }

        if (maxVariants > 0)
        {
            int index = Random.Range(1, maxVariants + 1);
            nodeName += $"_{index}";
        }

        return nodeName;
    }
}



public class CharacterDialogueBrain : MonoBehaviour
{
    [SerializeField] private string characterName = "Character";

    [SerializeField] private List<DialogueTopic> dialogueTopics = new List<DialogueTopic>();
    [SerializeField] private List<BackgroundDialogueTopic> backgroundDialogueTopics = new List<BackgroundDialogueTopic>();


    private Coroutine backgroundDialogueLoop;

    [Tooltip("Minimum time between background dialogues (seconds).")]
    [SerializeField] private float minInterval = 1.0f;

    [Tooltip("Maximum time between background dialogues (seconds).")]
    [SerializeField] private float maxInterval = 2.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartDialogue()
    {
        if (DialogueManager.Instance.IsDialogueRunning())
        {
            return;
        }

        DialogueManager.Instance.StartDialogue(ChooseDialogueNode());
    }

    public void StartBackgroundDialogueLoop()
    {
        backgroundDialogueLoop = StartCoroutine(BackgroundDialogueLoop());
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
            {
                continue;
            }

            DialogueManager.Instance.StartBackgroundDialogue(ChooseBackgroundDialogueNode());
        }
    }

    public void StopBackgroundDialogueLoop()
    {
        Debug.Log("Stopping background dialogue loop for " + characterName);
        if (backgroundDialogueLoop != null)
        { 
            StopCoroutine(backgroundDialogueLoop);
        }
    }



    private string ChooseDialogueNode()
    {
        string nodeName = dialogueTopics[Random.Range(0, dialogueTopics.Count)].GetTopicNodeName().Replace("{actor}", characterName.ToLower());
        return nodeName;
    }

    private string ChooseBackgroundDialogueNode()
    {
        string nodeName = backgroundDialogueTopics[Random.Range(0, backgroundDialogueTopics.Count)].GetTopicNodeName().Replace("{actor}", characterName.ToLower());
        return nodeName;
    }


    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        StopBackgroundDialogueLoop();
    }
}