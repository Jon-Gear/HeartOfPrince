using UnityEngine;


/// <summary>
/// A base class for different types of dialogue topics in the game.
/// </summary>
public abstract class Topic : ScriptableObject
{
    public string TopicName = "Topic Name";
    public abstract string GetTopicNodeName();
}



/*

TopicCharacterToPlayer

TopicPlayerToCharacter

TopicPlayerMonologue

TopicCharacterMonologue

TopicCharacterToCharacter

- Reactive Dialogue (immediate reaction to in-game triggers) 
- Instructional Dialogue (dialogue with the main intention to teach)
*/