using System;

namespace HeartOfPrince.Domain
{
    [Serializable]
    public sealed class CharacterRelationshipState
    {
        public CharacterID CharacterId { get; }
        public int Trust { get; private set; }

        public CharacterRelationshipState(CharacterID characterId)
        {
            CharacterId = characterId;
        }

        public void ChangeTrust(int amount)
        {
            Trust += amount;
        }

        public void Reset()
        {
            Trust = 0;
        }
    }
}
