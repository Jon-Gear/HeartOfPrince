using GameCreator.Runtime.Melee;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class ActivityPlayerToCharacterDialogue : Activity
{
    public override float EvaluateScore(CharacterBrain brain)
    {   
        if(!brain.Dialogue().CanStartPlayerToCharacterDialogue())
        {
            return 0.0f;
        }


        float score = 10.0f;

        // Idea: You can compare traits to lower or higher the score!
        // brain.Traits();

        return score;
    }

    public override bool IsCompleted(CharacterBrain brain)
    {
        return brain.Dialogue().IsFree == true;
    }

    public override void Start(CharacterBrain brain)
    {
        
        var dialogueManager = GameManager.Instance.GetSystem<DialogueManager>();
        var playerCharacter = GameManager.Instance.GetSystem<CharacterManager>().GetPlayerCharacter();

        if (playerCharacter.Traits().GetAttribute("attribute-energy") == 0.0f)
        {
            dialogueManager.StartDialogue("prince_tired");
            return;
        }

        brain.Dialogue().TriggerPlayerDialogueWithCharacter();
    }

    public override void Tick(CharacterBrain brain)
    {
    }
    public override void Finish(CharacterBrain brain)
    {
    }
}
