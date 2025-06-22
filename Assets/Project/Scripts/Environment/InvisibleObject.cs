using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class HideInGame : MonoBehaviour
{
    private void Update()
    {
        if (Application.isPlaying)
        {
            SetVisibility(false); // Hide in Game view
        }
        else
        {
            SetVisibility(true); // Show in Scene view
        }
    }

    private void SetVisibility(bool visible)
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = visible;
        }
    }
}
