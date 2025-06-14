using UnityEngine;

public class Actor : MonoBehaviour
{
    [Tooltip("This must match the character name used in Yarn dialogue scripts.")]
    public string actorName = "Actor";
    public bool isPlayerCharacter = false;

    [Tooltip("When positioning the message bubble in worldspace, YarnCharacterManager adds this additional offset to this gameObject's position. Taller characters should use taller offsets, etc.")]
    public Vector3 messageBubbleOffset = new Vector3(0f, 1.8f, 0f);

    [Tooltip("if true, then apply messageBubbleOffset relative to this transform's rotation and scale")]
    public bool offsetUsesRotation = false;

    public Vector3 positionWithOffset
    {
        get
        {
            if (!offsetUsesRotation)
            {
                return transform.position + messageBubbleOffset;
            }
            else
            {
                return transform.position + transform.TransformPoint(messageBubbleOffset); // convert offset into local space
            }
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        ActorRegistry.Instance.RegisterActor(this);
        if (isPlayerCharacter)
        {
            if (ActorRegistry.Instance.playerActor != null)
            {
                Debug.LogError("Actor Registry Error: There cannot be two player actors");
                return;
            }

            ActorRegistry.Instance.playerActor = this;
        }
    }

    private void OnDestroy()
    {
        ActorRegistry.Instance?.UnregisterActor(this);
    }
}
