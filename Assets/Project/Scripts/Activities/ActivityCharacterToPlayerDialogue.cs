using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Melee;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class ActivityCharacterToPlayerDialogue : Activity
{

    private bool isWaitingPeriodFinished = true;
    private float minInterval = 1.0f;
    private float maxInterval = 2.0f;

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


        float score = 1.5f;

        // Idea: You can compare traits to lower or higher the score!
        // brain.Traits();

        return score;
    }

    protected override void CreateSteps(CharacterBrain brain)
    {
        steps.Add(new ApproachPlayerStep());
        steps.Add(new StartCharacterToPlayerDialogueStep());
    }

    protected override void Init(CharacterBrain brain)
    {
    }

    protected override void Shutdown(CharacterBrain brain)
    {
        RechargeTimer();
    }

    private async void RechargeTimer()
    {
        isWaitingPeriodFinished = false;
        await Task.Delay((int)(Random.Range(minInterval, maxInterval) * 1000));
        isWaitingPeriodFinished = true;
    }

}
