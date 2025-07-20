using System;
using UnityEngine;

[Serializable]
public class SpawnNPCEntry
{
    [SerializeField] public string actorName;
    [SerializeField] public string timeStart;
    [SerializeField] public string timeEnd;
    [SerializeField] public bool isSpawned;
}
