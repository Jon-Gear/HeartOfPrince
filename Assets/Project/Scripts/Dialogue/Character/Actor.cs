using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class Actor : MonoBehaviour
{
    [SerializeField] public string actorName = "Actor";
    
    private Character character;
    private CharacterBrain characterBrain;

    public Character Character() => character;
    public CharacterBrain Brain() => characterBrain;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        character = GetComponent<Character>();

        characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(actorName);
        var actorRegistry = GameManager.Instance.GetSystem<ActorRegistry>();


        actorRegistry.RegisterActor(this);
        
        if (character.IsPlayer)
        {
            if (actorRegistry.playerActor != null)
            {
                Debug.LogError("Actor Registry Error: There cannot be two player actors");
                return;
            }

            actorRegistry.playerActor = this;
        }
    }




    private void OnDestroy()
    {
        GameManager.Instance.GetSystem<ActorRegistry>().UnregisterActor(this);
    }
}
