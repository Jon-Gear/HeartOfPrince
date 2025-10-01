using GameCreator.Runtime.Common;
using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A generic trigger component that raises events when something enters, stays, or exits.
/// Attach this to a GameObject with a Collider (set as Trigger).
/// </summary>
[RequireComponent(typeof(Collider))]
public class TriggerCollider : MonoBehaviour
{
    [SerializeField] private PropertyGetGameObject m_Target = new PropertyGetGameObject();
    [SerializeField] private LayerMask targetLayer;

    /// <summary>
    /// Raised when another collider enters this trigger.
    /// </summary>
    public event UnityAction<Collider> TriggerEntered;

    /// <summary>
    /// Raised when another collider stays inside this trigger.
    /// </summary>
    public event UnityAction<Collider> TriggerStayed;

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
        if (IsValidTarget(other))
        {
            TriggerEntered?.Invoke(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsValidTarget(other))
        {
            TriggerStayed?.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsValidTarget(other))
        {
            TriggerExited?.Invoke(other);
        }
    }

    /// <summary>
    /// Checks if the given collider belongs to the configured target or target layer.
    /// </summary>
    private bool IsValidTarget(Collider other)
    {
        GameObject target = m_Target.Get(gameObject);
        bool isTarget = target != null && other.gameObject == target;
        bool isInLayer = ((1 << other.gameObject.layer) & targetLayer.value) != 0;
        return isTarget || isInLayer;
    }
}
