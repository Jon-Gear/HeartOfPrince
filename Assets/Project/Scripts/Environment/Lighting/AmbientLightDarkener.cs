using UnityEngine;

public class AmbientLightDarkener : MonoBehaviour
{
    [SerializeField] private Color ambientColor = Color.black;
    [SerializeField, Range(0f, 1f)] private float weight = 1.0f;

    void Update()
    {
        // Invert the color (1 - color)
        Color inverted = Color.white - ambientColor;

        // Multiply by weight to control strength
        Color darkening = (-1.0f) * inverted * weight;

        // Subtract this from the ambient light instead of adding
        //GameManager.Instance.GetSystem<LightingManager>().AddToAmbientLight(darkening);
    }
}
