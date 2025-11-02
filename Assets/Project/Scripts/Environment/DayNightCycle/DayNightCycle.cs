using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using Unity.Multiplayer.Center.Common;
using UnityEngine;

//[ExecuteAlways] 
public class DayNightCycle : MonoBehaviour
{
    [Header("Ambient Color")]
    [SerializeField] private Gradient ambientColor;
    [SerializeField] private Gradient fogColor;
    [SerializeField] private AnimationCurve fogDensityCurve;



    [Header("Sun Light")]
    [SerializeField] private Light sun;
    [SerializeField] private float sunBaseIntensity = 1f;
    [SerializeField] private float sunIntensityVariation = 1.5f;
    [SerializeField] private Gradient sunColor;
    private float intensity;
    
    [Header("Rotations")]
    [SerializeField] private Transform dailyRotation;

    private void Start()
    {
        RenderSettings.sun = sun;
        RenderSettings.fog = true;
    }

    // Update is called once per frame
    private void Update()
    {
        CalculateAmbience();


        RotateSun();
        SetIntensity();
        AdjustColor();
    }

    private void CalculateAmbience()
    {
        RenderSettings.fogColor = fogColor.Evaluate(intensity);
        GameManager.Instance.GetSystem<LightingManager>().AddToFogDensity(fogDensityCurve.Evaluate(intensity));
    }



    private void RotateSun()
    {
        float _sunAngle = GameManager.Instance.GetSystem<TimeManager>().GetSunAngle();

        dailyRotation.transform.localRotation = Quaternion.Euler(new Vector3(_sunAngle, 0f, 0f));
    }

    private void SetIntensity()
    {
        intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
        intensity = Mathf.Clamp01(intensity);

        sun.intensity = intensity * sunIntensityVariation + sunBaseIntensity;
    }

    private void AdjustColor()
    {
        sun.color = sunColor.Evaluate(intensity);

    }
}
