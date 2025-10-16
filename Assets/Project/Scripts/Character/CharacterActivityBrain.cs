using System.Collections.Generic;
using UnityEngine;


public abstract class ActivityStep
{
    public bool IsComplete { get; protected set; }
    public abstract void Start(CharacterBrain brain);
    public abstract void Tick(CharacterBrain brain);
    public abstract void Finish(CharacterBrain brain);

}

public abstract class Activity
{
    public abstract string Name();

    protected List<ActivityStep> steps = new List<ActivityStep>();
    private int currentStepIndex = 0;
    private bool started = false;

    public abstract float EvaluateScore(CharacterBrain brain);


    protected abstract void Init(CharacterBrain brain);

    protected abstract void CreateSteps(CharacterBrain brain);

    protected abstract void Shutdown(CharacterBrain brain);


    public bool IsCompleted()
    {
        return currentStepIndex >= steps.Count;
    }

    // Execution
    public void Start(CharacterBrain brain)
    {
        Init(brain);
        CreateSteps(brain);

        if (steps.Count == 0)
        {
            UnityEngine.Debug.LogWarning($"Activity {Name()} has no steps!");
            return;
        }

        started = true;
        currentStepIndex = 0;

        // Start first step
        steps[currentStepIndex].Start(brain);
    }
    public void Tick(CharacterBrain brain)
    {
        if (!started || IsCompleted())
        {
            return;
        }

        var currentStep = steps[currentStepIndex];
        currentStep.Tick(brain);

        if (currentStep.IsComplete)
        {
            currentStep.Finish(brain);

            currentStepIndex++;

            if (!IsCompleted())
            {
                steps[currentStepIndex].Start(brain);
            }
        }
    }

    public void Continue(CharacterBrain brain)
    {
        var currentStep = steps[currentStepIndex];
        currentStep.Start(brain);
    }

    public void Interrupt(CharacterBrain brain)
    {
        var currentStep = steps[currentStepIndex];
        currentStep.Finish(brain);
    }

    public void Finish(CharacterBrain brain)
    {
        steps.Clear();
        Shutdown(brain);
    }
}

public class CharacterActivityBrain : MonoBehaviour
{
    private CharacterBrain brain;

    private List<Activity> availableActivities = new();
    private Stack<Activity> currentActivities = new();


    public void ClearAllActivities()
    {
        currentActivities.Clear();
        availableActivities.Clear();
    }

    public void ForceStartActivity<T>() where T : Activity, new()
    {
        T activity = new T();
        StartNewActivity(activity);
    }

    public void AddActivity<T>() where T : Activity, new()
    {
        T activity = new T();
        availableActivities.Add(activity);
        Debug.Log($"Added activity {activity.Name()}");
    }

    public void RemoveActivity<T>() where T : Activity
    {
        Activity activity = availableActivities.Find(a => a is T);

        if(activity != null)
        {
            availableActivities.Remove(activity);
            Debug.Log($"Removed activity {activity.Name()}");
        }
        else
        {
            Debug.Log($"No activity of type {typeof(T).Name} found to remove.");
        }


    }
    

    private void StartNewActivity(Activity newActivity)
    {
        if (newActivity != null)
        {
            if (currentActivities.Count > 0)
            {
                Debug.Log($"Interrupting activity {currentActivities.Peek().Name()}");
                currentActivities.Peek().Interrupt(brain);
            }
            Debug.Log($"Starting activity {newActivity.Name()}");
            currentActivities.Push(newActivity);
            currentActivities.Peek().Start(brain);
        }
    }
    private void FinishCurrentActivity()
    {
        if (currentActivities.Count > 0)
        {
            Debug.Log($"Finished activity {currentActivities.Peek().Name()}");
            currentActivities.Peek().Finish(brain);
            currentActivities.Pop();

            if (currentActivities.Count > 0)
            {
                Debug.Log($"Continuing activity {currentActivities.Peek().Name()}");
                currentActivities.Peek().Continue(brain);
            }

        }
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
        if(currentActivities.Count == 0)
        {
            StartNewActivity(ChooseHighestScoringActivity());
        }

        if (currentActivities.Count > 0)
        {
            currentActivities.Peek().Tick(brain);
        }

        if (currentActivities.Count > 0 && currentActivities.Peek().IsCompleted())
        {
            FinishCurrentActivity();
        }
    }
}
