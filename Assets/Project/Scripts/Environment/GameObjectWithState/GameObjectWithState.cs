using UnityEngine;

public class GameObjectWithState : MonoBehaviour
{
    [Tooltip("Each element represents a different state of the object.")]
    public GameObject[] states;

    [Tooltip("The current active state index.")]
    public int currentState = 0;

    private void Start()
    {
        ApplyState();
    }

    /// <summary>
    /// Switch to a specific state by index.
    /// </summary>
    public void SetState(int stateIndex)
    {
        if (stateIndex < 0 || stateIndex >= states.Length)
        {
            Debug.LogWarning("Invalid state index: " + stateIndex);
            return;
        }

        currentState = stateIndex;
        ApplyState();
    }

    /// <summary>
    /// Cycle to the next state (wraps around).
    /// </summary>
    public void NextState()
    {
        currentState = (currentState + 1) % states.Length;
        ApplyState();
    }

    /// <summary>
    /// Applies the current state by enabling the correct GameObject.
    /// </summary>
    private void ApplyState()
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i] != null)
                states[i].SetActive(i == currentState);
        }
    }
}
