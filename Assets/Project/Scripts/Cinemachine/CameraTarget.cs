using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] private List<Transform> targets = new List<Transform>();

    void Update()
    {
        if (targets.Count == 0) return;

        Vector3 sum = Vector3.zero;
        foreach (Transform t in targets)
        {
            if (t != null)
                sum += t.position;
        }

        transform.position = sum / targets.Count;
    }

    // Call this to add a target
    public void AddTarget(Transform target)
    {
        if (target != null && !targets.Contains(target))
            targets.Add(target);
    }

    // Call this to remove a target
    public void RemoveTarget(Transform target)
    {
        if (target != null)
            targets.Remove(target);
    }

    // Optional: clear all targets
    public void ClearTargets()
    {
        targets.Clear();
    }

}
