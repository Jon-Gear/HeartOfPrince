using UnityEngine;

public class FogContributor : MonoBehaviour
{
    [SerializeField] private float fogDensity = 0.01f;

    // Update is called once per frame
    void Update()
    {
        GameManager.Instance.GetSystem<LightingManager>().AddToFogDensity(fogDensity);
    }
}
