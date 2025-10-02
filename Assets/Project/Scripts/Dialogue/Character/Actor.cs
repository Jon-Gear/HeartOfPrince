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


    public Vector3 messageBubbleOffset = new Vector3(0f, 1.0f, 0f);
    public Vector3 positionWithOffset
    {
        get
        {
            return transform.position + messageBubbleOffset;   
        }
    }
    public Character Character() => character;
    public CharacterBrain Brain() => characterBrain;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        character = GetComponent<Character>();

        characterBrain = CharacterManager.Instance.GetCharacter(actorName);

        ActorRegistry.Instance.RegisterActor(this);
        
        if (character.IsPlayer)
        {
            if (ActorRegistry.Instance.playerActor != null)
            {
                Debug.LogError("Actor Registry Error: There cannot be two player actors");
                return;
            }

            ActorRegistry.Instance.playerActor = this;
            CinemachineManager.Instance.targetGroup.AddMember(character.transform, 1f, 0.5f) ;
            CinemachineManager.Instance.longShot.PreviousStateIsValid = false;
        }
    }


    /*
    public void Gesture(string gestureName)
    {
        GestureEntry gesture = gestures.FirstOrDefault(g => g.m_Name == gestureName);
        
        Debug.Log($"Emote called: {gestureName} for actor: {actorName}");

        Debug.Log($"Gesture found: {gesture != null} with name {gesture.m_Name}");


        if (gesture == null)
        {
            Debug.LogError($"Emote '{gestureName}' not found for actor '{actorName}'");
            return;
        }

        gesture.PlayGesture(character, null);
    }

    public void EnterState(string animationStateName)
    {
        AnimationStateEntry animationState = animationStates.FirstOrDefault(s => s.m_Name == animationStateName);

        if(animationState == null)
        {
            Debug.LogError($"Animation state '{animationStateName}' not found for actor '{actorName}'");
            return;
        }
        animationState.EnterAnimationState(character, null);
    }
    
    public void ExitState(string animationStateName)
    {
        AnimationStateEntry animationState = animationStates.FirstOrDefault(s => s.m_Name == animationStateName);
        if(animationState == null)
        {
            Debug.LogError($"Animation state '{animationStateName}' not found for actor '{actorName}'");
            return;
        }
        animationState.ExitAnimationState(character, null);
    }
    */


    private void OnDestroy()
    {
        if (ActorRegistry.IsQuitting) return;

        CinemachineManager.Instance.targetGroup.RemoveMember(character.transform);

        ActorRegistry.Instance.UnregisterActor(this);
    }
}
