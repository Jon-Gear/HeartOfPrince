using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Melee;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class ActivityCharacterToPlayerDialogue : Activity
{
    public override string Name() => "Activity Character -> Player Dialogue";

    bool isWaitingPeriodFinished = true;
    private float minMonologueInterval = 1.0f;
    private float maxMonologueInterval = 5.0f;

    public override float EvaluateScore(CharacterBrain brain)
    {
        if (!isWaitingPeriodFinished)
        {
            return 0.0f;
        }

        if (!brain.Dialogue().CanStartCharacterToPlayerDialogue())
        {
            return 0.0f;
        }


        float score = 1.0f;

        // Idea: You can compare traits to lower or higher the score!
        // brain.Traits();

        return score;
    }

    public override void Start(CharacterBrain brain)
    {
        steps.Add(new ApproachPlayerStep());
        steps.Add(new StartCharacterToPlayerDialogueStep());
        RechargeTimer();
    }


    public override void Finish(CharacterBrain brain)
    {
        steps.Clear();
    }

    private async void RechargeTimer()
    {
        isWaitingPeriodFinished = false;
        await Task.Delay((int)(Random.Range(minMonologueInterval, maxMonologueInterval) * 1000));
        isWaitingPeriodFinished = true;
    }
}
