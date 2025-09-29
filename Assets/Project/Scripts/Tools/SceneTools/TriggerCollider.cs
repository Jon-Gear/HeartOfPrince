using GameCreator.Runtime.Common;
using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A generic trigger component that raises events when something enters or exits.
/// Attach this to a GameObject with a Collider (set as Trigger).
/// </summary>
[RequireComponent(typeof(Collider))]
public class TriggerCollider : MonoBehaviour
{
    [SerializeField] private PropertyGetGameObject m_Target = new PropertyGetGameObject();

    /// <summary>
    /// Raised when another collider enters this trigger.
    /// </summary>
    public event UnityAction<Collider> TriggerEntered;

    /// <summary>
    /// Raised when another collider exits this trigger.
    /// </summary>
    public event UnityAction<Collider> TriggerExited;

    private void Reset()
    {
        // Ensure the collider is set to trigger when this script is added
        var col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject target = m_Target.Get(gameObject);
        if(target == null || other.gameObject == target)
        {
            TriggerEntered?.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject target = m_Target.Get(gameObject);
        if (target == null || other.gameObject == target)
        {
            TriggerExited?.Invoke(other);
        }
    }
}

