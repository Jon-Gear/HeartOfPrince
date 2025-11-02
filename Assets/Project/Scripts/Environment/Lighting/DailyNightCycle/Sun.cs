using Unity.VisualScripting;
using UnityEngine;
using VLB;


[RequireComponent(typeof(Light))]
public class Sun : MonoBehaviour
{
    [SerializeField] private Light directionaLight;
    [SerializeField] private VolumetricLightBeamHD lightBeam;

    [SerializeField] private float sunBaseIntensity = 1f;
    [SerializeField] private float sunIntensityVariation = 1.5f;
    [SerializeField] private Gradient sunColor;
    private float intensity;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RenderSettings.sun = directionaLight;
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
        directionaLight.intensity = intensity * sunIntensityVariation + sunBaseIntensity;
        directionaLight.color = sunColor.Evaluate(intensity);
    }

    private void UpdateLightBeam()
    {
        if(lightBeam == null)
        {
            return;
        }

        lightBeam.intensity = intensity * sunIntensityVariation + sunBaseIntensity;
        lightBeam.colorFlat = sunColor.Evaluate(intensity);
    }


    private void CalculateIntensity()
    {
        intensity = Vector3.Dot(transform.forward, Vector3.down);
        intensity = Mathf.Clamp01(intensity);
    }
}
