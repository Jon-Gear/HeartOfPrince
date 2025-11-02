using UnityEngine;

public class DailyFog : MonoBehaviour
{
    [SerializeField] private Gradient fogColor;
    [SerializeField] private AnimationCurve fogDensityCurve;
    [SerializeField] private GameObject sun;

    private float intensity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RenderSettings.fog = true;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateIntensity();
        CalculateFog();
    }

    private void CalculateFog()
    {
        RenderSettings.fogColor = fogColor.Evaluate(intensity);
        GameManager.Instance.GetSystem<LightingManager>().AddToFogDensity(fogDensityCurve.Evaluate(intensity));

        //Debug.Log("Fog Color: " + RenderSettings.fogColor + " | Fog Density Contribution: " + fogDensityCurve.Evaluate(intensity) + " | Intensity: " + intensity);
    }

    private void CalculateIntensity()
    {
        intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
        intensity = Mathf.Clamp01(intensity);
    }
}
