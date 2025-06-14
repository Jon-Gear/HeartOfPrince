using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class Actor : MonoBehaviour
{
    [SerializeField] public string actorName = "Actor";
    [SerializeField] private List<GestureEntry> gestures = new List<GestureEntry>();


    [SerializeField ]private Character character;

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

    public void Emote(string emoteName)
    {
        GestureEntry gesture = gestures.FirstOrDefault(g => g.m_Name == emoteName);
        
        Debug.Log($"Emote called: {emoteName} for actor: {actorName}");

        Debug.Log($"Gesture found: {gesture != null} with name {gesture.m_Name}");


        if (gesture == null)
        {
            Debug.LogError($"Emote '{emoteName}' not found for actor '{actorName}'");
            return;
        }

        gesture.PlayGesture(character, null);
    }

    private void OnDestroy()
    {
        ActorRegistry.Instance?.UnregisterActor(this);
    }
}
