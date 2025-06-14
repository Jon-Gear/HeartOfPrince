using UnityEngine;
using Yarn.Unity;

public class YarnCommands : MonoBehaviour
{

    [YarnCommand("emote")]
    public static void Emote(string actorName, string emoteName)
    {
        // Find the actor by name
        Actor actor = ActorRegistry.Instance.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }

        // Trigger the emote on the actor
        actor.Emote(emoteName);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
