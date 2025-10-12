using GameCreator.Runtime.Dialogue;
using GameCreator.Runtime.Stats;
using System;
using UnityEngine;


/*

How stats interact with other systems:
- Dialogue layer - Stats unlock or block dialogue options. (e.g., High Stress -> “snaps” at someone; High Faith -> “restrains anger.”)

*/


[RequireComponent(typeof(Traits))]
public class TraitsOperator : MonoBehaviour
{
    public enum StatOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Set
    }

    private Traits traits;

    private void Awake()
    {
        traits = GetComponent<Traits>();
    }


    private void OnEnable()
    {
        traits.EventChange += OnChange;
    }

    private void OnDisable()
    {
        traits.EventChange -= OnChange;
    }

    public float GetStat(string statID)
    {
        RuntimeStatData runtimeStat = traits.RuntimeStats.Get(statID);
        if (runtimeStat == null)
        {
            Debug.LogError($"No stat by ID {statID} found");
            return 0.0f;
        }
        return (float)runtimeStat.Base;
    }

    public float GetAttribute(string attributeID)
    {
        RuntimeAttributeData runtimeAttribute = traits.RuntimeAttributes.Get(attributeID);
        if (runtimeAttribute == null)
        {
            Debug.LogError($"No attribute by ID {attributeID} found");
            return 0.0f;
        }
        return (float)runtimeAttribute.Value;
    }

    public void StatSet(string statID, float value)
    {
        RuntimeStatData runtimeStat = traits.RuntimeStats.Get(statID);
        if (runtimeStat == null) return;

        float current = (float)runtimeStat.Base;
        float result = ApplyOperation(current, value, StatOperation.Set);
        runtimeStat.Base = result;
    }

    public void StatAdd(string statID, float value)
    {
        RuntimeStatData runtimeStat = traits.RuntimeStats.Get(statID);
        if (runtimeStat == null) return;

        float current = (float)runtimeStat.Base;
        float result = ApplyOperation(current, value, StatOperation.Add);
        runtimeStat.Base = result;
    }

    public void StatSubtract(string statID, float value)
    {
        RuntimeStatData runtimeStat = traits.RuntimeStats.Get(statID);
        if (runtimeStat == null) return;

        float current = (float)runtimeStat.Base;
        float result = ApplyOperation(current, value, StatOperation.Subtract);
        runtimeStat.Base = result;
    }

    public void StatMultiply(string statID, float value)
    {
        RuntimeStatData runtimeStat = traits.RuntimeStats.Get(statID);
        if (runtimeStat == null) return;

        float current = (float)runtimeStat.Base;
        float result = ApplyOperation(current, value, StatOperation.Multiply);
        runtimeStat.Base = result;
    }

    public void StatDivide(string statID, float value)
    {
        RuntimeStatData runtimeStat = traits.RuntimeStats.Get(statID);
        if (runtimeStat == null) return;

        float current = (float)runtimeStat.Base;
        float result = ApplyOperation(current, value, StatOperation.Divide);
        runtimeStat.Base = result;
    }

    public void AttributeSet(string attributeID, float value)
    {
        RuntimeAttributeData runtimeAttribute = traits.RuntimeAttributes.Get(attributeID);
        if (runtimeAttribute == null) return;

        float current = (float)runtimeAttribute.Value;
        float result = ApplyOperation(current, value, StatOperation.Set);
        runtimeAttribute.Value = result;
    }

    public void AttributeAdd(string attributeID, float value)
    {
        RuntimeAttributeData runtimeAttribute = traits.RuntimeAttributes.Get(attributeID);
        if (runtimeAttribute == null) return;

        float current = (float)runtimeAttribute.Value;
        float result = ApplyOperation(current, value, StatOperation.Add);
        runtimeAttribute.Value = result;
    }

    public void AttributeSubtract(string attributeID, float value)
    {
        RuntimeAttributeData runtimeAttribute = traits.RuntimeAttributes.Get(attributeID);
        if (runtimeAttribute == null) return;

        float current = (float)runtimeAttribute.Value;
        float result = ApplyOperation(current, value, StatOperation.Subtract);
        runtimeAttribute.Value = result;
    }

    public void AttributeMultiply(string attributeID, float value)
    {
        RuntimeAttributeData runtimeAttribute = traits.RuntimeAttributes.Get(attributeID);
        if (runtimeAttribute == null) return;

        float current = (float)runtimeAttribute.Value;
        float result = ApplyOperation(current, value, StatOperation.Multiply);
        runtimeAttribute.Value = result;
    }

    public void AttributeDivide(string attributeID, float value)
    {
        RuntimeAttributeData runtimeAttribute = traits.RuntimeAttributes.Get(attributeID);
        if (runtimeAttribute == null) return;

        float current = (float)runtimeAttribute.Value;
        float result = ApplyOperation(current, value, StatOperation.Divide);
        runtimeAttribute.Value = result;
    }

    private void OnChange()
    {

    }


    private static float ApplyOperation(float current, float value, StatOperation operation)
    {
        switch (operation)
        {
            case StatOperation.Add: return current + value;
            case StatOperation.Subtract: return current - value;
            case StatOperation.Multiply: return current * value;
            case StatOperation.Divide: return current / Mathf.Max(0.0001f, value);
            case StatOperation.Set: return value;
            default: return current;
        }
    }

}
