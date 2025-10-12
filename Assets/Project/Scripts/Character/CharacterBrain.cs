using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
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
}
