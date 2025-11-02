using System.Collections.Generic;
using UnityEngine;

public class ArtificialLightSwitch : MonoBehaviour
{


    [SerializeField] List<ArtificialLightToggle> lightToggles = new List<ArtificialLightToggle>();
    
    private bool lightsOn = true;

    private void Start()
    {
        ToggleArtificialLights();
    }

    public void ToggleArtificialLights()
    {
        lightsOn = !lightsOn;

        foreach (var lightToggle in lightToggles)
        {
            lightToggle.ToggleArtificialLights(lightsOn);
        }
    }
}
