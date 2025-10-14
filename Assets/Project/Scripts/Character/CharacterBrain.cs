using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using GameCreator.Runtime.Stats;
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

    private StateMachine mainStateMachine;
    private TraitsOperator traits;
    private CharacterDialogueBrain characterDialogueBrain;
    private CharacterScheduleBrain characterScheduleBrain;

    public GameObject Prefab() => characterPrefab.Get(gameObject);
    public TraitsOperator Traits() => traits;
    public CharacterDialogueBrain Dialogue() => characterDialogueBrain;
    public CharacterScheduleBrain Schedule() => characterScheduleBrain;
    
    
    public StateMachine MainStateMachine() => mainStateMachine;





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        traits = GetComponent<TraitsOperator>();
        characterDialogueBrain = GetComponent<CharacterDialogueBrain>();
        characterScheduleBrain = GetComponent<CharacterScheduleBrain>();
        mainStateMachine = GetComponent<StateMachine>();
    }


    public void TryStartInteraction()
    {

    }




    // I don't think it is supposed to be doing this.

    public void SpawnCharacterAtMarker(Marker marker)
    {
        if (Prefab() == null)
        {
            Debug.LogWarning($"SpawnCharacter: Character '{gameObject.name}' has no assigned prefab.");
            return;
        }

        Vector3 offset = new Vector3(0, 1, 0);

        GameObject instance = Instantiate(Prefab(), marker.transform.position + offset, marker.transform.rotation);

    }

    public void SpawnCharacterAtMarkerID(string markerID)
    {
        Marker targetMarker = Marker.GetMarkerByID(markerID);

        if (targetMarker == null)
        {
            Debug.LogWarning($"SpawnCharacter: No Marker found with ID '{markerID}'.");
            return;
        }

        SpawnCharacterAtMarker(targetMarker);
    }

    public void DespawnCharacter(Character character, bool foo)
    {
        Destroy(character.gameObject);
    }


    public void MoveCharacterToMarkerThenDespawn(Marker marker)
    {
        var actorRegistry = GameManager.Instance.GetSystem<ActorRegistry>();

        Actor actor = actorRegistry.GetActorByName(Dialogue().characterName);

        if (actor == null)
        {
            Debug.LogWarning($"{Dialogue().characterName} character not found");
            return;
        }

        actor.Character().Motion.MoveToMarker(marker, 0.1f, DespawnCharacter);
    }

    public void MoveCharacterToMarkerIDThenDespawn(string markerID)
    {
        
        Marker targetMarker = Marker.GetMarkerByID(markerID);

        if (targetMarker == null)
        {
            Debug.LogWarning($"SpawnCharacter: No Marker found with ID '{markerID}'.");
            return;
        }

        MoveCharacterToMarkerThenDespawn(targetMarker);

    }

    public void MoveCharacterToMarkerID(string markerID)
    {
        var actorRegistry = GameManager.Instance.GetSystem<ActorRegistry>();

        Actor actor = actorRegistry.GetActorByName(Dialogue().characterName);

        if (actor == null)
        {
            Debug.LogWarning($"{Dialogue().characterName} character not found");
            return;
        }

        Marker targetMarker = Marker.GetMarkerByID(markerID);

        if (targetMarker == null)
        {
            Debug.LogWarning($"SpawnCharacter: No Marker found with ID '{markerID}'.");
            return;
        }

        actor.Character().Motion.MoveToMarker(targetMarker, 0.1f, OnMarkerReached);
    }

    private void OnMarkerReached(Character character, bool foo)
    {
        Debug.Log($"{character.name} reached marker {foo}");
    }

}
