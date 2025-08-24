using UnityEngine;


[ExecuteAlways]
public class LightRayController : MonoBehaviour
{

    [SerializeField] private Gradient lightRayColor;

    [SerializeField] private Renderer lightRayRenderer;

    private Material lightRayMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightRayMaterial = lightRayRenderer.sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        SetLightRayColor();
    }

    void SetLightRayColor()
    {
        lightRayMaterial.SetColor("_TintColor", lightRayColor.Evaluate(TimeManager.Instance.GetSunIntensity()));
    }

    // Original color: A98D45
}
