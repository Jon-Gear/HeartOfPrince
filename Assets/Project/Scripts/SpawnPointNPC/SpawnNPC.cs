using GameCreator.Runtime.Common;
using System.Drawing;
using UnityEngine;

public class SpawnNPC : MonoBehaviour
{
    [SerializeField] SpawnNPCEntry[] spawnNPCEntries;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        foreach(SpawnNPCEntry entry in spawnNPCEntries) 
        {
            if (entry.isSpawned) continue;

            if(TimeManager.Instance.IsTimeWithin(entry.timeStart, entry.timeEnd))
            {
                Spawn(entry.actorName);
            }
        }
    }

    void Spawn(string spawnedActorName)
    {
        GameObject prefab = CharacterManager.Instance.GetCharacterPrefab(spawnedActorName);

        
        GameObject npc = Instantiate(prefab, gameObject.transform.position, gameObject.transform.rotation);
    }

}
