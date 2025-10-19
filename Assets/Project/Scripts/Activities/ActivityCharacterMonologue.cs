using GameCreator.Runtime.Melee;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class ActivityCharacterMonologue : Activity
{

    bool isWaitingPeriodFinished = true;
    private float minMonologueInterval = 1.0f;
    private float maxMonologueInterval = 5.0f;

    public override float EvaluateScore(CharacterBrain brain)
    {
        if(!isWaitingPeriodFinished)
        {
            return 0.0f;
        }

        if(!brain.Dialogue().CanStartCharacterMonologue())
        {
            return 0.0f;
        }

        
        float score = 0.0f;

        score += 0.1f;

        // Idea: You can compare traits to lower or higher the score!
        // brain.Traits();

        return score;
    }

    protected override void CreateSteps(CharacterBrain brain)
    {
        steps.Add(new StartMonologueStep());
    }
    protected override void Init(CharacterBrain brain)
    {
    }

    private async void RechargeTimer()
    {
        isWaitingPeriodFinished = false;
        await Task.Delay((int)(Random.Range(minMonologueInterval, maxMonologueInterval) * 1000));
        isWaitingPeriodFinished = true;
    }

    protected override void Shutdown(CharacterBrain brain)
    {
        RechargeTimer();
    }
}
