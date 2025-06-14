using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class Actor : MonoBehaviour
{
    [SerializeField] public string actorName = "Actor";
    [SerializeField] private List<AnimationStateEntry> animationStates = new List<AnimationStateEntry>();
    [SerializeField] private List<GestureEntry> gestures = new List<GestureEntry>();
    
    private Character character;

    public Vector3 messageBubbleOffset = new Vector3(0f, 1.8f, 0f);
    public Vector3 positionWithOffset
    {
        get
        {
            return transform.position + messageBubbleOffset;   
        }
    }


    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        character = GetComponent<Character>();

        ActorRegistry.Instance.RegisterActor(this);
        
        if (character.IsPlayer)
        {
            if (ActorRegistry.Instance.playerActor != null)
            {
                Debug.LogError("Actor Registry Error: There cannot be two player actors");
                return;
            }

            ActorRegistry.Instance.playerActor = this;
        }
    }

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


    private void OnDestroy()
    {
        ActorRegistry.Instance?.UnregisterActor(this);
    }
}
