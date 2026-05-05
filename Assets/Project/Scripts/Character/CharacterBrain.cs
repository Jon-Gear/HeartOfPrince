using GameCreator.Runtime.Common;

using Tools;
using UnityEngine;



/*

What the character can be doing:

Nothing
Talking




None,                // free to start something
ToPlayer,            // character intends to talk to the player
ToCharacter,         // character intends to talk to another NPC
Monologue,           // character intends to monologue
SpokenTo,            // character has been spoken to and is responding
ApproachingPlayer,   // character is approaching the player to start dialogue



*/

public enum InteractionType
{
    // Core
    Idle,
    Walking,
    ApproachingPlayer,
    EnteringScene,
    ExitingScene,

    // Dialogue
    TalkingToPlayer,
    TalkingToCharacter,
    Monologue,
    SpokenTo,
    Greeting,

    // Task / Schedule
    PerformingTask,
    Working,
    Praying,
    Eating,
    Socializing,
    Sleeping,

    // Reactive / Expressive
    Reacting,
    Observing,
    Thinking,

    // System / Control
    Disabled,
    CutsceneControlled,
}




[RequireComponent(typeof(CharacterDialogueBrain))]
public class CharacterBrain : MonoBehaviour
{
    [SerializeField] private PropertyGetGameObject characterPrefab = GetGameObjectInstance.Create();


    private CharacterDialogueBrain characterDialogueBrain;


    public GameObject Prefab() => characterPrefab.Get(gameObject);
    public CharacterDialogueBrain Dialogue() => characterDialogueBrain;
    
    public Actor Actor()
    {
        return GameManager.Instance.GetSystem<CharacterManager>().GetActorByName(Dialogue().characterName);
    }


    void Start()
    {

        characterDialogueBrain = GetComponent<CharacterDialogueBrain>();
    }

    public void SpawnActor(Marker spawnMarker)
    {
        if (Prefab() == null)
        {
            Debug.LogWarning($"SpawnCharacter: Character '{gameObject.name}' has no assigned prefab.");
            return;
        }

        Vector3 offset = new Vector3(0, 1, 0);
        Instantiate(Prefab(), spawnMarker.transform.position + offset, spawnMarker.transform.rotation);
    }


    public void DespawnActor()
    {
        Actor actor = Actor();
        if(actor != null) 
        {
            Destroy(actor.gameObject);
        }
    }
}
