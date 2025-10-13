using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using GameCreator.Runtime.Stats;
using UnityEngine;


[RequireComponent(typeof(CharacterDialogueBrain))]
public class CharacterBrain : MonoBehaviour
{
    [SerializeField] private PropertyGetGameObject characterPrefab = GetGameObjectInstance.Create();

    private TraitsOperator traits;
    private CharacterDialogueBrain characterDialogueBrain;
    private CharacterScheduleBrain characterScheduleBrain;

    public GameObject Prefab() => characterPrefab.Get(gameObject);
    public TraitsOperator Traits() => traits;
    public CharacterDialogueBrain Dialogue() => characterDialogueBrain;
    public CharacterScheduleBrain Schedule() => characterScheduleBrain;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        traits = GetComponent<TraitsOperator>();
        characterDialogueBrain = GetComponent<CharacterDialogueBrain>();
        characterScheduleBrain = GetComponent<CharacterScheduleBrain>();
    }



    // Update is called once per frame
    void Update()
    {
        
    }

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
