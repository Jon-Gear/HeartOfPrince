using System.Collections.Generic;
using UnityEngine;


public abstract class Activity
{
    public abstract float EvaluateScore(CharacterBrain brain);
    public abstract bool IsCompleted(CharacterBrain brain);

    public abstract void Start(CharacterBrain brain);
    public abstract void Tick(CharacterBrain brain);
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
            Debug.Log("No activity, successful interruption of Activity");
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
    }

    public void RemoveActivity(Activity oldActivity)
    {
        availableActivities.Remove(oldActivity);
    }
    private void ChooseNewActivity()
    {
        Activity newActivity = ChooseHighestScoringActivity();
        
        //if (newActivity == null) return;

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
        if(currentActivity == null || currentActivity.IsCompleted(brain))
        {
            ChooseNewActivity();
        }
        currentActivity?.Tick(brain);
    }





}
