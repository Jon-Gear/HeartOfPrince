using GameCreator.Runtime.Behavior;
using System;
using UnityEngine;

[Serializable]
public class ThoughtData
{
    [SerializeField] public string thoughtName;
    [SerializeField] public int maxVariants;
    [SerializeField] public int maxUseLimit = 3;
    
    private int useCount = 0;

    public ThoughtData(string thoughtName, int maxVariants)
    {
        this.thoughtName = thoughtName;
        this.maxVariants = maxVariants;
    }

    public string GetThoughtNodeName()
    {
        if (useCount >= maxUseLimit && maxUseLimit != -1)
        {
            return null; // Thought has reached its use limit
        }

        int index = UnityEngine.Random.Range(1, maxVariants + 1);
        useCount++;
        return $"{thoughtName}_{index}";
    }

    public bool IsThoughtExhausted()
    {
        return useCount >= maxUseLimit && maxUseLimit != -1;
    }
}
