using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "MyData/CharacterAsset")]
public class CharacterAsset : ScriptableObject
{
    [SerializeField] private GameObject characterPrefab; // The prefab for the character
    [SerializeField] private string characterName; // The name of the character
    [SerializeField] private string description; // A description of the character
    public GameObject CharacterPrefab => characterPrefab; // Property to access the character prefab
    public string CharacterName => characterName; // Property to access the character name
    public string Description => description; // Property to access the character description
}
