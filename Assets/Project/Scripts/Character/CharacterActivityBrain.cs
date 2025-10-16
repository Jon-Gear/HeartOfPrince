using System.Collections.Generic;
using UnityEngine;


public abstract class ActivityStep
{
    public bool IsComplete { get; protected set; }
    public abstract void Start(CharacterBrain brain);
    public abstract void Tick(CharacterBrain brain);

}

public abstract class Activity
{
    public abstract string Name();

    protected List<ActivityStep> steps = new List<ActivityStep>();
    private int currentStepIndex = 0;
    public abstract float EvaluateScore(CharacterBrain brain);
    public bool IsCompleted()
    {
        return currentStepIndex >= steps.Count;
    }

    // Execution
    public abstract void Start(CharacterBrain brain);
    public void Tick(CharacterBrain brain)
    {
        if (IsCompleted())
        {
            return;
        }

        var currentStep = steps[currentStepIndex];
        currentStep.Tick(brain);
        if(currentStep.IsComplete)
        {
            currentStepIndex++;
            if (!IsCompleted())
            {
                steps[currentStepIndex].Start(brain);
            }
        }
    }

    public abstract void Finish(CharacterBrain brain);

    

}

public class CharacterActivityBrain : MonoBehaviour
{
    private CharacterBrain brain;

    private List<Activity> availableActivities = new();
    private Activity currentActivity = null;


    public void InterruptActivity(Activity newActivity)
    {
        if (currentActivity == null)
        {
            Debug.Log("Current activity is none. Interrupting");
            SwitchActivity(newActivity);
            return;
        }
        float currentActivityScore = currentActivity.EvaluateScore(brain);
        float newActivityScore = newActivity.EvaluateScore(brain);


        if (currentActivityScore < newActivityScore)
        {
            Debug.Log($"Interrupting Activity: Current Activity Score {currentActivityScore} vs New Activity Score {newActivityScore}");
            SwitchActivity(newActivity);
            return;
        }
        else
        {
            Debug.Log($"Interrupt Failed: Current Activity Score {currentActivityScore} vs New Activity Score {newActivityScore}");
        }

    }


    public void AddActivity(Activity newActivity)
    {
        availableActivities.Add(newActivity);
        Debug.Log($"Added activity {newActivity.Name()}");
    }

    public void RemoveActivity(Activity oldActivity)
    {
        availableActivities.Remove(oldActivity);
        Debug.Log($"removed activity {oldActivity.Name()}");
    }
    private void ChooseNewActivity()
    {
        Activity newActivity = ChooseHighestScoringActivity();

        if (newActivity != null)
        {
            Debug.Log($"Chose activity {newActivity?.Name()}");
        }

        SwitchActivity(newActivity);
    }

    private void SwitchActivity(Activity newActivity)
    {
        currentActivity?.Finish(brain);
        currentActivity = newActivity;
        currentActivity?.Start(brain);
    }


    private Activity ChooseHighestScoringActivity()
    {
        float bestScore = 0.0f;
        Activity bestActivity = null;

        if(availableActivities.Count == 0)
        {
            return null;
        }

        foreach (Activity activity in availableActivities)
        {
            float activityScore = activity.EvaluateScore(brain);

            if (bestScore < activityScore)
            {
                bestScore = activityScore;
                bestActivity = activity;
            }
        }

        return bestActivity;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        brain = GetComponent<CharacterBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentActivity == null || currentActivity.IsCompleted())
        {
            ChooseNewActivity();
        }
        currentActivity?.Tick(brain);
    }





}
