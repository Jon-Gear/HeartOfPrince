using UnityEngine;
using VLB;

public class Moon : MonoBehaviour
{
    [SerializeField] private Light directionaLight;
    [SerializeField] private VolumetricLightBeamHD lightBeam;

    [SerializeField] private float moonBaseIntensity = 0.5f;
    [SerializeField] private float moonIntensityVariation = 1.0f;
    [SerializeField] private Gradient moonColor;
    private float intensity;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CalculateIntensity();
        UpdateSun();
        UpdateLightBeam();
    }

    private void UpdateSun()
    {
        directionaLight.intensity = intensity * moonIntensityVariation + moonBaseIntensity;
        directionaLight.color = moonColor.Evaluate(intensity);
    }

    private void UpdateLightBeam()
    {
        if (lightBeam == null)
        {
            return;
        }

        lightBeam.intensity = intensity * moonIntensityVariation + moonBaseIntensity;
        lightBeam.colorFlat = moonColor.Evaluate(intensity);
    }


    private void CalculateIntensity()
    {
        intensity = Vector3.Dot(transform.forward, Vector3.down);
        intensity = Mathf.Clamp01(intensity);
    }
}
