using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : GameSystem
{
    private Color finalAmbientColor = Color.black;
    private int ambientLightContributorsCount = 0;
    private float ambientLightColorTotalWeight = 0.0f;


    private float finalFogDensity = 0f;
    private int fogContributorCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAmbientLight();
        UpdateFog();
    }


    void UpdateAmbientLight()
    {
        if (ambientLightColorTotalWeight > 0f)
        {
            RenderSettings.ambientLight = finalAmbientColor;

            //RenderSettings.ambientIntensity = ambientLightColorTotalWeight / ambientLightContributorsCount;
        }
        //RenderSettings.ambientLight = finalAmbientColor / ambientLightContributorsCount;

        finalAmbientColor = Color.black;
        ambientLightContributorsCount = 0;
        ambientLightColorTotalWeight = 0.0f;
    }

    void UpdateFog()
    {
        RenderSettings.fogDensity = Mathf.Max(finalFogDensity / fogContributorCount, 0.0f);
        finalFogDensity = 0f;
        fogContributorCount = 0;
    }


    public void AddToAmbientLight(Color ambientLight, float weight = 1.0f)
    {

        finalAmbientColor += ambientLight * weight;
        ambientLightContributorsCount += 1;
        ambientLightColorTotalWeight += weight;
    }

    public void AddToFogDensity(float fogDensity)
    {
        finalFogDensity += fogDensity;
        fogContributorCount += 1;
    }


    public override void Init()
    {
    }

    public override void Shutdown()
    {
    }
}
