using UnityEngine;

public class AmbientLightContributor : MonoBehaviour
{
    [SerializeField] private Color ambientColor = Color.black;
    [SerializeField] private float weight = 1.0f;
    void Update()
    {
        //GameManager.Instance.GetSystem<LightingManager>().AddToAmbientLight(ambientColor * weight);
    }
}
