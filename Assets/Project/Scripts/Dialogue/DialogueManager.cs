using UnityEngine;
using Yarn.Unity;



public class DialogueManager : GameSystem 
{
    [SerializeField] private DialogueRunner primary;
    [SerializeField] private DialogueRunner secondary;
    public DialogueRunner Primary() => primary;
    bool PlayerMonologue;
    bool CharacterToPlayer;
    bool PlayerToCharacter;

    public DialogueRunner Secondary() => secondary;
    bool CharacterMonologue;
    bool CharacterToCharacter;


    public override void Init()
    {
    }

    public override void Shutdown()
    {
    }

    public void SetPlayerMonologue()
    {
        PlayerMonologue = true;
        CharacterToPlayer = false;
        PlayerToCharacter = false;
    }




    [YarnFunction("PlayerToCharacter")]
    public static bool IsPlayerToCharacter() 
    {
        return true;
    }

    [YarnFunction("CharacterToPlayer")]
    public static bool IsCharacterToPlayer()
    {
        return false;
    }

    [YarnFunction("CharacterMonologue")]
    public static bool IsCharacterMonologue()
    {
        return false;
    }

    [YarnFunction("PlayerMonologue")]
    public static bool IsPlayerMonologue()
    {
        return false;
    }

    [YarnFunction("CharacterToCharacter")]
    public static bool IsCharacterToCharacter()
    {
        return false;
    }






    [YarnFunction("CurrentActor")]
    public static bool CurrentActor(string actorName)
    {
        if (actorName == "Munir")
        { 
            return true;
        }
        else
        {
            return false;
        }
        //GameManager.Instance.GetSystem<DialogueManager>().currentActor?.actorName;
    }

    [YarnFunction("GetCurrentActor")]
    public static string CurrentActor()
    {
        return "Munir";
        //GameManager.Instance.GetSystem<DialogueManager>().currentActor?.actorName;
    }


}
