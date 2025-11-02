using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialLightToggle : MonoBehaviour
{
    [SerializeField] List<GameObject> artificialLights = new List<GameObject>();

    public void ToggleArtificialLights(bool isOn)
    {
        foreach (var light in artificialLights)
        {
            light.SetActive(isOn);
        }
    }

}
