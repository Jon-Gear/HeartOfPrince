using System;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "New Activity Catalog",
        menuName = "Heart of Prince/Activities/Activity Catalog")]
    public sealed class ActivityCatalog : ScriptableObject
    {
        [SerializeField] private DayRules dayRules;

        [SerializeField]
        private ActivityDefinition[] activities =
            Array.Empty<ActivityDefinition>();

        [SerializeField]
        private CharacterDefinition[] characters =
            Array.Empty<CharacterDefinition>();

        public DayRules DayRules => dayRules;
        public ActivityDefinition[] Activities => activities;
        public CharacterDefinition[] Characters => characters;

        public ActivityDefinition FindActivity(string activityId)
        {
            if (string.IsNullOrWhiteSpace(activityId) || activities == null)
            {
                return null;
            }

            foreach (ActivityDefinition activity in activities)
            {
                if (activity != null &&
                    string.Equals(
                        activity.Id,
                        activityId,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return activity;
                }
            }

            return null;
        }

        public CharacterDefinition FindCharacter(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId) || characters == null)
            {
                return null;
            }

            foreach (CharacterDefinition character in characters)
            {
                if (character != null &&
                    string.Equals(
                        character.Id,
                        characterId,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return character;
                }
            }

            return null;
        }

        public ActivityDefinition FindActivityForScene(string sceneName)
        {
            if (activities == null)
            {
                return null;
            }

            foreach (ActivityDefinition activity in activities)
            {
                if (activity != null && activity.ContainsScene(sceneName))
                {
                    return activity;
                }
            }

            return null;
        }
    }
}
