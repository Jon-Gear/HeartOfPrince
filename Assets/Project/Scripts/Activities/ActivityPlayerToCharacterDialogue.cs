using GameCreator.Runtime.Melee;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class ActivityPlayerToCharacterDialogue : Activity
{
    public override string Name() => "Activity Player -> Character Dialogue";
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

    public override void Start(CharacterBrain brain)
    {
        steps.Add(new StartPlayerToCharacterDialogueStep());

    }
    public override void Finish(CharacterBrain brain)
    {
        steps.Clear();
    }

    
}
