using GameCreator.Runtime.Common;
using System;
using UnityEngine;

[Serializable]
public class CharacterEntry
{
    [SerializeField] public GameObject CharacterPrefab;
    [SerializeField] public bool IsAvailable = true;
    [SerializeField] public Marker CurrentPosition;
    [SerializeField] public string DespawnTime;
}

