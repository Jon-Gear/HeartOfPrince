using UnityEngine;

//[ExecuteAlways]
public class ArtificialLightEmissiveController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [SerializeField] private float sunIntensityThreshold = 0.3f;
    [SerializeField] private Renderer targetRenderer;

    private Material targetMaterial;
    private void TurnOnLight()
    {
        targetMaterial.SetColor("_EmissionColor", Color.white);
    }
    private void TurnOffLight()
    {
        targetMaterial.SetColor("_EmissionColor", Color.black);
    }

    private void UpdateLight()
    {
        // if (GameManager.Instance.GetSystem<TimeManager>().GetSunIntensity() < sunIntensityThreshold)
        // {
        //     TurnOnLight();
        // }
        // else if (GameManager.Instance.GetSystem<TimeManager>().GetSunIntensity() >= sunIntensityThreshold)
        // {
        //     TurnOffLight();
        // }
    }
    private void Start()
    {
        targetMaterial = targetRenderer.sharedMaterial;
        targetMaterial.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLight();
    }
}
