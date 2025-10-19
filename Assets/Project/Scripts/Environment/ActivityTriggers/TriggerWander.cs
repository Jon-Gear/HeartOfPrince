using System.Collections.Generic;
using UnityEngine;

public class TriggerWander : MonoBehaviour
{
    private TriggerCollider triggerArea;
    private BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        triggerArea = GetComponent<TriggerCollider>();
        triggerArea.TriggerEntered += OnZoneEntered;
        triggerArea.TriggerExited += OnZoneExited;
    }

    private void OnZoneEntered(Collider other)
    {
        Actor otherActor = other.gameObject.GetComponent<Actor>();
        if (otherActor == null)
        {
            return;
        }
        
        otherActor.Brain().Activity().AddActivity<ActivityWanderInScene>(boxCollider);
    }

    private void OnZoneExited(Collider other)
    {
        Actor otherActor = other.gameObject.GetComponent<Actor>();
        if (otherActor == null)
        {
            return;
        }
        
        otherActor.Brain().Activity().RemoveActivity<ActivityWanderInScene>();

    }
}
