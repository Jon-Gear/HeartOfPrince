using GameCreator.Runtime.Common;
using Unity.Multiplayer.Center.Common;
using UnityEngine;


[ExecuteAlways] public class DayNightCycle : Singleton<DayNightCycle>
{   
    [Header("Sun Light")]
    [SerializeField] private Light sun;
    [SerializeField] private float sunBaseIntensity = 1f;
    [SerializeField] private float sunIntensityVariation = 1.5f;
    [SerializeField] private Gradient sunColor;
    [SerializeField] private Gradient ambientColor;
    private float intensity;

    [Header("Rotations")]
    [SerializeField] private Transform dailyRotation;

    private void Awake()
    {
        
    }

    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        RotateSun();
        SetIntensity();
        AdjustColor();
    }

    private void RotateSun()
    {
        float _sunAngle = TimeManager.Instance.GetSunAngle();

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
        RenderSettings.ambientLight = ambientColor.Evaluate(intensity);
    }
}
