using UnityEngine;

public class DailyAmbientLightContributor : MonoBehaviour
{
    [SerializeField] private Gradient ambientColor;
    [SerializeField] private GameObject sun;

    private float intensity;


    // Update is called once per frame
    void Update()
    {
        CalculateIntensity();
        GameManager.Instance.GetSystem<LightingManager>().AddToAmbientLight(ambientColor.Evaluate(intensity));
    }

    private void CalculateIntensity()
    {
        intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
        intensity = Mathf.Clamp01(intensity);
    }
}
