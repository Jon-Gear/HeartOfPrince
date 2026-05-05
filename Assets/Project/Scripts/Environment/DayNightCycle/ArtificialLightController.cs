using UnityEngine;

//[ExecuteAlways]
public class ArtificialLightController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [SerializeField] private float sunIntensityThreshold = 0.3f;
    [SerializeField] private GameObject artificialLight;
    
    private void ToggleLight()
    {
        artificialLight.SetActive(!artificialLight.activeSelf);
    }

    private void TurnOnLight()
    {
        artificialLight.SetActive(true);
    }
    private void TurnOffLight()
    {
        artificialLight.SetActive(false);
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
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLight();
    }
}
