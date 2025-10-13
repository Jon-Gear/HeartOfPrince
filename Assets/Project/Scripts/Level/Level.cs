using GameCreator.Runtime.Common;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Level Entrances and Exits")]
    [SerializeField] private List<Marker> Entrances;
    [SerializeField] private List<Marker> Exits;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Marker GetRandomEntrance()
    {
        return Entrances[Random.Range(0, Entrances.Count)];
    }

    public Marker GetRandomExit()
    {
        return Exits[Random.Range(0, Exits.Count)];
    }
}
